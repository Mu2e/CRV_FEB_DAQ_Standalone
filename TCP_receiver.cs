using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.IO;

namespace TB_mu2e
{
    static class TCP_receiver
    {
        private static StreamWriter sw;
        public static DateTime time_start;
        public static DateTime time_read_done;
        public static DateTime time_save_done;
        public static bool NewSpill;
        public static bool SaveEnabled;
        public static bool ParseEnabled;
        public static bool AllDone;
        public static string source_name;

        //To save data in binary
        private static void Save(byte[] buf)
        {
            //sw = PP.myRun.sw;
            using (sw = new StreamWriter(PP.myRun.OutFileName, true))
            {
                try
                {
                    if (sw == null)
                    {
                        return;
                    }
                    int i = 0;
                    NewSpill = true;

                    sw.WriteLine("--Begin of spill");
                    sw.WriteLine("--** SOURCE = " + source_name);
                    foreach (byte b in buf)
                    {
                        sw.Write(b.ToString());
                        sw.Write(" ");
                        i++;
                        if (i == 16) { sw.WriteLine(); i = 0; }
                    }
                    time_save_done = DateTime.Now;

                    sw.WriteLine("--wrote " + buf.Length.ToString() + " bytes");
                    sw.WriteLine("--Read took (in ms):" + time_read_done.Subtract(time_start).TotalMilliseconds.ToString(""));
                    sw.WriteLine("--Save took (in ms):" + time_save_done.Subtract(time_read_done).TotalMilliseconds.ToString(""));

                }
                catch { Console.WriteLine("bad break"); }
            }
        }

        //To save data in human-readable format
        private static void Save(SpillData spill)
        {
            //sw = PP.myRun.sw;
            using (sw = new StreamWriter(PP.myRun.OutFileName, true))
            {
                try
                {
                    if (sw == null)
                        return;

                    int i = 0;
                    NewSpill = true;

                    sw.WriteLine("--Begin of spill (ascii)");
                    sw.WriteLine("--** Source = " + source_name);
                    sw.WriteLine("--SpillHeader");
                    sw.Write(spill.SpillWordCount + " "); //Word counts in spill
                    sw.Write(spill.SpillTrigCount + " "); //Number of triggers in spill
                    sw.Write(spill.SpillCounter + " "); //Spill number
                    sw.Write(spill.ExpectNumCh + " "); //Channel mask
                    sw.Write(spill.BoardID + " "); //Board ID
                    sw.Write(spill.SpillStatus); //Spill status
                    sw.WriteLine("--End SpillHeader");
                    sw.WriteLine("--Events");
                    foreach (Mu2e_Event spillEvent in spill.SpillEvents)
                    {
                        sw.WriteLine("--EventHeader");
                        sw.Write(spillEvent.EventWordCount + " "); //Word count in event
                        sw.Write(spillEvent.EventTimeStamp + " "); //Timestamp for event
                        sw.Write(spillEvent.TrigCounter + " "); //Trigger number for event
                        sw.Write(spillEvent.NumSamples + " "); //Number of ADC samples per event
                        sw.Write(spillEvent.TrigType + " "); //Type of trigger
                        sw.Write(spillEvent.EventStatus); //Event status
                        sw.WriteLine("--End EventHeader");

                        foreach (Mu2e_Ch chan in spillEvent.ChanData)
                        {
                            sw.Write(chan.num_ch + " "); //Write the channel number
                            foreach (int adc in chan.data)
                                sw.Write(adc + " "); //Write the ADC data
                            sw.Write("\r\n");
                        }

                    }
                    sw.WriteLine("--End Events");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception thrown in TCP_receiver::Save(SpillData spill) : " + e);
                }
            }
        }

        public static void ReadFeb(Mu2e_FEB_client feb, /*TcpClient feb_client Socket feb_socket,*/ out long lret) //out List<byte> buf,
        {
            Socket feb_socket = feb.TNETSocket;
            NetworkStream feb_stream = feb.stream;
            source_name = feb.name;
            //PP.myMain.SpillTimer.Enabled = false;
            PP.myRun.ACTIVE = false; //Let the timer continue but don't let it do anything except update messages and whatnot
            feb_socket.ReceiveTimeout = 1000;
            if (feb_socket.Available > 0)
            {
                byte[] junk = new byte[feb_socket.Available];
                //feb_socket.Receive(junk);
                feb_stream.Read(junk, 0, feb_socket.Available);
            }

            time_start = DateTime.Now;
            lret = 0;
            feb.SendStr("rd 6A");
            Thread.Sleep(10);
            feb.ReadStr(out string word_cnt_byte_uppr_fpga0, out int ret_time);
            if (word_cnt_byte_uppr_fpga0.Length > 4)
                word_cnt_byte_uppr_fpga0 = word_cnt_byte_uppr_fpga0.Take(4).ToArray().ToString();
            long word_cnt_uppr_fpga0 = Convert.ToInt64(word_cnt_byte_uppr_fpga0+"0000",16);

            feb.SendStr("rd 6B");
            Thread.Sleep(10);
            feb.ReadStr(out string word_cnt_byte_lowr_fpga0, out ret_time);
            if (word_cnt_byte_lowr_fpga0.Length > 4)
                word_cnt_byte_lowr_fpga0 = word_cnt_byte_lowr_fpga0.Take(4).ToArray().ToString();
            long word_cnt_lowr_fpga0 = Convert.ToInt64(word_cnt_byte_lowr_fpga0,16);

            long expected_word_count = word_cnt_uppr_fpga0 + word_cnt_lowr_fpga0;

            byte[] mem_buff = new byte[expected_word_count * 4 + 16 + 32000];

            byte[] b = PP.GetBytes("RDB\r\n");
            feb_socket.Send(b);
            //int old_available = 0;
            Thread.Sleep(10);



            //while (old_available < feb_socket.Available) //wait for data
            //{
            //    old_available = feb_socket.Available;
            //    Thread.Sleep(100);
            //}
            do
            {
                //Console.WriteLine(feb_socket.Available);
                
                try
                {
                    int read = feb_stream.Read(mem_buff, (int)lret, feb_socket.Available);//feb_socket.Available);//feb_socket.Available);
                    //int read = feb_socket.Receive(mem_buff, (int)lret, 1460, 0);//feb_socket.Available, 0);//, out SocketError err);
                    lret += read;
                    //if (err != 0)
                    //    Console.WriteLine(err);
                }
                catch(System.IO.IOException){ break; }
                //Thread.Sleep(10);
            } while (lret < (16 + (expected_word_count * 4))); //(feb_stream.DataAvailable);

            mem_buff = mem_buff.Take((int)lret).ToArray();
            long SpillWordCount = (mem_buff[0] * 256 * 256 * 256 +
                                   mem_buff[1] * 256 * 256 +
                                   mem_buff[2] * 256 +
                                   mem_buff[3]);


            //sock_buf = new byte[SpillWordCount * 2];
            //long words_left = SpillWordCount;

            //while(words_left > 0)
            //{
            //    long num_words_to_read = 0;
            //    if (words_left > 10000)
            //        num_words_to_read = 10000;
            //    else
            //        num_words_to_read = words_left;

            //    words_left -= num_words_to_read;
            //    b = PP.GetBytes("rdb " + num_words_to_read.ToString() + "\r\n");
                
            //    feb_socket.Send(b);
            //    old_available = 0;
            //    Thread.Sleep(100);
            //    while(old_available < feb_socket.Available)
            //    {
            //        old_available = feb_socket.Available;
            //        Thread.Sleep(10);
            //    }
            //    byte[] datachunk = new byte[feb_socket.Available];
            //    feb_socket.Receive(datachunk, datachunk.Length, SocketFlags.None);
            //    sock_buf.Concat(datachunk);

            //    Console.WriteLine(source_name + " Got: " + num_words_to_read + " out of: " + SpillWordCount + " words.");
            //}

            //int tint = (int)(mem_buff[4] * 256 * 256 * 256 +
            //                 mem_buff[5] * 256 * 256 +
            //                 mem_buff[6] * 256 +
            //                 mem_buff[7]);

            //byte[] mem_buf=new byte[5242879*8]; //Currently never used!
            //int mem_ind = 0;

            //PP.myRun.UpdateStatus(source_name + ": Got " + sock_buf.Length + " bytes, enough for " + ((sock_buf.Length-16)/ 4112)+ " trig => " + (tint * 0x808*2+16).ToString() + ")");
            //Thread.Sleep(5);
            //byte[] buf = new byte[lret];
            //rec_buf.CopyTo(buf, 0);

            //PP.myRun.UpdateStatus("Ended recieving " + buf.LongCount() + " bytes");
            //Thread.Sleep(25);
            //spill word count is the first 4 bytes
            //int ind = 0;
            //long RecWordCount = sock_buf.LongCount<byte>();
            //DateTime start_rec = DateTime.Now;

            //RecWordCount = rec_buf.LongCount<byte>();
            TimeSpan elapsed = DateTime.Now.Subtract(time_start);
            PP.myRun.UpdateStatus(source_name + " read: " + lret.ToString()/*sock_buf.Length.ToString()*/ + " bytes out of " + (SpillWordCount*2).ToString() + " bytes in " + elapsed.TotalMilliseconds + " ms");

            time_read_done = DateTime.Now;

            //SpillData new_spill = new SpillData();
            //bool parse_ok = new_spill.ParseInput(mem_buff/*sock_buf*/);
            if (true)//(parse_ok)
            {
                //PP.myRun.Spills.AddLast(new_spill);
                //if (PP.myRun.Spills.Count > 2)
                //{
                //    if (PP.myRun.Spills.First.Value.IsDisplayed) { PP.myRun.Spills.Remove(PP.myRun.Spills.First.Next); }
                //    else { PP.myRun.Spills.RemoveFirst(); }
                //}
                if (PP.myRun != null)// && PP.myRun.sw != null)
                {
                        if (SaveEnabled && !PP.myRun.SaveAscii) { Save(mem_buff /*sock_buf*/); } //If it should NOT save in a human readable format
                        //else if (SaveEnabled && PP.myRun.SaveAscii) { Save(new_spill); } //If it should save in a human readable format
                }
            }
            else
            { }

            PP.myRun.ACTIVE = true;
            //PP.myMain.SpillTimer.Enabled = true;
            
        }
    }
}

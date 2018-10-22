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

                    //close after 30 min
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

        public static void ReadFeb(string BoardName, Socket s, out long l) //out List<byte> buf,
        {
            source_name = BoardName;
            //PP.myMain.SpillTimer.Enabled = false;
            PP.myRun.ACTIVE = false; //Let the timer continue but don't let it do anything except update messages and whatnot
            s.ReceiveTimeout = 500;
            if (s.Available > 0)
            {
                byte[] junk = new byte[s.Available];
                s.Receive(junk);
            }

            byte[] b = PP.GetBytes("rdb\r\n");
            time_start = DateTime.Now;
            l = 0;

            s.Send(b);
            int old_available = 0;
            while (old_available < s.Available || old_available==0) //wait for data
            {
                old_available = s.Available;
                Thread.Sleep(10);
            }
            byte[] rec_buf = new byte[s.Available];
            //byte[] mem_buf=new byte[5242879*8]; //Currently never used!
            //int mem_ind = 0;
            
            int lret = s.Receive(rec_buf, rec_buf.Length, SocketFlags.None);

            int tint = (int)(rec_buf[4] * 256 * 256 * 256 + 
                             rec_buf[5] * 256 * 256 + 
                             rec_buf[6] * 256 + 
                             rec_buf[7]);
            PP.myRun.UpdateStatus("Got " + rec_buf.Length + " bytes, enough for " + ((rec_buf.Length-16)/ 4112)+ " trig => " + (tint * 0x808*2+16).ToString() + ")");
            //Thread.Sleep(5);
            //byte[] buf = new byte[lret];
            //rec_buf.CopyTo(buf, 0);
            
            //PP.myRun.UpdateStatus("Ended recieving " + buf.LongCount() + " bytes");
            //Thread.Sleep(25);
            //spill word count is the first 4 bytes
            //int ind = 0;
            Int64 t = 0;
            t = (Int64)(rec_buf[0] * 256 * 256 * 256 + 
                        rec_buf[1] * 256 * 256 + 
                        rec_buf[2] * 256 + 
                        rec_buf[3]);
            long SpillWordCount = t * 2;
            long RecWordCount = rec_buf.LongCount<byte>();
            DateTime start_rec = DateTime.Now;
            TimeSpan MaxTimeSpan = TimeSpan.FromSeconds(2);

            //RecWordCount = rec_buf.LongCount<byte>();
            TimeSpan elapsed = DateTime.Now.Subtract(start_rec);
            //Console.WriteLine("read... " + rec_buf.LongCount<byte>().ToString() + " out of " + SpillWordCount.ToString() + " in " + elapsed.TotalMilliseconds + " ms");

            time_read_done = DateTime.Now;

            SpillData new_spill = new SpillData();
            bool parse_ok = new_spill.ParseInput(rec_buf);
            if (true)//(parse_ok)
            {
                PP.myRun.Spills.AddLast(new_spill);
                if (PP.myRun.Spills.Count > 2)
                {
                    if (PP.myRun.Spills.First.Value.IsDisplayed) { PP.myRun.Spills.Remove(PP.myRun.Spills.First.Next); }
                    else { PP.myRun.Spills.RemoveFirst(); }
                }
                if (PP.myRun != null)// && PP.myRun.sw != null)
                {
                        if (SaveEnabled && !PP.myRun.SaveAscii) { Save(rec_buf); } //If it should NOT save in a human readable format
                        else if (SaveEnabled && PP.myRun.SaveAscii) { Save(new_spill); } //If it should save in a human readable format
                }
            }
            else
            { }

            PP.myRun.ACTIVE = true;
            //PP.myMain.SpillTimer.Enabled = true;
            
        }
    }
}

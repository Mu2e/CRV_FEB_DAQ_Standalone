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
        private static BinaryWriter bw;
        public static DateTime time_start;
        public static DateTime time_read_done;
        public static DateTime time_save_done;
        public static bool SaveEnabled;
        public static bool ParseEnabled;
        public static string source_name;

        //To save data in "binary"
        private static void Save(byte[] buf)
        {
            ReaderWriterLock locker = new ReaderWriterLock();
            try
            {
                locker.AcquireWriterLock(10000);//timeout of 10 seconds,
                using (bw = new BinaryWriter(File.Open(PP.myRun.OutFileName, FileMode.Append)))
                {
                    try
                    {
                        int i = 0;

                        bw.Write("--Begin of spill\r\n");
                        bw.Write("--** SOURCE = " + source_name + "\r\n");
                        foreach (byte b in buf)
                        {
                            bw.Write(b);//.ToString());
                            //sw.Write(" ");
                            i++;
                            if (i == 16) { bw.Write("\r\n"); i = 0; }
                        }
                        time_save_done = DateTime.Now;

                        bw.Write("--wrote " + buf.Length.ToString() + " bytes\r\n");
                        bw.Write("--Read took (in ms):" + time_read_done.Subtract(time_start).TotalMilliseconds.ToString("") + "\r\n");
                        bw.Write("--Save took (in ms):" + time_save_done.Subtract(time_read_done).TotalMilliseconds.ToString("")+ "\r\n");
                        bw.Write("--End of spill\r\n");

                        //sw.WriteLine("--wrote " + buf.Length.ToString() + " bytes");
                        //sw.WriteLine("--Read took (in ms):" + time_read_done.Subtract(time_start).TotalMilliseconds.ToString(""));
                        //sw.WriteLine("--Save took (in ms):" + time_save_done.Subtract(time_read_done).TotalMilliseconds.ToString(""));
                        //sw.WriteLine("--End of spill");
                    }
                    catch { Console.WriteLine("bad break"); }
                }

            }
            finally
            {
                locker.ReleaseLock();
            }
        }

        //To save data in human-readable format
        private static void Save(SpillData spill)
        {
            ReaderWriterLock locker = new ReaderWriterLock();
            try
            {
                locker.AcquireWriterLock(10000);//timeout of 10 seconds,
                using (sw = new StreamWriter(PP.myRun.OutFileName, true))
                {
                    try
                    {
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
                        sw.WriteLine("--End of spill (ascii)");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception thrown in TCP_receiver::Save(SpillData spill) : " + e);
                    }
                }
            }
            finally
            {
                locker.ReleaseLock();
            }
        }

        public static void ReadFeb(ref Mu2e_FEB_client feb, /*TcpClient feb_client Socket feb_socket,*/ out long lret) //out List<byte> buf,
        {
            //Socket feb_socket = feb.TNETSocket;
            //NetworkStream feb_stream = feb.stream;
            source_name = feb.name;
            //PP.myMain.SpillTimer.Enabled = false;
            PP.myRun.ACTIVE = false; //Halt the status queries to the boards (single socket atm, so we cannot get data and status concurrently)
            if (feb.TNETSocket.Available > 0)
            {
                byte[] junk = new byte[feb.TNETSocket.Available];
                feb.stream.Read(junk, 0, feb.TNETSocket.Available);
            }

            time_start = DateTime.Now;
            lret = 0;
            byte[] mem_buff;
            byte[] b = PP.GetBytes("RDB 1\r\n"); //get the spill header (and 1 word) & reset read pointers
            byte[] hdr_buf = new byte[18];
            long spillwrdcnt = 0; //spill word count will be the total spill count for the FEB (which means BOTH FPGAs)
            feb.TNETSocket.Send(b);
            Thread.Sleep(10);
            if (feb.TNETSocket.Available > 0) //if we can snag the wordcount from the spill header, then we know how much we should be reading
            {
                feb.TNETSocket.Receive(hdr_buf);
                spillwrdcnt = (hdr_buf[0] * 256 * 256 * 256 + //spillwrdcnt is how many words there are for ALL fpgas in a given spill
                               hdr_buf[1] * 256 * 256 +
                               hdr_buf[2] * 256 +
                               hdr_buf[3]);

                mem_buff = new byte[spillwrdcnt*2]; //each word = 2 bytes, we are going to allocate a buffer in memory to read the FEB data into that is the correct size.
                int bytesread = 0;
                int bytesleft = (int)spillwrdcnt * 2;
                try
                {
                    b = PP.GetBytes("RDB\r\n");
                    feb.TNETSocket.Send(b);
                    Thread.Sleep(10);

                    do
                    {
                        int bytes_now = feb.stream.Read(mem_buff, bytesread, feb.TNETSocket.Available);
                        bytesread += bytes_now;
                        bytesleft -= bytes_now;
                        //Console.WriteLine(source_name + " got " + bytesread + " / " + spillwrdcnt * 2);
                        Thread.Sleep(10);
                    } while (feb.stream.DataAvailable);

                    lret = bytesread;
                }
                catch (System.IO.IOException)
                {
                    feb.Close(); //IO exception thrown if socket was forcibly closed by FEB, so lets tell the C# FEB client it is closed
                } //something went wrong skip spill

                TimeSpan elapsed = DateTime.Now.Subtract(time_start);
                PP.myRun.UpdateStatus(source_name + " read: " + bytesread.ToString() + " bytes out of " + (spillwrdcnt*2).ToString() + " bytes in " + elapsed.TotalMilliseconds + " ms");

                time_read_done = DateTime.Now;

                PP.myRun.ACTIVE = true;

                if (PP.myRun != null)// && PP.myRun.sw != null)
                {
                    //Spawn a thread that will take care of writing the data to file
                    Thread save = new Thread(() => Save(mem_buff));
                    save.Start();
                }

            }

            ////SpillData new_spill = new SpillData();
            ////bool parse_ok = new_spill.ParseInput(mem_buff/*sock_buf*/);
            //if (true)//(parse_ok)
            //{
            //    //PP.myRun.Spills.AddLast(new_spill);
            //    //if (PP.myRun.Spills.Count > 2)
            //    //{
            //    //    if (PP.myRun.Spills.First.Value.IsDisplayed) { PP.myRun.Spills.Remove(PP.myRun.Spills.First.Next); }
            //    //    else { PP.myRun.Spills.RemoveFirst(); }
            //    //}
            //}
            //else
            //{ }

            //PP.myMain.SpillTimer.Enabled = true;

        }
    }
}

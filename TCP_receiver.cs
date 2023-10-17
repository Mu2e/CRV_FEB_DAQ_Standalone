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
        //changed by Ralf 5/18/21
        //private static void Save(byte[] buf, string source)

        public static void ResetTempReading(ref Mu2e_FEB_client feb)
        {

        }

        private static void Save(byte[] buf, string source, System.Net.Sockets.Socket feb_socket)
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

                        //copied from frmMain.cs by Ralf 5/18/21 to get the stab at every spill
                        if (feb_socket != null)
                        {
                            byte[] buff;
                            if (feb_socket.Available > 0) //discard any junk before asking it about the current settings
                            {
                                buff = new byte[feb_socket.Available];
                                feb_socket.Receive(buff);
                            }
//                            socket.Send(stab); //ask it about the current settings for the first two FPGAs
                            byte[] stab = Encoding.ASCII.GetBytes("stab\r\n");
                            feb_socket.Send(stab); //ask it about the current settings for the first two FPGAs
                            System.Threading.Thread.Sleep(10);
                            buff = new byte[feb_socket.Available];
                            feb_socket.Receive(buff);
                            bw.Write("stab\r\n");
                            bw.Write(Encoding.ASCII.GetString(buff));
                        }                            

                        bw.Write("--** SOURCE = " + source + "\r\n");
                        foreach (byte b in buf)
                        {
                            bw.Write(b);//.ToString());
                            //sw.Write(" ");
                            i++;
                            if (i == 16) { bw.Write("\r\n"); i = 0; }
                        }
                        time_save_done = DateTime.Now;

                        bw.Write("--wrote " + buf.Length.ToString() + " bytes\r\n");
                        //bw.Write("--Read took (in ms):" + time_read_done.Subtract(time_start).TotalMilliseconds.ToString("") + "\r\n");
                        //bw.Write("--Save took (in ms):" + time_save_done.Subtract(time_read_done).TotalMilliseconds.ToString("")+ "\r\n");
                        bw.Write("--End of spill\r\n");

                        //sw.WriteLine("--wrote " + buf.Length.ToString() + " bytes");
                        //sw.WriteLine("--Read took (in ms):" + time_read_done.Subtract(time_start).TotalMilliseconds.ToString(""));
                        //sw.WriteLine("--Save took (in ms):" + time_save_done.Subtract(time_read_done).TotalMilliseconds.ToString(""));
                        //sw.WriteLine("--End of spill");
                    }
                    catch { Console.WriteLine("bad break"); }
                }
            }
            catch
            {
                Console.WriteLine("File stream error");
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
                Console.WriteLine("PP.myRun.OutFileName: " + PP.myRun.OutFileName);

                using (sw = new StreamWriter(PP.myRun.OutFileName, true))
                {
                    try
                    {
                        sw.WriteLine("--Begin of spill (ascii)");
                        sw.WriteLine("--** Source = " + spill.Source);
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

        //To save trace data (from test beam spills) to file
        public static void ReadFeb(ref Mu2e_FEB_client feb, /*TcpClient feb_client Socket feb_socket,*/ out long lret) //out List<byte> buf,
        {
            lret = 0;
            //Socket feb_socket = feb.TNETSocket;
            //NetworkStream feb_stream = feb.stream;
            source_name = feb.name;
            //PP.myMain.SpillTimer.Enabled = false;
            //PP.myRun.ACTIVE = false; //Halt the status queries to the boards (single socket atm, so we cannot get data and status concurrently)
            if (feb.TNETSocket.Available > 0)
            {
                byte[] junk = new byte[feb.TNETSocket.Available];
                feb.stream.Read(junk, 0, feb.TNETSocket.Available);
            }

            time_start = DateTime.Now;
            byte[] mem_buff;
            byte[] b = PP.GetBytes("RDB 1\r\n"); //get the spill header (and 1 word) & reset read pointers
            byte[] hdr_buf = new byte[18];
            long spillwrdcnt = 0; //spill word count will be the total spill count for the FEB (which means BOTH FPGAs)
            int readattempts = 0;
            do 
            {
                try { feb.TNETSocket.Send(b); }
                catch (Exception e)
                {
                    PP.myRun.UpdateStatus($"{feb.name} TNETSocket Send failure!");
                    PP.myRun.UpdateStatus(e.Message);
                    feb.Close();
                    feb.Open(); // can check connections on ROC using "lca sock 1"
                }
                Thread.Sleep(50);
                if(feb.TNETSocket.Available > 0)
                    feb.TNETSocket.Receive(hdr_buf);
                readattempts++;
                Console.WriteLine(readattempts);
            } while ((hdr_buf[10] != 255 || hdr_buf[11] != 255 || hdr_buf[16] != 8 || hdr_buf[17] != 8) && readattempts < 3); //10 and 11 are channel mask, but have always been FFFF. 16 and 17 are always 8 8


            if (hdr_buf[10] == 255 || hdr_buf[11] == 255 || hdr_buf[16] == 8 || hdr_buf[17] == 8) //if we can snag the wordcount from the spill header, then we know how much we should be reading
            {
                PP.myRun.READING = true;
                Console.WriteLine("+ READ");

                //feb.TNETSocket.Receive(hdr_buf);
              
                spillwrdcnt = (hdr_buf[0] * 256 * 256 * 256 + //spillwrdcnt is how many words there are for ALL fpgas in a given spill
                               hdr_buf[1] * 256 * 256 +
                               hdr_buf[2] * 256 +
                               hdr_buf[3]);

                mem_buff = new byte[spillwrdcnt * 2]; //each word = 2 bytes, we are going to allocate a buffer in memory to read the FEB data into that is the correct size.
                int bytesread = 0;
                int bytesleft = (int)spillwrdcnt * 2;
                try
                {
                    b = PP.GetBytes("RDB\r\n");
                    feb.TNETSocket.Send(b);
                    Thread.Sleep(100);
                    readattempts = 0;
                    do
                    {
                        int bytes_now = feb.stream.Read(mem_buff, bytesread, feb.TNETSocket.Available);
                        bytesread += bytes_now;
                        bytesleft -= bytes_now;
                        if (bytes_now == 0)
                            readattempts++;
                        else
                            readattempts = 0;
                        Console.WriteLine("  READ " + readattempts);
                        Console.WriteLine(source_name + " got " + bytesread + " / " + spillwrdcnt * 2);
                        Thread.Sleep(100);
                    } while (feb.stream.DataAvailable || (bytesleft > 0 && readattempts < 3));

                    lret = bytesread;
                }
                catch (System.IO.IOException)
                {
                    PP.myRun.UpdateStatus(feb.name + " took too long to respond, continuing.");
                } //something went wrong skip spill
                catch (System.ArgumentOutOfRangeException)
                {
                    PP.myRun.UpdateStatus(feb.name + " got more data than specified in spill header.");
                }



                TimeSpan elapsed = DateTime.Now.Subtract(time_start);
                PP.myRun.UpdateStatus(source_name + " read: " + bytesread.ToString() + " bytes out of " + (spillwrdcnt * 2).ToString() + " bytes in " + elapsed.TotalMilliseconds + " ms");

                time_read_done = DateTime.Now;

                PP.myRun.READING = false;
                Console.WriteLine("- READ");

                if (PP.myRun.validateParse)
                {
                    SpillData new_spill = new SpillData();
                    bool parse_ok = new_spill.ParseInput(mem_buff);
                    if (parse_ok)
                    {
                        if (PP.myRun != null)
                        {
                            if (PP.myRun.SaveAscii)
                            {
                                Thread save = new Thread(() => Save(new_spill));
                                save.Start();
                            }
                            else
                            {
                                //changed by Ralf 5/18/21
                                string savename = feb.name;
                                System.Net.Sockets.Socket feb_socket = feb.TNETSocket;
                                //Thread save = new Thread(() => Save(mem_buff, savename)); //Spawn a thread that will take care of writing the data to file
                                Thread save = new Thread(() => Save(mem_buff, savename, feb_socket)); //Spawn a thread that will take care of writing the data to file
                                save.Start();
                            }

                        }
                    }
                    else //if it fails to parse, don't bother trying to save the spill, but notify the user
                        System.Console.WriteLine(source_name + " failed to parse! Skipping save!"); //PP.myRun.UpdateStatus(source_name + " failed to parse! Skipping save!");
                }
                else
                {
                    if (PP.myRun != null)
                    {
                        if (PP.myRun.SaveAscii)
                        {
                            SpillData new_spill = new SpillData();
                            bool parse_ok = new_spill.ParseInput(mem_buff);
                            Thread save = new Thread(() => Save(new_spill));
                            save.Start();
                        }
                        else
                        {
                            string savename = feb.name;
                            //changed by Ralf 5/18/21
                            System.Net.Sockets.Socket feb_socket = feb.TNETSocket;
                            //Thread save = new Thread(() => Save(mem_buff, savename)); //Spawn a thread that will take care of writing the data to file
                            Thread save = new Thread(() => Save(mem_buff, savename, feb_socket)); //Spawn a thread that will take care of writing the data to file
                            save.Start();
                        }

                    }
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

        //To just read trace data from the FEB and work only in memory, returns true if able to parse, false if not
        public static bool ReadFeb(ref Mu2e_FEB_client feb, ref SpillData spill, out long lret) 
        {
            source_name = feb.name;
            if (feb.TNETSocket.Available > 0)
            {
                byte[] junk = new byte[feb.TNETSocket.Available];
                feb.stream.Read(junk, 0, feb.TNETSocket.Available);
            }

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

                mem_buff = new byte[spillwrdcnt * 2]; //each word = 2 bytes, we are going to allocate a buffer in memory to read the FEB data into that is the correct size.
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
                        Thread.Sleep(100);
                    } while (feb.stream.DataAvailable);

                    lret = bytesread;
                }
                catch (System.IO.IOException)
                {
                    feb.Close(); //IO exception thrown if socket was forcibly closed by FEB, so lets tell the C# FEB client it is closed
                } //something went wrong skip spill

                spill = new SpillData();
                bool parse_ok = spill.ParseInput(mem_buff);
                return parse_ok;
            }
            else
                return false;
        }
    }
}

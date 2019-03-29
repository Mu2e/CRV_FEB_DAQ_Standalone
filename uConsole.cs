using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace TB_mu2e
{
    class uConsole
    {
        public Queue<string> messg_que = new Queue<string>(1024);
        public int max_lines = 128;
        public bool LogSave { get; set; }
        private string logFile = "";

        public string Add_messg(string m)
        {
            if(LogSave)
            {
                try
                {
                    using (System.IO.StreamWriter logStream = new System.IO.StreamWriter(logFile, true))
                    {
                        logStream.WriteLine(m + "\r\n");
                    }
                }
                catch
                {
                    Console.WriteLine("Cannot find/write log file: " + logFile);
                    LogSave = false;
                }
            }
            string[] messages;
            string new_m = "";

            messages = m.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (string msg in messages) //Add the lines from the message to the queue
                messg_que.Enqueue(msg);
            messg_que.Enqueue("-------------"); //Add a spacer to indicate a single set of console messages (grouping of lines)

            while (messg_que.Count > max_lines) //Remove any lines which fall outside of the queue
                messg_que.Dequeue();

            new_m = string.Join("\r\n", messg_que); //Make each line a line again

            return new_m; //return the concatination
        }

        public bool SetLogFile(string filename)
        {
            logFile = filename;
            try
            {
                using (System.IO.StreamWriter logStream = new System.IO.StreamWriter(logFile, true))
                {
                    return true; //if we are able to create/write the file, return true to indicate the console is ready
                }
            }
            catch
            {
                Console.WriteLine("Cannot find/write log file: " + logFile);
                LogSave = false;
                return false;
            }            
        }
    }
}

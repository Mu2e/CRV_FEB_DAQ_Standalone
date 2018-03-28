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
        public int current_lines;

        public string add_messg(string m)
        {
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace TB_mu2e
{
    class uConsole
    {
        public Queue<string> messg_que = new Queue<string>(200);
        public int max_lines = 12;
        public int current_lines;

        public string add_messg(string m)
        {
            current_lines = messg_que.Count + 1;
            while (m.Contains("\r\n"))
            {
                string q = m.Substring(0, m.IndexOf("\r\n") + 2);
                messg_que.Enqueue(q);
                if (messg_que.Count > max_lines) { messg_que.Dequeue(); }
                m = m.Substring(m.IndexOf("\r\n") + 2);
            }
            string new_m = "";
            while (messg_que.Count > max_lines)
            { messg_que.Dequeue(); }

            foreach (string t in messg_que)
            {
                new_m += t;
            }

            return new_m;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.IO;
using ZedGraph;

namespace TB_mu2e
{
    class IV_curve
    {
        public DateTime created_time;
        public DateTime updated_time;
        public DateTime saved_time;
        public int num_point;
        public bool saved;
        public int board;
        public int fpga;
        public int chan;
        public double min_v;
        public double max_v;
        public double max_I;
        public double min_I;
        public PointPairList list;
        public PointPairList loglist;

        public void Erase()
        {
            num_point = 0;
            created_time = Convert.ToDateTime("1/1/2001 00:00:00");
            updated_time = Convert.ToDateTime("1/1/2001 00:00:00");
            saved_time = Convert.ToDateTime("1/1/2001 00:00:00");
            list = new PointPairList();
            loglist = new PointPairList();
            max_v = 0; min_v = 1000;
            max_I = 0; min_I = 5000;
        }

        public void AddPoint(double V, double I)
        {
            if (list == null)
            { list = new PointPairList(); loglist = new PointPairList(); }
            if (num_point == 0) { created_time = DateTime.Now; }
            updated_time = DateTime.Now;
            num_point++;
            if (V > max_v) { max_v = V; }
            if (V < min_v) { max_v = V; }
            if (I > max_I) { max_I = I; }
            if (I < min_I) { min_I = I; }
            list.Add(V, I);
            double lI;
            try { lI = Math.Log10(I*1000); }
            catch { lI = 0; }
            if (lI < -10) { lI = -10; }
            loglist.Add(V, lI);
        }

        public void Save()
        {
            string hName = "";
            string dirName = "c://data//";

            hName += "IV_FEB" + this.board.ToString();
            hName += "_FPGA" + this.fpga.ToString();
            hName += "_ch" + this.chan.ToString();
            hName += "_" + this.created_time.Year.ToString("0000");
            hName += this.created_time.Month.ToString("00");
            hName += this.created_time.Day.ToString("00");
            hName += "_" + this.created_time.Hour.ToString("00");
            hName += this.created_time.Minute.ToString("00");
            hName += this.created_time.Second.ToString("00");

            hName = dirName + hName + ".IV";

            try
            {
                StreamWriter sw = new StreamWriter(hName);
                //int i = 0;
                saved_time = DateTime.Now;
                sw.Write("-- created_time "); sw.WriteLine(this.created_time);
                //sw.Write("-- updated_time "); sw.WriteLine(this.updated_time);
                sw.Write("-- saved_time "); sw.WriteLine(this.saved_time);
                sw.Write("-- num_point "); sw.WriteLine(this.num_point);
                sw.Write("-- min_V "); sw.WriteLine(this.min_v);
                sw.Write("-- max_V "); sw.WriteLine(this.max_v);
                //sw.Write(this.max_count); sw.WriteLine(this.max_count);
                //sw.Write(this.min_count); sw.WriteLine(this.min_count);
                //sw.Write("-- integral "); sw.WriteLine(this.integral);
                sw.Write("-- board "); sw.WriteLine(this.board);
                sw.Write("-- fpga "); sw.WriteLine(this.fpga);
                sw.Write("-- chan "); sw.WriteLine(this.chan);
                sw.WriteLine("--------------");
                foreach (PointPair p in this.list)
                {
                    sw.WriteLine(p.X.ToString("0.000") + "," + p.Y.ToString("0.000000"));
                }
                sw.Close();
                this.saved = true;
            }
            catch { }
        }
    }

    class HISTO_curve
    {
        public DateTime created_time;
        public DateTime updated_time;
        public DateTime saved_time;
        public int interval;
        public int num_point;
        public int min_thresh;
        public int max_thresh;
        public double max_count;
        public double min_count;
        public bool integral;
        public bool saved;
        public int board;
        public int chan;
        public PointPairList list;
        public PointPairList loglist;
        public double V;
        public double I;
        public void Erase()
        {
            num_point = 0;
            created_time = Convert.ToDateTime("1/1/2001 00:00:00");
            updated_time = Convert.ToDateTime("1/1/2001 00:00:00");
            saved_time = Convert.ToDateTime("1/1/2001 00:00:00");
            list = new PointPairList();
            loglist = new PointPairList();
        }

        public void AddPoint(double X, double Y)
        {
            saved = false;
            if (num_point == 0) { created_time = DateTime.Now; }
            updated_time = DateTime.Now;
            num_point++;
            list.Add(X, Y);
            double l;
            try { l = Math.Log10(Y); }
            catch { l = 0; }
            loglist.Add(X, l);
        }


        public void Save()
        {
            string hName = "";
            string dirName = "c://data//";

            hName += "Hist_FEB" + this.board.ToString();
            hName += "_ch" + this.chan.ToString();
            hName += "_" + this.created_time.Year.ToString("0000");
            hName += this.created_time.Month.ToString("00");
            hName += this.created_time.Day.ToString("00");
            hName += "_" + this.created_time.Hour.ToString("00");
            hName += this.created_time.Minute.ToString("00");
            hName += this.created_time.Second.ToString("00");

            hName = dirName + hName + ".hist";

            try
            {
                StreamWriter sw = new StreamWriter(hName);
                //int i = 0;
                saved_time = DateTime.Now;
                sw.Write("-- created_time "); sw.WriteLine(this.created_time);
                //sw.Write("-- updated_time "); sw.WriteLine(this.updated_time);
                sw.Write("-- saved_time "); sw.WriteLine(this.saved_time);
                sw.Write("-- interval "); sw.WriteLine(this.interval);
                sw.Write("-- num_point "); sw.WriteLine(this.num_point);
                sw.Write("-- min_thresh "); sw.WriteLine(this.min_thresh);
                sw.Write("-- max_thresh "); sw.WriteLine(this.max_thresh);
                //sw.Write(this.max_count); sw.WriteLine(this.max_count);
                //sw.Write(this.min_count); sw.WriteLine(this.min_count);
                //sw.Write("-- integral "); sw.WriteLine(this.integral);
                sw.Write("-- board "); sw.WriteLine(this.board);
                sw.Write("-- chan "); sw.WriteLine(this.chan);
                sw.Write("-- V "); sw.WriteLine(this.V);
                sw.Write("-- I "); sw.WriteLine(this.I);
                sw.WriteLine("--------------");
                foreach (PointPair p in this.list)
                {
                    sw.WriteLine(p.X + "," + p.Y);
                }
                sw.Close();
                this.saved = true;
            }
            catch { }
        }

    }

    public static class Histo_helper
    {
        public static void InitColorList(ref Color[] myColorList)
        {
            for (int i = 0; i < myColorList.Length; i++)
            {
                if (i == 0) { myColorList[i] = Color.FromArgb(32, 0, 0); }
                if (i == 1) { myColorList[i] = Color.FromArgb(64, 0, 0); }
                if (i == 2) { myColorList[i] = Color.FromArgb(128, 0, 0); }
                if (i == 3) { myColorList[i] = Color.FromArgb(255, 0, 0); }
                if (i == 4) { myColorList[i] = Color.FromArgb(0, 32, 0); }
                if (i == 5) { myColorList[i] = Color.FromArgb(0, 64, 0); }
                if (i == 6) { myColorList[i] = Color.FromArgb(0, 128, 0); }
                if (i == 7) { myColorList[i] = Color.FromArgb(0, 255, 0); }
                if (i == 8) { myColorList[i] = Color.FromArgb(0, 0, 32); }
                if (i == 9) { myColorList[i] = Color.FromArgb(0, 0, 64); }
                if (i == 10) { myColorList[i] = Color.FromArgb(0, 0, 128); }
                if (i == 11) { myColorList[i] = Color.FromArgb(0, 0, 255); }
                if (i > 11) { myColorList[i] = Color.FromArgb(0, 0, 0); }
            }
        }
    }


}

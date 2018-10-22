using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace TB_mu2e
{
    public class SpillData
    {
        public DateTime RecTime { get; set; }
        public String Source { get; set; }
        public UInt32 SpillWordCount { get; set; }
        public UInt32 SpillTrigCount { get; set; }
        public UInt16 SpillCounter { get; set; }
        public UInt16 Mask { get; set; }
        public UInt16 ExpectNumCh { get; set; }
        public UInt16 BoardID { get; set; }
        public UInt16 SpillStatus { get; set; }
        public bool SpillParsed { get; set; }
        public bool NoError { get; set; }
        public bool IsDisplayed { get; set; }
        public LinkedList<Mu2e_Event> SpillEvents;
        public Dictionary<uint, Mu2e_Event> spillEvent_Table;
        public int MaxTrigPerSpill = 4000;

        public SpillData()
        {
            SpillParsed = false;
            NoError = false;
            SpillEvents = new LinkedList<Mu2e_Event>();
            spillEvent_Table = new Dictionary<uint, Mu2e_Event>();
        }

        public bool ParseInput(byte[] pack, int err = 0)
        {
            if (pack.Length <= 0x10) //0x810) //Check if less or equal to 16 bytes (0x10), since that is only the length of a spill header
                return false; //empty event

            while (pack[0] == 0x3e)
                pack = pack.Skip(1).ToArray();

            try //catch exceptions to prevent program from crashing... usually caused by array index out of bounds due to data problems
            {
                this.RecTime = DateTime.Now; //mark the receipt of the spill with the current time
                #region SpillHeader
                //spill word count, word: 1 upper, 2 lower
                int i = 0; //i is the byte offset counter
                UInt32 t32 = 0;
                t32 = (UInt32)(pack[i] * 256 * 256 * 256 +  //shifting to upper half of upper word
                               pack[i + 1] * 256 * 256 +    //shifting to lower half of upper word
                               pack[i + 2] * 256 +          //shifting to upper half of lower word
                               pack[i + 3]);                //lower half of lower word
                this.SpillWordCount = t32;

                //spill trig count, 3 upper, 4 lower
                i = 4;
                t32 = (UInt32)(pack[i] * 256 * 256 * 256 + 
                               pack[i + 1] * 256 * 256 + 
                               pack[i + 2] * 256 + 
                               pack[i + 3]);
                this.SpillTrigCount = t32;

                if (this.SpillTrigCount > MaxTrigPerSpill) //If the number of triggers is too high, don't bother doing anything else...
                {
                    if (PP.glbDebug) { Console.WriteLine("too many trig, I quit"); }
                    return false;
                }
                UInt32 event_num_left = this.SpillTrigCount; //Number of events to expect when reading data

                //spill counter, 5
                i = 8;
                UInt16 t16 = 0;
                t16 = (UInt16)(pack[i] * 256 + 
                               pack[i + 1]);
                this.SpillCounter = t16;

                //mask, 6
                i = 10;
                t16 = (UInt16)(pack[i] * 256 + 
                               pack[i + 1]);
                this.Mask = t16;
                
                this.ExpectNumCh = 0;
                for (int p = 0; p < 16; p++) //Get the number of possible channels (from the mask)
                {
                    if ((Convert.ToUInt16(Math.Pow(2, p)) & this.Mask) == Convert.ToUInt16(Math.Pow(2, p))) { this.ExpectNumCh = (UInt16)(this.ExpectNumCh + 4); }
                }

                //board id, 7
                i = 12;
                t16 = (UInt16)(pack[i] * 256 + 
                               pack[i + 1]);
                this.BoardID = t16;

                //spill status, 8 --- 1st 3-bits only
                i = 14;
                t16 = (UInt16)(pack[i] * 256 + 
                               pack[i + 1]);
                this.SpillStatus = t16;

                if (PP.glbDebug)
                {
                    System.Console.WriteLine("Spill Word Count: {0}\n", this.SpillWordCount);
                    System.Console.WriteLine("Spill Trig Count: {0}\n", this.SpillTrigCount);
                    System.Console.WriteLine("Spill Counter: {0}\n", this.SpillCounter);
                    System.Console.WriteLine("Channel Count: {0}\n", this.ExpectNumCh);
                    System.Console.WriteLine("Board ID: {0}\n", this.BoardID);
                }
                #endregion SpillHeader

                //i = 17; //skip the 0x3e spacer between the spill header and event header. Obsolete: i = 16;
                i = 16; //This is for the prototype FEBs. The pre-production FEBs (not proto) will send a '>' (0x3e) between event and spill header
                DateTime start_parsing = DateTime.Now; //used to determine how much time is spent parsing the incoming data
                Mu2e_Event this_event = null; //create reference to mu2e_Event objects

                //trigger count starts at 1
                while (event_num_left > 1) //while there are still events to be read
                {
                    this_event = new Mu2e_Event(); //set this_event to new event object

                    #region EventHeader
                    //i is 17 at this point for the first event

                    //event word count, word 1 of event header
                    t16 = (UInt16)(pack[i++] * 256 + 
                                   pack[i++]);
                    this_event.EventWordCount = t16;

                    //event time stamp, 2 upper, 3 lower
                    t32 = (UInt32)(pack[i++] * 256 * 256 * 256 +
                                   pack[i++] * 256 * 256 + 
                                   pack[i++] * 256 + 
                                   pack[i++]);

                    this_event.EventTimeStamp = t32;
                    if (PP.glbDebug) { Console.WriteLine("time_stamp=" + t32); }

                    //trigger counter, 4 upper, 5 lower
                    t32 = (UInt32)(pack[i++] * 256 * 256 * 256 + 
                                   pack[i++] * 256 * 256 + 
                                   pack[i++] * 256 + 
                                   pack[i++]);
                    this_event.TrigCounter = t32;
                    if (PP.glbDebug) { Console.WriteLine("trig=" + t32); }

                    //event num samples, 6 --- first 8 bits only (1st half of word = 1 byte)
                    t16 = (UInt16)(pack[i++] * 256 + 
                                   pack[i++]);
                    this_event.NumSamples = t16;

                    //event trig type, 7 --- first 4 bits only
                    t16 = (UInt16)(pack[i++] * 256 + 
                                   pack[i++]);
                    this_event.TrigType = t16;

                    //event status, 8 --- first 3 bytes only
                    t16 = (UInt16)(pack[i++] * 256 + 
                                   pack[i++]);
                    this_event.EventStatus = t16;

                    #endregion EventHeader

                    #region EventData
                    if (this_event.NumSamples == 0)
                    {
                        if (PP.glbDebug) { Console.WriteLine("stop!"); }
                        return false;
                    }

                    int num_ch = Convert.ToInt32(this_event.EventWordCount / this_event.NumSamples);
                    int loop_limit = (num_ch > 15) ? 16 : num_ch; //Stop the event-reading loop from reading beyond 16 channels per FPGA
                    UInt16 ChNum = 0;

                    for (int k = 0; k < loop_limit; k++)
                    {
                        int[] ch_data = new int[this_event.NumSamples];
                        for (int j = 0; j < this_event.NumSamples; j++)
                        {
                            t16 = (UInt16)(pack[i++] * 256 + pack[i++]);
                            if ((t16 & 0x8000) == 0x8000)//bitwise compare t16 against 'channel format'... if it has the channel flag bit then it must be a channel
                            {
                                if (PP.glbDebug) { Console.WriteLine(t16.ToString("x")); }

                                t16 = (UInt16)(t16 & 0xfff);
                                ChNum = t16;

                                if (PP.glbDebug)
                                {
                                    Console.WriteLine("ch: {0}  i: {1}", ChNum, i);
                                    if (j != 0)
                                    {
                                        Console.WriteLine("   Something's fucky...");
                                    }
                                }
                            }
                            ch_data[j] = t16;
                            if (ch_data[j] > 0x7ff) { ch_data[j] = ch_data[j] - 0xfff; } //dealing with signed binary numbers
                        }

                        Mu2e_Ch this_ch = new Mu2e_Ch();
                        this_event.ChNum++;
                        this_ch.data = ch_data;
                        this_ch.num_ch = ChNum;
                        this_event.ChanData.Add(this_ch);
                    }


                    //if (PP.glbDebug) { Console.WriteLine("end of event {0} at time {1}-----------------", this_event.TrigCounter, this_event.EventTimeStamp); }

                    #endregion EventData

                    #region Event Logic & Merging
                    if (spillEvent_Table.ContainsKey(this_event.TrigCounter))
                    {
                        //update the event already in the table
                        //Reference to the event in the dictionary, which will be updated
                        if (spillEvent_Table.TryGetValue(this_event.TrigCounter, out Mu2e_Event eventToAppend))
                        {
                            eventToAppend.AppendEvent(this_event); //Add the channel data from this_event to the event already in the table
                            if (eventToAppend.ChNum >= ExpectNumCh) //If the updated event now has the expected number of channels, we are approaching the end of event-reading
                                event_num_left--;
                        }
                        else //In the unlikely event that the trigger/event we have found was expected to already exist, but doesn't, stop parsing as there is likely a problem with the data
                        {
                            Console.WriteLine("Error in SpillData:ParseInput:EventLogic&Merging:\n  Found a trigger/event that hasn't been seen before, but was expected to already exist.");
                            return false;
                        }

                    }
                    else //make new dictionary entry
                    {
                        spillEvent_Table.Add(this_event.TrigCounter, this_event);
                        if (loop_limit == this_event.ChNum && this_event.ChNum == ExpectNumCh) //If all of the channels are from just one FPGA, then we shouldn't expect to see any additional correlated events later on in the data
                            event_num_left--;
                    }
                    #endregion Event Logic & Merging

                }

                foreach (var trigEvent in spillEvent_Table) //Put the list of events from the table into the SpillEvents Object
                    this.SpillEvents.AddLast(trigEvent.Value);

                DateTime end_parsing = DateTime.Now; //Timestamp of when parsing ended
                TimeSpan time_to_parse = end_parsing.Subtract(start_parsing); //Compute time required to parse data
                if (PP.glbDebug) { Console.WriteLine("Time to parse {0} events was {1} ms", this.SpillTrigCount, time_to_parse.TotalMilliseconds); }

                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine("Caught exception when parsing spilldata:\n  {0}", e);
                return false;
            }
        }
    }
}

public class Mu2e_Event
{
    public UInt16 EventWordCount { get; set; }
    public UInt32 EventTimeStamp { get; set; }
    public UInt32 TrigCounter { get; set; }
    public UInt16 NumSamples { get; set; }
    public UInt16 TrigType { get; set; }
    public UInt16 EventStatus { get; set; }
    public UInt16 ChNum { get; set; }

    public List<Mu2e_Ch> ChanData;

    public Mu2e_Event()
    {
        EventWordCount = 0;
        EventTimeStamp = 0;
        ChanData = new List<Mu2e_Ch>(16);
    }

    public void AppendEvent(Mu2e_Event eventToAppend)
    {
        this.EventWordCount += eventToAppend.EventWordCount;
        this.ChNum += eventToAppend.ChNum;
        foreach (Mu2e_Ch extraChanData in eventToAppend.ChanData)
            this.ChanData.Add(extraChanData);
    }
}

public class Mu2e_Ch
{
    public int num_ch;
    public int[] data;
}

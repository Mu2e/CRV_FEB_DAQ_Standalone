using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZedGraph;

namespace TB_mu2e
{
    public class FEBSettingsBiasGain
    {
        public int FEBNumber { get; set; }
        public double FEBTemp { get; set; }
        public double CMBTemp { get; set; }
        public int[] GainSettings { get; set; }
        public int[] BiasTrimSettings { get; set; }
        public int[] BiasBulkSettings { get; set; }

        public FEBSettingsBiasGain() {
            FEBNumber = -1; 
            BiasBulkSettings = Enumerable.Repeat(0xFFFF, 8).ToArray();
            BiasTrimSettings = Enumerable.Repeat(0xFFFF, 64).ToArray();
            GainSettings = Enumerable.Repeat(0xFFFF, 8).ToArray();
            CMBTemp = -999.0;
            FEBTemp = -999.0;
        }

        // constructor: retrieve from txt
        public FEBSettingsBiasGain(StreamReader cmdFile, int myFEBNumber, double myCMBTemp, double myFEBTemp)
        {
            int[] TempBiasBulkSettings = Enumerable.Repeat(0xFFFF, 8).ToArray();
            int[] TempBiasTrimSettings = Enumerable.Repeat(0xFFFF, 64).ToArray();
            int[] TempGainSettings = Enumerable.Repeat(0xFFFF, 8).ToArray();
            
            while (!cmdFile.EndOfStream)
            {
                string cmd = cmdFile.ReadLine();
                char commentChk = cmd.First();
                if (commentChk != '$' || commentChk != '#' || commentChk != '/')
                {
                    string[] parts = cmd.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] allowedPrompt = { "wr", "WR", "wR", "Wr" };
                    if (parts.Length >= 3 && allowedPrompt.Contains(parts[0]))
                    {
                        string thisAddress = parts[1];
                        int thisValue = Convert.ToInt32(parts[2], 16);

                        if (PP.biasBulkRegisterList.Contains(thisAddress))
                        {
                            int index = Array.IndexOf(PP.biasBulkRegisterList, thisAddress);
                            TempBiasBulkSettings[index] = thisValue;
                        }
                        else if (PP.biasTrimRegisterList.Contains(thisAddress))
                        {
                            int index = Array.IndexOf(PP.biasTrimRegisterList, thisAddress);
                            TempBiasTrimSettings[index] = thisValue;
                        }
                        else if (PP.gainRegisterList.Contains(thisAddress))
                        {
                            int index = Array.IndexOf(PP.gainRegisterList, thisAddress);
                            TempGainSettings[index] = thisValue;
                        }
                        else
                        {

                        }
                    }
                }
            }

            FEBNumber = myFEBNumber;
            CMBTemp = myCMBTemp;
            FEBTemp = myFEBTemp;
            BiasBulkSettings = TempBiasBulkSettings;
            BiasTrimSettings = TempBiasTrimSettings;
            GainSettings = TempGainSettings;
        }

        // copy from a previous class object
        public FEBSettingsBiasGain Copy (int newFEBNumber = -1)
        {
            FEBSettingsBiasGain copy = new FEBSettingsBiasGain();
            if (newFEBNumber == -1)
            {
                copy.FEBNumber = this.FEBNumber;
            }
            else
            {
                copy.FEBNumber = newFEBNumber;
            }
            copy.GainSettings = this.GainSettings;
            copy.BiasTrimSettings = this.BiasTrimSettings;
            copy.BiasBulkSettings = this.BiasBulkSettings;
            copy.CMBTemp = this.CMBTemp;
            copy.FEBTemp = this.FEBTemp;

            return copy;
        }

        // copy with modifications
        public FEBSettingsBiasGain CopyWithNewGain (int newGain, int newFEBNumber = -1)
        {
            FEBSettingsBiasGain copy = new FEBSettingsBiasGain();
            if (newFEBNumber == -1)
            {
                copy.FEBNumber = this.FEBNumber;
            }
            else
            {
                copy.FEBNumber = newFEBNumber;
            }
            int[] newGainArray = Enumerable.Repeat(newGain, 8).ToArray();
            copy.GainSettings = newGainArray;
            copy.BiasTrimSettings = this.BiasTrimSettings;
            copy.BiasBulkSettings = this.BiasBulkSettings;
            copy.CMBTemp = this.CMBTemp;
            copy.FEBTemp = this.FEBTemp;

            return copy;
        }
        public FEBSettingsBiasGain CopyWithNewBulk(int newBulk, int newFEBNumber = -1)
        {
            FEBSettingsBiasGain copy = new FEBSettingsBiasGain();
            if (newFEBNumber == -1)
            {
                copy.FEBNumber = this.FEBNumber;
            }
            else
            {
                copy.FEBNumber = newFEBNumber;
            }
            int[] newBulkArray = this.BiasBulkSettings;
            for (int i = 0; i < 8; i++)
            {
                newBulkArray[i] = newBulk;
            }
            copy.GainSettings = this.GainSettings;
            copy.BiasTrimSettings = this.BiasTrimSettings;
            copy.BiasBulkSettings = newBulkArray;
            copy.CMBTemp = this.CMBTemp;
            copy.FEBTemp = this.FEBTemp;

            return copy;
        }
        public FEBSettingsBiasGain CopyAddToBulk(int bulkAddition, int newFEBNumber = -1)
        {
            FEBSettingsBiasGain copy = new FEBSettingsBiasGain();
            if (newFEBNumber == -1)
            {
                copy.FEBNumber = this.FEBNumber;
            }
            else
            {
                copy.FEBNumber = newFEBNumber;
            }
            int[] newBulkArray = this.BiasBulkSettings;
            for (int i = 0; i < 8; i++)
            {
                newBulkArray[i] += bulkAddition;
            }
            copy.GainSettings = this.GainSettings;
            copy.BiasTrimSettings = this.BiasTrimSettings;
            copy.BiasBulkSettings = newBulkArray;
            copy.CMBTemp = this.CMBTemp;
            copy.FEBTemp = this.FEBTemp;

            return copy;
        }
        public FEBSettingsBiasGain CopyAddToTrim(int trimAddition, int newFEBNumber = -1)
        {
            FEBSettingsBiasGain copy = new FEBSettingsBiasGain();
            if (newFEBNumber == -1)
            {
                copy.FEBNumber = this.FEBNumber;
            }
            else
            {
                copy.FEBNumber = newFEBNumber;
            }
            int[] newTrimArray = this.BiasBulkSettings;
            for (int i = 0; i < 64; i++)
            {
                newTrimArray[i] += trimAddition;
            }
            copy.GainSettings = this.GainSettings;
            copy.BiasTrimSettings = newTrimArray;
            copy.BiasBulkSettings = this.BiasBulkSettings;
            copy.CMBTemp = this.CMBTemp;
            copy.FEBTemp = this.FEBTemp;

            return copy;
        }

        // calculate expected voltage
        // only a nominal value, as bulk/trim gains and offsets are not included
        public double[] GetBiasInV() {
            double[] bias = new double[64];
            for (int i = 0; i < 64; i++)
            {
                bias[i] = PP.mVPerBulkUnit * (double)this.BiasBulkSettings[i/8] + PP.mVPerTrimUnit * (double)(this.BiasTrimSettings[i]-0x800);
                bias[i] /= 1000.0;
            }
            return bias;
        }

        // balance trims and bulks so trim values are the closest to 0x800

        public void BalanceBiasTrimBulk (int trimGoal = 0x800)
        {   
            int[] newBulk = (int[])this.BiasBulkSettings.Clone();
            int[] newTrim = (int[])this.BiasTrimSettings.Clone();

            for (int iFEB = 0; iFEB < 8; iFEB++)
            {
                int sumTrims = 0;
                for (int i = 0; i < 8; i++) {
                    int chNum = iFEB * 8 + i;
                    sumTrims += this.BiasTrimSettings[chNum];
                }
                int sumDiff = sumTrims - trimGoal * 8;
                int moveBulk = sumDiff / 8 / ((int)(PP.mVPerBulkUnit / PP.mVPerTrimUnit));
                int moveTrim = moveBulk * ((int)(PP.mVPerBulkUnit / PP.mVPerTrimUnit));

                newBulk[iFEB] += moveBulk;
                for (int i = 0; i < 8; i++)
                {
                    int chNum = iFEB * 8 + i;
                    newTrim[chNum] -= moveTrim;
                }
            }
            this.BiasBulkSettings = newBulk;
            this.BiasTrimSettings = newTrim;
        }

        // adjusted by temperature etc; newCMBTemp is a double[16] of all CMB temperatures on the FEB
        //
        // balancing not much affect by bulk/bias gain differences. As bulk gain is between 1.000 and 1.005; trim gain between 0.996 and 0.998,
        // the balancing can have up tp 1% error. For a 55.4mV/C * 30C = 1.62V range, this is slightly smaller than 1 tick in bulk bias. 
        public FEBSettingsBiasGain AdjustBulkByTemperature(double newFEBTemp, double coefFEB, double[] newCMBTemp, double coefCMB, bool smartBalance = true, bool forceBalance = false)
        {
            FEBSettingsBiasGain copy = new FEBSettingsBiasGain();
            // copy these values
            copy.FEBNumber = this.FEBNumber;
            copy.GainSettings = this.GainSettings;
            copy.BiasTrimSettings = this.BiasTrimSettings;
            // adjust thes values
            copy.FEBTemp = newFEBTemp;
            copy.CMBTemp = newCMBTemp.Average();
            // adjust by bulk
            int[] newBulkArray = new int[8];
            for (int i = 0; i < 8; i++) 
            {
                double meanCMBTemp = (newCMBTemp[2 * i] + newCMBTemp[2 * i + 1]) / 2.0;
                double mVNeedToShift = -(meanCMBTemp - this.CMBTemp) * coefCMB - (newFEBTemp - this.FEBTemp) * coefFEB;
                int bulkNeedToShift = (int)Math.Round(mVNeedToShift / PP.mVPerBulkUnit);
                newBulkArray[i] = this.BiasBulkSettings[i] + bulkNeedToShift;
            }
            copy.BiasBulkSettings = newBulkArray;
            // balancing
            if (smartBalance)
            {
                // no need. changing bulk here, never will get negative trims
                // check if any trim is < 0 or > 0xfff. If so, apply balancing 
            }
            if (forceBalance)
            {
                copy.BalanceBiasTrimBulk();
            }            
            return copy;
        }

        // write to FEB registers
        public void FEBWriteTrims()
        {
            Mu2e_FEB_client selectedFEB = PP.FEB_clients[this.FEBNumber];
            if (selectedFEB != null && selectedFEB.ClientOpen) {
                for (int i = 0; i < this.BiasTrimSettings.Length; i++)
                {
                    string cmd = "wr " + PP.biasTrimRegisterList[i] + " " + this.BiasTrimSettings[i].ToString("x");
                    selectedFEB.SendStr(cmd);
                }
            }
        }
        public void FEBWriteBulks()
        {
            Mu2e_FEB_client selectedFEB = PP.FEB_clients[this.FEBNumber];
            if (selectedFEB != null && selectedFEB.ClientOpen)
            {
                for (int i = 0; i < this.BiasBulkSettings.Length; i++)
                {
                    string cmd = "wr " + PP.biasBulkRegisterList[i] + " " + this.BiasBulkSettings[i].ToString("x");
                    selectedFEB.SendStr(cmd);
                }
            }
        }
        public void FEBWriteGains()
        {
            Mu2e_FEB_client selectedFEB = PP.FEB_clients[this.FEBNumber];
            if (selectedFEB != null && selectedFEB.ClientOpen)
            {
                for (int i = 0; i < this.GainSettings.Length; i++)
                {
                    string cmd = "wr " + PP.gainRegisterList[i] + " " + this.GainSettings[i].ToString("x");
                    selectedFEB.SendStr(cmd);
                }
            }
        }
        public void FEBWriteBias()
        {
            this.FEBWriteBulks();
            this.FEBWriteTrims();
        }
        public void FEBWriteSettingsBiasGain()
        {
            this.FEBWriteBias();
            this.FEBWriteGains();
        }

        // dump to display
        public string DumpToText(bool printBiasInV = false)
        {
            string output = "\r\n";
            output += $"--- FEB {this.FEBNumber}\r\n# CMB Temp.: {this.CMBTemp}\r\n# FEB Temp.: {this.FEBTemp}\r\n";
            output += "# Bulk Settings:\r\n";
            for (int i = 0; i < this.BiasBulkSettings.Length; i++)
            {
                output += this.BiasBulkSettings[i].ToString("x3");
                output += " ";
            }
            output += "\r\n";
            output += "# Trim Settings:\r\n";
            for (int i = 0; i < this.BiasTrimSettings.Length; i++)
            {
                output += this.BiasTrimSettings[i].ToString("x3");
                output += " ";
                if (i % 16 == 15 || i == (this.BiasTrimSettings.Length-1))
                {
                    output += "\r\n";
                }
            }
            output += "# Gain Settings:\r\n";
            for (int i = 0; i < this.GainSettings.Length; i++)
            {
                output += this.GainSettings[i].ToString("x3");
                output += " ";
            }
            output += "\r\n\r\n";

            if (printBiasInV)
            {
                double[] bias = this.GetBiasInV();
                output += "Nominal bias values (not considering bulk/trim gains/offsets) [V]:\r\n";
                for (int i = 0; i < bias.Length; i++)
                {
                    output += bias[i].ToString("F3");
                    output += "  ";
                    if (i % 8 == 7 || i == (this.BiasTrimSettings.Length - 1))
                    {
                        output += "\r\n";
                    }
                }
                output += "\r\n";
            }

            return output;

        }

        public string WriteGainsToString()
        {
            string entry = "";
            for (int i = 0; i < this.GainSettings.Length; i++)
            {
                entry += this.GainSettings[i].ToString();
                entry += "-";
            }
            entry = entry.Substring(0, entry.Length - 1);
            return entry;
        }

        public double GetHighestBiasInV()
        {
            double[] bias = this.GetBiasInV();
            double max = 0.0;
            for (int i = 0; i < bias.Length; i++)
            {
                if (bias[i] > max) { max = bias[i]; }
            }
            return max;
        }
        public double GetLowestBiasInV()
        {
            double[] bias = this.GetBiasInV();
            double min = 999.0;
            for (int i = 0; i < bias.Length; i++)
            {
                if (bias[i] < min ) { min = bias[i]; }
            }
            return min;
        }

        public int GetHighestTrim()
        {
            int max = 0;
            for (int i = 0; i < this.BiasTrimSettings.Length; i++)
            {
                if (this.BiasTrimSettings[i] > max) max = this.BiasTrimSettings[i];
            }
            return max;
        }

        public int GetLowestTrim()
        {
            int min = 0xFFFF;
            for (int i = 0; i < this.BiasTrimSettings.Length; i++)
            {
                if (this.BiasTrimSettings[i] < min) min = this.BiasTrimSettings[i];
            }
            return min;
        }

        public bool CheckValid()
        {
            bool isValid = true;

            if (this.FEBNumber == -1) { isValid = false; }
            if (this.CMBTemp == -999.0) { isValid = false; }
            if (this.FEBTemp == -999.0) { isValid = false; }
            for (int i = 0; i < this.BiasBulkSettings.Length; i++)
            {
                if (this.BiasBulkSettings[i] == 0xFFFF) { isValid = false; }
            }
            for (int i = 0; i < this.BiasTrimSettings.Length; i++)
            {
                if (this.BiasTrimSettings[i] == 0xFFFF) { isValid = false; }
            }
            for (int i = 0; i < this.GainSettings.Length; i++)
            {
                if (this.GainSettings[i] == 0xFFFF) { isValid = false; }
            }

            return isValid;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.ComponentModel;

namespace TB_mu2e
{
    public static class AddRegisters
    {
        public static void Add_FEB_reg(out List<Mu2e_Register> list_of_reg)
        {
            list_of_reg = new List<Mu2e_Register>(10);
            Mu2e_Register r1 = new Mu2e_Register()
            {
                name = "CONTROL_STATUS",
                addr = 0x00,
                fpga_offset_mult = 0x400,
                pref_hex = true,
                comment = "Control and status register",
                bit_comment = new string[16]
            };
            r1.bit_comment[0] = "AFE 0 Power. 0 = Run. 1 = Power down.";
            r1.bit_comment[1] = "AFE 1 Power. 0 = Run. 1 = Power down.";
            r1.bit_comment[2] = "Issue a reset to the AFE deserializer logic in the FPGA.";
            r1.bit_comment[3] = "Issue a MIG DDR interface reset.";
            r1.bit_comment[4] = "Issue a readout sequencer reset. Force MIG DDR Write command.";
            r1.bit_comment[5] = "Issue a general reset. The AFE FIFOs, trigger counter, spill counter, and readout sequencer are reset.";
            r1.bit_comment[6] = "Reset the serial controller in the AFE chips.";
            r1.bit_comment[7] = "Clear FM receive parity error.";
            r1.bit_comment[8] = "Not Used";
            r1.bit_comment[9] = r1.bit_comment[8];
            r1.bit_comment[10] = r1.bit_comment[8];
            r1.bit_comment[11] = r1.bit_comment[8];
            r1.bit_comment[12] = r1.bit_comment[8];
            r1.bit_comment[13] = r1.bit_comment[8];
            r1.bit_comment[14] = r1.bit_comment[8];
            r1.bit_comment[15] = r1.bit_comment[8];
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "SDRAM_WritePointer",
                addr = 0x03,
                pref_hex = true,
                fpga_offset_mult = 0x400,
                comment = "SDRam Write address (0x02=upper bits, 0x03=lower bits)",
                bit_comment = new string[15],
                width = 32,
                upper_addr = 0x02,
                lower_addr = 0x03
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "SDRAM_ReadPointer",
                addr = 0x05,
                pref_hex = true,
                fpga_offset_mult = 0x400,
                comment = "SDRam Read address (0x04=upper bits, 0x05=lower bits)",
                bit_comment = new string[16],
                width = 32,
                upper_addr = 0x04,
                lower_addr = 0x05
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "SDRAM_Read_swapped",
                addr = 0x06,
                fpga_offset_mult = 0x400,
                comment = "Byte swapped SDRam read data port",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "SDRAM_Read",
                addr = 0x07,
                fpga_offset_mult = 0x400,
                comment = "Un-swapped SDRam data port",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "MIG_STATUS",
                addr = 0x08,
                pref_hex = true,
                fpga_offset_mult = 0x400,
                comment = "MIG status register",
                bit_comment = new string[16]
            };
            r1.bit_comment[0] = "DDR Reset Busy";
            r1.bit_comment[1] = "DDR Cal Done";
            r1.bit_comment[2] = "WRITE command FIFO full";
            r1.bit_comment[3] = "WRITE command FIFO empty";
            r1.bit_comment[4] = "WRITE data FIFO full";
            r1.bit_comment[5] = "WRITE data FIFO empty";
            r1.bit_comment[6] = "READ command FIFO full";
            r1.bit_comment[7] = "READ command FIFO empty";
            r1.bit_comment[8] = "READ data FIFO full";
            r1.bit_comment[9] = "READ data FIFO full";
            r1.bit_comment[10] = "Not Used";
            r1.bit_comment[11] = r1.bit_comment[10];
            r1.bit_comment[12] = r1.bit_comment[10];
            r1.bit_comment[13] = r1.bit_comment[10];
            r1.bit_comment[14] = r1.bit_comment[10];
            r1.bit_comment[15] = r1.bit_comment[10];
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "MIG_FIFO_COUNT",
                addr = 0x09,
                pref_hex = true,
                fpga_offset_mult = 0x400,
                comment = "MIG FIFO Count",
                bit_comment = new string[16]
            };
            r1.bit_comment[0] = "Bits 5..0: MIG DDR Write Count";
            r1.bit_comment[1] = r1.bit_comment[0];
            r1.bit_comment[2] = r1.bit_comment[0];
            r1.bit_comment[3] = r1.bit_comment[0];
            r1.bit_comment[4] = r1.bit_comment[0];
            r1.bit_comment[5] = r1.bit_comment[0];
            r1.bit_comment[6] = "Not Used";
            r1.bit_comment[7] = r1.bit_comment[6];
            r1.bit_comment[8] = "Bits 13..8: MIG DDR Write Count";
            r1.bit_comment[9] = r1.bit_comment[8];
            r1.bit_comment[10] = r1.bit_comment[8];
            r1.bit_comment[11] = r1.bit_comment[8];
            r1.bit_comment[12] = r1.bit_comment[8];
            r1.bit_comment[13] = r1.bit_comment[8];
            r1.bit_comment[14] = "Not Used";
            r1.bit_comment[15] = r1.bit_comment[14];
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "MIG_BURST_SIZE",
                addr = 0x0A,
                fpga_offset_mult = 0x400,
                comment = "Fixed at 8 long words",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "LVDS_TRANSMIT_FIFO",
                addr = 0x0B,
                fpga_offset_mult = 0x400,
                comment = "A write to this location will send a 16 bit FM word on the LVDS FEB to controller link.",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "HISTO_CONTROL",
                addr = 0x10,
                fpga_offset_mult = 0x400,
                comment = "Histogram control register",
                bit_comment = new string[16]
            };
            r1.bit_comment[0] = "Bits 2..0: Channel Select for both AFE histogrammers";
            r1.bit_comment[1] = r1.bit_comment[0];
            r1.bit_comment[2] = r1.bit_comment[0];
            r1.bit_comment[3] = "Not Used";
            r1.bit_comment[4] = "Mode bit. 0 = Histogrammer is free running over the accumulation interval. 1 = Histogram is qualified by an external gate and the accumulation interval.";
            r1.bit_comment[5] = "Start Histogrammer for AFE 0";
            r1.bit_comment[6] = "Start Histogrammer for AFE 1";
            r1.bit_comment[7] = "Not Used";
            r1.bit_comment[8] = "Bits 9..8: AFE 0 Binning. 0 = 1 ADC/bin, 1 = 2, 2 = 4, 3 = 8.";
            r1.bit_comment[9] = r1.bit_comment[9];
            r1.bit_comment[10] = "Bits 11..10: AFE 1 Binning. 0 = 1 ADC/bin, 1 = 2, 2 = 4, 3 = 8.";
            r1.bit_comment[11] = r1.bit_comment[10];
            r1.bit_comment[12] = "Not Used";
            r1.bit_comment[13] = r1.bit_comment[12];
            r1.bit_comment[14] = r1.bit_comment[12];
            r1.bit_comment[15] = r1.bit_comment[12];
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "HISTO_ACCUMULATION_INTERVAL",
                addr = 0x11,
                fpga_offset_mult = 0x400,
                comment = "Histogram accumulation interval. 12-bit interval in steps of 1 ms. The range is 1-4096 ms. First 5ms of the interval is used for clearing histogram memory.",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "HISTO_MEM_PTR_0",
                addr = 0x14,
                fpga_offset_mult = 0x400,
                comment = "AFE 0. Histogram memory is 512 deep x 32-bits wide. 10-bit pointer. Readout is 2 words for each 32-bit integer.",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "HISTO_MEM_PTR_1",
                addr = 0x15,
                fpga_offset_mult = 0x400,
                comment = "AFE 1. Histogram memory is 512 deep x 32-bits wide. 10-bit pointer. Readout is 2 words for each 32-bit integer.",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "HISTO_READ_0",
                addr = 0x16,
                fpga_offset_mult = 0x400,
                comment = "AFE 0. Histogram memory data port. Each read increments the memory word pointer. The data is read out high word first. Two reads are required to fetch a 32-bit bin count.",
                bit_comment = new string[16],
                width = 32,
                upper_addr = 0x16,
                lower_addr = 0x16
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "HISTO_READ_0",
                addr = 0x17,
                fpga_offset_mult = 0x400,
                comment = "AFE 1. Histogram memory data port. Each read increments the memory word pointer. The data is read out high word first. Two reads are required to fetch a 32-bit bin count.",
                bit_comment = new string[16],
                width = 32,
                upper_addr = 0x17,
                lower_addr = 0x17
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "MUX_SEL",
                addr = 0x20,
                pref_hex = true,
                fpga_offset_mult = 0x400,
                comment = "Selects which of 16 DAC trim resistors is connected to the second level multiplexer. Set Mux enable to 0 before taking SiPM pulsed data.",
                bit_comment = new string[16]
            };
            r1.bit_comment[0] = "Bits 3..0: Channel Select";
            r1.bit_comment[1] = r1.bit_comment[0];
            r1.bit_comment[2] = r1.bit_comment[0];
            r1.bit_comment[3] = r1.bit_comment[0];
            r1.bit_comment[4] = "MUX enable";
            r1.bit_comment[5] = "Not Used";
            r1.bit_comment[6] = r1.bit_comment[5];
            r1.bit_comment[7] = r1.bit_comment[5];
            r1.bit_comment[8] = r1.bit_comment[5];
            r1.bit_comment[9] = r1.bit_comment[5];
            r1.bit_comment[10] = r1.bit_comment[5];
            r1.bit_comment[11] = r1.bit_comment[5];
            r1.bit_comment[12] = r1.bit_comment[5];
            r1.bit_comment[13] = r1.bit_comment[5];
            r1.bit_comment[14] = r1.bit_comment[5];
            r1.bit_comment[15] = r1.bit_comment[5];
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "CHAN_MASK",
                addr = 0x21,
                pref_hex = true,
                fpga_offset_mult = 0x400,
                comment = "A four bit register to select which CMBs to read out. Bits 0..3 enable CMBs 1..4",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "TEST_COUNTER",
                addr = 0x23,
                fpga_offset_mult = 0x400,
                comment = "A write to this address defines the 32 bit test counter.  A read from this address displays the value of bits and increments all 32 bits of the counter after the read.",
                bit_comment = new string[16],
                width = 32,
                upper_addr = 0x22,
                lower_addr = 0x23
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "ONE_WIRE_COMMAND",
                addr = 0x24,
                pref_hex = true,
                fpga_offset_mult = 0x400,
                comment = "One wire command register",
                bit_comment = new string[16]
            };
            r1.bit_comment[0] = "Bits 7..0: if an individual write transaction is requested, the lower eight bits of this register are sent to the one wire interface.";
            r1.bit_comment[1] = r1.bit_comment[0];
            r1.bit_comment[2] = r1.bit_comment[0];
            r1.bit_comment[3] = r1.bit_comment[0];
            r1.bit_comment[4] = r1.bit_comment[0];
            r1.bit_comment[5] = r1.bit_comment[0];
            r1.bit_comment[6] = r1.bit_comment[0];
            r1.bit_comment[7] = r1.bit_comment[0];
            r1.bit_comment[8] = "Start the read temperature sequencer. A complete read temperature sequence will execute when this bit is set. The read value of this bit will return a 0 until the sequence is complete.";
            r1.bit_comment[9] = "Start the read ROM sequencer. A complete ROM read sequence will execute when this bit is set. The read value of this bit will return a 0 until the sequence is complete.";
            r1.bit_comment[10] = "Not Used";
            r1.bit_comment[11] = r1.bit_comment[10];
            r1.bit_comment[12] = r1.bit_comment[10];
            r1.bit_comment[13] = r1.bit_comment[10];
            r1.bit_comment[14] = r1.bit_comment[10];
            r1.bit_comment[15] = r1.bit_comment[10];
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "ONE_WIRE_CONTROL",
                addr = 0x25,
                pref_hex = true,
                fpga_offset_mult = 0x400,
                comment = "One wire control register",
                bit_comment = new string[16]
            };
            r1.bit_comment[0] = "Bits 3..0: Selects which CMB read data is stored in the register file";
            r1.bit_comment[1] = r1.bit_comment[0];
            r1.bit_comment[2] = r1.bit_comment[0];
            r1.bit_comment[3] = r1.bit_comment[0];
            r1.bit_comment[4] = "Request a write transaction";
            r1.bit_comment[5] = "Request a read transaction";
            r1.bit_comment[6] = "Request a reset transaction";
            r1.bit_comment[7] = "Transaction status. Returns a ‘1’ when transaction is in progress";
            r1.bit_comment[8] = "Bits 15..8: Transaction bitcount (N-1). For a write set to seven. For a 72 bit read set to 71.";
            r1.bit_comment[9] = r1.bit_comment[8];
            r1.bit_comment[10] = r1.bit_comment[8];
            r1.bit_comment[11] = r1.bit_comment[8];
            r1.bit_comment[12] = r1.bit_comment[8];
            r1.bit_comment[13] = r1.bit_comment[8];
            r1.bit_comment[14] = r1.bit_comment[8];
            r1.bit_comment[15] = r1.bit_comment[8];
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "ONE_WIRE_READ0",
                addr = 0x26,
                pref_hex = true,
                fpga_offset_mult = 0x400,
                comment = "One wire returnd register file. Contents depend on specifics of the one-wire transaction",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "ONE_WIRE_READ1",
                addr = 0x27,
                pref_hex = true,
                fpga_offset_mult = 0x400,
                comment = "One wire returnd register file. Contents depend on specifics of the one-wire transaction",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "ONE_WIRE_READ2",
                addr = 0x28,
                pref_hex = true,
                fpga_offset_mult = 0x400,
                comment = "One wire returnd register file. Contents depend on specifics of the one-wire transaction",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "ONE_WIRE_READ3",
                addr = 0x29,
                pref_hex = true,
                fpga_offset_mult = 0x400,
                comment = "One wire returnd register file. Contents depend on specifics of the one-wire transaction",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "ONE_WIRE_READ4",
                addr = 0x2a,
                pref_hex = true,
                fpga_offset_mult = 0x400,
                comment = "One wire returnd register file. Contents depend on specifics of the one-wire transaction",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "AFE_INPUT_FIFO_EMPTY_FLAG",
                addr = 0x2F,
                pref_hex = true,
                fpga_offset_mult = 0x400,
                comment = "Shows the level of the empty flags of the 16 FIFOs used to buffer data destined for the DDR RAM.",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            for (int i = 0; i < 16; i++)
            {
                r1 = new Mu2e_Register()
                {
                    name = "BIAS_DAC_CH" + i.ToString(),
                    addr = (ushort)(0x30 + i),
                    fpga_offset_mult = 0x400,
                    comment = "12 bit DACs with a span of ±4.096V. Chan" + i.ToString(),
                    bit_comment = new string[16]
                };
                list_of_reg.Add(r1);
            }

            for (int i = 0; i < 4; i++)
            {
                r1 = new Mu2e_Register()
                {
                    name = "LED_INTENSITY_DAC_CH" + i.ToString(),
                    addr = (ushort)(0x40 + i),
                    fpga_offset_mult = 0x400,
                    comment = "Four 12 bit DACs with a span of 0..14V.  Addresses 0x40..0x43 Apply to CMB 1..4. Chan" + i.ToString(),
                    bit_comment = new string[16]
                };
                list_of_reg.Add(r1);
            }

            r1 = new Mu2e_Register()
            {
                name = "BIAS_BUS_DAC0",
                addr = 0x44,
                fpga_offset_mult = 0x0,
                comment = "Two 12 bits DACs with a possible span of 0..80V. Address 0x44 applies to CMB1..2",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "BIAS_BUS_DAC1",
                addr = 0x45,
                fpga_offset_mult = 0x0,
                comment = "Two 12 bits DACs with a possible span of 0..80V. Address 0x45 applies to CMB3..4",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "AFE_VGA0",
                addr = 0x46,
                fpga_offset_mult = 0x400,
                comment = "Two 12 bits DACs with a span of 0..1.54V. Address 0x46 applies to AFE 0",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "AFE_VGA1",
                addr = 0x47,
                fpga_offset_mult = 0x400,
                comment = "Two 12 bits DACs with a span of 0..1.54V. Address 0x47 applies to AFE 1",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            for (int i = 0; i < 7; i++)
            {
                r1 = new Mu2e_Register()
                {
                    name = "SETUP_DAC_CH" + i.ToString(),
                    addr = (ushort)(0x48 + i),
                    fpga_offset_mult = 0x400,
                    comment = "Dont' touch this. Chan" + i.ToString(),
                    bit_comment = new string[16]
                };
                list_of_reg.Add(r1);
            }

            r1 = new Mu2e_Register()
            {
                name = "SPILL_TRIG_COUNT",
                addr = 0x67,
                fpga_offset_mult = 0x0,
                comment = "Reads the number of triggers received during the spill.",
                bit_comment = new string[16],
                width = 32,
                upper_addr = 0x66,
                lower_addr = 0x67
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "SPILL_NUMBER",
                addr = 0x68,
                fpga_offset_mult = 0x0,
                comment = "Increments once per spill. Use buffer reset to reset this counter.",
                bit_comment = new string[16],
                width = 16
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "EVENT_WORD_COUNT",
                addr = 0x69,
                fpga_offset_mult = 0x0,
                comment = "Increments once per spill. Use buffer reset to reset this counter.",
                bit_comment = new string[16],
                width = 16
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "SPILL_WORD_COUNT",
                addr = 0x6B,
                fpga_offset_mult = 0x0,
                comment = "Read the word count from the most recent spill",
                bit_comment = new string[16],
                width = 32,
                upper_addr = 0x6A,
                lower_addr = 0x6B
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "UPTIME",
                addr = 0x6D,
                fpga_offset_mult = 0x0,
                comment = "A counter showing the number of seconds since the last FPGA reset",
                bit_comment = new string[16],
                width = 32,
                upper_addr = 0x6C,
                lower_addr = 0x6D
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "TRIG_TIME_STAMP",
                addr = 0x73,
                fpga_offset_mult = 0x400,
                comment = "A 32 bit register showing the time stamp of the most recent trigger",
                bit_comment = new string[16],
                width = 32,
                upper_addr = 0x72,
                lower_addr = 0x73
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "PULSER_TRIG_DELAY",
                addr = 0x74,
                fpga_offset_mult = 0x400,
                comment = "An eight bit value in steps of 6.28ns that specifies the delay between receipt of a pulser trigger command from the controller and the issuing of a test pulse to the CMB LEDs. The desire is to obviate the need for pipeline readjustment between data taking and pulser triggers.",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "SPILL_ERROR",
                addr = 0x75,
                pref_hex = true,
                fpga_offset_mult = 0x0,
                comment = "Read the spill error register",
                bit_comment = new string[16]
            };
            r1.bit_comment[0] = "[0] One or more of the AFE FIFOs has overflowed";
            r1.bit_comment[1] = "[1] Event FIFO empty. This should be the case at the end of a spill.";
            r1.bit_comment[2] = "[2] Parity error on the command link from the controller to the TDC.";
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "SPILL_STATE",
                addr = 0x76,
                pref_hex = true,
                fpga_offset_mult = 0x0,
                comment = "Read the spill state register",
                bit_comment = new string[16]
            };
            r1.bit_comment[0] = "[0] The DDR write sequencer is busy.";
            r1.bit_comment[1] = "[1] Spill End Flag.";
            r1.bit_comment[2] = "[2] Spill Gate.";
            list_of_reg.Add(r1);

            #region Broadcast Registers
            r1 = new Mu2e_Register()
            {
                name = "FLASH_GATE_CONTROL",
                addr = 0x300,
                fpga_offset_mult = 0x400,
                comment = "Broadcast. Flash gate control.",
                bit_comment = new string[16]
            };
            r1.bit_comment[0] = "[0] Enable the flash gate.";
            r1.bit_comment[1] = "[1] Select the CMB pulse routing. 0: Flash Gate, 1: LED flasher.";
            r1.bit_comment[2] = "[2] LED Flasher signal source. 0: Test pulser, 1: Flash Gate.";
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "FLASH_GATE_TURN_ON",
                addr = 0x301,
                fpga_offset_mult = 0x400,
                comment = "Broadcast. A write to this address defines the time in the microbunch in steps of 6.28ns that the flash gate is asserted, that is when the SiPM voltage is lowered. The microbunch duration is 270 steps",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "FLASH_GATE_TURN_OFF",
                addr = 0x302,
                fpga_offset_mult = 0x400,
                comment = "Broadcast. A write to this address defines the time in the microbunch in steps of 6.28ns that the flash gate is de-asserted, that is when the SiPM voltage is raised.",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "TRIG_CONTROL",
                addr = 0x303,
                pref_hex = true,
                fpga_offset_mult = 0x400,
                comment = "Broadcast. Trigger control register.",
                bit_comment = new string[16]
            };
            r1.bit_comment[0] = "[0] Writing a ‘1’ to this bit position sends a software trigger.";
            r1.bit_comment[1] = "[1] Selects the trigger input type as a pulse or an FM data stream. The assumption is that a trigger pulse comes from the LEMO connector, the FM encoded trigger message comes from the RJ-45 connector. The microprocessor controls the multiplexer that routes either the LEMO or the RJ-45 signal to the trigger input on the FPGAs.";
            r1.bit_comment[2] = "[2] Trigger Inhibit. if trigger inhibit is enabled, this bit goes to one in response to a trigger";
            r1.bit_comment[3] = "[3] Trigger Inhibit enable.";
            r1.bit_comment[4] = "[4] Spill Inhibit. if spill inhibit is enabled, this bit goes to one in response to end of spill. Writing a ‘1’ to this position sets a request to clear the inhibit. Clear spill inhibit will wait until any spill in progress finishes before clearing and allow more triggers.";
            r1.bit_comment[5] = "[5] Clear spill inhibit."; 
            r1.bit_comment[6] = "[6] Spill Inhibit enable.";
            r1.bit_comment[7] = "[7] Not used.";
            r1.bit_comment[8] = "[8] Enable the on card test pulser.";
            r1.bit_comment[9] = "[9] Run the test pulser for one spill or continuously. 1: Run once 0: Run continuously";
            list_of_reg.Add(r1);


            r1 = new Mu2e_Register()
            {
                name = "HIT_DEL_REG",
                addr = 0x304,
                fpga_offset_mult = 0x400,
                comment = "Broadcast. Specifies the number of pipeline stages the hit data traverses before being presented to the first level FIFO. This is used to compensate for trigger delays. The least count is 12.56 ns and the span is eight bits. The minimum delay setting is one and increases monotonically up to a setting of 255. ",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "NUM_SAMPLE_REG",
                addr = 0x305,
                fpga_offset_mult = 0x400,
                comment = "Broadcast. Specifies the number ADC samples per trigger to record. 1..254 Samples.",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "TEST_PULSE_FREQ",
                addr = 0x307,
                fpga_offset_mult = 0x400,
                comment = "Broadcast. Test Pulser frequency word. The rate is 0.0741 Hz per count.",
                bit_comment = new string[16],
                width = 32,
                upper_addr = 0x306,
                lower_addr = 0x307
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "TEST_PULSE_DURATION",
                addr = 0x308,
                fpga_offset_mult = 0x400,
                comment = "Broadcast. A write to this address defines the length of the internally generated spill in seconds.",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "TEST_PULSE_INTERSPILL",
                addr = 0x309,
                fpga_offset_mult = 0x400,
                comment = "Broadcast. A write to this address defines the length of the internally generated gap between spills in seconds. This is only significant if the test pulser is set to run continuously.",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "COARSE_COUNT_INIT",
                addr = 0x30A,
                fpga_offset_mult = 0x400,
                comment = "Broadcast. A write to this address defines the lower eight bits of the initial count of the time stamp counter. This can be used to match the delay between receipt of a trigger at the controller and the FEB.",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "TEST_PULSE_DELAY",
                addr = 0x30B,
                fpga_offset_mult = 0x400,
                comment = "Broadcast. A write to this address defines the delay in 6.28 ns clock ticks between receipt of a test pulse trigger from the controller and the firing of the trigger logic on the FEB",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "SELF_TRIG_CONTROL",
                addr = 0x30E,
                fpga_offset_mult = 0x400,
                width = 2,
                comment = "Broadcast. 2-bit register. A write to this address can select between two types of self trigger.",
                bit_comment = new string[16]
            };
            r1.bit_comment[0] = "Self trigger qualified by an external trigger and the spill gate. If a hit occurs during the gate interval, the data is recorded, otherwise no data is written.";
            r1.bit_comment[1] = "Self trigger qualified by the spill gate.";
            list_of_reg.Add(r1);

            r1 = new Mu2e_Register()
            {
                name = "SELF_TRIG_THRESH",
                addr = 0x30F,
                fpga_offset_mult = 0x400,
                comment = "Broadcast. A write to this address defines the ADC counts above pedestal required for a self-trigger.",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);

            #endregion Broadcast Registers


            //for (int i = 0; i < 59; i++)
            //{
            //    r1 = new Mu2e_Register();
            //    r1.name = "AFE0_REG_0x" + Convert.ToString(i, 16);
            //    r1.addr = (ushort)(0x100 + i);
            //    r1.pref_hex = true;
            //    r1.fpga_offset_mult = 0x400;
            //    r1.comment = "This space is mapped onto to the AFE5807 register map.." + i.ToString();
            //    r1.bit_comment = new string[16];
            //    list_of_reg.Add(r1);

            //    r1 = new Mu2e_Register();
            //    r1.name = "AFE1_REG_0x" + Convert.ToString(i, 16);
            //    r1.addr = (ushort)(0x200 + i);
            //    r1.pref_hex = true;
            //    r1.fpga_offset_mult = 0x400;
            //    r1.comment = "This space is mapped onto to the AFE5807 register map.." + i.ToString();
            //    r1.bit_comment = new string[16];
            //    list_of_reg.Add(r1);
            //}

        }

        public static void Add_WC_reg(out List<Mu2e_Register> list_of_reg)
        {
            list_of_reg = new List<Mu2e_Register>(1);
            Mu2e_Register r1 = new Mu2e_Register()
            {
                name = "CSR",
                addr = 0x00,
                fpga_offset_mult = 0,
                pref_hex = true,
                comment = "Control and status register",
                bit_comment = new string[16]
            };
            list_of_reg.Add(r1);
        }
    }

    
    public class Mu2e_Register
    {
        public delegate double Conv_to_Double(UInt32 val);
        public delegate UInt32 Conv_from_Double(double v);
        public string name;
        public string comment;
        public string[] bit_comment;
        public UInt16 addr;
        public UInt16 fpga_offset_mult=0x400;
        public UInt16 fpga_index=0;
        public uint width = 16;
        public UInt16 upper_addr;
        public UInt16 lower_addr;
        public UInt32 prev_val = 0;
        public UInt32 val = 0;
        public double dv = 0;
        public bool pref_hex = false;
        public bool pref_double = false;
        public Conv_to_Double myUint2Double = Simple_Conv2Double;
        public enum RegError { Unknown, Timeout }


        public static bool FindName(string reg_name, ref List<Mu2e_Register> reg_list, out Mu2e_Register reg)
        {
            reg = null;
            foreach (Mu2e_Register r in reg_list)
            {
                if (r.name == reg_name)
                { reg = r; return true; }
            }
            { return false; }
            {

            }
        }

        public static bool FindAddr(UInt16 reg_addr, ref List<Mu2e_Register> reg_list, out Mu2e_Register reg)
        {
            reg = null;
            foreach (Mu2e_Register r in reg_list)
            {
                if (r.addr == reg_addr)
                { reg = r; return true; }
            }
            { return false; }
            {

            }
        }

        public static void ReadReg(ref Mu2e_Register reg, ref TcpClient myClient)
        {
            ushort addr = (ushort)(reg.addr + (reg.fpga_index * reg.fpga_offset_mult));
            ushort upper_addr = (ushort)(reg.upper_addr + (reg.fpga_index * reg.fpga_offset_mult));
            ushort lower_addr = (ushort)(reg.lower_addr + (reg.fpga_index * reg.fpga_offset_mult));
            try
            {
                if (myClient.Connected)
                {
                    NetworkStream TNETStream = myClient.GetStream();
                    //StreamWriter SW = new StreamWriter(TNETStream);
                    //StreamReader SR = new StreamReader(TNETStream);
                    if (reg.width <= 16)
                    {
                        string lin = "rd " + Convert.ToString(addr, 16) + "\r\n";
                        byte[] buf = PP.GetBytes(lin);
                        TNETStream.Write(buf, 0, buf.Length);

                        System.Threading.Thread.Sleep(5);
                        if (myClient.Available > 0)
                        {
                            byte[] rec_buf = new byte[myClient.Available];
                            Thread.Sleep(1);
                            int ret_len = TNETStream.Read(rec_buf, 0, rec_buf.Length);
                            reg.prev_val = reg.val;
                            BufVal(rec_buf, out uint t, out double dv);
                            reg.val = t;
                            reg.dv = dv;
                        }
                        else
                        { }
                    }
                    else if (reg.width > 16)
                    {
                        string lin = "rd " + Convert.ToString(upper_addr, 16) + "\r\n";
                        byte[] buf = PP.GetBytes(lin);
                        TNETStream.Write(buf, 0, buf.Length);
                        System.Threading.Thread.Sleep(10);
                        UInt32[] tv = new UInt32[2];
                        if (myClient.Available > 0)
                        {
                            byte[] rec_buf = new byte[myClient.Available];
                            Thread.Sleep(1);
                            int ret_len = TNETStream.Read(rec_buf, 0, rec_buf.Length);
                            reg.prev_val = reg.val;
                            BufVal(rec_buf, out uint t, out double dv);
                            tv[0] = t;
                        }
                        lin = "rd " + Convert.ToString(lower_addr, 16) + "\r\n";
                        buf = PP.GetBytes(lin);
                        TNETStream.Write(buf, 0, buf.Length);
                        System.Threading.Thread.Sleep(10);

                        if (myClient.Available > 0)
                        {
                            byte[] rec_buf = new byte[myClient.Available];
                            Thread.Sleep(1);
                            int ret_len = TNETStream.Read(rec_buf, 0, rec_buf.Length);
                            reg.prev_val = reg.val;
                            BufVal(rec_buf, out uint t, out double dv);
                            tv[1] = t;
                        }

                        reg.val = (tv[0] * 256*256) + tv[1];
                    }
                }
                else { }
            }
            catch { }
        }

        public static void WriteReg(UInt32 v, ref Mu2e_Register reg, ref TcpClient myClient)
        {
            if (myClient == null) { return; }
            ushort addr = (ushort)(reg.addr + (reg.fpga_index * reg.fpga_offset_mult));
            ushort upper_addr = (ushort)(reg.upper_addr + (reg.fpga_index * reg.fpga_offset_mult));
            ushort lower_addr = (ushort)(reg.lower_addr + (reg.fpga_index * reg.fpga_offset_mult));
            if (myClient.Connected)
            {
                NetworkStream TNETStream = myClient.GetStream();
                //StreamWriter SW = new StreamWriter(TNETStream);
                //StreamReader SR = new StreamReader(TNETStream);
                if (reg.width <= 16)
                {
                    if (v >= Math.Pow(2, 16)) { Exception e = new Exception("write val greater than possible"); throw e; }
                    string sv = Convert.ToString(v, 16);
                    string lout = "wr " + Convert.ToString(addr, 16) + " " + sv + "\r\n";
                    byte[] buf = PP.GetBytes(lout);
                    TNETStream.Write(buf, 0, buf.Length);

                    System.Threading.Thread.Sleep(5);

                }
                else if (reg.width > 16)
                {
                    //if (v >= Math.Pow(2, 16)) !stupid! what if we want to write a zero?
                    {
                        uint vu = v % (uint)(Math.Pow(2, 16));
                        string sv = Convert.ToString(vu, 16);
                        string lout = "wr " + Convert.ToString(upper_addr, 16) + " " + sv + "\r\n";
                        byte[] buf = PP.GetBytes(lout);
                        TNETStream.Write(buf, 0, buf.Length);
                    }
                    //else
                    {
                        string sv = "0";
                        string lout = "wr " + Convert.ToString(upper_addr, 16) + " " + sv + "\r\n";
                        byte[] buf = PP.GetBytes(lout);
                        TNETStream.Write(buf, 0, buf.Length);
                    }

                    System.Threading.Thread.Sleep(5);
                    {
                        uint vl = v & (uint)(0xffff);
                        string sv = Convert.ToString(vl, 16);
                        string lout = "wr " + Convert.ToString(lower_addr, 16) + " " + sv + "\r\n";
                        byte[] buf = PP.GetBytes(lout);
                        TNETStream.Write(buf, 0, buf.Length);
                    }
                }
                reg.val = v;
            }
            else { Exception e = new Exception("can't scan without a connected FEB client"); }//            throw (e); }

        }

        #region helpers
        private static void BufVal(byte[] rec_buf, out UInt32 v, out double dv)
        {
            v = 0;
            dv = 0;
            int len = rec_buf.Length;
            char[] chars = new char[len];
            for (int i = 0; i < len; i++)
            {
                chars[i] = (char)rec_buf[i];
            }
            string t = new string(chars);
            char[] sep = new char[3];
            sep[0] = ' ';
            sep[1] = '\r';
            sep[2] = '\n';
            string[] tok = t.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            try { v = Convert.ToUInt32(tok[0],16); }
            catch { v = 0; }
            //catch { adc = -1; }
            //double adc;
            //try { adc = Convert.ToDouble(tok[10]); }
            //catch { adc = -1; }
            //if (adc > 4.096) { adc = 8.192 - adc; }
            //double I = adc / 8 * 250;
            //for (int i = 0; i < tok.Length; i++)
        }

        private static double Simple_Conv2Double(UInt32 v)
        { return Convert.ToDouble(v); }

       
        private static double Adc2Volts(UInt32 v)
        {
            double adc;
            try { adc = Convert.ToDouble(v); }
            catch { adc = -1; }
            if (adc > 4.096) { adc = 8.192 - adc; }
            double I = adc ;
            return I;
        }

        private static double Adc2microA(UInt32 v)
        {
            double adc;
            try { adc = Convert.ToDouble(v); }
            catch { adc = -1; }
            if (adc > 4.096) { adc = 8.192 - adc; }
            double I = adc / 8 * 250;
            return I;
        }
        #endregion helpers
    }

}

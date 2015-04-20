﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.IO;

namespace PIC_Sim
{
    class COMCOM
    {
        private byte[] ram;
        private SerialPort port = new SerialPort();

        public COMCOM()
        {
        }

        public COMCOM(byte[] ram)
        {
            this.ram = ram;
            port.BautRate = 4800;
            port.Handshake = Handshake.None;
            port.StopBits = StopBits.One;
            port.PortName = "COM4";
            port.Parity = Parity.None;
        }

        private byte lowerNibble(byte Byte)
        {
            // Code für lower Nibble eines Bytes
            byte retByte = 0;
            retByte = (byte)(Byte & (15));
            retByte = (byte)(retByte + 48);
            return retByte;
        }

        private byte upperNibble(byte Byte)
        {
            // Code für upper Nibble eines Byte
            byte retByte = 0;
            retByte = (byte)(Byte & (240));
            retByte >>= 4;
            retByte = (byte)(retByte + 48);
            return retByte;
        }

        private byte Decode(byte h_Nibble, byte l_Nibble)
        {
            h_Nibble = (byte)(h_Nibble & 15);
            h_Nibble = (byte)(h_Nibble << 4);
            l_Nibble = (byte)(l_Nibble & 15);
            return (byte)(l_Nibble | h_Nibble);
        }

        public void refresh()
        {
            //Läd die Hardware neu, sendet Tris und Ports und läd Ports
            byte[] pushBytes = new byte[9];
            byte[] pullBytes = new byte[5];

            pushBytes[0] = upperNibble(RAM[0x85]);
            pushBytes[1] = lowerNibble(RAM[0x85]);
            pushBytes[2] = upperNibble(RAM[5]);
            pushBytes[3] = lowerNibble(RAM[5]);
            pushBytes[4] = upperNibble(RAM[0x86]);
            pushBytes[5] = lowerNibble(RAM[0x86]);
            pushBytes[6] = upperNibble(RAM[6]);
            pushBytes[7] = lowerNibble(RAM[6]);
            pushBytes[8] = 13;
            try
            {
                Port.Open();
                Port.DiscardOutBuffer();
                Port.DiscardInBuffer();
                Port.Write(pushBytes, 0, 9);
                Port.DiscardOutBuffer();
                Port.DiscardInBuffer();
                for (int i = 0; i < 5; i++)
                {
                    pullBytes[i] = (byte)(Port.ReadByte());
                }
                ram[5] = Decode(pullBytes[0], pullBytes[1]);
                ram[6] = Decode(pullBytes[2], pullBytes[3]);
            }
            catch
            {
            }
            Port.Close();
        }

        public void setRam(byte[] newRam)
        {
            ram = newRam;
            //refresh();   <- whyy?
        }

        public byte[] returnRAM()
        {
            return ram;
        }
    }
}
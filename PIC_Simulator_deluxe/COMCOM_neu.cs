using System;
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
            port.BaudRate = 4800;
            port.Handshake = Handshake.None;
            port.StopBits = StopBits.One;
            port.PortName = "COM4";
            port.Parity = Parity.None;
        }

        private byte lowerNibble(byte Byte)
        {
            // Code für unteren 4Bits eines Bytes
            byte retByte = 0;
            retByte = (byte)(Byte & (15));
            retByte = (byte)(retByte + 48);
            return retByte;
        }

        private byte upperNibble(byte Byte)
        {
            // Code für oberen 4Bits eines Byte
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
            //Läd die Hardware neu, sendet Tris und Ports und läd die Ports
            byte[] pushBytes = new byte[9];
            byte[] pullBytes = new byte[5];

            pushBytes[0] = upperNibble(ram[0x85]);
            pushBytes[1] = lowerNibble(ram[0x85]);
            pushBytes[2] = upperNibble(ram[5]);
            pushBytes[3] = lowerNibble(ram[5]);
            pushBytes[4] = upperNibble(ram[0x86]);
            pushBytes[5] = lowerNibble(ram[0x86]);
            pushBytes[6] = upperNibble(ram[6]);
            pushBytes[7] = lowerNibble(ram[6]);
            pushBytes[8] = 13;
            try
            {
                port.Open();
                port.DiscardOutBuffer();
                port.DiscardInBuffer();
                port.Write(pushBytes, 0, 9);
                port.DiscardOutBuffer();
                port.DiscardInBuffer();
                for (int i = 0; i < 5; i++)
                {
                    pullBytes[i] = (byte)(port.ReadByte());
                }
                ram[5] = Decode(pullBytes[0], pullBytes[1]);
                ram[6] = Decode(pullBytes[2], pullBytes[3]);
            }
            catch
            {
            }
            port.Close();
        }

        public void setRam(byte[] newRam)
        {
            ram = newRam;
            //refresh();   <- whyy?
        }

        public byte[] returnRam()
        {
            return ram;
        }
    }
}
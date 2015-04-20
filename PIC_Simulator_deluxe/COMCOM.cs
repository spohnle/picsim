using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.IO;

namespace PIC_Simulator_deluxe
{
    class COMCOM
    {
        private byte[] RAM;
        private SerialPort Port = new SerialPort();

        public COMCOM()
        {
        }
        public COMCOM(byte[] RAM)
        {
            this.RAM = RAM;
            Port.BaudRate = 4800;
            Port.Handshake = Handshake.None;
            Port.StopBits = StopBits.One;
            Port.PortName = "COM4";
            Port.Parity = Parity.None;
        }

        private byte upperNibbleCoding(byte aByte)
        {
            // Codes upper Nibble of Byte
            byte returnByte = 0;
            returnByte = (byte)(aByte & (240));
            returnByte >>= 4;
            returnByte = (byte)(returnByte + 48);
            return returnByte;
        }

        private byte lowerNibbleCoding(byte aByte)
        {
            // Codes lower Nibble of Byte
            byte returnByte = 0;
            returnByte = (byte)(aByte & (15));
            returnByte = (byte)(returnByte + 48);
            return returnByte;
        }

        public void refresh()
        {
            // refreshes the hardware, sends Tris and Ports and receives Ports.


            byte[] sendBytes = new byte[9];
            byte[] receivedBytes = new byte[5];

            sendBytes[0] = upperNibbleCoding(RAM[0x85]);
            sendBytes[1] = lowerNibbleCoding(RAM[0x85]);
            sendBytes[2] = upperNibbleCoding(RAM[5]);
            sendBytes[3] = lowerNibbleCoding(RAM[5]);
            sendBytes[4] = upperNibbleCoding(RAM[0x86]);
            sendBytes[5] = lowerNibbleCoding(RAM[0x86]);
            sendBytes[6] = upperNibbleCoding(RAM[6]);
            sendBytes[7] = lowerNibbleCoding(RAM[6]);
            sendBytes[8] = 13;
            try
            {

                Port.Open();
                Port.DiscardOutBuffer();
                Port.DiscardInBuffer();

                Port.Write(sendBytes, 0, 9);
                Port.DiscardOutBuffer();
                Port.DiscardInBuffer();
                for (int i = 0; i < 5; i++)
                {
                    receivedBytes[i] = (byte)(Port.ReadByte());
                }
                RAM[5] = DecodeByte(receivedBytes[0], receivedBytes[1]);
                RAM[6] = DecodeByte(receivedBytes[2], receivedBytes[3]);
            }
            catch
            {
            }

            Port.Close();
        }

        public byte[] returnRAM()
        {
            return RAM;
        }
        public void setRam(byte[] RamNeu)
        {
            RAM = RamNeu;
            //refresh();
        }

        private byte DecodeByte(byte higherNibble, byte lowerNibble)
        {
            higherNibble = (byte)(higherNibble & 15);
            higherNibble = (byte)(higherNibble << 4);
            lowerNibble = (byte)(lowerNibble & 15);
            return (byte)(lowerNibble | higherNibble);

        }
    }
}
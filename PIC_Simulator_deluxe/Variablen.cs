Variablen


alt  		 -    neu


namespace PIC_Simulator_deluxe = namespace PIC_Sim

class Stack	
    int pointer - int 
    int[] returnPCLs = int[] returnPCL

class Log  <- falls Variable, Datei-Stack
    msg			=	Msg
    Length		=	Length

class Programm = class Programmstart

class SerialPort festgelegt??, Datei COMCOM

class COMCOM
    private byte[] RAM = ram
    private SerialPort Port = port
    private byte lowerNibbleCoding(byte aByte) = lowerNibblel(byte Byte)
        byte returnByte = retByte
     private byte upperNibbleCoding(byte aByte) = upperNibble(byte Byte)
         byte returnByte = retByte
    private byte DecodeByte(byte higherNibble, byte lowerNibble) = Decode(byte h_Nibble, byte l_Nibble)
    public void refresh()
           byte[] sendBytes = pushBytes
           byte[] receivedBytes = pullBytes
     public void setRam(byte[] RamNeu) = setRam(byte[] newRam)
    public byte[] returnRAM() = returnRam()



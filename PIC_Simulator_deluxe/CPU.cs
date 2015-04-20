using System;
using System.Collections.Generic;
using System.Text;


namespace PIC_Simulator_deluxe
{
    class CPU 
    {

        #region Eigenschaften

        //register des PICs, nur intern, keine Properties
        public int PC;
        private byte W;
        public byte[] RAM;
        public int[] opCodes;
        private string[] codeLines;
        public int cycles;
        private Stack stack;
        public Log log = new Log();
        byte ramRAAlt = 0;
        byte ramRBAlt = 0;


        #endregion

        

        #region shortcut-Eigenschaften

        //Port A setzten oder zurückgeben
        public byte ramRA
        {
            get { return RAM[5]; }
            set { RAM[5] = value; }
        }

        //Port B setzten oder zurückgeben
        public byte ramRB
        {
            get { return RAM[6]; }
            set { RAM[6] = value; }
        }
        public byte ramOptions
        {
            get { return RAM[3]; }
            set { RAM[3] = value; }
        }

        private bool ramCarry
        {
            get { return getBit(ref RAM[3], 0); }
            set { setBit(ref RAM[3], 0, value); }
        }

        private bool ramCarryDigit
        {
            get { return getBit(ref RAM[3], 1); }
            set { setBit(ref RAM[3], 1, value); }
        }

        private bool ramZero
        {
            get { return getBit(ref RAM[3], 2); }
            set { setBit(ref RAM[3], 2, value); }
        }

        private bool ramPowerDown
        {
            get { return getBit(ref RAM[3], 3); }
            set { setBit(ref RAM[3], 0, value); }
        }

        private bool ramTimeOut
        {
            get { return getBit(ref RAM[3], 4); }
            set { setBit(ref RAM[3], 0, value); }
        }

        /// <summary>
        /// 0 = Watchdog | 1 = Timer0
        /// </summary>
        private bool ramPrescalerAssignment
        {
            get { return getBit(ref RAM[0x81], 3); }
            set { setBit(ref RAM[0x81], 3, value); }
        }

        private int ramPrescaler
        {
            get
            {
                int rate = (RAM[0x81] & (exponent(3) - 1));
                return exponent(rate + (ramPrescalerAssignment ? 0 : 1));
            }
            set
            {
                if (value >= 0 && value <= 7)
                    RAM[0x81] = (byte)((RAM[0x81] & exponent(3, 4, 5, 6, 7)) | value);
                else
                    throw new IndexOutOfRangeException("ramPrescaler wurde mit " + value + " beschrieben. Der Wert muss jedoch zwischen 0 und 7 liegen.");
            }
        }

        #endregion

        #region Konstruktor

        public CPU(string[] programmZeilen)
        {
            //Construktor legt Standartwerte fest
            PC = 0;
            W = 0;
            cycles = 0;

            stack = new Stack();
            RAM = new byte[512];
            
            codeLines = programmZeilen;
            opCodes = holeOpcodes(programmZeilen);
            
        }
        public CPU() 
        {
            PC = 0;
            W = 0;
            cycles = 0;
            stack = new Stack();
            RAM = new byte[512];
            
        }
        
        public void addOpcode(string[] programmZeilen)
        {
            codeLines = programmZeilen;
            opCodes = holeOpcodes(programmZeilen);
        }

        #endregion

        #region private Funktionen

        private void reset()
        {
            PC = 0;
            W = 0;
            cycles = 0;

            for (int i = 0; i < RAM.Length; i++)
                RAM[i] = 0;

            W = 12;
            RAM[24] = 1;

            //INIT
            //Bank 0
            RAM[0x0] = 0x0;                               // "INDF"
            RAM[0x1] = 0x0;                               // "TMR0" in truth: state unknown
            RAM[0x2] = 0x0;                               // "PCL"
            RAM[0x3] = 0x18;                              // "STATUS" in truth: 0001 1xxx 
            RAM[0x4] = 0x0;                                // "FSR" in truth: xxxx xxxx
            RAM[0x5] = 0x00;                               // "PORTA" in truth: 000x xxxx
            RAM[0x6] = 0x00;                               // "PORTB" in truth: xxxx xxxx
            RAM[0x7] = 0x0;                               // "Unimplemented location" in truth: 0001 1xxx 
            RAM[0x8] = 0x0;                               // "EEDATA" in truth: xxxx xxxx
            RAM[0x9] = 0x0;                               // "EEADR" in truth: xxxx xxxx
            RAM[0xA] = 0x0;                               // "PCLATH" in truth: 0001 1xxx 
            RAM[0xB] = 0x0;                               // "INTCON" in truth: 0000 000x

            // ProgrammRAM
            int ii;
            for (ii = 0xC; ii < 0x80; ii++)
                RAM[ii] = 0x0;
            //Bank 1
            RAM[0x80] = RAM[0x0];
            RAM[0x81] = 0xFF;                             // "OPTION_REG"
            RAM[0x82] = RAM[0x2];
            RAM[0x83] = RAM[0x3];
            RAM[0x84] = RAM[0x4];
            RAM[0x85] = 0x1F;                             // "TRISA"
            RAM[0x86] = 0xFF;                             // "TRISB"
            RAM[0x87] = RAM[0x7];
            RAM[0x88] = 0x0;                              // "EECON1"in truth: ---0 x000
            RAM[0x89] = 0x0;                              //"EECON2"
            for (ii = 0x8A; ii <= 0xFF; ii++)
                RAM[ii] = 0x0;
            stack.reset();
        }
        /// <summary>
        /// Holt sich die Opcodes aus einem StringArray
        /// </summary>
        /// <param name="programmZeilen"></param>
        /// <returns></returns>
        private int[] holeOpcodes(string[] programmZeilen)
        {
            int[] result = new int[0];
            List<intTupel> zeilen = new List<intTupel>();

            if (programmZeilen != null && programmZeilen.Length > 0)
            {
                int laufvar = programmZeilen.Length+1;
                    for (int i = 0; i < laufvar  ;i++)
                    {
                        if (programmZeilen[i + 1] == null)
                            laufvar = 0;
                        if (programmZeilen[i].Length > 8 && programmZeilen[i].Substring(4,1) == " ")
                        {
                            try
                            {
                                int tmp_PCL = int.Parse(programmZeilen[i].Substring(0, 4), System.Globalization.NumberStyles.AllowHexSpecifier);
                                int tmp_opCode = int.Parse(programmZeilen[i].Substring(5, 4), System.Globalization.NumberStyles.AllowHexSpecifier);
                                zeilen.Add(new intTupel(tmp_PCL, tmp_opCode));
                            }
                            catch { }
                        }
                    }
                
                result = new int[zeilen.Count];
                bool valid = true;
                foreach (intTupel i in zeilen)
                    if (i.PCL < 0 || i.PCL >= zeilen.Count)
                        valid = false;
                if (valid)
                {
                    for (int i = 0; i < result.Length; i++)
                        result[i] = -1;
                    foreach (intTupel i in zeilen)
                        result[i.PCL] = i.opCode;
                    foreach (int i in result)
                        if (i == -1)
                            valid = false;
                }
                if (!valid)
                    result = new int[0];
            }
            return result;
        }

        private bool executeOperation(int opCode)
        {

            //Befehle werden ausgeführt
            cycles++;
            
            if (opCode < 0)
            {
            }
            else if ((opCode & ((exponent(14)-1) - (exponent(5, 6)))) == 0)
            // NOP
            {
                log.msg(4, "NOP wurde ausgeführt.");
                cycles += 1;
            }
            else if ((opCode & ((exponent(14)-1) - (exponent(7)-1))) == exponent(8))
            // CLRW
            {
                W = 0;
                ramZero = true;
                log.msg(4, "CLRW wurde ausgeführt.");
                cycles += 1;
            }
            else if (opCode == exponent(2, 5, 6))
            // CLRWDT
            {
                if (!ramPrescalerAssignment)
                    //wenn der Prescaler auf Watchdog gesetzt ist
                    ramPrescaler = 0;
                ramTimeOut = true;
                ramPowerDown = true;
                cycles += 1;

                log.msg(4, "CLRWDT wurde ausgeführt. Der Prescaler ist auf " + (ramPrescalerAssignment?"Timer0":"Watchdog") + " gesetzt.");
            }
            else if (opCode == exponent(0, 3))
            // RETFIE
            {
                cycles++;
                PC = stack.pop();
                setBit(ref RAM[0x0B], 7, true);
                log.msg(4, "RETFIE wurde ausgeführt.");
                cycles += 2;
            }
            else if (opCode == exponent(3))
            // RETURN
            {
                cycles++;
                PC = stack.pop();
                log.msg(4, "RETURN wurde ausgeführt.");
                cycles += 2;
            }
            else if (opCode == exponent(0, 1, 5, 6))
            // SLEEP
            {
                if (!ramPrescalerAssignment)
                    //wenn der Prescaler auf Watchdog gesetzt ist
                    ramPrescaler = 0;
                ramTimeOut = true;
                ramPowerDown = false;
                cycles += 1;

                log.msg(4, "SLEEP wurde ausgeführt.");
            }
            else if ((opCode & (exponent(14) - exponent(11))) == exponent(13))
            // CALL
            {
                cycles++;
                stack.push(PC);
                PC = (opCode & (exponent(11) - 1));
                log.msg(4, "CALL wurde ausgeführt. Sprung nach: " + PC + ".");
                cycles += 2;
            }
            else if ((opCode & (exponent(14)- exponent(11))) == exponent(13, 11))
            // GOTO 
            {
                PC = (opCode & (exponent(11) - 1));
                log.msg(4, "GOTO wurde ausgeführt. Sprung nach: " + PC + ".");
                cycles += 2;
            }
            else if (opCode < exponent(12))
            #region byte-oriented file register operations
            {
                bool d = ((opCode & exponent(7)) == 128);
                byte f = (byte)(opCode & (exponent(7) - 1));
                if (f == 0)
                    f = RAM[4];

                if (((RAM[3] & 32) != 0) && (opCode & (exponent(14) - exponent(10))) == exponent(12) == false)
                    f += 0x80;

                if ((opCode & (exponent(14) - exponent(8))) == exponent(8, 9, 10))
                // ADDWF - Add W and f
                {
                    byte result = (byte)(RAM[f] + W);
                    ramCarry = (RAM[f] + W > 255);
                    ramCarryDigit = ((RAM[f] % 16) + (W % 16) > 15);
                    ramZero = (result == 0);
                    cycles += 1;

                    log.msg(4, "ADDWF wurde ausgeführt. f=" + f + " RAM[f]=" + RAM[f] + " W=" + W + " result(" + (d?"f":"W") + ")=" + result + " C=" + ramCarry + " CD=" + ramCarryDigit + " Z=" + ramZero + ".");

                    if (d)
                        RAM[f] = result;
                    else
                        W = result;
                }
                else if ((opCode & (exponent(14) - exponent(8))) == exponent(8, 10))
                // ANDWF - And W and f
                {
                    byte result = (byte)(RAM[f] & W);
                    ramZero = (result == 0);

                    log.msg(4, "ANDWF wurde ausgeführt. f=" + f + "RAM[f]=" + RAM[f] + " W=" + W
                        + " result = " + result + " Z=" + ramZero + ".");

                    if (d)
                        RAM[f] = result;
                    else
                        W = result;
                    cycles += 1;
                }
                else if ((opCode & (exponent(14) - exponent(8))) == exponent(8))
                // CLRF - Clear f
                {
                    RAM[f] = 0;
                    ramZero = true;
                    cycles += 1;
                    
                    log.msg(4, "CLRF wurde ausgeführt. f=" + f + "RAM[f]=" + RAM[f] + " Z=" + ramZero + ".");
                }
                else if ((opCode & (exponent(14) - exponent(8))) == exponent(8, 11))
                // COMF - Complement f
                {
                    byte result = (byte)~RAM[f];
                    ramZero = (result == 0);
                    cycles += 1;

                    log.msg(4, "COMF wurde ausgeführt. f=" + f + "RAM[f]=" + RAM[f] + " result=" + result + ".");
                }
                else if ((opCode & (exponent(14) - exponent(8))) == exponent(8, 9))
                // DECF - Decrement f
                {
                    byte result = (byte)(RAM[f]-1);
                    ramZero = (result == 0);
                    cycles += 1;

                    log.msg(4, "DECF wurde ausgeführt. f=" + f + " RAM[f]=" + RAM[f] + " d=" + d + " W=" + W
                        + " result = " + result + " Z=" + ramZero + ".");

                    if (d)
                        RAM[f] = result;
                    else
                        W = result;
                }
                else if ((opCode & (exponent(14) - exponent(8))) == exponent(8, 9, 11))
                // DECFSZ - Decrement f, skip if 0
                {
                    byte result = (byte)(RAM[f] - 1);
                    cycles += 2;
                   

                    log.msg(4, "DECFSZ wurde ausgeführt. f=" + f + " RAM[f]=" + RAM[f] + " d=" + d + " W=" + W
                        + " result = " + result + ".");

                    if (result == 0)
                    {
                        cycles++;
                        PC++;
                    }

                    if (d)
                        RAM[f] = result;
                    else
                        W = result;
                    
                }
                else if ((opCode & (exponent(14) - exponent(8))) == exponent(9, 11))
                // INCF - Increment f
                {
                    byte result = (byte)(RAM[f] + 1);
                    ramZero = (result == 0);
                    cycles += 1;

                    log.msg(4, "INCF wurde ausgeführt. f=" + f + " RAM[f]=" + RAM[f] + " d=" + d + " W=" + W
                        + " result = " + result + " Z=" + ramZero + ".");

                    if (d)
                        RAM[f] = result;
                    else
                        W = result;
                }
                else if ((opCode & (exponent(14) - exponent(8))) == exponent(8, 9, 10, 11))
                // INCFSZ - Increment f, skip if 0
                {
                    byte result = (byte)(RAM[f] + 1);


                    log.msg(4, "INCFSZ wurde ausgeführt. f=" + f + " RAM[f]=" + RAM[f] + " d=" + d + " W=" + W
                        + " result = " + result + ".");
                    cycles += 2;

                    if (result == 0)
                    {
                        cycles++;
                        PC++;
                    }

                    if (d)
                        RAM[f] = result;
                    else
                        W = result;
                    
                }
                else if ((opCode & (exponent(14) - exponent(8))) == exponent(10))
                // IORWF - Inclusive OR W with f
                {

                    byte result = (byte)(RAM[f] | W);
                    ramZero = (result == 0);
                    cycles += 1;

                    log.msg(4, "IORWF wurde ausgeführt. f=" + f + "RAM[f]=" + RAM[f] + " W=" + W
                        + " result = " + result + " Z=" + ramZero + ".");

                    if (d)
                        RAM[f] = result;
                    else
                        W = result;
                    
                }
                else if ((opCode & (exponent(14) - exponent(8))) == exponent(11))
                // MOVF - Move f
                {
                    cycles += 1;
                    ramZero = (RAM[f] == 0);
                    if (d)
                    {
                        RAM[f] = RAM[f];
                    }
                    else
                    {
                        W = RAM[f];
                    }
                    log.msg(4, "MOVF wurde ausgeführt. d= " + d + " W= " + W + " f= " + f + " RAM[f]= " + RAM[f]+ ".");
                }
                else if ((opCode & (exponent(14) - exponent(8))) == 0)
                // MOVWF - Move W to f
                {

                    RAM[f] = W;
                    cycles += 1;
                    log.msg(4, "MOVWF wurde ausgeführt. f= " + f + " RAM[f]= " + RAM[f] + " W= " + W + ".");
                }
                else if ((opCode & (exponent(14) - exponent(8))) == exponent(8, 10, 11))
                // RLF - Rotate left f through Carry
                {
                    byte result = (byte)((RAM[f] << 1) | (ramCarry?1:0));
                    ramCarry = getBit(ref RAM[f], 7);

                    log.msg(4, "RLF wurde ausgeführt. f= " + f + " RAM[f]= " + RAM[f] + " C= " + ramCarry + ".");
                    cycles += 1;
                    if (d)
                        RAM[f] = result;
                    else
                        W = result;
                }
                else if ((opCode & (exponent(14) - exponent(8))) == exponent(10, 11))
                // RRF - Rotate right f through Carry
                {
                    byte result = (byte)((RAM[f] >> 1) | (ramCarry ? exponent(7) : 0));
                    ramCarry = getBit(ref RAM[f], 0);
                    cycles += 1;
                    log.msg(4, "RRF wurde ausgeführt. f= " + f + " RAM[f]= " + RAM[f] + " C= " + ramCarry + ".");

                    if (d)
                        RAM[f] = result;
                    else
                        W = result;
                }
                else if ((opCode & (exponent(14) - exponent(8))) == exponent(9))
                // SUBWF - Subtract W from f
                {
                    cycles += 1;
                    byte result = (byte)(RAM[f] - W);
                    ramCarry = !(RAM[f] - W > 255);
                    ramCarryDigit = !((RAM[f] % 16) + (W % 16) > 15);
                    ramZero = (result == 0);

                    log.msg(4, "SUBWF wurde ausgeführt. f=" + f + " RAM[f]=" + RAM[f] + " W=" + W 
                        + " result(" + (d ? "f" : "W") + ")=" + result + " C=" + ramCarry 
                        + " CD=" + ramCarryDigit + " Z=" + ramZero + ".");

                    if (d)
                        RAM[f] = result;
                    else
                        W = result;
                    
                }
                else if ((opCode & (exponent(14) - exponent(8))) == exponent(9, 10, 11))
                // SWAPF - Swap nibbles in f
                {
                    cycles += 1;
                    byte result = (byte)((RAM[f] / 16) | ((RAM[f] & exponent(4)-1) * 16));

                    log.msg(4, "SWAPF wurde ausgeführt. f=" + f + "RAM[f]=" + RAM[f] + " result=" + result + ".");
                    
                    if (d)
                        RAM[f] = result;
                    else
                        W = result;
                }
                else if ((opCode & (exponent(14) - exponent(8))) == exponent(9, 10))
                // XORWF - Exclusive OR W with f
                {
                    cycles += 1;
                    byte result = (byte)(RAM[f] ^ W);
                    ramZero = (result == 0);

                    log.msg(4, "XORWF wurde ausgeführt. f=" + f + "RAM[f]=" + RAM[f]+ " W=" + W
                        + " result = " + result + " Z=" + ramZero + ".");

                    if (d)
                        RAM[f] = result;
                    else
                        W = result;
                    
                }
                else
                {
                    log.msg(7, "Befehl konnte nicht ausgeführt werden. opCode = " + string.Format("{0:X}", opCode) + ".");
                    return false;
                }
            }
            #endregion
            else if (opCode < exponent(13))
            #region bit-oriented file register operations
            {
                //byte b = (byte)(opCode & 1920);
                byte b = (byte)Math.Pow(2,(byte)(((opCode & 896) >> 7) & 7));
                byte f = (byte)(opCode & (exponent(7)-1));

                if (f == 0)
                    f = RAM[4];

                if (((RAM[3] & 32) != 0) && (opCode & (exponent(14) - exponent(10))) == exponent(12) == false)
                    f += 0x80;

                if ((opCode & (exponent(14) - exponent(10))) == exponent(12))
                // BCF - Bit Clear f
                {
                    setBit(ref RAM[f], (int)b, false);
                    log.msg(4, "BCF wurde ausgeführt.");
                    cycles += 1;
                }
                else if ((opCode & (exponent(14) - exponent(10))) == exponent(10, 12))
                // BSF - Bit set f
                {
                    
                    setBit(ref RAM[f], (int)b, true);
                    log.msg(4, "BSF wurde ausgeführt.");
                    cycles += 1;
                }
                else if ((opCode & (exponent(14) - exponent(10))) == exponent(11, 12))
                // BTFSC - Bit Test f, Skip if Clear
                {
                    if (!(getBit(ref RAM[f], (int)b)))
                    {
                        cycles++;
                        PC++;
                    }
                    log.msg(4, "BTFSC wurde ausgeführt.");
                    cycles += 2;
                }
                else if ((opCode & (exponent(14) - exponent(10))) == exponent(10, 11, 12))
                // BTFSS - Bit Test f, Skip if Set
                {
                    if (getBit(ref RAM[f], (int)b))
                    {
                        cycles++;
                        PC++;
                    }
                    log.msg(4, "BTFSS wurde ausgeführt.");
                    cycles += 2;
                }
                else
                {
                    log.msg(7, "Befehl konnte nicht ausgeführt werden. opCode = " + string.Format("{0:X}", opCode) + ".");
                    return false;
                }
            }
            #endregion
            else if (opCode < exponent(14))
            #region literal and control operations
            {
                byte k = (byte)(opCode & (exponent(8) - 1));

                if ((opCode & (exponent(14) - exponent(9))) == exponent(9, 10, 11, 12, 13))
                // ADDLW - Add literal and W
                {
                    byte result = (byte)(W+k);
                    ramCarry = (k + W > 255);
                    ramCarryDigit = ((k % 16) + (W % 16) > 15);
                    ramZero = (result == 0);
                    log.msg(4, "ADDLW wurde ausgeführt. k=" + k + " W=" + W 
                        + " result = " + result + " C=" + ramCarry + " CD=" + ramCarryDigit + " Z=" + ramZero + ".");
                    W = result;
                    cycles += 1;
                }
                else if ((opCode & (exponent(14) - exponent(8))) == exponent(8, 11, 12, 13))
                // ANDLW - And literal and W
                {
                    byte result = (byte)(W & k);
                    ramZero = (result == 0);
                    log.msg(4, "ANDLW wurde ausgeführt. k=" + k + " W=" + W
                        + " result = " + result + " Z=" + ramZero + ".");
                    W = result;
                    cycles += 1;
                }
                else if ((opCode & (exponent(14) - exponent(8))) == exponent(11, 12, 13))
                // IORLW - Inclusive OR literal with W
                {
                    byte result = (byte)(W | k);
                    ramZero = (result == 0);
                    log.msg(4, "IORLW wurde ausgeführt. k=" + k + " W=" + W
                        + " result = " + result + " Z=" + ramZero + ".");
                    W = result;
                    cycles += 1;
                    
                }
                else if ((opCode & (exponent(14) - exponent(10))) == exponent(12, 13))
                // MOVLW - Move literal to W
                {
                    W = k;
                    log.msg(4, "MOVLW wurde ausgeführt. W= " + W);
                    cycles += 1;
                }
                else if ((opCode & (exponent(14) - exponent(10))) == exponent(10, 12, 13))
                // RETLW - Return with literal in W
                {
                    cycles++;
                    W = (byte)(opCode & (exponent(7) - 1));
                    PC = stack.pop();
                    log.msg(4, "RETLW wurde ausgeführt.");
                }
                else if ((opCode & (exponent(14) - exponent(9))) == exponent(10, 11, 12, 13))
                // SUBLW - Subtract W from literal
                {

                    byte result = (byte)(W - k);
                    ramCarry = !(k - W > 255);
                    ramCarryDigit = !((k % 16) + (W % 16) > 15);
                    ramZero = (result == 0);
                    log.msg(4, "SUBLW wurde ausgeführt. k=" + k + " W=" + W
                        + " result = " + result + " C=" + ramCarry + " CD=" + ramCarryDigit + " Z=" + ramZero + ".");
                    W = result;
                    cycles += 1;
                   
                }
                else if ((opCode & (exponent(14) - exponent(8))) == exponent(9, 11, 12, 13))
                // XORLW - Exclusive OR literal with W
                {

                    byte result = (byte)(W ^ k);
                    ramZero = (result == 0);
                    log.msg(4, "XORLW wurde ausgeführt. k=" + k + " W=" + W
                        + " result = " + result + " Z=" + ramZero + ".");
                    W = result;
                    cycles += 1;
                }
                else
                {
                    log.msg(7, "Befehl konnte nicht ausgeführt werden. opCode = " + string.Format("{0:X}", opCode) + ".");
                    return false;
                }
            }
            #endregion
            else
            {
                log.msg(7, "Befehl konnte nicht ausgeführt werden. opCode = " + string.Format("{0:X}", opCode) + ".");
                return false;
            }
            return true;

        }

        public byte[] getRam()
        {
            //Gibts Ram zurück
            return RAM;
        }
        public Log getLog()
        {
            //Gibt die Beschreibung des Befehls zurück
            return log;
        }


        private bool interrupted()
        {
            return true;
        }

        public void setInterrupts(byte ra, byte rb)
        {
          
                cycles++;
        }

        private void callInterrupt()
        {
        }

        #endregion

        #region öffentliche Funktionen

        public void initCPU()
        {
            //CPU initialisieren über Reset
            reset();
        }


        public bool nextOperation()
        {
            //Ausführen des nächsten Befehls
            PC++;
            

            if (opCodes.Length > PC - 1)
            {
                
                executeOperation(opCodes[PC - 1]);
                return true;
            }
            else
            {
                return false;
            }
           
        }

        public int getPC()
        {
            //Gibt den ProgrammCounter zurück
            return this.PC;
        }
        public int[] getStack() 
        {
            //Gibt den Stapel zurück
            return stack.getStack();
        }
        public int getW()
        {
            //Gibt W zurück
            return W;
        }
        #endregion

        #region BitOperationen

        //Funktion zum setzen eines Bits
        public static void setBit(ref byte Byte, int position, bool value)
        {
            if (value)
                Byte = (byte)(Byte | position);
            else
                Byte = (byte)(Byte & (255 - position));
        }

        //Abrufen eines Bits
        public static bool getBit(ref byte Byte, int position)
        {
            return (Byte & position) > 0;
        }

        //Expotentialfunktion
        public static int exponent(params int[] exponent)
        {
            double result = 0;
            foreach (int i in exponent)
                result += Math.Pow(2, i);
            return (int)result;
        }

        #endregion

        struct intTupel
        {
            public int opCode;
            public int PCL;
            public intTupel(int PCL, int opCode)
            {
                this.opCode = opCode;
                this.PCL = PCL;
            }
        }

    }
}

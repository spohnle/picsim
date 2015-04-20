using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PIC_Simulator_deluxe
{
    /// <summary>
    /// 
    /// </summary>
    public  partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }
        //Definitionen
        public string ConsoleText;
        //Setzt ein Array mit der maximalen Länge an Zeilen
        public string[] alLines = new string[200];
        //Legt einen CPU an
        CPU p1 = new CPU();
        COMCOM Com;

        
        
        private void button1_Click(object sender, EventArgs e)
        {
            
            //Datei öffnen
            openFileDialog1.ShowDialog();
            string sFileName = openFileDialog1.FileName;

            if (sFileName != "openFileDialog1")  //openfileDialog1 muss noch angepasst werden
            {
                StreamReader sr = new StreamReader(sFileName, Encoding.GetEncoding("Windows-1252"));
                string sLine = null;


                //Alle Zeilen lesen
                int i = 0;
                while ((sLine = sr.ReadLine()) != null)
                {
                    alLines[i] = sLine;
                    i++;
                }

                //Streamreader schließen
                sr.Close();

                //Datagridview mit Daten füllen
                foreach (string line in alLines)
                {
                    string sLineOpCode;
                    string sLineComment;
                    try
                    {
                        sLineComment = line.Substring(20);
                        sLineOpCode = line.Substring(0, 9);
                        if (line.Substring(0, 8) == "        ")
                        {
                        }
                        else
                        {
                            dataGridView1.Rows.Add(false, sLineOpCode, sLineComment);
                        }

                    }
                    catch
                    {
                    }

                }
            }
        }
            
        public void generateDevice()
        {
            try
            {
                p1.addOpcode(alLines);
            }
            catch
            {
                if (alLines == null)
                {
                    MessageBox.Show("Sie haben keine Datei ausgewählt!", "Fehler");
                }
            }
            
            
        }
        
        
        public void refreshGui(byte[] Ram) 
        {

            //Ram anzeigen
            #region ramtabelle

            //Bank 1
            textBox69.Text = FormatHex(Convert.ToInt16(Ram[128]), 2);
            textBox68.Text = FormatHex(Convert.ToInt16(Ram[129]), 2);
            textBox67.Text = FormatHex(Convert.ToInt16(Ram[130]), 2);
            textBox66.Text = FormatHex(Convert.ToInt16(Ram[131]), 2);
            textBox65.Text = FormatHex(Convert.ToInt16(Ram[132]), 2);
            textBox64.Text = FormatHex(Convert.ToInt16(Ram[133]), 2);
            textBox63.Text = FormatHex(Convert.ToInt16(Ram[134]), 2);
            textBox62.Text = FormatHex(Convert.ToInt16(Ram[135]), 2);

            textBox61.Text = FormatHex(Convert.ToInt16(Ram[136]), 2);
            textBox60.Text = FormatHex(Convert.ToInt16(Ram[137]), 2);
            textBox59.Text = FormatHex(Convert.ToInt16(Ram[138]), 2);
            textBox58.Text = FormatHex(Convert.ToInt16(Ram[139]), 2);
            textBox57.Text = FormatHex(Convert.ToInt16(Ram[140]), 2);
            textBox56.Text = FormatHex(Convert.ToInt16(Ram[141]), 2);
            textBox55.Text = FormatHex(Convert.ToInt16(Ram[142]), 2);
            textBox54.Text = FormatHex(Convert.ToInt16(Ram[143]), 2);


            textBox53.Text = FormatHex(Convert.ToInt16(Ram[144]), 2);
            textBox52.Text = FormatHex(Convert.ToInt16(Ram[145]), 2);
            textBox51.Text = FormatHex(Convert.ToInt16(Ram[146]), 2);
            textBox50.Text = FormatHex(Convert.ToInt16(Ram[147]), 2);
            textBox49.Text = FormatHex(Convert.ToInt16(Ram[148]), 2);
            textBox48.Text = FormatHex(Convert.ToInt16(Ram[149]), 2);
            textBox47.Text = FormatHex(Convert.ToInt16(Ram[150]), 2);
            textBox46.Text = FormatHex(Convert.ToInt16(Ram[151]), 2);


            textBox45.Text = FormatHex(Convert.ToInt16(Ram[152]), 2);
            textBox44.Text = FormatHex(Convert.ToInt16(Ram[153]), 2);
            textBox43.Text = FormatHex(Convert.ToInt16(Ram[154]), 2);
            textBox42.Text = FormatHex(Convert.ToInt16(Ram[155]), 2);
            textBox41.Text = FormatHex(Convert.ToInt16(Ram[156]), 2);
            textBox40.Text = FormatHex(Convert.ToInt16(Ram[157]), 2);
            textBox39.Text = FormatHex(Convert.ToInt16(Ram[158]), 2);
            textBox38.Text = FormatHex(Convert.ToInt16(Ram[159]), 2);



            //Bank 0
            textBox36.Text = FormatHex(Convert.ToInt16(Ram[24]), 2);
            textBox35.Text = FormatHex(Convert.ToInt16(Ram[25]), 2);
            textBox34.Text = FormatHex(Convert.ToInt16(Ram[26]), 2);
            textBox33.Text = FormatHex(Convert.ToInt16(Ram[27]), 2);
            textBox32.Text = FormatHex(Convert.ToInt16(Ram[28]), 2);
            textBox29.Text = FormatHex(Convert.ToInt16(Ram[29]), 2);
            textBox28.Text = FormatHex(Convert.ToInt16(Ram[30]), 2);
            textBox27.Text = FormatHex(Convert.ToInt16(Ram[31]), 2);

            textBox26.Text = FormatHex(Convert.ToInt16(Ram[16]), 2);
            textBox25.Text = FormatHex(Convert.ToInt16(Ram[17]), 2);
            textBox24.Text = FormatHex(Convert.ToInt16(Ram[18]), 2);
            textBox23.Text = FormatHex(Convert.ToInt16(Ram[19]), 2);
            textBox22.Text = FormatHex(Convert.ToInt16(Ram[20]), 2);
            textBox21.Text = FormatHex(Convert.ToInt16(Ram[21]), 2);
            textBox20.Text = FormatHex(Convert.ToInt16(Ram[22]), 2);
            textBox19.Text = FormatHex(Convert.ToInt16(Ram[23]), 2);


            textBox16.Text = FormatHex(Convert.ToInt16(Ram[8]), 2);
            textBox15.Text = FormatHex(Convert.ToInt16(Ram[9]), 2);
            textBox14.Text = FormatHex(Convert.ToInt16(Ram[10]), 2);
            textBox13.Text = FormatHex(Convert.ToInt16(Ram[11]), 2);
            textBox12.Text = FormatHex(Convert.ToInt16(Ram[12]), 2);
            textBox11.Text = FormatHex(Convert.ToInt16(Ram[13]), 2);
            textBox10.Text = FormatHex(Convert.ToInt16(Ram[14]), 2);
            textBox9.Text = FormatHex(Convert.ToInt16(Ram[15]), 2);


            textBox37.Text = FormatHex(Convert.ToInt16(Ram[7]), 2);
            textBoxRA1.Text = FormatHex(Convert.ToInt16(Ram[6]), 2);
            textBoxRA2.Text = FormatHex(Convert.ToInt16(Ram[5]), 2);
            textBoxRA3.Text = FormatHex(Convert.ToInt16(Ram[4]), 2);
            textBoxRA4.Text = FormatHex(Convert.ToInt16(Ram[3]), 2);
            textBoxRA5.Text = FormatHex(Convert.ToInt16(Ram[2]), 2);
            textBox31.Text = FormatHex(Convert.ToInt16(Ram[1]), 2);
            textBox30.Text = FormatHex(Convert.ToInt16(Ram[0]), 2);
            #endregion

            //Port A anzeigen
            #region PortA anzeigen
            if ((p1.ramRA & 1) != 0)
                btn_PortA_0.Text = "1";
            else
                btn_PortA_0.Text = "0";

            if ((p1.ramRA & 2) != 0)
                btn_PortA_1.Text = "1";
            else
                btn_PortA_1.Text = "0";

            if ((p1.ramRA & 4) != 0)
                btn_PortA_2.Text = "1";
            else
                btn_PortA_2.Text = "0";

            if ((p1.ramRA & 8) != 0)
                btn_PortA_3.Text = "1";
            else
                btn_PortA_3.Text = "0";

            if ((p1.ramRA & 16) != 0)
                btn_PortA_4.Text = "1";
            else
                btn_PortA_4.Text = "0";

            btn_PortA_5.Text = "--";
            btn_PortA_6.Text = "--";
            btn_PortA_7.Text = "--";
            #endregion

            //PortA Direction anzeigen
            #region PortA Directions
            byte[] buffer = p1.getRam();
            if ((buffer[133] & 1) != 0)
            {
                lbl_PortA_0.Text = "IN";
            }
            else
            {
                lbl_PortA_0.Text = "OUT";
            }
            
            if ((Ram[133] & 2) != 0)
            {
                lbl_PortA_1.Text = "IN";
            }
            else
            {
                lbl_PortA_1.Text = "OUT";
            }
            
            if ((Ram[133] & 4) != 0)
            {
                lbl_PortA_2.Text = "IN";
            }
            else
            {
                lbl_PortA_2.Text = "OUT";
            }
            
            if ((Ram[133] & 8) != 0)
            {
                lbl_PortA_3.Text = "IN";
            }
            else
            {
                lbl_PortA_3.Text = "OUT";
            }
            
            if ((Ram[133] & 16) != 0)
            {
                lbl_PortA_4.Text = "IN";
            }
            else
            {
                lbl_PortA_4.Text = "OUT";
            }
            #endregion

            //PortB Direction anzeigen
            #region PortB Direction
            if ((Ram[134] & 1) != 0)
            {
                lbl_PortB_0.Text = "IN";
            }
            else
            {
                lbl_PortB_0.Text = "OUT";
            }

            if ((Ram[134] & 2) != 0)
            {
                lbl_PortB_1.Text = "IN";
            }
            else
            {
                lbl_PortB_1.Text = "OUT";
            }

            if ((Ram[134] & 4) != 0)
            {
                lbl_PortB_2.Text = "IN";
            }
            else
            {
                lbl_PortB_2.Text = "OUT";
            }
            if ((Ram[134] & 8) != 0)
            {
                lbl_PortB_3.Text = "IN";
            }
            else
            {
                lbl_PortB_3.Text = "OUT";
            }
            if ((Ram[134] & 16) != 0)
            {
                lbl_PortB_4.Text = "IN";
            }
            else
            {
                lbl_PortB_4.Text = "OUT";
            }
            if ((Ram[134] & 32) != 0)
            {
                lbl_PortB_5.Text = "IN";
            }
            else
            {
                lbl_PortB_5.Text = "OUT";
            }
            if ((Ram[134] & 64) != 0)
            {
                lbl_PortB_6.Text = "IN";
            }
            else
            {
                lbl_PortB_6.Text = "OUT";
            }
            if ((Ram[134] & 128) != 0)
            {
                lbl_PortB_7.Text = "IN";
            }
            else
            {
                lbl_PortB_7.Text = "OUT";
            }
#endregion

            //Port B anzeigen
            #region PortB anzeigen
            if ((p1.ramRB & 1) != 0)
                btn_PortB_0.Text = "1";
            else
                btn_PortB_0.Text = "0";

            if ((p1.ramRB & 2) != 0)
                btn_PortB_1.Text = "1";
            else
                btn_PortB_1.Text = "0";
            if ((p1.ramRB & 4) != 0)
                btn_PortB_2.Text = "1";
            else
                btn_PortB_2.Text = "0";
            if ((p1.ramRB & 8) != 0)
                btn_PortB_3.Text = "1";
            else
                btn_PortB_3.Text = "0";
            if ((p1.ramRB & 16) != 0)
                btn_PortB_4.Text = "1";
            else
                btn_PortB_4.Text = "0";
            if ((p1.ramRB & 32) != 0)
                btn_PortB_5.Text = "1";
            else
                btn_PortB_5.Text = "0";
            if ((p1.ramRB & 64) != 0)
                btn_PortB_6.Text = "1";
            else
                btn_PortB_6.Text = "0";
            if ((p1.ramRB & 128) != 0)
                btn_PortB_7.Text = "1";
            else
                btn_PortB_7.Text = "0";

            #endregion

            //W-Register Anzeigen
            textBox17.Text = p1.getW().ToString();
            textBox17.Text = FormatHex(Convert.ToInt16(p1.getW()), 2);

            //PC -Anzeigen
            //textBox18.Text = p1.getPC().ToString();
            textBox18.Text = FormatHex(Convert.ToInt16(p1.getPC()), 2);

            //Pic Speed anzeigen
            textBox102.Text = timer1.Interval.ToString();

            //PC schreiben
            Ram[02] = (byte)p1.PC;

            //COM RESET
            //if ((Ram[5] & 32)  == 0)
            //    p1.initCPU();

            //Options anzeigen
            #region Options
            textBox1.Text = ((p1.ramOptions & 1)>>0).ToString();
            textBox2.Text = ((p1.ramOptions & 2) >>1).ToString();
            textBox3.Text = ((p1.ramOptions & 4)>>2).ToString();
            textBox4.Text = ((p1.ramOptions & 8)>>3).ToString();
            textBox5.Text = ((p1.ramOptions & 16)>>4).ToString();
            textBox6.Text = ((p1.ramOptions & 32)>>5).ToString();
            textBox7.Text = ((p1.ramOptions & 64)>>6).ToString();
            textBox8.Text = ((p1.ramOptions & 128)>>7).ToString();
            
            #endregion
            //Stack anzeigen
            #region Stack anzeigen
            int[] iStack = p1.getStack();
            tb_Stack_0.Text = FormatHex(iStack[0], 2);
            tb_Stack_1.Text = FormatHex(iStack[1], 2);
            tb_Stack_2.Text = FormatHex(iStack[2], 2);
            tb_Stack_3.Text = FormatHex(iStack[3], 2);
            tb_Stack_4.Text = FormatHex(iStack[4], 2);
            tb_Stack_5.Text = FormatHex(iStack[5], 2);
            tb_Stack_6.Text = FormatHex(iStack[6], 2);
            tb_Stack_7.Text = FormatHex(iStack[7], 2);
            #endregion

            //Cycles
            textBox70.Text = p1.cycles.ToString();

            //WatchDog
            if (checkBox1.Checked)
                if (p1.getPC() +1 == p1.opCodes.Length)
                {
                    for (int i = 65536; i > 0; i--)
                    {
                        //p1.nextOperation();
                    }
                    timer1.Stop();
                    generateDevice();
                    p1.initCPU();
                    timer1.Start();

                }
        }



        private void label1_Click(object sender, EventArgs e)
        {
        }
        /// <summary>
        /// Wandelt Dezimal in Hexadezimal um
        /// </summary>
        /// <param name="decimalNumber"></param>
        /// <param name="hexDigits"></param>
        /// <returns></returns>
        public static string FormatHex(int decimalNumber, byte hexDigits)
        {
            string hex = decimalNumber.ToString("X");

            if (hex.Length > hexDigits)
                return hex;

            hex = new String('0', hexDigits - hex.Length) + hex;

            return  hex;
        }
        /// <summary>
        /// Do Step Einzelsteps durhc das Programm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_doStep_Click(object sender, EventArgs e)
        {
            if (breakPoint(p1.getPC()) == false)
            {

                try
                {
                    //Nächste Operation wird ausgeführt(CPU aufgerufen)
                    p1.nextOperation();
                    //Erneuert die OberflächenAusgabe
                    refreshGui(p1.getRam());
                    //Selektiert in der Tabelle den aktuellen Befehl
                    dataGridView1.Rows[p1.getPC()].Selected = true;
                    //Ruft Die ScrollFocus Methode auf
                    ScrollFucus();
                }
                catch
                {
                    if (alLines == null)
                    {
                        MessageBox.Show("Sie haben keine Datei ausgewählt oder die Datei ist beschädigt !", "Fehler !");
                    }
                }
            }

            
        }
        public void ScrollFucus()
        {
            //Scrollt das Bild alle 4 Befehle auf den Aktuellen PC
            if(p1.getPC() % 4 == 0)
            dataGridView1.FirstDisplayedScrollingRowIndex = p1.getPC()-2;
        }
       

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void button_RunSimulation_Click(object sender, EventArgs e)
        {
            //Automatischer Programmdurchlauf startet Timer
            //Bei Breakpoint wird der Autodurchlauf gestopt
            if (breakPoint(p1.getPC()) == true)
            {
                timer1.Stop();
            }
            else
            {
                timer1.Start();
            }
            
        }
        public bool breakPoint(int ProgrammZeile)
        {
            //Value ist Objekt daher umwandeln in String
            //Fragt ab ob der BreakPoint gesetzt ist und pausiert das Programm
            //Gibt True zurück Falls Breakpoint gesetzt
            try
            {
                if (dataGridView1.Rows[ProgrammZeile].Cells["cbBreakpoint"].Value.ToString() == "True")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                p1.log.msg(0,"Programm kann nicht pausiert werden");
                return false;
            }
        }
        private void button_Reset_Click(object sender, EventArgs e)
        {
            //Setzt alle Buttons auf den Anfangszustand
            p1.PC = 0;
            timer1.Stop();
            p1.initCPU();
            refreshGui(p1.getRam());
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About ba = new About();
            ba.Show();
        }
        /// <summary>
        /// Anzeige des Ports B jedes einzelne Bit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region PortBAusgabe
        private void btn_PortB_0_Click(object sender, EventArgs e)
        {
            if (btn_PortB_0.Text == "0")
            {
                btn_PortB_0.Text = "1";  
                p1.ramRB = (byte)(p1.ramRB | 1);
            }
            else
            {
                btn_PortB_0.Text = "0";
                p1.ramRB = (byte)(p1.ramRB & 254);
            }
            refreshGui(p1.RAM);
        }
        private void btn_PortB_1_Click(object sender, EventArgs e)
        {
            if (btn_PortB_1.Text == "0")
            {
                btn_PortB_1.Text = "1";
                p1.ramRB = (byte)(p1.ramRB | 2);
            }
            else
            {
                btn_PortB_1.Text = "0";
                p1.ramRB = (byte)(p1.ramRB & (255 - 2));
            }
            refreshGui(p1.getRam());
        }
        private void btn_PortB_2_Click(object sender, EventArgs e)
        {
            if (btn_PortB_2.Text == "0")
            {
                btn_PortB_2.Text = "1";
                p1.ramRB = (byte)(p1.ramRB | 4);
            }
            else
            {
                btn_PortB_2.Text = "0";
                p1.ramRB = (byte)(p1.ramRB & (255 - 4));
            }
            refreshGui(p1.RAM);
        }
        private void btn_PortB_3_Click(object sender, EventArgs e)
        {
            if (btn_PortB_3.Text == "0")
            {
                btn_PortB_3.Text = "1";
                p1.ramRB = (byte)(p1.ramRB | 8);
            }
            else
            {
                btn_PortB_3.Text = "0";
                p1.ramRB = (byte)(p1.ramRB & (255 - 8));
            }
            refreshGui(p1.getRam());
        }
        private void btn_PortB_4_Click(object sender, EventArgs e)
        {
            if (btn_PortB_4.Text == "0")
            {
                btn_PortB_4.Text = "1";
                p1.ramRB = (byte)(p1.ramRB | 16);
            }
            else
            {
                btn_PortB_4.Text = "0";
                p1.ramRB = (byte)(p1.ramRB & (255 - 16));
            }
            refreshGui(p1.RAM);
        }
        private void btn_PortB_5_Click(object sender, EventArgs e)
        {
            if (btn_PortB_5.Text == "0")
            {
                btn_PortB_5.Text = "1";
                p1.ramRB = (byte)(p1.ramRB | 32);
            }
            else
            {
                btn_PortB_5.Text = "0";
                p1.ramRB = (byte)(p1.ramRB & (255 - 32));
            }
            refreshGui(p1.getRam());
        }
        private void btn_PortB_6_Click(object sender, EventArgs e)
        {
            if (btn_PortB_6.Text == "0")
            {
                btn_PortB_6.Text = "1";
                p1.ramRB = (byte)(p1.ramRB | 64);
            }
            else
            {
                btn_PortB_6.Text = "0";
                p1.ramRB = (byte)(p1.ramRB & (255 - 64));
            }
            refreshGui(p1.getRam());
        }
        private void btn_PortB_7_Click(object sender, EventArgs e)
        {
            if (btn_PortB_7.Text == "0")
            {
                btn_PortB_7.Text = "1";
                p1.ramRB = (byte)(p1.ramRB | 128);
            }
            else
            {
                btn_PortB_7.Text = "0";
                p1.ramRB = (byte)(p1.ramRB & (255 - 128));
            }
            refreshGui(p1.getRam());
        }
        #endregion
        /// <summary>
        /// Anzeige der Ports A jedes einzelne Bit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region PortAAusgabe
        private void btn_PortA_4_Click(object sender, EventArgs e)
        {
            if (btn_PortA_4.Text == "0")
            {
                btn_PortA_4.Text = "1";
                p1.ramRA = (byte)(p1.ramRA | 16);
            }
            else
            {
                btn_PortA_4.Text = "0";
                p1.ramRA = (byte)(p1.ramRA & (255 - 16));
            }
            refreshGui(p1.getRam());
        }
        private void btn_PortA_3_Click(object sender, EventArgs e)
        {
            if (btn_PortA_3.Text == "0")
            {
                btn_PortA_3.Text = "1";
                p1.ramRA = (byte)(p1.ramRA | 8);
            }
            else
            {
                btn_PortA_3.Text = "0";
                p1.ramRA = (byte)(p1.ramRA & (255 - 8));
            }
            refreshGui(p1.getRam());
        }
        private void btn_PortA_2_Click(object sender, EventArgs e)
        {
            if (btn_PortA_2.Text == "0")
            {
                btn_PortA_2.Text = "1";
                p1.ramRA = (byte)(p1.ramRA | 4);
            }
            else
            {
                btn_PortA_2.Text = "0";
                p1.ramRA = (byte)(p1.ramRA & (255 - 4));
            }
            refreshGui(p1.getRam());
        }
        private void btn_PortA_1_Click(object sender, EventArgs e)
        {
            if (btn_PortA_1.Text == "0")
            {
                btn_PortA_1.Text = "1";
                p1.ramRA = (byte)(p1.ramRA | 2);
            }
            else
            {
                btn_PortA_1.Text = "0";
                p1.ramRA = (byte)(p1.ramRA & (255 - 2));
            }
            refreshGui(p1.getRam());
        }
        private void btn_PortA_0_Click(object sender, EventArgs e)
        {
            if (btn_PortA_0.Text == "0")
            {
                btn_PortA_0.Text = "1";
                p1.ramRA = (byte)(p1.ramRA | 1);
            }
            else
            {
                btn_PortA_0.Text = "0";
                p1.ramRA = (byte)(p1.ramRA &(255 - 1));
            }
            refreshGui(p1.getRam());
        }
        #endregion
        

        private void button2_Click(object sender, EventArgs e)
        {
            //INIT
            generateDevice();

            p1.initCPU();

            if (checkBox2.Checked)
                Com = new COMCOM(p1.getRam());

            refreshGui(p1.getRam());

        }
        /// <summary>
        /// Timer der den Automatischen Programmdurchlauf steuert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {

            if (breakPoint(p1.getPC()) == true)
            {
                timer1.Stop();
            }
            else
            {
                try
                {

                    //Nächste Operation wird ausgeführt(CPU aufgerufen)
                    
                    p1.nextOperation();

                    
                    //Erneuert die OberflächenAusgabe
                    refreshGui(p1.getRam());
                    if (checkBox2.Checked)
                    {
                        Com.refresh();
                        p1.RAM = Com.returnRAM();
                        btn_PortB_0.Enabled = false;
                        btn_PortB_1.Enabled = false;
                        btn_PortB_2.Enabled = false;
                        btn_PortB_3.Enabled = false;
                        btn_PortB_4.Enabled = false;
                        btn_PortB_5.Enabled = false;
                        btn_PortB_6.Enabled = false;
                        btn_PortB_7.Enabled = false;
                        btn_PortA_0.Enabled = false;
                        btn_PortA_1.Enabled = false;
                        btn_PortA_2.Enabled = false;
                        btn_PortA_3.Enabled = false;
                        btn_PortA_4.Enabled = false;

                    }
                    else
                    {
                        btn_PortB_0.Enabled = true;
                        btn_PortB_1.Enabled = true;
                        btn_PortB_2.Enabled = true;
                        btn_PortB_3.Enabled = true;
                        btn_PortB_4.Enabled = true;
                        btn_PortB_5.Enabled = true;
                        btn_PortB_6.Enabled = true;
                        btn_PortB_7.Enabled = true;
                        btn_PortA_0.Enabled = true;
                        btn_PortA_1.Enabled = true;
                        btn_PortA_2.Enabled = true;
                        btn_PortA_3.Enabled = true;
                        btn_PortA_4.Enabled = true;
                    }
                    //Sekeltiert in der Tabelle den Aktuellen Befehl
                    dataGridView1.Rows[p1.getPC()].Selected = true;
                    //Ruft Die ScrollFucus Methode auf
                    ScrollFucus();
                }
                catch
                {   if(alLines == null)
                    MessageBox.Show( "Sie haben keine Datei ausgewählt oder die Datei ist beschädigt !","Fehler !");
                }
            }

        }

        private void groupBox6_Enter(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (p1.log.Visible != true)
                try
                {
                    p1.log.Show();
                }

            catch
            {
                p1.log.msg(0,"Log kann nicht geöffnet werden");
            }
        }

        private void textBox102_TextChanged(object sender, EventArgs e)
        {
            //Pic Speed Anzeige
            textBox102.Text = timer1.Interval.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //Pic Speed erhöhen
            timer1.Interval = timer1.Interval + 20;
            refreshGui(p1.getRam());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //Pic Speed erniedrigen
            if (timer1.Interval >= 25)
            {
                timer1.Interval = timer1.Interval - 20;
                refreshGui(p1.getRam());
            }
        }

        private void groupBox6_Enter_1(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            //STOP Button
            timer1.Stop();
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            

        }

        private void timer2_Tick(object sender, EventArgs e){
       
        }

        
    }

        
}
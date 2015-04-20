using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PIC_Sim
{
	static class Programmstart
	{
        //Einstiegspunkt der Anwendung
        [STAThread]
        static void main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PIC_Simulator_deluxe
{
    public partial class Log : Form
    {
        public Log()
        {
            InitializeComponent();
        }


        public void msg(int i, string message)
        {
            textBox1.Text = "Befehl: " + i + " " + message;
            textBox1.Update();
        }

        private void Log_Load(object sender, EventArgs e)
        {

        }
    }
}
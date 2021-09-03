using System;
using System.Windows.Forms;

namespace StayALiVE
{
    public partial class Ui : Form
    {
        internal static Ui GetInstance() { return new Ui(); }
        public Ui() => InitializeComponent();
        private void metroToggle1_CheckedChanged(object sender, EventArgs e)
        { 
        
        }
    }
}

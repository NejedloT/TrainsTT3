using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;

namespace TestDesignTT
{
    public partial class FormMainMenu : Form
    {
        public FormMainMenu()
        {
            InitializeComponent();
        }

        //stisknuti tlacitka pro zobrazeni aplikace na rizeni kolejiste pomoci jizdniho radu
        private void btnTimetable_Click(object sender, EventArgs e)
        {
            FormTimetable formtt = new FormTimetable();
            formtt.StartPosition = FormStartPosition.Manual;
            formtt.Location = this.Location;
            formtt.Size = this.Size;
            formtt.Show();
            this.Hide();
        }

        //stisknuti tlacitka pro zobrazeni aplikace na manualni rizeni kolejiste
        private void btnManual_Click(object sender, EventArgs e)
        {
            FormManual formman = new FormManual();
            formman.StartPosition = FormStartPosition.Manual;
            formman.Location = this.Location;
            formman.Size = this.Size;
            formman.Show();
            this.Hide();
        }

        //stisknuti tlacitka pro plne manualni rizeni kolejiste
        private void btnDebug_Click(object sender, EventArgs e)
        {
            FormDebug formdeb = new FormDebug();
            formdeb.StartPosition = FormStartPosition.Manual;
            formdeb.Location = this.Location;
            formdeb.Size = this.Size;
            formdeb.Show();
            this.Hide();
        }        
    }
}

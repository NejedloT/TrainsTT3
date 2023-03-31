using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestDesignTT
{
    public partial class UCHome : UserControl
    {
        /// <summary>
        /// Definice User Controlu domovske obrazovky
        /// </summary>
        public UCHome()
        {
            InitializeComponent();
            UCHome_Load();
        }

        private void UCHome_Load()
        {
            timer1.Start();
            labelTime.Text = DateTime.Now.ToLongTimeString();
            labelDate.Text = DateTime.Now.ToLongDateString();
        }

        /// <summary>
        /// Event handler pro aktualizaci casu
        /// </summary>
        /// <param name="sender">Event Handler na kazdou vterinu</param>
        /// <param name="e">Event Handler na kazdou vterinu</param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            labelTime.Text = DateTime.Now.ToLongTimeString();
        }
    }
}

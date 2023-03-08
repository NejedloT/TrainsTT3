using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;


namespace TestDesignTT
{
    public partial class UCDataLoad : UserControl
    {
        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler ButtonLoadClick;


        bool firstClick = true;
        //private static UCDataLoad? _instance;
        private string myfileName = String.Empty;

        //public string fileName { set; get; }

        
        
        public string fileName
        {
            set { myfileName = value; }
            get { return myfileName; }
        }
        
        
        

        /*
        public static UCDataLoad Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UCDataLoad();
                return _instance;
            }
        }

        */

        public UCDataLoad()
        {
            InitializeComponent();
            //textBoxPath.ForeColor = SystemColors.GrayText;
        }


        private void textBoxPath_Enter(object sender, EventArgs e)
        {
            if (textBoxPath.Text == "Please enter fath to you CSV file...")
            {
                textBoxPath.Text = string.Empty;
                firstClick = false;
            }
            textBoxPath.ForeColor = SystemColors.ControlText;
        }

        private void textBoxPath_Leave(object sender, EventArgs e)
        {
            if (textBoxPath.Text.Length == 0)
            {
                textBoxPath.Text = "Please enter fath to you CSV file...";
                textBoxPath.ForeColor = SystemColors.ScrollBar;
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Soubory dat (*.csv)|*.csv|Vsechny|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show(openFileDialog.FileName);
                    //myfileName = openFileDialog.FileName;
                    //this.fileName = myfileName;
                    this.fileName = openFileDialog.FileName;
                    labelFileName.Text = fileName;
                    ButtonLoadClick?.Invoke(this, e);
                }
                else
                    return;
            }
        }
    }
}

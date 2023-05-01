using ControlLogic;
using Microsoft.Win32;
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
using TrainTTLibrary;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace TestDesignTT
{
    public partial class UCDataLoad : UserControl
    {
        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler ButtonLoadClick;

        //private static UCDataLoad? _instance;
        //private string myfileName = String.Empty;

        public List<DataToLoad> dataToLoads = new List<DataToLoad>();

        //public string fileName { set; get; }
        

        public UCDataLoad()
        {
            InitializeComponent();
            CheckEnabled();
            //textBoxPath.ForeColor = SystemColors.GrayText;
        }

        public void CheckEnabled()
        {
            if (!rbInfinity.Checked && !rbPauses.Checked)
            {
                btnLoadDataPath.Enabled = false;
                btnLoadDataPickFile.Enabled = false;
            }
            else
            {
                //btnLoadData.Enabled = true;
                btnLoadDataPickFile.Enabled = true;
                if (textBoxPath.Text == "Please enter fath to you CSV file..." || textBoxPath.TextLength == 0)
                {
                    //btn_LoadData.Enabled = false;
                    btnLoadDataPath.Enabled = false;
                }
                else
                {
                    //btn_LoadData.Enabled = true;
                    btnLoadDataPath.Enabled = true;
                }
                
            }

            if (rbInfinity.Checked)
            {
                rbPauses.Checked = false;
            }
            if (rbPauses.Checked) { 
                rbInfinity.Checked = false;
            }
        }

        private void textBoxPath_Enter(object sender, EventArgs e)
        {
            if (textBoxPath.Text == "Please enter fath to you CSV file...")
            {
                textBoxPath.Text = string.Empty;
            }
            textBoxPath.ForeColor = SystemColors.ControlText;
        }

        private void textBoxPath_Leave(object sender, EventArgs e)
        {
            if (textBoxPath.Text.Length == 0)
            {
                textBoxPath.Text = "Please enter fath to you CSV file...";
                textBoxPath.ForeColor = SystemColors.ScrollBar;
                btnLoadDataPath.Enabled = false;
            }
        }

        /*
        private void btn_LoadData_Click(object sender, EventArgs e)
        {
            
        }
        */

        /*
        private void btnLoadData_Click(object sender, EventArgs e)
        {
            
        }
        */

        private void rbPauses_CheckedChanged(object sender, EventArgs e)
        {
            CheckEnabled();
        }

        private void rbInfinity_CheckedChanged(object sender, EventArgs e)
        {
            CheckEnabled();
        }

        private void btnLoadDataPath_Click(object sender, EventArgs e)
        {
            string filePath = textBoxPath.Text;
            if (File.Exists(filePath))
            {
                if (Path.GetExtension(filePath).Equals(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    // File exists and is a CSV file
                    // Put your code here

                    MessageBox.Show(filePath);

                    bool bb = rbInfinity.Checked;

                    labelFileName.Text = filePath;

                    dataToLoads.Add(new DataToLoad { Filename = filePath, InfinityData = bb });
                    ButtonLoadClick?.Invoke(this, e);
                }
                else
                {
                    MessageBox.Show("The selected file is not a CSV file.");
                }
            }
            else
            {
                MessageBox.Show("The selected file does not exist.");
            }
        }

        private void btn_LoadDataPickFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Soubory dat (*.csv)|*.csv|Vsechny|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show(openFileDialog.FileName);
                    //myfileName = openFileDialog.FileName;
                    //this.fileName = myfileName;

                    /*
                    this.fileName = openFileDialog.FileName;
                    labelFileName.Text = fileName;
                    */


                    bool bb = rbInfinity.Checked;

                    string fileName = openFileDialog.FileName;
                    labelFileName.Text = fileName;

                    dataToLoads.Add(new DataToLoad { Filename = fileName, InfinityData = bb });
                    ButtonLoadClick?.Invoke(this, e);
                }
                else
                    return;
            }
        }

        private void textBoxPath_TextChanged(object sender, EventArgs e)
        {
            CheckEnabled();
        }
    }

    public class DataToLoad
    {
        public string Filename;

        public bool InfinityData;
    }
}

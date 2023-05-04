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
        //Event Handler pro vyvolani toho, ze byl nacten jizdni rad
        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler ButtonLoadClick;

        //List informaci k nacteni dat - nazev a jak se ma vytvorit jizdni rad (jestli na prestavky nebo neustaly provoz)
        public List<DataToLoad> dataToLoads = new List<DataToLoad>();

        

        public UCDataLoad()
        {
            InitializeComponent();
            CheckEnabled();
        }

        /// <summary>
        /// MEtoda, ktera testuje povoleni jednotlivych buttonu a textfieldu. Za urcitych podminek dojde k jejich povoleni, jinak je nebude mozne stisknout
        /// </summary>
        public void CheckEnabled()
        {
            //neni zvoleno, jak se ma npracovat s jizdnim radem, nelze tedy nacist jizdni rad
            if (!rbInfinity.Checked && !rbPauses.Checked)
            {
                btnLoadDataPath.Enabled = false;
                btnLoadDataPickFile.Enabled = false;
            }
            else
            {
                //vybran mode na prestavky/bez preruseni, povol tlacitko na nahrani souboru
                btnLoadDataPickFile.Enabled = true;

                //tlacitko pro nahrani ze zadane cesty aktivni, pokud je vyplnen nejaky text
                if (textBoxPath.Text == "Please enter path to your CSV file..." || textBoxPath.TextLength == 0)
                {
                    btnLoadDataPath.Enabled = false;
                }
                else
                {
                    btnLoadDataPath.Enabled = true;
                }
                
            }

            //logika pro zajisteni mozne stisknuti pouze jednoho checkboxu
            if (rbInfinity.Checked)
            {
                rbPauses.Checked = false;
            }
            if (rbPauses.Checked) { 
                rbInfinity.Checked = false;
            }
        }

        /// <summary>
        /// Akce pro to, kdy uzivatel vleze do textboxu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxPath_Enter(object sender, EventArgs e)
        {
            if (textBoxPath.Text == "Please enter path to your CSV file...")
            {
                textBoxPath.Text = string.Empty;
            }
            textBoxPath.ForeColor = SystemColors.ControlText;
        }

        /// <summary>
        /// Akce na to, ze uzivatel vyleze z textboxu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxPath_Leave(object sender, EventArgs e)
        {
            //pokud je delka textu 0, vloz defaultni text
            if (textBoxPath.Text.Length == 0)
            {
                textBoxPath.Text = "Please enter path to your CSV file...";
                textBoxPath.ForeColor = SystemColors.ScrollBar;
                btnLoadDataPath.Enabled = false;
            }
        }

        //doslo ke zmene stavu pro nastaveni modu nahrani jizdniho radu
        private void rbPauses_CheckedChanged(object sender, EventArgs e)
        {
            CheckEnabled();
        }

        //doslo ke zmene stavu pro nastaveni modu nahrani jizdniho radu
        private void rbInfinity_CheckedChanged(object sender, EventArgs e)
        {
            CheckEnabled();
        }

        //stisknuti tlacitka pro nahrani jizdniho radu
        private void btnLoadDataPath_Click(object sender, EventArgs e)
        {
            //cesta k souboru
            string filePath = textBoxPath.Text;

            //pokud dany soubor existuje na zadane ceste
            if (File.Exists(filePath))
            {
                //pokud dnay soubor je v CSV formatu
                if (Path.GetExtension(filePath).Equals(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    //Zobraz cestu k souboru, ktery byl nahran
                    MessageBox.Show(filePath);

                    //zjisteni, jaky mode pro nahrani jizdniho radu byl vybran
                    bool bb = rbInfinity.Checked;

                    //zobrazeni cesty k souboru, ktery byl nahran
                    labelFileName.Text = filePath;

                    //ulozeni dat pro nasledne zpracovani souboru a vyvolani eventu
                    dataToLoads.Add(new DataToLoad { Filename = filePath, InfinityData = bb });
                    ButtonLoadClick?.Invoke(this, e);
                }
                else
                {
                    //soubor neni CSV, zobraz zpravu
                    MessageBox.Show("The selected file is not a CSV file.");
                }
            }
            else
            {
                //soubor neexustuje, zobraz Message Box
                MessageBox.Show("The selected file does not exist.");
            }
        }

        /// <summary>
        /// Metoda po stisknuti tlacitka na vybrani souboru jizdniho radu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_LoadDataPickFile_Click(object sender, EventArgs e)
        {
            //otevreni okna pro vyber souboru
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                //vyber pouze csv soubotu nebo vsech
                openFileDialog.Filter = "Soubory dat (*.csv)|*.csv|Vsechny|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //zobraz message box s cestou k souboru
                    MessageBox.Show(openFileDialog.FileName);

                    //boolovska hodnota pro zjisteni modu jizdniho radu
                    bool bb = rbInfinity.Checked;

                    //zobrazeni cesty k souboru
                    string fileName = openFileDialog.FileName;
                    labelFileName.Text = fileName;

                    //uloz data pro nasledne zpracovani a vyvolej event
                    dataToLoads.Add(new DataToLoad { Filename = fileName, InfinityData = bb });
                    ButtonLoadClick?.Invoke(this, e);
                }
                else
                    return;
            }
        }

        /// <summary>
        /// MEtoda, ktera je vyvolana zmenou textu v text fieldu pro zadani cesty k souboru
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxPath_TextChanged(object sender, EventArgs e)
        {
            CheckEnabled();
        }
    }

    /// <summary>
    /// Trida nesouci informace o souboru, ktery byl nacten
    /// </summary>
    public class DataToLoad
    {
        public string Filename;

        public bool InfinityData;
    }
}

﻿using ControlLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml.Linq;
using TrainTTLibrary;

namespace TestDesignTT
{
    public partial class UCEditJson : UserControl
    {
        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler ButtonChangeJsonClick;
        //Event znacici, ze maji byt zmenena data v JSONu

        private static List<Trains> trainsList = new List<Trains>();

        public List<ChangeJsonData> changeJsonData = new List<ChangeJsonData>();

        public UCEditJson()
        {
            InitializeComponent();
            ClearData();
            checkSaveButton();
        }

        /// <summary>
        /// Metoda, která slouží ke smazání dat v jednotlivých comboboxech
        /// </summary>
        public void ClearData()
        {
            cbPickTrain.SelectedIndex = -1;
            ClearAllExceptTrain();
            checkSaveButton();
        }

        /// <summary>
        /// Meroda, která smaže vše krome vlaku
        /// </summary>
        public void ClearAllExceptTrain()
        {
            cbCurrentPosition.SelectedIndex = -1;
            cbDirection.SelectedIndex = -1;
            cbPreviousPosition.SelectedIndex = -1;
            checkSaveButton();
        }

        /// <summary>
        /// Metoda pro overeni, zda ma byt tlacitko pro ulozeni dat aktivni
        /// </summary>
        public void checkSaveButton()
        {
            if ((cbPickTrain.SelectedIndex > -1) &&
                    (cbCurrentPosition.SelectedIndex > -1) &&
                    (cbPreviousPosition.SelectedIndex > -1) &&
                    (cbDirection.SelectedIndex > -1))
            {
                //pokud jsou vsechna data vyplnena
                btnSaveData.Enabled = true;
            }
            else
            {
                btnSaveData.Enabled = false;
            }

            //aktivni/neaktivni button na minulou pozici, pokud neni vyplnena soucasna
            // (ze soucasne polohy se hleda minula)
            if (cbCurrentPosition.SelectedIndex > -1)
            {
                cbPreviousPosition.Enabled = true;
            }
            else
            {
                cbPreviousPosition.Enabled = false;
            }
        }

        /// <summary>
        /// Event po vybrani nejakeho vlaku z JSONu
        /// </summary>
        /// <param name="sender">Event ze byl vybran vlak</param>
        /// <param name="e">Event ze byl vybran vlak</param>
        private void cbPickTrain_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkSaveButton();
        }

        /// <summary>
        /// Event po vybrani soucasne polohy
        /// </summary>
        /// <param name="sender">Event, kdyz byla zmenena soucasna pozice</param>
        /// <param name="e">Event, kdyz byla zmenena soucasna pozice</param>
        private void cbCurrentPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Najde casti, ktere se nachazi na minule a soucasne pozici
            IEnumerable<string> nextPositions = ControlLogic.MainLogic.GetNextPositions(cbCurrentPosition.SelectedItem.ToString());
            cbPreviousPosition.Items.Clear();
            foreach (string position in nextPositions)
            {
                cbPreviousPosition.Items.Add(position);
            }
            checkSaveButton();
        }

        private void cbPreviousPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkSaveButton();
        }

        private void cbDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkSaveButton();
        }

        /// <summary>
        /// Po clicknuti do pole s vlakem bude combobox naplnen vsemi vlaky z JSONu (jmena totozna s configuracnim souborem)
        /// </summary>
        /// <param name="sender">Event po kliknuti do comboboxu pro vyber vlaku</param>
        /// <param name="e">Event po kliknuti do comboboxu pro vyber vlaku</param>
        private void cbPickTrain_Click(object sender, EventArgs e)
        {
            trainsList = ControlLogic.MainLogic.GetData();

            cbPickTrain.Items.Clear();

            foreach (Trains train in trainsList)
            {
                if (train.move == "0")
                {
                    cbPickTrain.Items.Add(train.name);
                }
            }
            checkSaveButton();
        }

        /// <summary>
        /// Zobrazi vsechny zname pozice, ktere jsou definovany v konfiguracnim souboru
        /// </summary>
        /// <param name="sender">Event kliknuti na vyber soucasne pozice</param>
        /// <param name="e">Event kliknuti na vyber soucasne pozice</param>
        private void cbCurrentPosition_Click(object sender, EventArgs e)
        {
            foreach (Section sec in SectionInfo.listOfSection)
            {
                cbCurrentPosition.Items.Add(sec.Name);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearData();
        }

        /// <summary>
        /// Ulozeni vybranych dat
        /// </summary>
        /// <param name="sender">Kliknuti na ulozit</param>
        /// <param name="e">Kliknuti na ulozit</param>
        private void btnSaveData_Click(object sender, EventArgs e)
        {
            addJsonChange(cbPickTrain.SelectedItem.ToString(), cbCurrentPosition.SelectedItem.ToString(), cbPreviousPosition.SelectedItem.ToString(), cbDirection.SelectedItem.ToString());
            ButtonChangeJsonClick?.Invoke(this, e);
        }

        /// <summary>
        /// Ulozeni novych informaci
        /// Tyto informace jsou pote ulozeny do JSONu
        /// </summary>
        /// <param name="id">ID vlaku</param>
        /// <param name="currentPosition">Soucasna pozice</param>
        /// <param name="previousPosition">Minula pozice</param>
        /// <param name="direction">Dostal se vlak do teto pozice popredu/pozadu</param>
        public void addJsonChange(string id, string currentPosition, string previousPosition, string direction)
        {
            changeJsonData.Add(new ChangeJsonData { Id = id, CurrentPosition = currentPosition, PreviousPosition = previousPosition, Direction = direction });
        }
    }

    public class ChangeJsonData
    {
        public string Id { get; set; }
        public string CurrentPosition { get; set; }
        public string PreviousPosition { get; set; }
        public string Direction { get; set; }
    }
}

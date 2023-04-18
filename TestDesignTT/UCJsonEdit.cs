using ControlLogic;
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
using System.Windows;
using System.Windows.Forms;
using System.Xml.Linq;
using TrainTTLibrary;

namespace TestDesignTT
{
    public partial class UCJsonEdit : UserControl
    {
        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler ButtonChangeJsonClick;
        //Event znacici, ze maji byt zmenena data v JSONu

        private static List<Trains> trainsList = new List<Trains>();

        public List<ChangeJsonData> changeJsonData = new List<ChangeJsonData>();

        public UCJsonEdit()
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
            cbStart.Enabled = false;
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
            cbStart.SelectedIndex = -1;
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
                    (cbDirection.SelectedIndex > -1) &&
                    (cbStart.SelectedIndex > -1))
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

            if (cbCurrentPosition.SelectedItem == null)
                return;

            IEnumerable<string> nextPositions = ControlLogic.MainLogic.GetNextPositions(cbCurrentPosition.SelectedItem.ToString());
            cbPreviousPosition.Items.Clear();
            cbStart.SelectedIndex = -1;
            foreach (string position in nextPositions)
            {
                if (!cbPreviousPosition.Items.Contains(position) && position != "")
                {
                    cbPreviousPosition.Items.Add(position);
                }
            }
            //ClearAllExceptTrain();
            checkSaveButton();
        }

        private void cbPreviousPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkSaveButton();

            if (cbCurrentPosition.SelectedItem == null)
                return;

            cbStart.Text = null;

            int circuit = MainLogic.GetCurrentCircuit(cbCurrentPosition.SelectedItem.ToString());
            if (circuit == 0)
            {
                IEnumerable<string> fromStart = MainLogic.GetStartStationInCritical(cbCurrentPosition.SelectedItem.ToString(), cbPreviousPosition.SelectedItem.ToString());
                //string fromStart = toElement.FirstOrDefault();
                //cbStart.Text = fromStart;
                string startStation = fromStart.FirstOrDefault();
                if (startStation != null)
                {
                    cbStart.Text = startStation;
                }
            }
            else
            {
                IEnumerable<string> fromStart = MainLogic.GetStartStationOutside(cbCurrentPosition.SelectedItem.ToString(), cbPreviousPosition.SelectedItem.ToString());
                //string fromStart = toElement.FirstOrDefault();
                //cbStart.Text = fromStart;
                string startStation = fromStart.FirstOrDefault();
                if (startStation != null)
                {
                    cbStart.Text = startStation;
                }
            }
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
            TrainDataJSON td = new TrainDataJSON();
            trainsList = td.LoadJson();

            cbPickTrain.Items.Clear();

            foreach (Trains train in trainsList)
            {
                if (train.move == 0)
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
            cbCurrentPosition.Items.Clear();
            foreach (Section sec in SectionInfo.listOfSection)
            {
                cbCurrentPosition.Items.Add(sec.Name);
            }
            ClearAllExceptTrain();
        }

        /// <summary>
        /// Zmena startu lokomotivy, odkud tam dojela
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbStart_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkSaveButton();
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
            addJsonChange(cbPickTrain.SelectedItem.ToString(), cbCurrentPosition.SelectedItem.ToString(), cbPreviousPosition.SelectedItem.ToString(), cbDirection.SelectedItem.ToString() == "Direct" ? false : true, cbStart.SelectedItem.ToString());
            ButtonChangeJsonClick?.Invoke(this, e);
            ClearData();
        }

        /// <summary>
        /// Ulozeni novych informaci
        /// Tyto informace jsou pote ulozeny do JSONu
        /// </summary>
        /// <param name="id">ID vlaku</param>
        /// <param name="currentPosition">Soucasna pozice</param>
        /// <param name="previousPosition">Minula pozice</param>
        /// <param name="direction">Dostal se vlak do teto pozice popredu/pozadu</param>
        public void addJsonChange(string id, string currentPosition, string previousPosition, bool direction, string startPosition)
        {
            changeJsonData.Add(new ChangeJsonData { Id = id, CurrentPosition = currentPosition, PreviousPosition = previousPosition, Reverse = direction, StartPosition = startPosition });
        }
    }

    public class ChangeJsonData
    {
        public string Id { get; set; }
        public string CurrentPosition { get; set; }
        public string PreviousPosition { get; set; }
        public bool Reverse { get; set; }
        public string StartPosition { get; set; }
    }
}

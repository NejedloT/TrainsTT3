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

        //list s JSON daty vlaku
        private static List<Trains> trainsList = new List<Trains>();

        //list pro ulozeni zmeny dat JSONu
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
            //Pokud nebyl vybran soucasny usek, ukonci to
            if (cbCurrentPosition.SelectedItem == null)
                return;

            //zjisti sousedni useky od aktualni polohy vlaku
            IEnumerable<string> nextPositions = SearchLogic.GetNextPositions(cbCurrentPosition.SelectedItem.ToString());
            
            //vymaz data pro minuly usek a vloz nova
            cbPreviousPosition.Items.Clear();
            cbStart.SelectedIndex = -1;
            foreach (string position in nextPositions)
            {
                //vlozi data, ktera jeste nebyla pridana a ktera nejsou prazdna
                if (!cbPreviousPosition.Items.Contains(position) && position != "")
                {
                    cbPreviousPosition.Items.Add(position);
                }
            }
            //ClearAllExceptTrain();
            checkSaveButton();
        }

        /// <summary>
        /// Event po vybrani minule polohy vlaku
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbPreviousPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkSaveButton();

            //pokud nebyla vybrana soucasna poloha, tak konec
            if (cbCurrentPosition.SelectedItem == null)
                return;

            cbStart.Text = null;

            //zjisteni aktualni hodnoty okruhu
            int circuit = SearchLogic.GetCurrentCircuit(cbCurrentPosition.SelectedItem.ToString());

            //Pokud je vlak u nadrazi, hledej polohu mozne pocatecni stanice v definovanych cestach a zobraz prvni z nich
            if (circuit == 0 || circuit == 4 || circuit == 7)
            {
                IEnumerable<string> fromStart = SearchLogic.GetStartStationInCritical(cbCurrentPosition.SelectedItem.ToString(), cbPreviousPosition.SelectedItem.ToString());
                string startStation = fromStart.FirstOrDefault();
                if (startStation != null)
                {
                    cbStart.Text = startStation;
                }
            }
            else
            {
                //najdi pocatecni stanici mimo kriticke useky
                IEnumerable<string> fromStart = SearchLogic.GetStartStationOutside(cbCurrentPosition.SelectedItem.ToString(), cbPreviousPosition.SelectedItem.ToString());
                string startStation = fromStart.FirstOrDefault();
                if (startStation != null)
                {
                    cbStart.Text = startStation;
                }
            }
        }

        /// <summary>
        /// Akce po zmene orientace vlaku
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            //najdi JSON
            TrainDataJSON td = new TrainDataJSON();
            trainsList = td.LoadJson();

            //vymaz data
            cbPickTrain.Items.Clear();

            //vloz jednotlive vlaky, ktere maji priznak ze nejedou
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

            //zobraz vsechny pozice, ktere jsou mozne vlozit
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

        /// <summary>
        /// Metoda po stisknuti buttonu pro vymazani vsech dat z jednotlivych boxu a fieldu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearData();
        }

        /// <summary>
        /// Ulozeni vybranych dat a vyvolani eventu
        /// </summary>
        /// <param name="sender">Kliknuti na ulozit</param>
        /// <param name="e">Kliknuti na ulozit</param>
        private void btnSaveData_Click(object sender, EventArgs e)
        {
            string orientation = SearchLogic.GetOrientation(cbCurrentPosition.SelectedItem.ToString(), cbPreviousPosition.SelectedItem.ToString());

            addJsonChange(cbPickTrain.SelectedItem.ToString(), cbCurrentPosition.SelectedItem.ToString(), cbPreviousPosition.SelectedItem.ToString(), cbDirection.SelectedItem.ToString() == "Direct" ? false : true, cbStart.SelectedItem.ToString(), orientation);
            ButtonChangeJsonClick?.Invoke(this, e);
            ClearData();
        }

        /// <summary>
        /// Ulozeni novych informaci
        /// Tyto informace jsou pote ulozeny do JSONu
        /// </summary>
        /// <param name="name">ID vlaku</param>
        /// <param name="currentPosition">Soucasna pozice</param>
        /// <param name="previousPosition">Minula pozice</param>
        /// <param name="direction">Dostal se vlak do teto pozice popredu/pozadu</param>
        public void addJsonChange(string name, string currentPosition, string previousPosition, bool direction, string startPosition, string orientation)
        {
            changeJsonData.Add(new ChangeJsonData { Name = name, CurrentPosition = currentPosition, PreviousPosition = previousPosition, Reverse = direction, StartPosition = startPosition, Orientation = orientation });
        }
    }

    /// <summary>
    /// Trida nesouci data o zmene JSON dat pro dany vlak
    /// </summary>
    public class ChangeJsonData
    {
        public string Name { get; set; }
        public string CurrentPosition { get; set; }
        public string PreviousPosition { get; set; }
        public bool Reverse { get; set; }
        public string StartPosition { get; set; }

        public string Orientation { get; set; }
    }
}

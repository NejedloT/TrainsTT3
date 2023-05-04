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
using System.Xml.Linq;
using ControlLogic;

namespace TestDesignTT
{
    public partial class UCAddManualTrain : UserControl
    {
        //list pro pouzivani dat z JSONu
        private static List<Trains> trainsList = new List<Trains>();

        //Event handler, ze doslo k pridani nove lokomotivy pro rozjezd vlaku
        public event EventHandler ButtonAddLocoClick;

        //list pro pridani novych dat lokomotivy
        public List<AddDataToSend> addNewLocoData = new List<AddDataToSend>();

        //string, ve kterem bude ulozena hodnota pocatecni polohy pro rizeni vlaku
        private string startPosition = null;


        public UCAddManualTrain()
        {
            InitializeComponent();
            ClearData();
            CheckValues();
        }

        /// <summary>
        /// Metoda, ktera vymaze vsechny data, ktera byla vyplnena pro soucasnou lokomotivu
        /// </summary>
        public void ClearData()
        {
            cbPickTrain.SelectedIndex = -1;
            ClearAllExceptTrain();
            CheckValues();
        }

        /// <summary>
        /// Metoda, ktera vymaze vsechny data, ktera byla vyplnena pro soucasnou lokomotivu krome lokomotivy
        /// </summary>
        public void ClearAllExceptTrain()
        {
            cbDirect.SelectedIndex = -1;
            cbSpeed.SelectedIndex = -1;
            cbFinalStation.SelectedIndex = -1;
            cbFinalTrack.SelectedIndex = -1;
            cbFinalTrack.Visible = false;
            labelFinalTrack.Visible = false;
            tbStartPosition.Text = string.Empty;

        }

        /// <summary>
        /// Metoda, ktera povoli vyplneni dat, pokud je vyplnena lokomotiva a v zavislosti na vyplnene lokomotive
        /// </summary>
        /// <param name="bb">Hodnota, jestli ma byt povolen vyber z comboboxu</param>
        /// <param name="direct">Hodnota, zdali ma byt povoleno vyplnit smer lokomotivy</param>
        public void EnableDependingOnTrain(bool bb, bool direct)
        {
            cbDirect.Enabled = direct;
            cbSpeed.Enabled = bb;
            cbFinalStation.Enabled = bb;
            cbFinalTrack.Enabled = bb;
            radioButtonNo.Enabled = bb;
            radioButtonYes.Enabled = bb;
        }

        /// <summary>
        /// MEtoda, ktera ridi logiku povoleni/zakazani otevreni vyberu comboboxu a checkboxu
        /// </summary>
        public void CheckValues()
        {
            //vlak neni vybran, nic se neda vyplnit
            if (cbPickTrain.SelectedIndex == -1)
            {
                EnableDependingOnTrain(false, false);
                ClearAllExceptTrain();
            }
            //vlak vybran, nektera data by mela jit vlozit
            else
            {
                cbDirect.Enabled = true;
                EnableDependingOnTrain(true, true);
            }

            //Pokud neni vybrana orientace vlaku, nelze vybrat cilovou stanici
            if (cbDirect.SelectedIndex > -1)
                cbFinalStation.Enabled = true;
            else
                cbFinalStation.Enabled = false;

            //pokud neni vybrana cilova stanice, nelze zvolit vyber cilove koleje v dane stanici
            if (cbFinalStation.SelectedIndex > -1)
            {
                radioButtonYes.Enabled = true; 
                radioButtonYes.Enabled = true;
            }
            else
            {
                radioButtonYes.Enabled = false;
                radioButtonYes.Enabled = false;
            }

            //akce a povoleni, pokud uzivatel chce/nechce vybrat cilovou kolej
            if (radioButtonYes.Checked)
            {
                cbFinalTrack.Visible = true;
                labelFinalTrack.Visible = true;
                radioButtonNo.Checked = false;
            }
            if (radioButtonNo.Checked)
            {
                cbFinalTrack.Visible = false;
                labelFinalTrack.Visible = false;
                radioButtonYes.Checked = false;
            }

            checkAddButton();
        }

        /// <summary>
        /// Testovani, zdali jsou vyplnena vsechna data a lze tedy odeslat data pro rozjezd vlaku
        /// </summary>
        private void checkAddButton()
        {
            if ((cbPickTrain.SelectedIndex > -1) && (cbDirect.SelectedIndex > -1) && (cbSpeed.SelectedIndex > -1))
            {
                if ((cbFinalStation.SelectedIndex > -1))
                {
                    if ((radioButtonNo.Checked) || ((radioButtonYes.Checked) && cbFinalTrack.SelectedIndex > -1))
                    {
                        btnAddTrain.Enabled = true;
                        return;
                    }

                }
                btnAddTrain.Enabled = false;


            }
            btnAddTrain.Enabled = false;
        }

        /// <summary>
        /// Akce po vyvolani stiknuti comboboxu s lokomotivou
        /// Po stisknuti jsou nejprve smazana vsechna data a pote zobrazeny lokomotivy, ktere lze zaslat k rozjezdu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbPickTrain_Click(object sender, EventArgs e)
        {
            //nacti data z JSONu
            TrainDataJSON td = new TrainDataJSON();
            trainsList = td.LoadJson();

            //vymaz vsechna data a podstrc lokomotivy, ktere nejedou nebo jiz nemaji prikaz k rozjezdu
            cbPickTrain.Items.Clear();
            foreach (Trains train in trainsList)
            {
                if (train.move == 0)
                {
                    cbPickTrain.Items.Add(train.name);
                }
            }
            CheckValues();
        }

        /// <summary>
        /// Akce po vybrani konkretni lokomotivy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbPickTrain_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearAllExceptTrain();

            //zjisteni konkretniho vybraneho vlaku
            ComboBox cb = (ComboBox)sender;
            string selectedTrainName = (string)cb.SelectedItem;

            //data z JSONu
            TrainDataJSON td = new TrainDataJSON();
            trainsList = td.LoadJson();

            //zobrazeni aktualni pozice vlaku do textboxu
            foreach (Trains train in trainsList)
            {
                if (train.name == selectedTrainName)
                {
                    tbStartPosition.Text = train.currentPosition;
                }
            }
            CheckValues();
        }

        /// <summary>
        /// Akce, ze doslo k vybrani pozadovane orientace vlaku
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbDirect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPickTrain.SelectedItem != null)
            {
                //najdi aktualni vlak
                foreach (Trains train in trainsList)
                {
                    if (train.name == cbPickTrain.SelectedItem.ToString())
                    {
                        getFinalStation(train);
                    }
                }
            }

            //doslo k vyberu orientace, povol vyber cilove koleje
            radioButtonNo.Checked = true;
            CheckValues();

        }

        /// <summary>
        /// Metoda, ktera hleda a zjistuje mozne cilove stanice
        /// </summary>
        /// <param name="train"></param>
        private void getFinalStation(Trains train)
        {
            //preddefinovane kolekce pro mozna data pocatku a konce vlaku
            IEnumerable<string> fromStart = null;
            IEnumerable<string> final = null;
            
            //bool hodnota, zdali je vlak v kritickem useku ci nikoliv
            bool crit = false;

            //vymaz data cilovych stanic
            cbFinalStation.Items.Clear();

            //tzestovani, jestli je vlak na nadrazi (jine vyhledavani)
            if (train.circuit == 0 || train.circuit == 4 || train.circuit == 7)
            {
                //vlak je na nadrazi nebo u nadrazi mezi vyhybkama, hledej mozne pocatecni stanice pres zadefinovane cesty
                crit = true;
                fromStart = SearchLogic.GetStartStationInCritical(train.currentPosition, train.lastPosition);

            }
            else
            {
                //vlak neni na nadrazi nebo u nadrazi mezi vyhybkama, hledej mozne pocatecni stanice na otevrenem okruhu
                crit = false;
                fromStart = SearchLogic.GetStartStationOutside(train.currentPosition, train.lastPosition);
                //string fromStart = toElement.FirstOrDefault();
            }

            //zjisteni, jestli vlak ma jet popredu nebo pozadu
            bool reverse;
            if (cbDirect.SelectedItem.ToString() == "Direct")
                reverse = false;
            else
                reverse = true;

            //testovani, zdali je soucasna poloha vlaku v kritickem useku
            if (crit)
            {
                //vlak je na nadrazi nebo u nadrazi mezi vyhybkama, hledej mozne pocatecni stanice pres zadefinovane cesty
                final = SearchLogic.GetFinalStationInCritical(train.currentPosition, train.lastPosition);
            }
            else
            {
                //vlak neni na nadrazi nebo u nadrazi mezi vyhybkama, hledej mozne pocatecni stanice na otevrenem okruhu
                final = SearchLogic.GetFinalStationOutside(train.currentPosition, train.lastPosition);
            }

            //pokud vlak ma jet opacnym smerem, nez byl posledni smer, prohod pocatecni a cilove stanice
            if (reverse != train.reverse)
            {
                IEnumerable<string> HelpVar = fromStart;
                fromStart = final;
                final = HelpVar;
            }

            //zjisti jednotlive unikatni stanice a uloz jednu z nich do promenne
            //v pripade, ze se vlak nachazi na nadrazi, tak je to jen jako jedna z moznych stanic
            List<string> uniqueStart = fromStart.Distinct().ToList();
            startPosition = uniqueStart[0];

            //zobraz jednotlive mozne finalni stanice
            foreach (string s in final)
            {
                List<string> endTracks = SearchLogic.GetTracksForStation(s);
                if (!cbFinalStation.Items.Contains(s) && !endTracks.Contains(train.currentPosition))
                    cbFinalStation.Items.Add(s);   
            }
        }

        /// <summary>
        /// Metoda, která najde možnou finální stanici vlaku
        /// </summary>
        /// <param name="train"></param>
        private void getFinalTrack(Trains train)
        {
            //vymaz cilove koleje
            cbFinalTrack.Items.Clear();

            //najdi cilove koleje (uvazovano, ze je vlak mimo nadrazi)
            IEnumerable<XElement> finTrack = SearchLogic.GetFinalTrackOutside(train, cbFinalStation.SelectedItem.ToString());

            //zjisti cilovy okruh konecne stanice pro nadchazejici kontrolu
            int finalCircuit = SearchLogic.GetFinalStationCircuit(cbFinalStation.SelectedItem.ToString());

            //testovani, jestli vlak pojede opacnym smerem (v tom pripade minuly usek je usek nadchazejici)
            bool reverse;
            if (cbDirect.SelectedItem.ToString() == "Direct")
                reverse = false;
            else
                reverse = true;

            //testovani jednotlivych objevenych cilovych useku
            foreach (XElement x in finTrack)
            {
                //testovani, zdali je vlak v kritickem useku u ciloveho nadrazi ci nikoliv
                //pokud neni a hleda se cesta z nadrazi A, ale jede do nadrazi B, tak jsou urcene cesty do cilovych stanic z jednotlivych okruhu
                //pokud ano, tak se z kolejich ve stanci vyberou ty, kam muze vlak jet
                if ((train.circuit == 0 && finalCircuit == 0)
                    || (train.circuit == 4 && finalCircuit == 4)
                    || (train.circuit == 7 && finalCircuit == 7))
                {
                    bool bb;
                    //najdi mozne koleje
                    if (reverse == train.reverse)
                        bb = SearchLogic.GetFinalTrackInside(train.currentPosition, train.lastPosition, x.Value);
                    else
                    {
                        bb = SearchLogic.GetFinalTrackInside(train.lastPosition, train.currentPosition, x.Value);
                    }

                    //pokud na danou kolej vlak muze ze soucasne pozice jet
                    if (bb)
                        cbFinalTrack.Items.Add(x.Value);
                }

                //vlak neni v kritickem useku u nadrazi, do ktereho jede, muzu pridat vsechny koleje
                else
                {
                    cbFinalTrack.Items.Add(x.Value);
                }
            }

        }

        #region Actions when index of combobox or value of radiobuttons are changed
        /// <summary>
        /// Akce, ze uzivatel stisknul checkbox pro vybrani cilove koleje
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonYes_CheckedChanged(object sender, EventArgs e)
        {
            //radioButtonNo.Checked = true;
            CheckValues();
        }

        /// <summary>
        /// Akce, ze uzivatel stisknul checkbox pro to, ze nechce vybrat cilovou kolej (najde ji pak sama logika rizeni)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonNo_CheckedChanged(object sender, EventArgs e)
        {
            //radioButtonNo.Checked = true;
            CheckValues();
        }

        /// <summary>
        /// Vymazani vsech dat, ktera byla vyplnena
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearData();
        }

        /// <summary>
        /// Akce, ze doslo ke zmene hodnoty rychlosti vlaku
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSpeed_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckValues();
        }

        /// <summary>
        /// Akce, ze doslo k vybrani cilove stanice
        /// Vola metodu, ktera zjistuje mozne cilove koleje
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbFinalStation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPickTrain.SelectedItem != null)
            {
                //najde pozadovany vlak
                foreach (Trains train in trainsList)
                {
                    if (train.name == cbPickTrain.SelectedItem.ToString())
                    {
                        //vlak nalezen, zjisti mozne cilove koleje
                        getFinalTrack(train);
                    }
                }
            }
            CheckValues();
        }

        /// <summary>
        /// Doslo k vybrani cilove koleje
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbFinalTrack_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckValues();
        }
        #endregion

        private void btnAddTrain_Click(object sender, EventArgs e)
        {
            string final = "";

            //uzivatel vybral cilovou kolej, uloz tuto hodnotu jako konecnou, jinak dane nadrazi
            if (radioButtonYes.Checked)
                final = cbFinalTrack.SelectedItem.ToString();
            else
                final = cbFinalStation.SelectedItem.ToString();

            //jmeno vlaku
            string name = cbPickTrain.SelectedItem.ToString();

            //aktualni pozice vlaku
            string currentPosition = tbStartPosition.Text;

            //urci rychlost vlaku z vybrane hodnoty
            string spd = cbSpeed.SelectedItem.ToString();
            byte speed = byte.Parse(spd);

            //urci orientaci vlaku
            bool reverse;
            if (cbDirect.SelectedItem.ToString() == "Direct")
                reverse = false;
            else
                reverse = true;

            string start = startPosition;

            //uloz data a vyvolej event handler, ze jsou vytvorena nova data pro odeslani vlaku
            addTrainInfoData(name, currentPosition, speed, reverse, start, final);
            ButtonAddLocoClick?.Invoke(this, e);

            ClearData();
        }

        /// <summary>
        /// Metoda, ktera pouze zpracova zadana data a ulozi, aby bylo mozne rizeni vlaku
        /// </summary>
        /// <param name="name">Jmeno vlaku</param>
        /// <param name="currentPosition">Aktualni pozice vlaku</param>
        /// <param name="speed">Rychlost vlaku</param>
        /// <param name="direction">Smer vlaku vpred/vzad</param>
        /// <param name="start">Pocatecni stanice</param>
        /// <param name="final">Cilova stanice</param>
        public void addTrainInfoData(string name, string currentPosition, byte speed, bool direction, string start, string final)
        {
            addNewLocoData.Add(new AddDataToSend { Name = name, CurrentPosition = currentPosition, Speed = speed, Reverse = direction, StartPosition = start, FinalPosition = final });
        }
    }

    /// <summary>
    /// Trida pro informace, ktere jsou zaslany pro logiku rizeni
    /// </summary>
    public class AddDataToSend
    {
        public string Name { get; set; }
        public string CurrentPosition { get; set; }
        public byte Speed { get; set; }
        public bool Reverse { get; set; }
        public string StartPosition { get; set; }
        public string FinalPosition { get; set; }
    }
}

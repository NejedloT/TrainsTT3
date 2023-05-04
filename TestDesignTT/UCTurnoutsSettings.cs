using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrainTTLibrary;
using static TrainTTLibrary.Packet;

namespace TestDesignTT
{
    public partial class UCTurnoutsSettings : UserControl
    {
        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler TurnoutInstructionSetClick;
        //Event handler pro nastaveni jednotky vyhybek

        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler TurnoutDefinitionStopsClick;
        //Event handler pro nastaveni softwarovych dorazu

        //list s daty pro nastaveni jednotky vyhybek
        public List<SetNewTurnoutUnitData> newTurnoutData = new List<SetNewTurnoutUnitData>(); //pro ulozeni dat urcenych jednotce

        //list s daty pro nastaveni softwarovych dorazu
        public List<SetNewTurnoutStops> newTurnoutStops = new List<SetNewTurnoutStops>(); //pro ulozeni dat na nastaveni dorazu


        public UCTurnoutsSettings()
        {
            InitializeComponent();
            CheckStates();

            cbUnitNumber.Items.Clear();

            //vlozeni IDs jednotek vyhybek
            IEnumerable<int> UnitIDs = ControlLogic.SearchLogic.GetTurnoutIDs();
            foreach (int id in UnitIDs)
            {
                cbUnitNumber.Items.Add(id);
            }
        }

        #region Buttons and comboboxed events definition
        private void cbUnitNumber_Click(object sender, EventArgs e)
        {
            cbUnitNumber.SelectedIndex = -1;
            CheckStates();
        }

        private void cbUnitNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckStates();
        }

        private void cbPickTurnout_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckStates();
        }

        private void cbLeftStop_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckStates();
        }

        private void cbRightStop_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckStates();
            Control_Click(sender, e);
        }

        private void cbTimeDelay_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckStates();
            Control_Click(sender, e);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            CheckStates();
            Control_Click(sender, e);
        }

        private void btnReadState_Click(object sender, EventArgs e)
        {
            CheckStates();
            Control_Click(sender, e);
        }
        #endregion

        /// <summary>
        /// Metoda pro kontrolu stavů
        /// Zajistí aktivaci/deaktivace buttonu a comboboxu dle zvolenych kombinaci
        /// Tim je docileno spravneho chovani a ulozeni zvolenych dat
        /// </summary>
        private void CheckStates()
        {
            bool bb = false;
            //Pokud neni zvoleno cislo jendotky, nelze provest zadnou dalsi akci
            if (cbUnitNumber.SelectedIndex < 0)
            {
                bb = false;
                cbTimeDelay.SelectedIndex = -1;
                cbPickTurnout.SelectedIndex = -1;
                cbLeftStop.SelectedIndex = -1;
                cbRightStop.SelectedIndex = -1;
            }

            else
                bb = true;

            //povol/zakaz jednotliva tlacitka a comboboxy
            cbTimeDelay.Enabled = bb;
            cbPickTurnout.Enabled = bb;
            btnReadState.Enabled = bb;
            btnReset.Enabled = bb;

            //logika pro spravny vyber hodnot na softwarove dorazy
            if (cbPickTurnout.SelectedIndex > -1)
                cbLeftStop.Enabled = true;
            else
                cbLeftStop.Enabled = false;

            if (cbLeftStop.SelectedIndex > -1)
                cbRightStop.Enabled = true;
            else
                cbRightStop.Enabled = false;

        }

        /// <summary>
        /// Metoda pro vytvoreni instance dle stisknutych tlacitek ci vyplnenych comboboxu
        /// Aktivovana po finalnim vytvoreni nejake instrukce jednotce pro vyhybky
        /// </summary>
        /// <param name="sender">Combobox/Button</param>
        /// <param name="e">Combobox/Button</param>
        private void Control_Click(object sender, EventArgs e)
        {
            //zjisteni stisknuteho elementu
            var control = sender as Control;

            //otestovani, ze bylo skutecne vybrano ID jednotky
            if (cbUnitNumber.SelectedItem == null)
                return;

            //ziskani ID jednotky
            if (!int.TryParse(cbUnitNumber.SelectedItem.ToString(), out int selectedItem))
                return;
            byte numberOfUnit = (byte)selectedItem;


            if (cbRightStop.SelectedIndex < 0)
            {
                //pozadavek na zmenu prodlevy pred natocenim
                if (control == cbTimeDelay)
                {
                    //vyber instrukce pro nastaveni prodlevy
                    turnoutInstruction ti = Packet.turnoutInstruction.nastaveni_prodlevy_pred_natocenim;

                    //zjisteni hodnoty prodlevy mezi odesilanim odberu proudu
                    string value = cbTimeDelay.SelectedItem.ToString();

                    //extrakce doby prodlevy ze stringu stisknuteho, Prevod na integer a nadledne na byte
                    string numericString = value.Split(' ')[0];
                    int myValue = int.Parse(numericString) / 10;
                    byte data = (byte)myValue;

                    //ulozeni dat
                    addNewTurnoutUnitData(ti, numberOfUnit, data);
                }

                //pozadavek na precteni aktualniho stavu
                if (control == btnReadState)
                {
                    //vyber instrukce pro zjisteni stavu natoceni vyhybek
                    turnoutInstruction ti = Packet.turnoutInstruction.precteni_stavu_natoceni;

                    //vlozeni dat pro precteni stavu
                    byte data = 0x01;

                    //ulozeni dat
                    addNewTurnoutUnitData(ti, numberOfUnit, data);
                }

                //pozadavek na reser jednotky
                if (control == btnReset)
                {
                    //vyber instrukce pro reset jednotky
                    turnoutInstruction ti = Packet.turnoutInstruction.restart_jednotky;

                    //vlozeni dat pro reset jednotky
                    byte data = 0x01;

                    //ulozeni dat
                    addNewTurnoutUnitData(ti, numberOfUnit, data);
                }

                //data byla ulozena, nyni vymazani jednotky a zrusit zaktivneni tlacitek
                cbUnitNumber.SelectedIndex = -1;
                CheckStates();

                //vyvolani eventu, aby byl vytvoren paket a data zaslana
                TurnoutInstructionSetClick?.Invoke(this, e);
            }
            else
            {
                //vyber instrukce pro nastaveni softwarovych dorazu
                turnoutInstruction ti = Packet.turnoutInstruction.nastaveni_dorazu;

                //zjisteni vybrane vyhybky a kontrola pred crashnutim aplikace
                string selectedTurnout = cbPickTurnout.SelectedItem as string;
                if (selectedTurnout == null)
                    return;

                //zjisteni bytove hodnoty vyhybky
                byte numberOfTurnout;
                if (selectedTurnout == "All")
                    numberOfTurnout = 0xAA;
                else
                {
                    int myValue = int.Parse(selectedTurnout) - 1;
                    //vybrane  cislo vyhybky z comboboxu
                    numberOfTurnout = (byte)(myValue);
                }

                //////////////////////////////
                //ziskani dat o levem dorazu//
                string valueLeft = cbLeftStop.SelectedItem.ToString();


                //extrakce dorazu vlevo ze stringu stisknuteho
                string numericLeft = valueLeft.Split(' ')[0];

                // Prevod na integer a nadledne na byte
                int myValueLeft = int.Parse(numericLeft);

                byte left = (byte)myValueLeft;

                //////////////////////////////
                //ziskani dat o pravem dorazu//
                string valueRight = cbRightStop.SelectedItem.ToString();

                //extrakce dorazu vlevo ze stringu stisknuteho
                string numericRight = valueRight.Split(' ')[0];

                // Prevod na integer a nadledne na byte
                int myValueRight = int.Parse(numericRight);

                byte right = (byte)myValueRight;

                //ulozeni dat
                addNewTurnoutStops(ti,numberOfUnit,numberOfTurnout,left,right);

                //data byla ulozena, nyni vymazani jednotky a zrusit zaktivneni tlacitek
                cbUnitNumber.SelectedIndex = -1;
                CheckStates();

                //vyvolani eventu, aby byl vytvoren paket a data zaslana
                TurnoutDefinitionStopsClick?.Invoke(this, e);
            }
        }

        /// <summary>
        /// Metoda pro ulozeni nastaveni jednotky vyhybek (mimo nastaveni dorazu)
        /// </summary>
        /// <param name="type">Typ instrukce</param>
        /// <param name="numberOfUnit">ID jednotky</param>
        /// <param name="data">Data</param>
        public void addNewTurnoutUnitData(turnoutInstruction type, byte numberOfUnit, byte data)
        {
            newTurnoutData.Add(new SetNewTurnoutUnitData { Type = type, NumberOfUnit = numberOfUnit, Data = data });
        }

        /// <summary>
        /// Metoda pro ulozeni nastaveni softwarovych dorazu
        /// </summary>
        /// <param name="type">Typ instrukce</param>
        /// <param name="numberOfUnit">ID jednotky</param>
        /// <param name="numberOfTurnout">ID vybrane vyhybky /vsech vyhybek</param>
        /// <param name="left">Hodnota leveho dorazu</param>
        /// <param name="right">Hodnota praveho dorazu</param>
        public void addNewTurnoutStops(turnoutInstruction type, byte numberOfUnit, byte numberOfTurnout, byte left, byte right)
        {
            newTurnoutStops.Add(new SetNewTurnoutStops { Type = type, NumberOfUnit = numberOfUnit, NumberOfTurnout = numberOfTurnout, LeftStop = left, RightStop = right });
        }
    }

    /// <summary>
    /// Trida pro uchovani dat pro nastaveni jednotky vyhybek
    /// </summary>
    public class SetNewTurnoutUnitData
    {
        public turnoutInstruction Type { get; set; }
        public byte NumberOfUnit { get; set; }
        public byte Data { get; set; }
    }

    /// <summary>
    /// Trida pro nastaveni softwarovych dorazu
    /// </summary>
    public class SetNewTurnoutStops
    {
        public turnoutInstruction Type { get; set; }
        public byte NumberOfUnit { get; set; }
        public byte NumberOfTurnout { get; set; }
        public byte LeftStop { get; set; }
        public byte RightStop { get; set; }

    }
}

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

        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler TurnoutDefinitionStopsClick;

        public List<SetNewTurnoutUnitData> newTurnoutData = new List<SetNewTurnoutUnitData>(); //pro ulozeni dat urcenych jednotce

        public List<SetNewTurnoutStops> newTurnoutStops = new List<SetNewTurnoutStops>(); //pro ulozeni dat na nastaveni dorazu


        public UCTurnoutsSettings()
        {
            InitializeComponent();
            CheckStates();
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

            cbTimeDelay.Enabled = bb;
            cbPickTurnout.Enabled = bb;
            btnReadState.Enabled = bb;
            btnReset.Enabled = bb;

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

            string selectedItem = cbUnitNumber.SelectedItem as string;

            //kontrola pred crashnutim aplikace
            if (selectedItem == null)
                return;

            //vybrane  cislo jednotky z comboboxu
            byte numberOfUnit = byte.Parse(selectedItem);

            if (cbRightStop.SelectedIndex < 0)
            {
                //pozadavek na zmenu prodlevy pred natocenim
                if (control == cbTimeDelay)
                {
                    turnoutInstruction ti = Packet.turnoutInstruction.nastaveni_prodlevy_pred_natocenim;

                    string value = cbTimeDelay.SelectedItem.ToString();

                    //extrakce doby prodlevy ze stringu stisknuteho
                    string numericString = value.Split(' ')[0];

                    // Prevod na integer a nadledne na byte
                    int myValue = int.Parse(numericString) / 10;

                    byte data = (byte)myValue;

                    //ulozeni dat
                    addNewTurnoutUnitData(ti, numberOfUnit, data);
                }

                //pozadavek na precteni aktualniho stavu
                if (control == btnReadState)
                {
                    turnoutInstruction ti = Packet.turnoutInstruction.precteni_stavu_natoceni;

                    byte data = 0x01;

                    addNewTurnoutUnitData(ti, numberOfUnit, data);
                }

                if (control == btnReset)
                {
                    turnoutInstruction ti = Packet.turnoutInstruction.restart_jednotky;

                    byte data = 0x01;

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
                turnoutInstruction ti = Packet.turnoutInstruction.nastaveni_dorazu;

                string selectedTurnout = cbPickTurnout.SelectedItem as string;

                //kontrola pred crashnutim aplikace
                if (selectedTurnout == null)
                    return;

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

                addNewTurnoutStops(ti,numberOfUnit,numberOfTurnout,left,right);

                cbUnitNumber.SelectedIndex = -1;
                CheckStates();

                //vyvolani eventu, aby byl vytvoren paket a data zaslana
                TurnoutDefinitionStopsClick?.Invoke(this, e);
            }
        }

        public void addNewTurnoutUnitData(turnoutInstruction type, byte numberOfUnit, byte data)
        {
            newTurnoutData.Add(new SetNewTurnoutUnitData { Type = type, NumberOfUnit = numberOfUnit, Data = data });
        }

        public void addNewTurnoutStops(turnoutInstruction type, byte numberOfUnit, byte numberOfTurnout, byte left, byte right)
        {
            newTurnoutStops.Add(new SetNewTurnoutStops { Type = type, NumberOfUnit = numberOfUnit, NumberOfTurnout = numberOfTurnout, LeftStop = left, RightStop = right });
        }
    }

    public class SetNewTurnoutUnitData
    {
        public turnoutInstruction Type { get; set; }
        public byte NumberOfUnit { get; set; }
        public byte Data { get; set; }
    }

    public class SetNewTurnoutStops
    {
        public turnoutInstruction Type { get; set; }
        public byte NumberOfUnit { get; set; }
        public byte NumberOfTurnout { get; set; }
        public byte LeftStop { get; set; }
        public byte RightStop { get; set; }

    }
}

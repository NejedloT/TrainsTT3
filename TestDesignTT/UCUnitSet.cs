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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using static TrainTTLibrary.Packet;

namespace TestDesignTT
{
    public partial class UCUnitSet : UserControl
    {
        //Event handler pro data na zmenu ridici jednotky
        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler UnitInstructionEventClick;

        //list pro uchovani dat na nastaveni usekove jednotky
        public List<SetNewUnitData> newUnit = new List<SetNewUnitData>();

        public UCUnitSet()
        {
            InitializeComponent();
            CheckStates();

            //vlozeni IDs usekovych jednotek do comboboxu
            IEnumerable<int> UnitIDs = ControlLogic.SearchLogic.GetModulesId();
            foreach (int id in UnitIDs)
            {
                cbUnitNumber.Items.Add(id);
            }
        }

        #region Click events on buttons and comboboxes
        private void cbUnitNumber_Click(object sender, EventArgs e)
        {
            cbUnitNumber.SelectedIndex = -1;
            CheckStates();
        }

        private void cbUnitNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckStates();
        }

        private void cbCurrentConsumption_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckStates();
            Control_Click(sender, e);
        }

        private void btnResetProcessor_Click(object sender, EventArgs e)
        {
            CheckStates();
            Control_Click(sender, e);
        }

        private void btnResetH_Click(object sender, EventArgs e)
        {
            CheckStates();
            Control_Click(sender, e);
        }

        private void btnPowerOff_Click(object sender, EventArgs e)
        {
            CheckStates();
            Control_Click(sender, e);
        }

        private void btnPowerOn_Click(object sender, EventArgs e)
        {
            CheckStates();
            Control_Click(sender, e);
        }

        private void btnReadUnitInstruction_Click(object sender, EventArgs e)
        {
            CheckStates();
            Control_Click(sender, e);
        }
        #endregion

        /// <summary>
        /// Metoda pro kontrolu stavu
        /// Pokud neni vybrana jednotka, tak ostatni buttony/comboboxy jsou neaktivni
        /// Tim je docileno spravneho chovani a ulozeni zvolenych dat
        /// </summary>
        private void CheckStates()
        {
            bool bb = false;
            if (cbUnitNumber.SelectedIndex < 0)
            {
                bb = false;
                cbCurrentConsumption.SelectedIndex = -1;
            }

            else
                bb = true;

            //povoleni/zakazani sitknuti tlacitek/comboboxu pokud je/neni vybrano ID jednotky
            cbCurrentConsumption.Enabled = bb;
            btnPowerOff.Enabled = bb;
            btnPowerOn.Enabled = bb;
            btnResetProcessor.Enabled = bb;
            btnResetH.Enabled = bb;
            btnReadUnitInstruction.Enabled = bb;
        }

        /// <summary>
        /// Metoda vyvolana stisknutim tlaticka ci vybranim hodnoty v comboboxu
        /// Dle stisknuteho controlu je zjisten konkretni typ a data jsou ulozena
        /// Poto nasleduje odeslani ridici jednotce
        /// </summary>
        /// <param name="sender">Event stisknutim nejakeho controlu</param>
        /// <param name="e">Event stisknutim nejakeho controlu</param>
        private void Control_Click(object sender, EventArgs e)
        {
            //zjisteni stisknuteho controlu (tlacitko, combobox..)
            var control = sender as Control;

            //overeni, ze bylo vybrano ID jednotky
            if (cbUnitNumber.SelectedItem == null)
                return;

            //vybrane  cislo jednotky z comboboxu
            if (!int.TryParse(cbUnitNumber.SelectedItem.ToString(), out int selectedItem))
                return;
            byte numberOfUnit = (byte)(selectedItem);

            //pozadavek na zmenu prodlevy odesilani proudu
            if (control == cbCurrentConsumption)
            { 
                //vyber instrukce pro zmenu nastaveni prodlevy odeslanych proudu
                unitInstruction ui = Packet.unitInstruction.prodleva_odesilani_zmerenych_proudu;

                //extrakce doby prodlevy ze stringu stisknuteho a prevod na byte
                string value = cbCurrentConsumption.SelectedItem.ToString();
                string numericString = value.Split(' ')[0];
                int myValue = int.Parse(numericString)/10;
                byte data = (byte)myValue;

                //ulozeni instrukce
                addUnitInstruction(ui, numberOfUnit, data);
            }
            //pozadavek na restart procesoru ridici jednotky
            else if (control == btnResetProcessor)
            {
                //vyber instrukce pro restart ridici jednotky
                unitInstruction ui = Packet.unitInstruction.restart_jednotky;

                //vlozeni patricnych dat
                byte data = 0x01;

                //ulozeni instrukce
                addUnitInstruction(ui, numberOfUnit, data);
            }
            //pozadavek na restarh H mustku
            else if (control == btnResetH)
            {
                //vyber instrukce pro restart H-mustku ridici jednotky
                unitInstruction ui = Packet.unitInstruction.restart_H_mustku;

                //vlozeni patricnych dat
                byte data = 0x01;

                //ulozeni instrukce
                addUnitInstruction(ui, numberOfUnit, data);
            }
            //pozadavek na vypnuti napajeni
            else if (control == btnPowerOff)
            {
                //vyber instrukce pro nastaveni zdroje
                unitInstruction ui = Packet.unitInstruction.nastaveni_zdroje;

                //data pro vypnuti zdroje
                byte data = 0x00;

                //ulozeni instrukce
                addUnitInstruction(ui, numberOfUnit, data);
            }
            //pozadavek na zapnuti napajeni
            else if (control == btnPowerOn)
            {
                //vyber instrukce pro nastaveni zdroje
                unitInstruction ui = Packet.unitInstruction.nastaveni_zdroje;

                //data pro spusteni zdroje
                byte data = 0x01;

                //ulozeni instrukce
                addUnitInstruction(ui, numberOfUnit, data);
            }
            //pozadavek na ziskani dat z ridici jednotky
            else if (control == btnReadUnitInstruction)
            {
                //vyber instrukce pro precteni aktualniho stavu
                unitInstruction ui = Packet.unitInstruction.precteni_stavu_jednotky;

                //data pro precteni aktualniho stavu
                byte data = 0x01;

                //ulozeni instrukce
                addUnitInstruction(ui, numberOfUnit, data);
            }

            //data byla ulozena, nyni vymazani jednotky a zrusit zaktivneni tlacitek
            cbUnitNumber.SelectedIndex = -1;
            CheckStates();

            //vyvolani eventu, aby byl vytvoren paket a data zaslana
            UnitInstructionEventClick?.Invoke(this, e);
        }

        /// <summary>
        /// Ulozeni dat k zaslani konkretni ridici jednotce
        /// </summary>
        /// <param name="type">Typ instrukce</param>
        /// <param name="numberOfUnit">Cislo jednotky</param>
        /// <param name="data1">Data do prvniho bytu</param>
        public void addUnitInstruction(unitInstruction type, byte numberOfUnit, byte data1)
        {
            newUnit.Add(new SetNewUnitData { Type = type, NumberOfUnit = numberOfUnit, Data = data1 });
        }

    }

    /// <summary>
    /// Trida k uchovani dat pro zmenu nastaveni usekovych jednotek
    /// </summary>
    public class SetNewUnitData
    {
        public unitInstruction Type { get; set; }
        public byte NumberOfUnit { get; set; }
        public byte Data { get; set; }
    }
}

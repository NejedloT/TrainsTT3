using ControlLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace TestDesignTT
{
    public partial class UCTurnouts : UserControl
    {
        //Event handler znacici, ze byla zvolena data pro vyhybky
        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler TurnoutButtonSendClick;

        public List<Turnouts> turnouts = new List<Turnouts>();

        public UCTurnouts()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Vymazani dat v jednotlivych polich
        /// </summary>
        public void clearData()
        {
            cbIdUnit1.SelectedIndex = -1;
            cbIdUnit2.SelectedIndex = -1;
            cbIdUnit3.SelectedIndex = -1;
            cbIdUnit4.SelectedIndex = -1;
            cbIdUnit5.SelectedIndex = -1;
            cbTurnout1.SelectedIndex = -1;
            cbTurnout2.SelectedIndex = -1;
            cbTurnout3.SelectedIndex = -1;
            cbTurnout4.SelectedIndex = -1;
            cbTurnout5.SelectedIndex = -1;
            cbValue1.SelectedIndex = -1;
            cbValue2.SelectedIndex = -1;
            cbValue3.SelectedIndex = -1;
            cbValue4.SelectedIndex = -1;
            cbValue5.SelectedIndex = -1;
            //btnSave.SelectedIndex = -1;
            disableButtons();
            saveButtonReady();
            //addButtonReady();
        }

        /// <summary>
        /// Deaktivace vsech comboboxu, ktere nejsou potreba po vynulovani dat
        /// </summary>
        private void disableButtons()
        {
            cbIdUnit2.Enabled = false;
            cbIdUnit3.Enabled = false;
            cbIdUnit4.Enabled = false;
            cbIdUnit5.Enabled = false;
            cbTurnout1.Enabled = false;
            cbTurnout2.Enabled = false;
            cbTurnout3.Enabled = false;
            cbTurnout4.Enabled = false;
            cbTurnout5.Enabled = false;
            cbValue1.Enabled = false;
            cbValue2.Enabled = false;
            cbValue3.Enabled = false;
            cbValue4.Enabled = false;
            cbValue5.Enabled = false;
            btnSave.Enabled = false;
        }

        /// <summary>
        /// Testovani, zdali ma byt aktivni tlacitko na ulozeni dat.
        /// Tlacitko pro ulozeni dat je aktivni vzdy, kdyz je vyplnen cely radek, jinak je neaktivni
        /// </summary>
        private void saveButtonReady()
        {
            btnSave.Enabled = true;
            if (cbIdUnit1.SelectedIndex < 0)
                btnSave.Enabled = false;
            if (cbIdUnit1.SelectedIndex > -1 && cbValue1.SelectedIndex < 0)
                btnSave.Enabled = false;
            if (cbIdUnit2.SelectedIndex > -1 && cbValue2.SelectedIndex < 0)
                btnSave.Enabled = false;
            if (cbIdUnit3.SelectedIndex > -1 && cbValue3.SelectedIndex < 0)
                btnSave.Enabled = false;
            if (cbIdUnit4.SelectedIndex > -1 && cbValue4.SelectedIndex < 0)
                btnSave.Enabled = false;
            if (cbIdUnit5.SelectedIndex > -1 && cbValue5.SelectedIndex < 0)
                btnSave.Enabled = false;
        }

        #region Definice eventu po stisknuti jednotlivych comboboxu a povoleni nasledujiciho comboboxu
        private void btnClear_Click(object sender, EventArgs e)
        {
            clearData();
        }

        private void cbIdUnit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbIdUnit1.SelectedIndex > -1)
                cbTurnout1.Enabled = true;
            saveButtonReady();
        }

        private void cbIdUnit2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbIdUnit2.SelectedIndex > -1)
                cbTurnout2.Enabled = true;
            saveButtonReady();
        }

        private void cbIdUnit3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbIdUnit3.SelectedIndex > -1)
                cbTurnout3.Enabled = true;
            saveButtonReady();
        }

        private void cbIdUnit4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbIdUnit4.SelectedIndex > -1)
                cbTurnout4.Enabled = true;
            saveButtonReady();
        }

        private void cbIdUnit5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbIdUnit5.SelectedIndex > -1)
                cbTurnout5.Enabled = true;
            saveButtonReady();
        }

        private void cbTurnout1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTurnout1.SelectedIndex > -1)
                cbValue1.Enabled = true;
            saveButtonReady();
        }

        private void cbTurnout2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTurnout2.SelectedIndex > -1)
                cbValue2.Enabled = true;
            saveButtonReady();
        }

        private void cbTurnout3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTurnout3.SelectedIndex > -1)
                cbValue3.Enabled = true;
            saveButtonReady();
        }

        private void cbTurnout4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTurnout4.SelectedIndex > -1)
                cbValue4.Enabled = true;
            saveButtonReady();
        }

        private void cbTurnout5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTurnout5.SelectedIndex > -1)
                cbValue5.Enabled = true;
            saveButtonReady();
        }

        private void cbValue1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbValue1.SelectedIndex > -1)
                cbIdUnit2.Enabled = true;
            saveButtonReady();
        }

        private void cbValue2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbValue2.SelectedIndex > -1)
                cbIdUnit3.Enabled = true;
            saveButtonReady();
        }

        private void cbValue3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbValue3.SelectedIndex > -1)
                cbIdUnit4.Enabled = true;
            saveButtonReady();
        }

        private void cbValue4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbValue4.SelectedIndex > -1)
                cbIdUnit5.Enabled = true;
            saveButtonReady();
        }

        private void cbValue5_SelectedIndexChanged(object sender, EventArgs e)
        {
            saveButtonReady();
        }
        #endregion

        /// <summary>
        /// Vyplnena data jsou ulozena a je z nich vytvoren novy objekt
        /// Tato data jsou pote v hlavni forme zpracovana a je z nich vytvoren paket a jsou odeslana
        /// </summary>
        /// <param name="sender">Event pro stisknuti tlacitka na ulozeni dat</param>
        /// <param name="e">Event pro stisknuti tlacitka na ulozeni dat</param>
        private void btnSave_Click(object sender, EventArgs e)
        {

            //preddefinovane hodnoty
            ComboBox[] cbValues = { cbValue1, cbValue2, cbValue3, cbValue4, cbValue5 };
            ComboBox[] cbUnits = { cbIdUnit1, cbIdUnit2, cbIdUnit3, cbIdUnit4, cbIdUnit5 };
            ComboBox[] cbTurnouts = { cbTurnout1, cbTurnout2, cbTurnout3, cbTurnout4, cbTurnout5 };

            UInt32 id;
            //byte value = 0x00;

            for (int i = 0; i < 5; i++)
            {
                //testovani, zdali je radek vyplnen
                if (!(cbValues[i].SelectedIndex > -1))
                    break;

                //zvoleni polohy konkretniho bitu
                byte value = (byte)(0x01 << cbTurnouts[i].SelectedIndex);
                //id = 0x381 + Convert.ToUInt32(cbUnits[i].Text, 16);

                //id ridici jednotky
                id = Convert.ToUInt32(cbUnits[i].Text, 16);

                //zjisteni, zdali jiz data pro jednotku existuji
                Turnouts matchingTurnouts = turnouts.FirstOrDefault(t => t.UnitID == id);

                if (matchingTurnouts != null)
                {
                    //pridani vybrane pozice do jiz ziskane hodnoty
                    matchingTurnouts.Change = (byte)(value | matchingTurnouts.Change);

                    //nastaveni bitu pro nastaveni polohu
                    if (cbValues[i].SelectedIndex != 0)
                        matchingTurnouts.Position |= (byte)(1 << cbTurnouts[i].SelectedIndex);
                }
                else
                {
                    //pridani novych dat
                    byte pos = 0x00;
                    if (cbValues[i].SelectedIndex != 0)
                        pos |= (byte)(1 << cbTurnouts[i].SelectedIndex);

                    addTurnout(id, value, pos);
                }
            }

            //vyvolani eventu, ze data byla ulozena
            TurnoutButtonSendClick?.Invoke(this, e);
        }

        /// <summary>
        /// Ulozeni dat pro zmenu vyhybek
        /// </summary>
        /// <param name="id">ID jednotky</param>
        /// <param name="change">Pozice, ktera se ma zmenit</param>
        /// <param name="turnPosition">Hodnota na pozici "change"</param>
        public void addTurnout(UInt32 id, byte change, byte turnPosition)
        {
            turnouts.Add(new Turnouts { UnitID = id, Change = change, Position = turnPosition });
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace TestDesignTT
{
    public partial class UCTurnoutsMulti : UserControl
    {
        //Event handler znacici, ze byla zvolena data pro vyhybky
        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler MultiTurnoutButtonAddClick;

        public List<Turnouts> turnouts = new List<Turnouts>();

        public UCTurnoutsMulti()
        {
            InitializeComponent();
            ClearData();

            IEnumerable<int> UnitIDs = ControlLogic.SearchLogic.GetTurnoutIDs();
            foreach (int id in UnitIDs)
            {
                cbUnit.Items.Add(id);
            }
        }

        /// <summary>
        /// Vymazani dat v jednotlivych polich
        /// </summary>
        public void ClearData()
        {
            cbUnit.SelectedIndex = -1;
            tbChoose.Text = string.Empty;
            tbChange.Text = string.Empty;
            checkAddButton();
        }

        /// <summary>
        /// Testovani, zdali je mozne ulozit vepsana data
        /// </summary>
        public void checkAddButton()
        {
            if (tbChange.TextLength == 8)
            {
                if (tbChoose.Text.Length == 8)
                {
                    if (cbUnit.SelectedIndex > -1)
                    {
                        btnSave.Enabled = true;
                        return;
                    }
                }
            }
            btnSave.Enabled = false;

        }
        /// <summary>
        /// Stisknutim tlacitka je zavolana metoda, ktera smaze veskera data
        /// </summary>
        /// <param name="sender">Event stisknuti tlacitka na smazani vepsanych dat</param>
        /// <param name="e">Event stisknuti tlacitka na smazani vepsanych dat</param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearData();
        }

        /// <summary>
        /// Metoda, ktera umoznuje psani bytu do pole
        /// Testuje, zdali jsou vepsany pouze 0 a 1
        /// Pro nastaveni bitovych pozic vyhybek, ktere se maji prenastavit
        /// </summary>
        /// <param name="sender">Event handler pro psani do pole na vybrani vyhybek k prehozeni</param>
        /// <param name="e">Event handler pro psani do pole na vybrani vyhybek k prehozeni</param>
        private void tbChoose_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\b' || e.KeyChar == '\u007F')
            {
                return;
            }

            //OVereni, jestli je stisknuta klavesa 1 nebo 0
            if (e.KeyChar != '0' && e.KeyChar != '1')
            {
                e.Handled = true;  //Zablokovat stisknuti klaves
            }

            //Test, ze je delka 8 bitu
            string input = tbChoose.Text + e.KeyChar;
            if (input.Length > 8)
            {
                e.Handled = true;  //Zablokovat stisknuti klaves
            }
        }

        /// <summary>
        /// Metoda, ktera umoznuje psani bytu do pole
        /// Testuje, zdali jsou vepsany pouze 0 a 1
        /// Pro nastaveni jiz vybranych vyhybek vlevo/vpravo
        /// </summary>
        /// <param name="sender">Event handler pro psani do pole nastaveni hodnot danych vyhybek k prehozeni</param>
        /// <param name="e">Event handler pro psani do pole nastaveni hodnot danych vyhybek k prehozeni</param>
        private void tbChange_KeyPress(object sender, KeyPressEventArgs e)
        {
            //povoleni mazani znaku
            if (e.KeyChar == '\b' || e.KeyChar == '\u007F')
            {
                return;
            }

            //OVereni, jestli je stisknuta klavesa 1 nebo 0
            if (e.KeyChar != '0' && e.KeyChar != '1')
            {
                e.Handled = true;  // Block the key press.
            }

            //Test, ze je delka 8 bitu
            string input = tbChange.Text + e.KeyChar;
            if (input.Length > 8)
            {
                e.Handled = true;  // Block the key press.
            }
        }

        #region Combobox a pole eventy
        private void cbUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkAddButton();
        }

        private void tbChoose_TextChanged(object sender, EventArgs e)
        {
            checkAddButton();
        }

        private void tbChange_TextChanged(object sender, EventArgs e)
        {
            checkAddButton();
        }
        #endregion

        /// <summary>
        /// Akce po stiusknuti tlacitka ulozit
        /// Zadana data jsou ulozena a je vyvolan event, aby data byla zpracovana dale
        /// </summary>
        /// <param name="sender">Event stisknutim tlacitka</param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            //UInt32 id = 0x381 + Convert.ToUInt32(tbChange.Text, 16);
            
            if (!int.TryParse(cbUnit.SelectedItem.ToString(), out int selectedItem))
                return;

            UInt32 id = (UInt32)selectedItem;
            //UInt32 id = UInt32.Parse((string)cbUnit.SelectedItem);
            byte choose = Convert.ToByte(tbChoose.Text, 2);
            byte position = Convert.ToByte(tbChange.Text, 2);

            //turnouts.Add(new Turnouts { UnitID = id, Change = choose, Position = position });

            addTurnout(id, choose, position);
           
            MultiTurnoutButtonAddClick?.Invoke(this, e);

            // nasazena jednotka odberu 00110000 01100000 → 0011 | 0000011 → 0x183 (3 a 3)
            // nasazena jednotka prestavniku - pro zapis: 01110000 00100000 → 0111 | 0000001 → 0x381 (7 a 1) - pro cteni
            // nasazena jednotka prestavniku - pro cteni: 10000000 00100000 → 1000 | 0000001 → 0x401 (8 a 1) - pro cteni
        }

        public void addTurnout(UInt32 id, byte change, byte turnPosition)
        {
            turnouts.Add(new Turnouts { UnitID = id, Change = change, Position = turnPosition });
        }
    }
}

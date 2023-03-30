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
    public partial class UCMultiTurnout : UserControl
    {
        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler MultiTurnoutButtonAddClick;

        public List<Turnouts> turnouts = new List<Turnouts>();

        public UCMultiTurnout()
        {
            InitializeComponent();
        }

        public void ClearData()
        {
            cbUnit.SelectedIndex = -1;
            tbChoose.Text = string.Empty;
            tbChange.Text = string.Empty;
            checkAddButton();
        }

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

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearData();
        }

        private void tbChoose_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\b' || e.KeyChar == '\u007F')
            {
                return;
            }

            // Check if the key is a valid binary digit.
            if (e.KeyChar != '0' && e.KeyChar != '1')
            {
                e.Handled = true;  // Block the key press.
            }

            // Check if the input is exactly 8 characters long.
            string input = tbChoose.Text + e.KeyChar;
            if (input.Length > 8)
            {
                e.Handled = true;  // Block the key press.
            }
        }

        private void tbChange_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\b' || e.KeyChar == '\u007F')
            {
                return;
            }

            // Check if the key is a valid binary digit.
            if (e.KeyChar != '0' && e.KeyChar != '1')
            {
                e.Handled = true;  // Block the key press.
            }

            // Check if the input is exactly 8 characters long.
            string input = tbChange.Text + e.KeyChar;
            if (input.Length > 8)
            {
                e.Handled = true;  // Block the key press.
            }
        }

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

        private void btnSave_Click(object sender, EventArgs e)
        {
            //UInt32 id = 0x381 + Convert.ToUInt32(tbChange.Text, 16);
            //UInt32 id = Convert.ToUInt32((UInt32)cbUnit.SelectedItem);
            UInt32 id = UInt32.Parse((string)cbUnit.SelectedItem);
            byte choose = Convert.ToByte(tbChoose.Text, 2);
            byte position = Convert.ToByte(tbChange.Text, 2);

            addTurnout(id, choose, position);
           
            /*
            this.Change = Convert.ToByte(tbChange.Text, 2);
            this.Choose = Convert.ToByte(tbChoose.Text, 2);
            this.UnitID = 0x380 + Convert.ToUInt32(cbUnit.Text, 16);
            */
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestDesignTT
{
    public partial class UCSettings : UserControl
    {
        UCUnitSet uCUnitSet = new UCUnitSet();
        UCTurnoutsSettings uCTurnoutSet = new UCTurnoutsSettings();

        public UCSettings()
        {
            InitializeComponent();
        }

        private void DisplayInstance(UserControl uc)
        {
            if (!(Controls.Contains(uc)))
            {
                Controls.Add(uc);
                uc.Dock = DockStyle.Fill;
                uc.BringToFront();

            }
            else
            {
                uc.BringToFront();
            }
        }

        private void btnUnitSetting_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCUnitSet);
        }

        private void btnTurnoutSetting_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCTurnoutSet);
        }
    }
}

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
        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler UnitInstructionEventClick;

        public List<SetNewUnitData> newUnit = new List<SetNewUnitData>();

        public UCUnitSet()
        {
            InitializeComponent();
            CheckStates();
        }

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

            cbCurrentConsumption.Enabled = bb;
            btnPowerOff.Enabled = bb;
            btnPowerOn.Enabled = bb;
            btnResetProcessor.Enabled = bb;
            btnResetH.Enabled = bb;
            btnReadUnitInstruction.Enabled = bb;
            
        }

        private void Control_Click(object sender, EventArgs e)
        {
            var control = sender as Control;

            if (control == cbCurrentConsumption)
            {
                unitInstruction ui = Packet.unitInstruction.prodleva_odesilani_zmerenych_proudu;
                byte data0 = 0x02;

                string value = cbCurrentConsumption.SelectedItem.ToString();

                // Extract the numeric portion of the string (i.e. "500")
                string numericString = value.Split(' ')[0];

                // Parse the numeric string into an integer
                int myValue = int.Parse(numericString)/10;

                byte data1 = (byte)myValue;
                addUnitInstruction(ui, data0, data1);
            }
            else if (control == btnResetProcessor)
            {
                unitInstruction ui = Packet.unitInstruction.restart_jednotky;
                byte data0 = 0x01;
                byte data1 = 0x01;
                addUnitInstruction(ui, data0, data1);
            }
            else if (control == btnResetH)
            {
                unitInstruction ui = Packet.unitInstruction.restart_H_mustku;
                byte data0 = 0x03;
                byte data1 = 0x01;
                addUnitInstruction(ui, data0, data1);
            }
            else if (control == btnPowerOff)
            {
                unitInstruction ui = Packet.unitInstruction.nastaveni_zdroje;
                byte data0 = 0x04;
                byte data1 = 0x00;
                addUnitInstruction(ui, data0, data1);
            }
            else if (control == btnPowerOn)
            {
                unitInstruction ui = Packet.unitInstruction.nastaveni_zdroje;
                byte data0 = 0x04;
                byte data1 = 0x01;
                addUnitInstruction(ui, data0, data1);
            }
            else if (control == btnReadUnitInstruction)
            {
                unitInstruction ui = Packet.unitInstruction.precteni_stavu_jednotky;
                byte data0 = 0x10;
                byte data1 = 0x01;
                addUnitInstruction(ui, data0, data1);
            }

            UnitInstructionEventClick?.Invoke(this, e);
        }

        public void addUnitInstruction(unitInstruction unit, byte data0, byte data1)
        {
            newUnit.Add(new SetNewUnitData { Unit = unit, Data0 = data0, Data1 = data1 });
        }

    }

    public class SetNewUnitData
    {
        public unitInstruction Unit { get; set; }
        public byte Data0 { get; set; }
        public byte Data1 { get; set; }
    }
}

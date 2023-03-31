using ControlLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using TrainTTLibrary;

namespace TestDesignTT
{
    public partial class UCLokomotives : UserControl
    {

        // Event Handler, ktery znaci, ze doslo k pozadavku na rozjeti ci zastaveni lokomotivy
        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler ChangeOfTrainData;

        //seznam lokomotiv, ktere jsou vlozeny v jinem radku
        private static HashSet<string> addedLocomotives = new HashSet<string>();

        //list s daty na zmenu lokomotivy
        public List<ChangeTrainData> trainDataChange = new List<ChangeTrainData>();
        public UCLokomotives()
        {
            InitializeComponent();
            hideAll();
        }

        /// <summary>
        /// Smazani vsech radku z layoutu po inicializaci
        /// </summary>
        public void hideAll()
        {
            for (int i = 0; i < tableLayoutPanel1.RowCount; i++)
            {
                tableLayoutPanel1.RowStyles[i] = new RowStyle(SizeType.Absolute, 0);
                checkStartStopButton();
            }
        }

        /// <summary>
        /// Metoda, ktera zjisti, ktere vlaky jsou jiz v tabulce zobrazeny a nemohou byt dale vlozeny
        /// Nasledne jsou do kliknuteho comboboxu vlozeny nova data
        /// </summary>
        /// <param name="sender">Event vyvolany vybranim nejake lokomotivy</param>
        /// <param name="e"></param>
        private void updateHashLoco(object sender, EventArgs e)
        {
            ComboBox combo = sender as ComboBox;

            //zjisteni indexu comboboxu s vlakem
            int rowIndex = tableLayoutPanel1.GetRow(combo);

            //smazani zaznamu vsech lokomotiv
            addedLocomotives.Clear();

            for (int i = 0; i < tableLayoutPanel1.RowCount; i++)
            {
                if (i == rowIndex)
                    continue;

                //pokud je zvoleny dany vlak, pridej ho do listu a nebude ho mozne znovu zvolit
                ComboBox cbTrain = tableLayoutPanel1.GetControlFromPosition(1, i) as ComboBox;
                if (cbTrain.SelectedIndex > -1)
                    addedLocomotives.Add(cbTrain.SelectedItem.ToString());
            }

            for (int i = 0; i < tableLayoutPanel1.RowCount; i++)
            {
                if (i == rowIndex)
                {
                    //vymaz data pro dany vlak a nasun nova (nova data jsou data vlaku, ktere nebyly jeste vybrany)
                    ComboBox cbTrain = tableLayoutPanel1.GetControlFromPosition(1, i) as ComboBox;
                    cbTrain.Items.Clear();
                    for (int j = 0; j < LocomotiveInfo.listOfLocomotives.Count; j++)
                    {
                        string loco = Packet.UnderLineToGap(LocomotiveInfo.listOfLocomotives[j].Name);

                        if (!addedLocomotives.Contains(loco))
                        {
                            cbTrain.Items.Add(loco);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Metoda vyvolana stisknutim tlacitka pro pridani nove lokomotivy
        /// </summary>
        /// <param name="sender">Event na stisknuti tlacitka pro pridani lokomotivy</param>
        /// <param name="e">Event na stisknuti tlacitka pro pridani lokomotivy</param>
        private void btnAddTrain_Click(object sender, EventArgs e)
        {
            //otestovani moznosti pro spusteni lokomotivy a zda jsou pritomna vsechna data umoznujici spusteni lokomotivy
            checkStartStopButton();

            //nacti velikost radku
            int[] rowHeights = tableLayoutPanel1.GetRowHeights();

            /*
            for (int i = 0; i < tableLayoutPanel1.RowCount; i++)
            {
                ComboBox cbTrain = tableLayoutPanel1.GetControlFromPosition(1, i) as ComboBox;
                if (cbTrain.SelectedIndex > -1)
                    addedLocomotives.Add(cbTrain.SelectedItem.ToString());
            }
            */

            for (int i = 0; i < tableLayoutPanel1.RowCount; i++)
            {
                if (rowHeights[i] < 0.5)
                {
                    //zobraz novy radek
                    tableLayoutPanel1.RowStyles[i] = new RowStyle(SizeType.Absolute, 35);
                    break;
                }
            }
        }

        #region Buttons Start/Stop click events
        private void btnStartStop1_Click(object sender, EventArgs e)
        {
            startStopAction(sender, e);
        }

        private void btnStartStop2_Click(object sender, EventArgs e)
        {
            startStopAction(sender, e);
        }

        private void btnStartStop3_Click(object sender, EventArgs e)
        {
            startStopAction(sender, e);
        }

        private void btnStartStop4_Click(object sender, EventArgs e)
        {
            startStopAction(sender, e);
        }

        private void btnStartStop5_Click(object sender, EventArgs e)
        {
            startStopAction(sender, e);
        }

        private void btnStartStop6_Click(object sender, EventArgs e)
        {
            startStopAction(sender, e);
        }

        private void btnStartStop7_Click(object sender, EventArgs e)
        {
            startStopAction(sender, e);
        }

        private void btnStartStop8_Click(object sender, EventArgs e)
        {
            startStopAction(sender, e);
        }

        private void btnStartStop9_Click(object sender, EventArgs e)
        {
            startStopAction(sender, e);
        }
        #endregion

        #region Comboboxes for train and speed, click events and logic

        private void cbTrain1_Click(object sender, EventArgs e)
        {
            updateHashLoco(sender,e);
        }

        private void cbTrain2_Click(object sender, EventArgs e)
        {
            updateHashLoco(sender, e);
        }

        private void cbTrain3_Click(object sender, EventArgs e)
        {
            updateHashLoco(sender, e);
        }

        private void cbTrain4_Click(object sender, EventArgs e)
        {
            updateHashLoco(sender, e);
        }

        private void cbTrain5_Click(object sender, EventArgs e)
        {
            updateHashLoco(sender, e);
        }

        private void cbTrain6_Click(object sender, EventArgs e)
        {
            updateHashLoco(sender, e);
        }

        private void cbTrain7_Click(object sender, EventArgs e)
        {
            updateHashLoco(sender, e);
        }

        private void cbTrain8_Click(object sender, EventArgs e)
        {
            updateHashLoco(sender, e);
        }

        private void cbTrain9_Click(object sender, EventArgs e)
        {
            updateHashLoco(sender, e);
        }
        private void cbTrain1_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbSpeed1_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbTrain2_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbSpeed2_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbTrain3_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbSpeed3_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbTrain4_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbSpeed4_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbTrain5_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbSpeed5_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbTrain6_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbSpeed6_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbTrain7_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbSpeed7_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbTrain8_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbSpeed8_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbTrain9_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbSpeed9_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }
        #endregion

        /// <summary>
        /// Metoda, ktera zkouma logiku spusteni/zastaveni lokomotivy
        /// Pokud nema lokomotiva vsechna data, nebude umozneno jeji spusteni a button bude neaktivni
        /// </summary>
        private void checkStartStopButton()
        {
            for (int i = 0; i < 8; i++)
            {
                //ziskani vlastnosti comboboxu a tlacitek
                ComboBox cbTrain = tableLayoutPanel1.GetControlFromPosition(1, i) as ComboBox;
                ComboBox cbSpeed = tableLayoutPanel1.GetControlFromPosition(2, i) as ComboBox;
                Button btnStartStop = tableLayoutPanel1.GetControlFromPosition(4, i) as Button;

                //je zvolen vlak a jsou vyplnena data? Povol stisknuti tlacitka Start
                if ((cbSpeed.SelectedIndex > -1) && (cbTrain.SelectedIndex > -1))
                {
                    if (btnStartStop.Text != "Stop")
                    {
                        btnStartStop.Enabled = true;
                        btnStartStop.ForeColor = Color.DarkOliveGreen;
                        btnStartStop.Font = new Font(btnStartStop.Font, FontStyle.Bold);
                    }
                }

                //button Start bude neaktivni
                else
                {
                    btnStartStop.Enabled = false;
                    //btnStartStop.ForeColor = SystemColors.ControlDark;
                }
            }

            //omezeni mozneho poctu lokomotiv pro rizeni
            int[] rowHeights = tableLayoutPanel1.GetRowHeights();
            if (rowHeights[8] > 20)
            {
                btnAddLocomotive.Enabled = false;
            }
            else
                btnAddLocomotive.Enabled = true;
        }

        /// <summary>
        /// Ulozi data ktera byla pritomna po stisknuti tlacitka Start/Stop
        /// Vyvola Event, aby tato data byla dale zpracovana
        /// </summary>
        /// <param name="sender">Event vyvolany stisknutim tlaticka Start/Stop</param>
        /// <param name="e">Event vyvolany stisknutim tlaticka Start/Stop</param>
        private void startStopAction(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button == null)
                return; // Exit if sender is not a button

            //radek daneho tlacitka
            int rowIndex = tableLayoutPanel1.GetRow(button);

            //ziskani vlastnosti comboboxu a tlacitek
            ComboBox cbTrain = tableLayoutPanel1.GetControlFromPosition(1, rowIndex) as ComboBox;
            ComboBox cbSpeed = tableLayoutPanel1.GetControlFromPosition(2, rowIndex) as ComboBox;
            CheckBox cbReverse = tableLayoutPanel1.GetControlFromPosition(3, rowIndex) as CheckBox;

            //Zmena textu Start/Stop a deaktivace ostatnich tlacitek/comboboxu, pokud vlak jede
            if (button.Text == "Start")
            {
                button.Text = "Stop";
                cbTrain.Enabled = false;
                cbSpeed.Enabled = false;
                cbReverse.Enabled = false;
                button.ForeColor = Color.Red;
                button.Font = new Font(button.Font, FontStyle.Bold);
            }
            else
            {
                button.Text = "Start";
                cbTrain.Enabled = true;
                cbReverse.Enabled = true;
                cbSpeed.Enabled = true;
                button.ForeColor = Color.DarkOliveGreen;
                button.Font = new Font(button.Font, FontStyle.Bold);
                //Stop locomotive
            }

            //ziskani rychlosti z comboboxu
            byte speedToByte;
            byte.TryParse(cbSpeed.SelectedItem.ToString(), out speedToByte);

            //Najit konkretni lokomotivu z konfiguracniho seznamu
            ChangeTrainData matchingTrain = trainDataChange.FirstOrDefault(t => t.Lokomotive == cbTrain.SelectedItem.ToString());

            //pokud data jsou nulova (mela by byt na 100%, ale overeni, ze nebudou dva pozadavky na stejnou lokomotivu)
            if (matchingTrain != null)
            {
                matchingTrain.Speed = speedToByte;
                matchingTrain.Reverze = cbReverse.Checked ? true : false;
                matchingTrain.StartStop = button.Text == "Stop" ? true : false;
            }

            //prirad data pro lokomotivu
            else
            {
                addTrainData(cbTrain.SelectedItem.ToString(), speedToByte, cbReverse.Checked ? true : false, button.Text == "Stop" ? true : false);
            }

            //vyvolej event handler
            ChangeOfTrainData?.Invoke(this, e);
        }

        /// <summary>
        /// Ulozeni dat, ktera budou dale zpracovana a zaslana
        /// </summary>
        /// <param name="locomotive">Jmeno lokomotivy</param>
        /// <param name="speed">Rychlost vlaku</param>
        /// <param name="reverze">Smer vlaku</param>
        /// <param name="startStop">Zdali je pozadavek na rozjezd vlaku nebo zastaveni</param>
        public void addTrainData(string locomotive, byte speed, bool reverze, bool startStop)
        {
            trainDataChange.Add(new ChangeTrainData { Lokomotive = locomotive, Speed = speed, Reverze = reverze, StartStop = startStop });
        }
    }

    /// <summary>
    /// Trida slouzici pro ulozeni dat
    /// </summary>
    public class ChangeTrainData
    {
        public string Lokomotive;
        public byte Speed;
        public bool Reverze;
        public bool StartStop;
    }
}

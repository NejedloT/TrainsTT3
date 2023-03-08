using System.ComponentModel;
using System.Xml;

namespace TestDesignTT
{
    public partial class UCEditMoving : UserControl
    {
        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler ButtonEditMoving;


        //public BindingList<DataTimetable> timetable = new BindingList<DataTimetable>();

        //private List<DataForTimetable> dataForTimetable = new List<DataForTimetable>();

        public UCEditMoving()
        {
            InitializeComponent();
            //loadTimetamble();
        }

        public void loadData(BindingList<MovingInTimetamble> timetableMoving, BindingList<DataTimetable> timetableData)
        {
            timetableMovingList(timetableMoving);
            changeSize();
        }

        private void timetableMovingList(BindingList<MovingInTimetamble> timetableMoving)
        {
            if (timetableMoving.Count < 1)
                labelTitle.Text = "Žádné vlaky momentálně nejedou";
            else
                labelTitle.Text = "Zde lze upravit data jedoucích vlaků";

            BindingList<MovingInTimetamble> onScreen = new BindingList<MovingInTimetamble>();

            for (int i = 0; i < timetableMoving.Count; i++)
            {

                onScreen.Add(timetableMoving[i]);
                //onScreen[i].FinalStation.Name = Packet.UnderLineToGap(onScreen[i].FinalStation.Name);
                //onScreen[i].StartStation.Name = Packet.UnderLineToGap(onScreen[i].StartStation.Name);
                //timetable[i].HeaderCell.Value = String.Format("{0}", row.Index + 1);
            }

            dgwMoving.DataSource = onScreen;
            dgwMoving.Columns[6].Visible = false;
            dgwMoving.Columns[4].Visible = false;
            dgwMoving.Columns[3].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";

            changeSize();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {

        }

        public void changeSize()
        {
            panel5.Height = dgwMoving.ColumnHeadersHeight + dgwMoving.RowCount * dgwMoving.RowTemplate.Height;


            if (dgwMoving.RowCount < 1)
                panel5.Height = 0;
        }

        private void btnEditMoving_Click(object sender, EventArgs e)
        {
            ButtonEditMoving?.Invoke(this, e);
        }
    }
}

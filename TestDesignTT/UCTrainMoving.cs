using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace TestDesignTT
{
    public partial class UCTrainMoving : UserControl
    {

        public BindingList<MovingInTimetamble> movingTimetable = new BindingList<MovingInTimetamble>();

        public UCTrainMoving()
        {
            InitializeComponent();
        }

        public void loadMovingTimetamble(BindingList<MovingInTimetamble> timetable)
        {
            if (timetable.Count < 1)
                titleDisplayData.Text = "Žádné vlaky momentálně nejedou";
            else
                titleDisplayData.Text = "Tyto vlaky jsou momentálně na trase";

            //dataGridView1.DataSource = timetable;


            BindingList<MovingInTimetamble> onScreen = new BindingList<MovingInTimetamble>();

            for (int i = 0; i < timetable.Count; i++)
            {

                onScreen.Add(timetable[i]);
                //onScreen[i].FinalStation.Name = Packet.UnderLineToGap(onScreen[i].FinalStation.Name);
                //onScreen[i].StartStation.Name = Packet.UnderLineToGap(onScreen[i].StartStation.Name);
                //timetable[i].HeaderCell.Value = String.Format("{0}", row.Index + 1);
            }


            dataGridView1.DataSource = onScreen;
            dataGridView1.Columns[4].Visible = false;
            dataGridView1.Columns[6].Visible = false;
            dataGridView1.Columns[3].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";

            changeSize();
        }

        public void changeSize()
        {
            panel1.Height = dataGridView1.ColumnHeadersHeight + dataGridView1.RowCount * dataGridView1.RowTemplate.Height;
            //if (panel1.Height < 
            if (dataGridView1.RowCount < 1)
                panel1.Height = 0;
        }

    }
}

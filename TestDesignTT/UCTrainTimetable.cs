using System.ComponentModel;
using System.Data;

namespace TestDesignTT
{
    public partial class UCTrainTimetable : UserControl
    {

        public BindingList<DataTimetable> timetable = new BindingList<DataTimetable>();


        public UCTrainTimetable()
        {
            InitializeComponent();

        }

        public void loadTimetamble(BindingList<DataTimetable> timetable)
        {
            if (timetable.Count < 1)
                titleDisplayData.Text = "Žádné vlaky nebyly bohužel načteny";
            else
                titleDisplayData.Text = "Zde lze upravit data v jízdním řádu";

            dataGridView1.DataSource = timetable;

            BindingList<DataTimetable> onScreen = new BindingList<DataTimetable>();

            for (int i = 0; i < timetable.Count; i++)
            {
                onScreen.Add(timetable[i]);
            }

            dataGridView1.DataSource = onScreen;
            dataGridView1.Columns[3].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";
            
            changeSize();
        }

        public void changeSize()
        {
            panel2.Height = dataGridView1.ColumnHeadersHeight + (dataGridView1.RowCount+1) * dataGridView1.RowTemplate.Height;

            if (dataGridView1.RowCount < 1)
                panel2.Height = 0;
        }
    }
}

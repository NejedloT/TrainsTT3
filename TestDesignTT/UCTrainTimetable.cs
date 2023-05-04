using System.ComponentModel;
using System.Data;

namespace TestDesignTT
{
    public partial class UCTrainTimetable : UserControl
    {
        //list s daty pro zobrazeni jizdniho radu
        public BindingList<DataTimetable> timetable = new BindingList<DataTimetable>();


        public UCTrainTimetable()
        {
            InitializeComponent();

        }

        //zobrazeni dat jizdniho radu
        public void loadTimetamble(BindingList<DataTimetable> timetable)
        {
            //Zobrazeni textu v zavislosti na velikost dat nacteneho jizdniho radu
            if (timetable.Count < 1)
                titleDisplayData.Text = "No timetable have been loaded.";
            else
                titleDisplayData.Text = "These are loaded timetable data.";

            //pridani zdroje
            dataGridView1.DataSource = timetable;
            BindingList<DataTimetable> onScreen = new BindingList<DataTimetable>();

            //vloz data do jednotlivych radku
            for (int i = 0; i < timetable.Count; i++)
            {
                onScreen.Add(timetable[i]);
            }

            //zobraz data
            dataGridView1.DataSource = onScreen;
            dataGridView1.Columns[3].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";
            
            changeSize();
        }

        /// <summary>
        /// Metoda pro upravu velikost radku a sloupcu
        /// </summary>
        public void changeSize()
        {
            panel2.Height = dataGridView1.ColumnHeadersHeight + (dataGridView1.RowCount+1) * dataGridView1.RowTemplate.Height;

            if (dataGridView1.RowCount < 1)
                panel2.Height = 0;
        }
    }
}

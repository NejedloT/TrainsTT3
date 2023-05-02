using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDesignTT
{
    public class DataTimetable
    {
        public string Name { get; set; } // vlastnost typu vlaku (v podstatě pouze pracovní název spoje)
        [DisplayName("Start station")] // Aby se v dataGridView napsala mezi slovy mezera
        public string StartStation { get; set; } // vlastost říkající od kud vlak vyjíždí
        [DisplayName("Final station")]
        public string FinalStation { get; set; } // vlastost říkající kam vlak jede
        public DateTime Departure { get; set; } // čas odjezdu

        public int Speed { get; set; }

        public bool Reverse { get; set; }

        public string Line;

        public DataTimetable() { }

        public DataTimetable(string line, DateTime departure) // konstruktor třídy
        {

            if (String.IsNullOrEmpty(line)) // nejprve se zjistí zda řádek z jízdního řádu není prázdný
            {
                return; // pokud je, rovnou konstrktor končí
            }

            Line = line;

            String[] data = line.Split(';'); // rozdělím data s oddělovačem ";"

            Name = data[0]; // přiřazení informací z řádku do jednotlivých proměných ( string trimuji a časová data parsuji) 
            StartStation = new string(data[1].Trim());
            FinalStation = new string(data[2].Trim());
            Departure = departure;
            Speed = int.Parse(data[3]);
            //Reverse = (data[5].Trim() == "ahead") ? false : true;
            if (data[4].Trim() == "ahead")
            {
                Reverse = false;
            }
            else
            {
                Reverse = true;
            }
        }

        public DataTimetable(string type, string startSection, string finalSection, DateTime departure)
        {
            Name = type;
            StartStation = startSection;
            FinalStation = finalSection;
            Departure = departure;

            Line = String.Format("{0};{1};{2};{3}", type, startSection, finalSection, departure.ToString("HH:mm:ss"));
        }
    }

    public class MovingInTimetamble
    {
        public string Type { get; set; } // vlastnost typu vlaku (v podstatě pouze pracovní název spoje)
        
        [DisplayName("Start station")] // Aby se v dataGridView napsala mezi slovy mezera
        public string StartStation { get; set; } // vlastost říkající od kud vlak vyjíždí
        [DisplayName("Final station")]
        public string FinalStation { get; set; } // vlastost říkající kam vlak jede
        public DateTime Departure { get; set; } // čas odjezdu

        public int Speed { get; set; }
        
        public bool Reverse { get; set; }
        [DisplayName("Reverse")]


        public uint WaitTime { get; set; }

        //public string Line { get; set; }

        public MovingInTimetamble(string type, string startSection, string finalSection, DateTime departure, int speed, bool reverse, uint waittime)
        { 
            Type = type;
            StartStation = startSection;
            FinalStation = finalSection;
            Departure = departure;
            Speed = speed;
            Reverse = reverse;
            WaitTime = waittime;
            //Line = String.Format("{0};{1};{2};{3};{4};{5}", type, startSection, finalSection, departure.ToString("HH:mm:ss"), speed, timeOnTrack);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ControlLogic
{
    public class SearchLogic
    {
        private static XDocument xdoc = new XDocument();

        /// <summary>
        /// Inicializace - nacteni konfiguracniho souboru
        /// </summary>
        public static void InitSearch()
        {
            xdoc = XDocument.Load("C:\\Users\\Tomáš\\Documents\\ZCU_FEL\\v1_diplomka\\TestDesign\\TestDesignTT\\ControlLogic\\conf_kolejiste.xml");
        }

        /// <summary>
        /// Vrati jednotlive definovane ID desek vyhybek z konfiguracniho souboru
        /// </summary>
        /// <returns>ID desek vyhybek</returns>
        public static IEnumerable<int> GetTurnoutIDs()
        {
            if (xdoc.Root == null)
                xdoc = XDocument.Load("C:\\Users\\Tomáš\\Documents\\ZCU_FEL\\v1_diplomka\\TestDesign\\TestDesignTT\\ControlLogic\\conf_kolejiste.xml");

            IEnumerable<int> unitValues = xdoc.Descendants("unit")
                .Select(u => Convert.ToInt32(u.Value))
                .Distinct();

            IEnumerable<int> sortedTurnoutIDs = unitValues.OrderBy(x => x);

            return sortedTurnoutIDs;

        }

        /// <summary>
        /// Metoda pro zjisteni ID usekovvych jednotek z konfiguracniho souboru
        /// </summary>
        /// <returns>ID usekovych jednotek</returns>
        public static IEnumerable<int> GetModulesId()
        {
            if (xdoc.Root == null)
                xdoc = XDocument.Load("C:\\Users\\Tomáš\\Documents\\ZCU_FEL\\v1_diplomka\\TestDesign\\TestDesignTT\\ControlLogic\\conf_kolejiste.xml");

            IEnumerable<int> moduleIds = xdoc.Descendants("section")
            .Select(x => (int)x.Element("moduleid"))
            .Distinct();

            IEnumerable<int> sortedUnitIDs = moduleIds.OrderBy(x => x);
            return sortedUnitIDs;
        }


        public static IEnumerable<XElement> GetFromElements(Trains train)
        {
            if (xdoc.Root == null)
                xdoc = XDocument.Load("C:\\Users\\Tomáš\\Documents\\ZCU_FEL\\v1_diplomka\\TestDesign\\TestDesignTT\\ControlLogic\\conf_kolejiste.xml");

            return xdoc.Descendants("from").Where(e => (string)e.Attribute("id") == train.currentPosition);
        }

        /// <summary>
        /// Metoda, která hledá kritické úseky, když se vlak nachází v kritickém úseku vjezdu do kritického úseku nebo
        /// </summary>
        /// <param name="currentPosition">Soucasna poloha vlaku</param>
        /// <param name="previousPosition">Nadchazejici poloha vlaku</param>
        /// <param name="final">Cilova stanice/kolej</param>
        /// <returns></returns>
        public static IEnumerable<XElement> GetCriticalReservedSection(Trains train)
        {
            List<string> stationNames = SearchLogic.GetStationNames();
            if (!(stationNames.Contains(train.finalPosition)) && train.finalPosition != null && (train.circuit == 0 || train.circuit == 4 || train.circuit == 7))
            {

                var matchingToElements = xdoc.Descendants("to")
                .Where(t => t.Attribute("id").Value == train.finalPosition
                && t.Element("parts")?.Descendants().Select(p => p.Value).Contains(train.lastPosition) == true // check if previousPosition is present
                && t.Element("parts")?.Descendants().Select(p => p.Value).Contains(train.currentPosition) == true // check if currentPosition is present
                && t.Element("parts")?.Descendants().Select(p => p.Value).ToList().IndexOf(train.lastPosition) + 1 == t.Element("parts")?.Descendants().Select(p => p.Value).ToList().IndexOf(train.currentPosition) // check if previousPosition is earlier than currentPosition and they are next to each other
                );
                /*
                .Select(t => t.Element("parts").Value)
                .ToList();
                */

                if (matchingToElements.Any())
                {
                    return matchingToElements;
                }
            }

            var matchingToElem = xdoc.Descendants("to")
                .Where(t => t.Element("parts")?.Descendants().Select(p => p.Value).Contains(train.lastPosition) == true // check if previousPosition is present
                && t.Element("parts")?.Descendants().Select(p => p.Value).Contains(train.currentPosition) == true // check if currentPosition is present
                && t.Element("parts")?.Descendants().Select(p => p.Value).ToList().IndexOf(train.lastPosition) + 1 == t.Element("parts")?.Descendants().Select(p => p.Value).ToList().IndexOf(train.currentPosition) // check if previousPosition is earlier than currentPosition and they are next to each other
                );
            /*
                .Select(t => t.Element("parts"))
                .ToList();
            */

            return matchingToElem;
        }


        /// <summary>
        /// Najde mozne finalni stanice, ktere jsou zadefinovane
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static IEnumerable<XElement> GetFinalStation(string position)
        {
            return xdoc.Descendants("toCircuit")
                .Where(toCircuit => toCircuit.Elements()
                .Any(item => (string)item == position));
        }

        /// <summary>
        /// Metoda, ktera hleda v konfiguracnim souboru fromCircuit
        /// </summary>
        /// <param name="train">vlak</param>
        /// <returns>Mozne cilove stanice</returns>
        public static IEnumerable<XElement> getFromCircuit(Trains train)
        {
            var x = xdoc.Descendants("fromCircuit")
                          .Where(e => (string)e.Attribute("id") == train.circuit.ToString())
                          .Elements("toCircuit");
            return x;
        }

        /// <summary>
        /// Metoda, ktera najde moznou finalni kolej dle aktualniho okruhu
        /// Pouze v zavislosti na aktualnim okruhu
        /// </summary>
        /// <param name="train">konkretni vlak, ktery si vybere uzivatel</param>
        /// <param name="wantedName">Pozadovana kolej ve vybrane stanici</param>
        /// <returns></returns>
        public static IEnumerable<XElement> GetFinalTrackOutside(Trains train, string wantedName)
        {
            return xdoc.Descendants("fromCircuit")
            .Where(fc => (string)fc.Attribute("id") == train.circuit.ToString())
            .Elements("toCircuit")
            .Where(tc => (string)tc.Attribute("name") == wantedName)
            .Elements();
        }

        /// <summary>
        /// Metoda ktera najde moznou finalni kolej dle aktualni polohy
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <param name="previousPosition"></param>
        /// <param name="finalStation"></param>
        /// <returns></returns>
        public static bool GetFinalTrackInside(string currentPosition, string previousPosition, string finalStation)
        {
            var matchingToElements = xdoc.Descendants("to")
                .Where(t => t.Attribute("id")?.Value == finalStation) // check for specific "to" value
                .Where(t => t.Element("parts")?.Descendants().Select(p => p.Value).Contains(previousPosition) == true // check if previousPosition is present
                    && t.Element("parts")?.Descendants().Select(p => p.Value).Contains(currentPosition) == true // check if currentPosition is present
                    && t.Element("parts")?.Descendants().Select(p => p.Value).ToList().IndexOf(previousPosition) + 1 == t.Element("parts")?.Descendants().Select(p => p.Value).ToList().IndexOf(currentPosition) // check if previousPosition is earlier than currentPosition and they are next to each other
                )
                .Any();

            return matchingToElements;
        }


        /// <summary>
        /// Metoda, ktera najde v konfiguracnim souboru nasledujici usek
        /// Vstupem do metody je vybrana polohy na screenu pro aktualizaci polohy manualne
        /// </summary>
        /// <param name="position">String hodnota soucasne pozice, kterou vybral uzivatel</param>
        /// <returns>Vedlejsi pozici z obou stran</returns>
        public static IEnumerable<string> GetNextPositions(string position)
        {
            return xdoc.Descendants("section")
           .Where(e => (string)e.Attribute("id") == position)
           .Elements()
           .Where(e => e.Name.LocalName == "prevsec" || e.Name.LocalName == "nextsec" || e.Name.LocalName == "prevsections" || e.Name.LocalName == "nextsections")
           .SelectMany(e =>
           {
               if (e.Name.LocalName == "prevsec" || e.Name.LocalName == "nextsec")
                   return new[] { (string)e };

               else
                   return e.Elements().Select(x => (string)x);
           });
        }

        /// <summary>
        /// Metoda, ktera vraci finalni stanici v zavislosti na poloze
        /// </summary>
        /// <param name="currentPosition">soucasna poloha</param>
        /// <param name="previousPosition">minula poloha</param>
        /// <returns></returns>
        public static IEnumerable<string> GetFinalStationInCritical(string currentPosition, string previousPosition)
        {
            var matchingToElements = xdoc.Descendants("to")
            .Where(t => t.Element("parts")?.Descendants().Select(p => p.Value).Contains(previousPosition) == true // check if previousPosition is present
        && t.Element("parts")?.Descendants().Select(p => p.Value).Contains(currentPosition) == true // check if currentPosition is present
        && t.Element("parts")?.Descendants().Select(p => p.Value).ToList().IndexOf(previousPosition) + 1 == t.Element("parts")?.Descendants().Select(p => p.Value).ToList().IndexOf(currentPosition) // check if previousPosition is earlier than currentPosition and they are next to each other
    )
    .Select(t => t.Element("toFinal").Value)
    .ToList();

            return matchingToElements;
        }

        /// <summary>
        /// Metoda, ktera nalezne mozne cesty na nadrazi v zavislosti na minule poloze
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <param name="previousPosition"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetFinalStationOutside(string currentPosition, string previousPosition)
        {
            var matchingFromStartElements = xdoc.Descendants("fromStartOutside")
        .Where(fs => fs.Descendants("items").Descendants().Select(p => p.Value).Contains(previousPosition)
                    && fs.Descendants("items").Descendants().Select(p => p.Value).Contains(currentPosition)
                    && fs.Descendants("items").Descendants().Select(p => p.Value).ToList().IndexOf(previousPosition) + 1 == fs.Descendants("items").Descendants().Select(p => p.Value).ToList().IndexOf(currentPosition))
        .Select(fs => fs.Element("toFinalPosition").Value)
        .ToList();

            return matchingFromStartElements;
        }

        /// <summary>
        /// Metoda, ktera nalezne pocatecni stanici v pripade, ze je vlak v kritickem useku
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <param name="previousPosition"></param>
        /// <returns></returns>
        //public static string GetStartStationInCritical(string currentPosition, string previousPosition)
        public static IEnumerable<string> GetStartStationInCritical(string currentPosition, string previousPosition)
        {
            var matchingToElements = xdoc.Descendants("to")
            .Where(t => t.Element("parts")?.Descendants().Select(p => p.Value).Contains(previousPosition) == true // check if previousPosition is present
        && t.Element("parts")?.Descendants().Select(p => p.Value).Contains(currentPosition) == true // check if currentPosition is present
        && t.Element("parts")?.Descendants().Select(p => p.Value).ToList().IndexOf(previousPosition) + 1 == t.Element("parts")?.Descendants().Select(p => p.Value).ToList().IndexOf(currentPosition) // check if previousPosition is earlier than currentPosition and they are next to each other
    )
    .Select(t => t.Element("fromStart").Value)
    .ToList();

            return matchingToElements;
        }

        /// <summary>
        /// Metoda, ktera nalezne pocatecni stanici, kdyz je vlak na "otevrenem prostoru"
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <param name="previousPosition"></param>
        /// <returns></returns>
        /// t.Element("items")?.

        //public static string GetStartStationOutside(string currentPosition, string previousPosition)
        public static IEnumerable<string> GetStartStationOutside(string currentPosition, string previousPosition)
        {
            var matchingFromStartElements = xdoc.Descendants("fromStartOutside")
        .Where(fs => fs.Descendants("items").Descendants().Select(p => p.Value).Contains(previousPosition)
                    && fs.Descendants("items").Descendants().Select(p => p.Value).Contains(currentPosition)
                    && fs.Descendants("items").Descendants().Select(p => p.Value).ToList().IndexOf(previousPosition) + 1 == fs.Descendants("items").Descendants().Select(p => p.Value).ToList().IndexOf(currentPosition))
        .Select(fs => fs.Attribute("id").Value)
        .ToList();

            return matchingFromStartElements;
        }

        /// <summary>
        /// Vraci aktualni okruh v zavislosti na poloze vlaku
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <returns></returns>
        public static int GetCurrentCircuit(string currentPosition)
        {
            XElement section = xdoc.Descendants("section")
                          .FirstOrDefault(e => e.Attribute("id").Value == currentPosition);

            if (section != null)
                return int.Parse(section.Element("circuit").Value);

            else
                return -1; // error
        }

        /// <summary>
        /// Metoda, ktera zjisti orientaci po okruhu pri aktualizaci JSONu
        /// Dle posledni polohy a smeru jizdy urci orientaci, cimz je umoznena spravna orintace pri naslednem rizeni
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <param name="previousPosition"></param>
        /// <returns></returns>
        public static string GetOrientation(string currentPosition, string previousPosition)
        {
            string orientation = null;

            IEnumerable<string> getOrientationNext = xdoc.Descendants("section")
           .Where(e => (string)e.Attribute("id") == currentPosition)
           .Elements()
           .Where(e => e.Name.LocalName == "nextsec" || e.Name.LocalName == "nextsections")
           .SelectMany(e =>
           {
               if (e.Name.LocalName == "nextsec")
                   return new[] { (string)e };

               else
                   return e.Elements().Select(x => (string)x);
           });

            IEnumerable<string> getOrientationPrev = xdoc.Descendants("section")
           .Where(e => (string)e.Attribute("id") == currentPosition)
           .Elements()
           .Where(e => e.Name.LocalName == "prevsec" || e.Name.LocalName == "prevsections")
           .SelectMany(e =>
           {
               if (e.Name.LocalName == "prevsec")
                   return new[] { (string)e };

               else
                   return e.Elements().Select(x => (string)x);
           });

            if (getOrientationNext.Contains(previousPosition))
            {
                orientation = "prevConnection";
            }
            else
            {
                orientation = "nextConnection";
            }

            return orientation;
        }

        /// <summary>
        /// Metoda, ktera slouzi k ziskani vsech finalnich koleji na nadrazich
        /// </summary>
        /// <returns>List s nazvy jendotlivymi konecnymi stanicemi</returns>
        public static List<string> GetAllStationTracks()
        {
            List<string> items = xdoc.Descendants("stationTracks")
                .Elements()
                .Elements()
                .SelectMany(e => e.Elements())
                .Select(e => e.Value)
                .ToList();

            return items;
        }

        public static List<string> GetStationNames()
        {
            List<string> stationNames = xdoc.Descendants("nameOfStation")
            .Select(e => e.Attribute("id").Value)
            .ToList();

            return stationNames;
        }

        /// <summary>
        /// Metoda, ktera vraci mozne stanice pro nadrazi (bez kontroly, zdali tam vede cesta)
        /// </summary>
        /// <param name="stationName">Nazev nadrazi</param>
        /// <returns>list vsech moznych stanic pro dane nadrazi</returns>
        public static List<string> GetTracksForStation(string stationName)
        {
            List<string> items = xdoc.Descendants("nameOfStation")
            .Where(t => t.Attribute("id").Value == stationName)
            .Elements("items")
            .Elements()
            .Select(e => e.Value)
            .ToList();

            return items;
        }

        /// <summary>
        /// Metoda, ktera slouzi k ziskani cilovych stanic z okruhu lokomotivy (mimo nadrazi)
        /// </summary>
        /// <param name="finalStation"></param>
        /// <returns></returns>
        public static int GetFinalStationCircuit(string finalStation)
        {
            var circuitValue = xdoc.Descendants("stationTracks")
                .Elements("nameOfStation")
                .Where(e => e.Attribute("id")?.Value == finalStation)
                .Select(x => (int)x.Element("circuit"))
                .FirstOrDefault();

            return circuitValue;
        }

        public static string getStationNameFromTrack(string trackName)
        {
            string stationName = xdoc.Descendants("items")
        .Where(e => e.Elements().Any(c => c.Value == trackName))
        .Select(e => e.Parent.Attribute("id").Value)
        .FirstOrDefault();
            return stationName;
        }

        /// <summary>
        /// Metoda pro definovani softwarovych dorazu vyhybek z konfiguracniho souboru
        /// </summary>
        /// <returns></returns>
        public static List<XElement> GetTurnoutStopDefinitions()
        {
            if (xdoc.Root == null)
                xdoc = XDocument.Load("C:\\Users\\Tomáš\\Documents\\ZCU_FEL\\v1_diplomka\\TestDesign\\TestDesignTT\\ControlLogic\\conf_kolejiste.xml");

            List<XElement> turnoutsStops = xdoc.Descendants("turnoutStopDefinitions")
                .Elements()
                .ToList();

            return turnoutsStops;
        }
    }
}

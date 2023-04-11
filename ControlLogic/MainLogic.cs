using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Timers;
using System.Threading;
using System.Net.WebSockets;
using TrainTTLibrary;
using System.ComponentModel;

namespace ControlLogic
{
    public class MainLogic
    {
        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler<LocomotiveDataSend> LocomotiveDataEvent;

        public List<LocomotiveDataSend> dataToSendLoco = new List<LocomotiveDataSend>();

        //list s odbery proudu
        private static List<CurrentDrain> currentDrain = new List<CurrentDrain>();

        //list rezervovanych useku
        private static List<ReservedSections> reservedSections = new List<ReservedSections>();

        //list obsahujici nejdulezitejsi data o kazdem vlaku, po kazdem cyklu ukladana
        private static List<Trains> trainsList = new List<Trains>();

        //konfiguracni dokument s namapovanym kolejistem
        private static XDocument xdoc = new XDocument();
        
        private static TrainDataJSON td = new TrainDataJSON();

        //timer, po jake dobe bude probihat kontrola kolejiste
        private static System.Timers.Timer timerCheck;

        public static bool testing = true;

        private static System.Timers.Timer timeToStop;
        private static Trains currentTrain;

        private static object locking = new object();

        //static void Main(string[] args)
        public static void Initialization()
        {
            //nastaveni timeru, ktery kontroluje logiku

            //StartTimers();

            //nacist posledni ulozena data
            TrainDataJSON td = new TrainDataJSON();
            trainsList = td.LoadJson();

            //nacteni configurace namapovaneho kolejiste
            xdoc = XDocument.Load("C:\\Users\\Tomáš\\Documents\\ZCU_FEL\\v1_diplomka\\TestDesign\\TestDesignTT\\ControlLogic\\conf_kolejiste.xml");
            //ještě load sections a locomotives

            foreach (Trains train in trainsList)
            {
                //jestli vlak jede:
                AddCurrentDrain(train.id, train.currentPosition);
                AddCurrentDrain(train.id, train.lastPosition);
            }

            //LocomotiveDataEvent?.Invoke(this, e);
        }



        //nastaveni timeru, ktery kontroluje logiku


        /*
        private static void StartTimers()
        {
            timerCheck = new System.Timers.Timer(1000);

            timerCheck.Elapsed += ControlLogic_Tick;

            timerCheck.AutoReset = true;

            timerCheck.Enabled = true;
        }
        */

        //Funkce ridici celou logiku zabezpeceni

        public static void controlLogic()
        //private static void ControlLogic_Tick(object source, System.Timers.ElapsedEventArgs e)
        {
            //pokud se pri inicializaci nenacetly data, nacti je jeste jednou
            if (!(trainsList.Count > 0))
            {
                TrainDataJSON td = new TrainDataJSON();
                trainsList = td.LoadJson();
            }

            if (testing)
            {
                AddCurrentDrain(0x11, "id1");
                AddCurrentDrain(0x11, "id198");
                AddReservedSections(0x31, "id12");
            }


            //postupna kontrola vsech jedoucich vlaku
            foreach (Trains train in trainsList)
            {
                //jestli vlak jede:
                if (train.move == 1)
                {
                    //kontrola odberu proudu daneho vlaku
                    CheckCurrentDrain(train);

                    //kontrola pozice vlaku, aktualizace
                    UpdatePosition(train);

                    //kontrola pred kolizi
                    CheckColision(train);

                    //kontrola kritickych casti
                    if (CheckCritical(train))
                        FindRoute(train);
                }
            }

            //kontrola vsech vlaku cekajicich na vyjeti
            foreach (Trains train in trainsList)
            {
                //jestli vlak jede:
                if (train.move == 2)
                {
                    //kontrola rezervovanych useku
                    if (!TrainWantsToMove(train, true))
                        continue;

                    if (!CheckColisionToMove(train))
                        continue;

                    if (!SameCircuitToMove(train))
                        continue;

                    if (reservedSections.Any(rs => rs.TrainIdReserved == train.id))
                    {
                        //nastav vyhybky
                    }
                    //rozjed vlak
                }
            }

            td.SaveJson(trainsList);
            //storeJson.SaveJson(trainsList);

            
        }
        public static bool SameCircuitToMove(Trains train)
        {
            var trainInfo = trainsList.Where(t => t.circuit == train.circuit && t.mapOrientation != train.mapOrientation).ToList();
            if (trainInfo.Any())
            {
                return false;
            }
            return true;
        }

        public static bool CheckColisionToMove(Trains train)
        {
            //neni odber proudu v nadchazejicim useku od jineho vlaku a jestli v nadchazejicim useku neni jiny vlak?
            if ((!(currentDrain.Any(cd => cd.Section == train.nextPosition))) && !(trainsList.Any(tl => tl.currentPosition != train.nextPosition)))
            {
                return true;
            }
            return false;
        }

        public static bool TrainWantsToMove(Trains train, bool test)
        {
            //najdi kriticke useky
            var trainInfo = xdoc.Descendants("crit")
            .Where(e => e.Attribute("last")?.Value == train.lastPosition && e.Attribute("current")?.Value == train.currentPosition).ToList();

            //ma vlak rezervovane useky?
            if (reservedSections.Any(rs => rs.TrainIdReserved == train.id))
            {
                int countReserved = reservedSections.Count(t => t.TrainIdReserved == train.id);

                //list obsahujici informace, jestli ma testovany vlak jet pres rezervovany usek, ktery byl rezervovan drive
                var reservationCheck = reservedSections
                        .Where(rs => rs.TrainIdReserved != train.id &&
                            rs.Section == reservedSections
                                .Where(p => p.TrainIdReserved == train.id && reservedSections.IndexOf(p) < reservedSections.IndexOf(rs))
                                .Select(p => p.Section)
                                .FirstOrDefault())
                        .ToList();

                //je neco v rezervovanem useku nebo neni volna cilova kolej?
                if (countReserved != 0 && reservationCheck != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            //nema rezervovane useky
            else if (trainInfo.Any())
            {
                //kontrola jestli vjizdim do kritickeho useku
                if (test)
                {
                    //najdi cestu, pokud jsi v kritickem useku
                    CheckCritical(train);

                    //rekurze, cesta byla nalezena nebo konec a cesta momentalne zadna neni
                    return TrainWantsToMove(train, false);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e, Trains train)
        {
            // Stop the timer
            timeToStop.Stop();

            // Do something after the time interval (e.g., resume the train)
            train.move = 2;

            ((System.Timers.Timer)sender).Dispose();

            //TODO
            //Stop the train!
            Locomotive locomotive = new Locomotive(train.name);

            //bool reverze = train.reverse;

            MainLogic mainLogic = new MainLogic();

            mainLogic.OnMyEvent(new LocomotiveDataSend { Loco = locomotive });

        }

        //
        public static void InCritical(Trains train, bool fintrack)
        {

            //int countReserved = reservedSections.Count(t => t.TrainIdReserved == train.id);

            //list obsahujici informace, jestli ma testovany vlak jet pres rezervovany usek, ktery byl rezervovan drive
            var reservationCheck = reservedSections
                .Where(rs => rs.TrainIdReserved != train.id &&
                    reservedSections
                        .Where(p => p.TrainIdReserved == train.id && reservedSections.IndexOf(p) > reservedSections.IndexOf(rs))
                        .Any(p => p.Section == rs.Section))
                .ToList();


            //je neco v rezervovanem useku nebo neni volna cilova kolej?
            //if (countReserved != 0 && (reservationCheck.Count != 0 || !fintrack))
            if (reservationCheck.Count != 0 || !fintrack)
            {
                //stop the train
                //critical = 1

                //train.move = "2";

                timeToStop = new System.Timers.Timer(3000);

                currentTrain = train;
                // Set the event handler for the Elapsed event
                timeToStop.Elapsed += (sender, e) => Timer_Elapsed(sender, e, currentTrain);

                // Start the timer
                timeToStop.Start();
            }

            else
            {
                //pokracuj v jizde
                //nastav vyhybky pokud budou
                train.critical = false;
            }
        }

        public static void FindRoute(Trains train)
        {
            if (!(trainsList.Count > 0))
            {
                TrainDataJSON td = new TrainDataJSON();
                trainsList = td.LoadJson();
            }


            //bool testujici volnou cilovou kolej ci ze vlak bude odjizdet
            bool fintrack = false;

            //hodnoty, pokud z nejake koleje ma jet vlak, ale zadna kolej neni volna
            bool potfintrack = false;
            string potToTrack;

            if (new[] { "Beroun", "Karlstejn", "Lhota" }.Contains(train.finalPosition))
            {

                //nacti cesty ze soucasne polohy na cilove koleje
                IEnumerable<XElement> fromElements = xdoc.Descendants("from").Where(e => (string)e.Attribute("id") == train.currentPosition);

                foreach (XElement element in fromElements)
                {
                    foreach (XElement toElement in element.Elements("to"))
                    {
                        //vraci postupne jednotlive id definovanych finalnich koleji
                        string getToId = (string)toElement.Attribute("id");

                        //najdi, jestli ma nejaky vlak stejnou finalni pozici (ta bude zmenena pri vytvoreni prikazu pro jizdu),
                        //takze by se cekalo, nez vlak odjede
                        var occupiedTrack = trainsList.Where(t => t.finalPosition == getToId).ToList();

                        //Nema vlak toto jako finalni kolej (pojede) a zaroven v ni zadny vlak nestoji?
                        if (!(occupiedTrack.Any()) && !(currentDrain.Any(cd => cd.Section == train.finalPosition)))
                        {
                            var reserveSections = toElement.Element("parts")
                               .Elements()
                               .Select(e => e.Value)
                               .ToList();
                            /*
                             * prepared for switches to add
                            var reserveSwitches = toElement.Element("switches")
                               .Elements()
                               .Select(e => e.Value)
                               .ToList();
                            */

                            //vytvor rezervovane useky
                            for (int i = 0; i < reserveSections.Count; i++)
                            {
                                if (i == 1)
                                    AddReservedSections(0x11, "id9");
                                AddReservedSections(train.id, reserveSections[i]);
                            }

                            train.finalPosition = getToId;
                            //test na value
                            fintrack = true;

                            //splneno, konec cyklu
                            goto EndLoop;
                        }
                    }
                }
            }
            else
            {
                //vrati vsechny izolovane_rezervovane useky mezi body soucasna poloha a cilova stanice
                var reserveSections = xdoc.Descendants("from")
                    .Where(f => (string)f.Attribute("id") == train.currentPosition)
                    .Elements("to")
                    .TakeWhile(t => (string)t.Attribute("id") != train.finalPosition)
                    .SelectMany(t => t.Element("parts").Elements("part"))
                    .ToList();
                /*
                 * prepared for switches to add
                var reserveSwitches = toElement.Element("switches")
                   .Elements()
                   .Select(e => e.Value)
                   .ToList();
                */

                //vytvor rezervovane useky
                for (int i = 0; i < reserveSections.Count; i++)
                {
                    AddReservedSections(train.id, reserveSections[i].Value);
                }
                fintrack = true;
            }

        EndLoop:
            //zkontroluj jestli vlak muze jet dal, kdyz uz je URCITE v kritickem useku
            InCritical(train, fintrack);

        }

        public static bool CheckCritical(Trains train)
        {

            //vrati hodnoty, pokud vlak vjizdi do kritickeho useku
            //var trainInfo = xdoc.Descendants("crit").Where(e => e.Attribute("last")?.Value == train.lastPosition && e.Attribute("current")?.Value == train.currentPosition).ToList();
            var trainInfo = xdoc.Descendants("crit")
                .Where(e => (string)e.Element("last") == train.lastPosition && (string)e.Element("current") == train.currentPosition)
                .ToList();
            int alreadyReserved = reservedSections.Count(cd => cd.TrainIdReserved == train.id);

            //vjizdim do kritickeho useku? (ANO = v listu jsou nejake hodnoty) - testuju pouze vjezd
            if (trainInfo.Any() && alreadyReserved < 2)
            {
                //pokud obsahuje cilovou stanici a ne kolej, najdi kolej
                return true;
            }
            return false;
        }

        //kontrola pred kolizi
        public static void CheckColision(Trains train)
        {
            //neni odber proudu v nadchazejicim useku od jineho vlaku a jestli v nadchazejicim useku neni jiny vlak?
            if ((!(currentDrain.Any(cd => cd.Section == train.nextPosition))) && !(trainsList.Any(tl => tl.currentPosition == train.nextPosition)) && !(trainsList.Any(t => t.lastPosition == train.nextPosition)))
            {
                //najdi, jestli nejaky jiny jedouci vlak jeste ma stejnou nextPosition (jedou do stejneho useku)
                var collidingTrainsNext = trainsList.Where(t => t.id != train.id
                                            && t.move == 1
                                            && t.nextPosition == train.nextPosition)
                              .ToList();
                //nema zadny jedouci vlak stejnou nadchazejici polohu?
                if (!(collidingTrainsNext.Any()))
                {
                    //toto je podminka na vic pro jistotu, nemela by byt nutna
                    //najdi, jestli nejaky jiny vlak jeste ma stejnou nextPosition (jedou do stejneho useku)
                    var collidingTrainsCurrent = trainsList.Where(t => t.id != train.id
                            && t.currentPosition == train.currentPosition).ToList();

                    //nema zadny jiny vlak stejnou soucasnou polohu
                    if (!(collidingTrainsCurrent.Any()))
                    {
                        return;
                    }
                }
            }
            //TODO
            //stop the train
            //TrainMotion (train, speed 0)
			//MainLogic ml = new MainLogic();
			
            train.move = 2;
        }

        //metoda pro neustale updatovani pozice vlaku
        public static void UpdatePosition(Trains train)
        {
            //je soucasna poloha cilova?
            if (train.currentPosition == train.finalPosition)
            {

                //pokud je odber proudu pouze v jedinem useku (nepresahuju do vyhybky), tak zastav
                int currentDrainCount = currentDrain.Where(x => x.TrainIdDrain == train.id).Count();
                if (currentDrainCount == 1)
                {
                    train.move = 0;
                    //trainMotion (speed 0)
                }
            }
            else
            {
                //pro ucely testovani, netestuji aktualizaci polohy
                int i = 1;
                //TODO
                //TODO
                if (i == 0) //je odber proudu v nadchazejicim useku?
                {
                    //Doslo ke zmene polohy. Poloha udavajici jako soucasna je jiz minula
                    train.lastPosition = train.currentPosition;

                    //Doslo ke zmene polohy. Poloha udavajici jako budouci (nadchazejici) je jiz soucasna
                    train.currentPosition = train.nextPosition;
                    AddCurrentDrain(train.id, train.currentPosition);

                    //jsou nejake rezervovane sekce?
                    ReservedSections rp = reservedSections.FirstOrDefault(x => x.TrainIdReserved == train.id);
                    if (rp != null)
                    {
                        train.nextPosition = rp.Section;
                    }

                    //pokud je ma poloha (po aktualizaci) finalni, neni zadna dalsi pozice kam jet
                    else if (train.currentPosition == train.finalPosition)
                    {
                        train.nextPosition = null;
                    }

                    else
                    {
                        //najdi novou nasledujici (budouci) polohu z konfiguracniho souboru
                        var x = train.currentPosition;
                        var mapSection = xdoc.Descendants("section")
                            .FirstOrDefault(e => e.Attribute("id")?.Value == train.currentPosition);
                        string nextSecId;
                        if (train.mapOrientation == "nextConnection")
                        {
                            nextSecId = mapSection.Element("nextsec")?.Value;
                        }
                        else
                        {
                            nextSecId = mapSection.Element("prevsec")?.Value;
                        }
                    }
                }
            }
        }

        //testovani a aktualizace odberu proudu jedoucich vlaku - testuji se zde pouze minule a soucasne polohy
        public static void CheckCurrentDrain(Trains train)
        {
            //var trainDrain = currentDrain.Where(cd => cd.TrainIdDrain == train.id).ToList();
            for (int i = 0; i < currentDrain.Count; i++)
            {
                if (train.id == currentDrain[i].TrainIdDrain)
                {
                    //if there is current drain, do ....

                    /*
                    if (!(Je odebírán proud?))  //dodelat v zavislosti na poloze z OccupacySection. Pokud neni odebiran proud, tak odstranit
                    {
                        ReservedSections reserved = reservedSections.Find(x => x.TrainIdReserved == currentDrain[i].TrainIdReserved && x.Section == currentDrain[i].Section);

                        if (reserved != null)
                            reservedSections.Remove(reserved);

                        currentDrain.RemoveAt(i);
                        i--;

                    }
                    */

                    if (testing)
                    {
                        if (i == 1)
                        {
                            currentDrain.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Metoda na pridani useku odberu proudu do seznamu listu
        /// </summary>
        /// <param name="id">ID lokomotivy, ktera je v useku</param>
        /// <param name="section">nazev useku</param>
        public static void AddCurrentDrain(uint id, string section)
        {
            currentDrain.Add(new CurrentDrain { TrainIdDrain = id, Section = section });
        }

        /// <summary>
        /// metoda na pridani rezervovanych useku lokomotivou do seznamu useku
        /// </summary>
        /// <param name="id">ID lokomotivy, ktera je v useku</param>
        /// <param name="section">nazev useku</param>
        public static void AddReservedSections(uint id, string section)
        {
            reservedSections.Add(new ReservedSections { TrainIdReserved = id, Section = section });
        }

        public static List<Trains> GetData()
        {
            return trainsList;
        }

        public static void addNewTrainDataFromClient(string name, string currentPosition, byte speed, bool reverse, string final)
        {

            foreach (Trains train in trainsList)
            {
                if (train.name == name)
                {
                    string orientation = train.mapOrientation;

                    if (!(train.reverse == reverse))
                    {
                        if (train.mapOrientation == "nextConnection")
                            orientation = "prevConnection";
                        else
                            orientation = "nextConnection";
                    }

                    /*
                    XElement toCircuit = xdoc.Descendants("fromCircuit")
                        .Descendants("toCircuit")
                        .FirstOrDefault(tc => tc.Descendants()
                        .Any(d => d.Value == final));
                    */

                    var toCircuit = xdoc.Descendants("fromCircuit")
                        .Where(fc => fc.Attribute("id")?.Value == train.circuit.ToString())
                        .Descendants("toCircuit")
                        .FirstOrDefault(tc => tc.Elements()
                        .Any(e => e.Value == final));

                    string toCircuitName;
                    if (toCircuit != null)
                        toCircuitName = toCircuit.Attribute("name")?.Value;
                    else
                        toCircuitName = final;

                    IEnumerable<XElement> fromElements = xdoc.Descendants("from").Where(e => (string)e.Attribute("id") == train.currentPosition);

                    if (fromElements != null)
                    {
                        foreach (XElement element in fromElements)
                        {
                            foreach (XElement toElement in element.Elements("to"))
                            {

                                var getRoad = toElement.Element("toFinal");
                                if (getRoad.Value == toCircuitName)
                                {

                                    //znam cestu dle chtene cilove polohy a hodnotu circuit z jsonu
                                    var getVarCircuit = toElement.Element("circuit");

                                    int getCircuit = int.Parse(getVarCircuit.Value);



                                    var reserveSections = toElement.Element("parts")
                                       .Elements()
                                       .Select(e => e.Value)
                                       .ToList();

                                    //otestovani zvoleni spravne cesty - pokud jede vlak stejnym smerem, tak nesmi obsahovat list minulou pozici
                                    if (((reserveSections.Contains(train.lastPosition)) && train.reverse != reverse) ||
                                        (!reserveSections.Contains(train.lastPosition) && train.reverse == reverse))
                                    {
                                        /*
                                         * prepared for switches to add
                                        var reserveSwitches = toElement.Element("switches")
                                           .Elements()
                                           .Select(e => e.Value)
                                           .ToList();
                                        */

                                        //vytvor rezervovane useky
                                        for (int i = 0; i < reserveSections.Count; i++)
                                        {
                                            AddReservedSections(train.id, reserveSections[i]);
                                        }

                                        train.circuit = getCircuit;
                                    }


                                }
                            }
                        }
                    }
                    train.currentPosition = currentPosition;
                    train.reverse = reverse;
                    train.speed = speed;
                    train.finalPosition = final;
                    train.mapOrientation = orientation;
                    train.move = 2;
                }
            }
        }

        public static IEnumerable<XElement> GetFromElements(Trains train)
        {
            return xdoc.Descendants("from").Where(e => (string)e.Attribute("id") == train.currentPosition);
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
            return xdoc.Descendants("fromCircuit")
                          .Where(e => (string)e.Attribute("id") == train.circuit.ToString());
        }

        /// <summary>
        /// Metoda, ktera najde moznou finalni stanici dle aktualni stanice
        /// </summary>
        /// <param name="train">konkretni vlak, ktery si vybere uzivatel</param>
        /// <param name="wantedName">Pozadovana kolej ve vybrane stanici</param>
        /// <returns></returns>
        public static IEnumerable<XElement> GetFinalTrack(Trains train, string wantedName)
        {
            /*
            return xdoc.Descendants("fromCircuit")
                .Where(fc => (string)fc.Attribute("id") == train.circuit)
                .Elements("toCircuit")
                .Where(tc => (string)tc.Attribute("name") == wantedName)
                .Elements()
                .Select(e => (string)e)
                .Cast<XElement>();
            */
            return xdoc.Descendants("fromCircuit")
            .Where(fc => (string)fc.Attribute("id") == train.circuit.ToString())
            .Elements("toCircuit")
            .Where(tc => (string)tc.Attribute("name") == wantedName)
            .Elements();
        }

        /// <summary>
        /// Metoda, ktera najde v konfiguracnim souboru nasledujici usek
        /// Vstupem do metody je vybrana polohy na screenu pro aktualizaci polohy manualne
        /// </summary>
        /// <param name="position">String hodnota soucasne pozice, kterou vybral uzivatel</param>
        /// <returns>Vedlejsi pozici z obou stran</returns>
        public static IEnumerable<string> GetNextPositions(string position)
        {
            return xdoc.Descendants()
                .Where(e => e.Name.LocalName == "prevsec" || e.Name.LocalName == "nextsec" || e.Name.LocalName == "prevsections" || e.Name.LocalName == "nextsections")
                .Where(e => e.Ancestors("section").Any(a => (string)a.Attribute("id") == position))
                .Select(e => (string)e)
                .ToList();
        }

        protected void OnMyEvent(LocomotiveDataSend e)
        {
            LocomotiveDataEvent?.Invoke(this, e);
        }
    }

    public class LocomotiveDataSend
    {
        public Locomotive Loco { get; set; }

        public bool Reverze { get; set; }

        public byte Speed { get; set; }

    }

    public class CurrentDrain
    {
        public uint TrainIdDrain { get; set; }
        public string Section { get; set; }


    }

    public class ReservedSections
    {
        public uint TrainIdReserved { get; set; }
        public string Section { get; set; }

    }
}
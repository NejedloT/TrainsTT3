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
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace ControlLogic
{
    public class MainLogic
    {
        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler<LocomotiveDataSend> LocomotiveDataEvent;

        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler<TurnoutsDataSend> TurnoutsDataEvent;

        private static MainLogic instance = null;

        private static readonly object lockingInstance = new object();

        public static readonly object lockingOccupancy = new object();

        private static readonly object lockingLogic = new object();


        public static List<LocomotiveDataSend> dataToSendLoco = new List<LocomotiveDataSend>();

        //list s odbery proudu
        private static List<CurrentDrain> currentDrain = new List<CurrentDrain>();

        //list rezervovanych useku
        public static List<ReservedSections> reservedSections = new List<ReservedSections>();

        public static List<SwitchesChange> switchesChange = new List<SwitchesChange>();

        //list s odbery proudu
        private static List<Section> occupancySections = new List<Section>();

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

        private static List<double> stopwatchValues = new List<double>(); //test mereni rychlosti cyklu


        public static MainLogic GetInstance()
        {
            lock (lockingInstance)
            {
                if (instance == null)
                {
                    instance = new MainLogic();
                }
                return instance;
            }
        }

        public static void InitTrainListAndXdoc()
        {
            //nacist posledni ulozena data
            TrainDataJSON td = new TrainDataJSON();
            trainsList = td.LoadJson();

            //nacteni configurace namapovaneho kolejiste
            xdoc = XDocument.Load("C:\\Users\\Tomáš\\Documents\\ZCU_FEL\\v1_diplomka\\TestDesign\\TestDesignTT\\ControlLogic\\conf_kolejiste.xml");
        }


        public static void Initialization(MainLogic ml)
        {

            instance = ml; //ulozeni instance, aby windows form dokazal chytat eventy

            InitTrainListAndXdoc();

            //ještě load sections a locomotives

            //inicializace pocatecnich podminek
            SetInitialConditions();

            //nastaveni timeru, ktery kontroluje logiku
            StartTimers();

            //pro testovani chovani
            //controlLogic();
        }


        public static void SetInitialConditions()
        {
            foreach (Trains train in trainsList)
            {
                //Vlozeni proudoveho odberu soucasneho a minuleho useku:
                AddCurrentDrain(train.id, train.currentPosition);
                AddCurrentDrain(train.id, train.lastPosition);

                //zjisteni, jestli je vlak v kritickem useku

                /*

                if (train.circuit == 0) //nebo jine kriticke useky
                {
                    
                    //najdi cesty
                    IEnumerable<XElement> critical = GetCriticalReservedSection(train);
                    if (FindRouteInCritical(critical, train))
                        continue;


                }
                */
            }
            //controlLogic();
        }


        /// <summary>
        /// Spusteni timeru, ktery periodicky vykonava logiku zabezpeceni
        /// </summary>
        private static void StartTimers()
        {
            timerCheck = new System.Timers.Timer(350);

            timerCheck.Elapsed += ControlLogic_Tick;

            timerCheck.AutoReset = true;

            timerCheck.Enabled = true;
        }

        /// <summary>
        /// Vypnuti timeru pro logiku zabezpeceni
        /// </summary>
        public static void StopTimers()
        {
            timerCheck.Enabled = false;

            lock (lockingLogic)
            {
                TrainDataJSON td = new TrainDataJSON();
                trainsList = td.LoadJson();
                foreach (Trains train in trainsList)
                {
                    train.move = 0;
                }
            }

            //timeToStop.Enabled = false;
        }

        /// <summary>
        /// Timer, ktery se vyvola pro odpocet pro zastaveni vlaku
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="train"></param>
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

            MainLogic ml = GetInstance();

            dataToSendLoco.Add(new LocomotiveDataSend { Loco = locomotive, Reverze = train.reverse, Speed = (byte)3 });
            ml.OnMyEventLoco(new LocomotiveDataSend { Loco = locomotive, Reverze = train.reverse, Speed = (byte)3 });

        }


        /// <summary>
        /// Funkce ridici celou logiku zabezpeceni
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>

        //public static void controlLogic()
        private static void ControlLogic_Tick(object source, System.Timers.ElapsedEventArgs e)
        {
            //nacti aktualni data o poloze
            //Execution Time Start
            Stopwatch sw = new Stopwatch();
            sw.Start();

            lock (lockingLogic)
            {

                TrainDataJSON td = new TrainDataJSON();
                trainsList = td.LoadJson();

                occupancySections = ProcessDataFromTCP.GetSavedOccupancySection();


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
                            FindRouteOutsideCritical(train);
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

                        MainLogic ml = GetInstance();

                        if (reservedSections.Any(rs => rs.TrainIdReserved == train.id))
                        {
                            for (int i = 0; i < switchesChange.Count(); i++)
                            {
                                if (switchesChange[i].TrainId == train.id)
                                {
                                    uint numbOfUnit = switchesChange[i].NumberOfUnit;
                                    byte turnouts = switchesChange[i].Turnouts;
                                    byte value = switchesChange[i].Value;
                                    ml.OnMyEventTurnout(new TurnoutsDataSend {NumberOfUnit = numbOfUnit, Turnouts = turnouts, Value = value});
                                }
                            }
                        }

                        Locomotive locomotive = new Locomotive(train.name);

                        //bool reverze = train.reverse;

                        train.move = 1;

                        dataToSendLoco.Add(new LocomotiveDataSend { Loco = locomotive, Reverze = train.reverse, Speed = train.speed });
                        ml.OnMyEventLoco(new LocomotiveDataSend { Loco = locomotive, Reverze = train.reverse, Speed = train.speed });
                        //rozjed vlak
                    }
                }


                foreach (Trains train in trainsList)
                {
                    //jestli vlak jede:
                    if (train.move != 0)
                    {
                        td.UpdateTrainData(train.name, (t) =>
                        {
                            t.move = train.move;
                            t.speed = train.speed;
                            t.finalPosition = train.finalPosition;
                            t.currentPosition = train.currentPosition;
                            t.lastPosition = train.lastPosition;
                            t.nextPosition = train.nextPosition;
                            t.circuit = train.circuit;
                            t.critical = train.critical;
                            t.mapOrientation = train.mapOrientation;
                        });
                    }
                }
            }


            //td.SaveJson(trainsList);

            //storeJson.SaveJson(trainsList);

            //Execution Time Stop and save
            sw.Stop();
            double elapsedSeconds = sw.Elapsed.TotalMilliseconds;
            stopwatchValues.Add(elapsedSeconds);
        }

        #region Funkce a metody na testovani jedoucich vlaku (kontrola kolize a proudu, aktualizace polohy, hledani cesty)
        /// <summary>
        /// Testovani a aktualizace odberu proudu jedoucich vlaku - testuji se zde pouze minule a soucasne polohy
        /// </summary>
        /// <param name="train">Aktualni vlak</param>
        public static void CheckCurrentDrain(Trains train)
        {
            List<Section> occupancySec = ProcessDataFromTCP.GetSavedOccupancySection();
            for (int i = 0; i < currentDrain.Count(); i++)
            //for (int i = currentDrain.Count() - 1; i >= 0; i--)
            {
                if (train.id == currentDrain[i].TrainIdDrain && currentDrain[i].Section != null)
                {
                    for (int j = 0; j < occupancySec.Count(); j++)
                    {
                        if (occupancySec[j].Name == currentDrain[i].Section)
                        {
                            if (occupancySections[j].current > 0)
                                continue;
                            else
                            {
                                /*
                                ReservedSections reserved = reservedSections.Find(x => x.TrainIdReserved == currentDrain[i].TrainIdDrain && x.Section == currentDrain[i].Section);

                                if (reserved != null)
                                    reservedSections.Remove(reserved);
                                */

                                currentDrain.RemoveAt(i);
                                i--;
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Metoda pro neustale updatovani pozice vlaku
        /// </summary>
        /// <param name="train"></param>
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
                    td.UpdateTrainData(train.name, (t) =>
                    {
                        t.currentPosition = train.currentPosition;
                        t.lastPosition = train.lastPosition;
                        t.nextPosition = null;
                        t.move = 0;
                        t.circuit = train.circuit;
                        t.critical = train.critical;
                        t.finalPosition = null;
                    });

                    MainLogic ml = GetInstance();

                    Locomotive locomotive = new Locomotive(train.name);

                    dataToSendLoco.Add(new LocomotiveDataSend { Loco = locomotive, Reverze = train.reverse, Speed = 3 });
                    ml.OnMyEventLoco(new LocomotiveDataSend { Loco = locomotive, Reverze = train.reverse, Speed = 3 });

                    return;

                }
            }
            else
            {
                List<Section> occupancySec;
                lock (lockingOccupancy)
                {
                    occupancySec = ProcessDataFromTCP.GetSavedOccupancySection();
                }

                foreach (Section s in occupancySec)
                {
                    if ((train.nextPosition == s.Name) && (s.current > 5))
                    {


                        AddCurrentDrain(train.id, train.nextPosition);

                        //Doslo ke zmene polohy. Poloha udavajici jako soucasna je jiz minula
                        string last = train.currentPosition;
                        string cur = train.nextPosition;

                        train.lastPosition = last;

                        //Doslo ke zmene polohy. Poloha udavajici jako budouci (nadchazejici) je jiz soucasna
                        train.currentPosition = cur;

                        var mapSection = xdoc.Descendants("section")
                                .FirstOrDefault(e => e.Attribute("id")?.Value == train.currentPosition);

                        // Get the value of the `circuit` element
                        int circuit = (int)mapSection.Element("circuit");

                        train.circuit = circuit;

                        string nextSecId;

                        //jsou nejake rezervovane sekce?
                        //ReservedSections rp = reservedSections.FirstOrDefault(x => x.TrainIdReserved == train.id);
                        IEnumerable <ReservedSections> rp = reservedSections.Where(x => x.TrainIdReserved == train.id);
                        if (rp.Count() > 0)
                        {
                            //train.nextPosition = rp.Section;
                            /*
                            
                            List<ReservedSections> reservedSectionsCopy = new List<ReservedSections>(reservedSections);

                            // Iterate over the copy of the collection
                            bool bb = true;
                            for (int i = 0; i < reservedSectionsCopy.Count(); i++)
                            {
                                ReservedSections resSec = reservedSectionsCopy[i];
                                if (train.lastPosition == resSec.Section && train.id == resSec.TrainIdReserved)
                                {
                                    reservedSections.Remove(resSec);
                                    i--;
                                }
                            }
                            */
                            ReservedSections ress = reservedSections.FirstOrDefault(x => x.TrainIdReserved == train.id && train.lastPosition == x.Section);
                            if (ress != null)
                                reservedSections.Remove(ress);


                            ReservedSections rs = reservedSections.FirstOrDefault(x => x.TrainIdReserved == train.id && train.currentPosition != x.Section && train.lastPosition != x.Section);
                            if (rs != null)
                                train.nextPosition = rs.Section;

                            //nextSecId = rs.Section;
                            
                        }

                        //pokud je ma poloha (po aktualizaci) finalni, neni zadna dalsi pozice kam jet
                        else if (train.currentPosition == train.finalPosition)
                        {
                            train.nextPosition = null;
                            nextSecId = null;
                        }

                        else
                        {
                            //najdi novou nasledujici (budouci) polohu z konfiguracniho souboru
                            
                            if (train.mapOrientation == "nextConnection")
                            {
                                nextSecId = mapSection.Element("nextsec")?.Value;
                            }
                            else
                            {
                                nextSecId = mapSection.Element("prevsec")?.Value;
                            }
                            train.nextPosition = nextSecId;
                        }
                        /*
                        td.UpdateTrainData(train.name, (t) =>
                        {
                            t.currentPosition = cur;
                            t.lastPosition = last;
                            t.nextPosition = nextSecId;
                            t.circuit = circuit;
                            t.critical = train.critical;
                            //t.finalPosition = null;
                        });
                        */

                    }

                    //test, jestli jedouci vlak ma alespon v jednom useku odber proudu, jinak zastavit!
                    bool testCurrent = currentDrain.Any(cd => cd.TrainIdDrain == train.id);
                    if (!testCurrent)
                    {
                        MainLogic ml = GetInstance();

                        Locomotive locomotive = new Locomotive(train.name);

                        dataToSendLoco.Add(new LocomotiveDataSend { Loco = locomotive, Reverze = train.reverse, Speed = 0 });
                        ml.OnMyEventLoco(new LocomotiveDataSend { Loco = locomotive, Reverze = train.reverse, Speed = 0 });

                        train.move = 0;
                        td.UpdateTrainData(train.name, (t) =>
                        {
                            t.currentPosition = train.currentPosition;
                            t.lastPosition = train.lastPosition;
                            t.nextPosition = null;
                            t.move = 0;
                            t.circuit = train.circuit;
                            t.critical = train.critical;
                            t.finalPosition = null;
                        });
                    }

                }
            }
        }


        /// <summary>
        /// kontrola pred kolizi
        /// </summary>
        /// <param name="train">aktualni vlak</param>
        public static void CheckColision(Trains train)
        {
            //neni odber proudu v nadchazejicim useku od jineho vlaku a jestli v nadchazejicim useku neni jiny vlak?
            if ((!(currentDrain.Any(cd => cd.Section == train.nextPosition && cd.TrainIdDrain != train.id))) && !(trainsList.Any(tl => tl.currentPosition == train.nextPosition && tl.id != train.id)) && !(trainsList.Any(t => t.lastPosition == train.nextPosition && t.id != train.id)))
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

            MainLogic ml = GetInstance();

            Locomotive locomotive = new Locomotive(train.name);

            dataToSendLoco.Add(new LocomotiveDataSend { Loco = locomotive, Reverze = train.reverse, Speed = 0 });
            ml.OnMyEventLoco(new LocomotiveDataSend { Loco = locomotive, Reverze = train.reverse, Speed = 0 });


            train.move = 2;
        }

        /// <summary>
        /// Testování, jestli jsem v kritickém úseku
        /// </summary>
        /// <param name="train"></param>
        /// <returns></returns>
        public static bool CheckCritical(Trains train)
        {
            var trainInfo = xdoc.Descendants("crit")
                .Where(e => (string)e.Element("last") == train.lastPosition && (string)e.Element("current") == train.currentPosition)
                .ToList();

            int alreadyReserved = reservedSections.Count(cd => cd.TrainIdReserved == train.id);

            //vjizdim do kritickeho useku? (ANO = v listu jsou nejake hodnoty) - testuju pouze vjezd
            if (trainInfo.Any() && alreadyReserved < 1)
            {
                //pokud obsahuje cilovou stanici a ne kolej, najdi kolej
                return true;
            }
            return false;
        }

        /// <summary>
        /// Najde cestu na nadrazi
        /// </summary>
        /// <param name="train"></param>
        public static void FindRouteOutsideCritical(Trains train)
        {
            if (!(trainsList.Count() > 0))
            {
                TrainDataJSON td = new TrainDataJSON();
                trainsList = td.LoadJson();
            }


            //bool testujici volnou cilovou kolej ci ze vlak bude odjizdet
            bool fintrack = false;

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

                            //prepared for switches to add
                            /*
                            var reserveSwitches = toElement.Element("switches")
                               .Elements()
                               .Select(e => e.Value)
                               .ToList();
                            */
                            var switches = toElement.Element("switches");
                            int k = 0;
                            if (switches != null)
                            {
                                var reserveSwitches = switches.Elements()
                                    .Select(unit =>
                                    {
                                        string unitNum = unit.Element("unit").Value;
                                        string turnouts = unit.Element("turnouts").Value;
                                        string value = unit.Element("value").Value;

                                        uint unitN = uint.Parse(unitNum);
                                        byte turn = byte.Parse(turnouts);
                                        byte val = byte.Parse(value);
                                        return new { UnitNum = unitN, Turnouts = turn, Value = val };
                                    })
                                    .ToList();


                                for (int i = 0; i < reserveSwitches.Count(); i++)
                                {
                                    AddSwitchesReservation(train.id, reserveSwitches[i].UnitNum, reserveSwitches[i].Turnouts, reserveSwitches[i].Value);
                                }
                            }


                            //vytvor rezervovane useky
                            for (int i = 0; i < reserveSections.Count(); i++)
                            {
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
                /*
                var reserveSections = xdoc.Descendants("from")
                    .Where(f => (string)f.Attribute("id") == train.currentPosition)
                    .Elements("to")
                    .TakeWhile(t => (string)t.Attribute("id") != train.finalPosition)
                    .SelectMany(t => t.Element("parts").Elements("part"))
                    .ToList();
                */

                var resSections = xdoc.Descendants("from")
                          .Where(f => (string)f.Attribute("id") == train.currentPosition)
                          .Elements("to")
                          .Where(t => (string)t.Attribute("id") == train.finalPosition)
                          .SelectMany(t => t.Element("parts").Elements())
                          .ToList();

                var resSwitches = xdoc.Descendants("from")
                        .Where(f => (string)f.Attribute("id") == train.currentPosition)
                        .Elements("to")
                        .Where(t => (string)t.Attribute("id") == train.finalPosition)
                        .SelectMany(t => t.Element("switches").Elements())
                        .Select(unit =>
                        {
                            string unitNum = unit.Element("unit").Value;
                            string turnouts = unit.Element("turnouts").Value;
                            string value = unit.Element("value").Value;

                            uint unitN = uint.Parse(unitNum);
                            byte turn = byte.Parse(turnouts);
                            byte val = byte.Parse(value);
                            return new { UnitNum = unitN, Turnouts = turn, Value = val };
                        })
                       .ToList();

                //vytvor rezervovane useky
                for (int i = 0; i < resSections.Count(); i++)
                {
                    AddReservedSections(train.id, resSections[i].Value);
                }

                for (int i = 0; i < resSwitches.Count(); i++)
                {
                    AddSwitchesReservation(train.id, resSwitches[i].UnitNum, resSwitches[i].Turnouts, resSwitches[i].Value);
                }
                fintrack = true;
            }

        EndLoop:
            //zkontroluj jestli vlak muze jet dal, kdyz uz je URCITE v kritickem useku
            InCritical(train, fintrack);

        }

        /// <summary>
        /// Metoda, ktera slouzi k nalezeni a rezervace useku, pokud vlak stoji v kritickem useku
        /// Tato metoda je vyvolana pouze pokud vlak zustal stat v kritickem useku a ne na zacatku, kde by mel
        /// Vyvolana napriklad po spusteni aplikace
        /// </summary>
        /// <param name="critical"></param>
        /// <param name="train"></param>
        /// <returns></returns>
        public static bool FindRouteInCritical(IEnumerable<XElement> critical, Trains train)
        {
            foreach (XElement s in critical)
            {
                //string toValue = s.Element("to")?.Attribute("id")?.Value;
                string toValue = s.Attribute("id").Value; //kam vede - dany usek
                string toFinalValue = s.Element("toFinal").Value; //nadrazi kam
                string toStartValue = s.Element("fromStart").Value;

                //jednotlive useky, pres ktere by vlak musel jet
                string[] parts = s.Descendants("parts").Elements()
                    .Where(p => p.Name.LocalName.StartsWith("part"))
                    .Select(p => p.Value)
                    .ToArray();



                string[] validPositions = { "Beroun", "Karlstejn", "Lhota" };

                if (!(trainsList.Any(tl => tl.currentPosition == toValue)) && validPositions.Contains(toFinalValue))
                {
                    //bool hasMatch = trainsList.Any(tl => tl != train && parts.Any(p => tl.currentPosition == p));

                    bool hasMatch = trainsList.Any(tl => tl != train && tl.currentPosition != null && parts.Any(p => tl.currentPosition == p));

                    IEnumerable<Trains> matchingTrains = trainsList.Where(tl => tl != train && parts.Any(p => tl.currentPosition == p));

                    string theMatch = string.Join(", ", matchingTrains.Select(mt => mt.name));

                    if (!hasMatch)
                    {
                        for (int i = 0; i < parts.Count(); i++)
                            AddReservedSections(train.id, parts[i]);

                        var switches = s.Element("switches");
                        if (switches != null)
                        {
                            var reserveSwitches = switches.Elements()
                                .Select(unit =>
                                {
                                    string unitNum = unit.Element("unit").Value;
                                    string turnouts = unit.Element("turnouts").Value;
                                    string value = unit.Element("value").Value;

                                    uint unitN = uint.Parse(unitNum);
                                    byte turn = byte.Parse(turnouts);
                                    byte val = byte.Parse(value);
                                    return new { UnitNum = unitN, Turnouts = turn, Value = val };
                                })
                                .ToList();


                            for (int i = 0; i < reserveSwitches.Count(); i++)
                            {
                                AddSwitchesReservation(train.id, reserveSwitches[i].UnitNum, reserveSwitches[i].Turnouts, reserveSwitches[i].Value);
                            }

                            if (toFinalValue == "Beroun" && train.circuit == 0)
                                train.finalPosition = toValue;
                        }
                        return true;
                    }
                }
            }
            return false;
        }

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
            if (reservationCheck.Count() != 0 || !fintrack)
            {
                //stop the train
                //critical = 1

                //train.move = "2";

                bool testing = true;
                train.move = 2;


                if (testing)
                {
                    //TODO
                    //Stop the train!
                    Locomotive locomotive = new Locomotive(train.name);

                    //bool reverze = train.reverse;

                    MainLogic ml = GetInstance();

                    dataToSendLoco.Add(new LocomotiveDataSend { Loco = locomotive, Reverze = train.reverse, Speed = (byte)3 });
                    ml.OnMyEventLoco(new LocomotiveDataSend { Loco = locomotive, Reverze = train.reverse, Speed = (byte)3 });
                }

                else
                {
                    timeToStop = new System.Timers.Timer(1000);

                    currentTrain = train;
                    // Set the event handler for the Elapsed event
                    timeToStop.Elapsed += (sender, e) => Timer_Elapsed(sender, e, currentTrain);

                    // Start the timer
                    timeToStop.Start();
                }
            }

            else
            {
                //pokracuj v jizde
                //nastav vyhybky pokud budou

                MainLogic ml = GetInstance();

                for (int i = 0; i < switchesChange.Count(); i++)
                {
                    if (switchesChange[i].TrainId == train.id)
                    {
                        uint numbOfUnit = switchesChange[i].NumberOfUnit;
                        byte turnouts = switchesChange[i].Turnouts;
                        byte value = switchesChange[i].Value;
                        ml.OnMyEventTurnout(new TurnoutsDataSend { NumberOfUnit = numbOfUnit, Turnouts = turnouts, Value = value });
                    }
                }

                train.critical = false;
            }
        }

        #endregion

        #region Funkce a metody pro testovani moznosti jizdy vlaku, ktere cekaji na rozjezd

        /// <summary>
        /// Prvni kontrola pro rozjezd vlaku
        /// Ma vlak rezervovane useky nebo je v kritickem useku? (Test, jestli stoji, protoze musel stat)
        /// </summary>
        /// <param name="train"></param>
        /// <param name="testFirstCheck"></param>
        /// <returns></returns>
        /// 
        public static bool TrainWantsToMove(Trains train, bool testFirstCheck)
        {

            var trainInfo = xdoc.Descendants("crit")
                .Where(e => (string)e.Element("last") == train.lastPosition && (string)e.Element("current") == train.currentPosition)
                .ToList();

            //test jestli uz vlak stoji v kritickem useku
            IEnumerable<XElement> critical = GetCriticalReservedSection(train);


            //ma vlak rezervovane useky?
            if (reservedSections.Any(rs => rs.TrainIdReserved == train.id))
            {
                int countReserved = reservedSections.Count(t => t.TrainIdReserved == train.id);

                /*
                var reservationCheck = reservedSections
                    .Where(rs => rs.TrainIdReserved != train.id &&
                        rs.Section == reservedSections
                            .Where(p => p.TrainIdReserved == train.id && reservedSections.IndexOf(p) < reservedSections.IndexOf(rs))
                            .OrderBy(p => reservedSections.IndexOf(p))
                            .Select(p => p.Section)
                            .FirstOrDefault())
                    .ToList();
                */

                bool reservationCheck = true;

                int testCount = 0;

                for (int i = 0; i < reservedSections.Count(); i++)
                {
                    if (reservedSections[i].TrainIdReserved == train.id)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            if (reservedSections[j].Section == reservedSections[i].Section && reservedSections[j].TrainIdReserved != train.id)
                            {
                                reservationCheck = false;
                                break;
                            }

                        }
                        if (!reservationCheck)
                            break;

                        testCount++;
                        if (testCount == countReserved)
                            break;
                    }
                }

                //je neco v rezervovanem useku nebo neni volna cilova kolej?
                //if (countReserved != 0 && (reservationCheck != null || reservationCheck.Count() != 0))
                if (countReserved != 0 && !reservationCheck)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else if (critical.Any())
            {
                return FindRouteInCritical(critical, train);
            }

            //nema rezervovane useky
            else if (trainInfo.Any())
            {
                //kontrola jestli vjizdim do kritickeho useku
                if (testFirstCheck)
                {
                    //najdi cestu, pokud jsi v kritickem useku
                    if (CheckCritical(train))
                        FindRouteOutsideCritical(train);

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

        /// <summary>
        /// Kontrola jednotlivych useku, pres ktere ma vlak vyjet
        /// Pokud je odber proudu v nadchazejicim useku, tak vlak nevyjede
        /// Pokud je nejaky vlak v nadchazejicim useku, tak vlak nevyjede
        /// Pokud nadchazejici poloha je minula poloha nejakeho vlaku, tak vlak nevyjede
        /// </summary>
        /// <param name="train"></param>
        /// <returns></returns>
        public static bool CheckColisionToMove(Trains train)
        {

            //neni odber proudu v nadchazejicim useku od jineho vlaku a jestli v nadchazejicim useku neni jiny vlak?
            if ((!(currentDrain.Any(cd => cd.Section == train.nextPosition && cd.TrainIdDrain != train.id))) && !(trainsList.Any(tl => tl.currentPosition == train.nextPosition && tl.id != train.id))
                && !(trainsList.Any(tl => tl.lastPosition == train.nextPosition && tl.id != train.id)))
            {
                return true;
            }
            return false;

            //return true;
        }


        public static bool SameCircuitToMove(Trains train)
        {
            int circuit = 0;
            bool check = true;

            //kontrola, jestli jsou rezervovane useky pro vlak
            for (int i = reservedSections.Count() - 1; i > 0; i--)
            {
                if (reservedSections[i].TrainIdReserved == train.id)
                {
                    var mapSection = xdoc.Descendants("section")
                                .FirstOrDefault(e => e.Attribute("id")?.Value == reservedSections[i].Section);

                    //Ziskej "circuit" hodnotu - polohu na kolejisti
                    circuit = (int)mapSection.Element("circuit");

                    check = false;

                    break;
                }

            }
            if (check)
            {
                circuit = train.circuit; //vlak nema rezervovany useky, bere se soucasna poloha
            }

            if (circuit != 0) //pozdeji pridany dalsi hodnoty useku, kde se nachazi nadrazi. Tam neni potreba kontrolovat
            {
                var trainInfo = trainsList.Where(t => t.circuit == circuit && t.mapOrientation != train.mapOrientation && t.id != train.id).ToList();
                if (trainInfo.Any())
                {
                    return false;
                }
                return true;
            }
            return true;
        }

        #endregion

        #region Region pro pridani dat do listu (odbery proudu vlaku, rezervovane useky vlaku, pozadavky na vyhybky vlaku)
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

        /// <summary>
        /// Metoda
        /// </summary>
        /// <param name="id">ID lokomotivy</param>
        /// <param name="numberOfUnit">Jednotka kolejovych useku, pro niz jsou data nize</param>
        /// <param name="turnouts">Vybrane vyhybky pro prehozeni</param>
        /// <param name="data">Nastaveni vlevo/vpravo</param>
        public static void AddSwitchesReservation(uint id, uint numberOfUnit, byte turnouts, byte data)
        {
            switchesChange.Add(new SwitchesChange { TrainId = id, NumberOfUnit = numberOfUnit, Turnouts = turnouts, Value = data });
        }
        #endregion

        /// <summary>
        /// Metoda, ktera aktualizuje JSON na zaklade prijatych dat
        /// </summary>
        /// <param name="name">nazev lokmotivy</param>
        /// <param name="currentPosition">soucasna pozice</param>
        /// <param name="speed">rychlost</param>
        /// <param name="reverse">jede obracenym smerem? (pozpatku)</param>
        /// <param name="final">Nazev cilove stanice/koleje</param>
        public static void addNewTrainDataFromClient(string name, string currentPosition, byte speed, bool reverse, string final)
        {
            lock (lockingLogic)
            {
                TrainDataJSON td = new TrainDataJSON();
                trainsList = td.LoadJson();

                foreach (Trains train in trainsList)
                {
                    if (train.name == name)
                    {
                        string orientation = train.mapOrientation;

                        //testovani, zdali doslo ke zmene orientace po kolejisti a urcit spravny smer orientace
                        if (!(train.reverse == reverse))
                        {
                            if (train.mapOrientation == "nextConnection")
                                orientation = "prevConnection";
                            else
                                orientation = "nextConnection";
                        }

                        //omezeno na to, ze vlak nestoji v kritickem useku
                        var mapSection = xdoc.Descendants("section")
                                    .FirstOrDefault(e => e.Attribute("id")?.Value == train.currentPosition);


                        string nextSecId;
                        if (orientation == "nextConnection")
                        {
                            nextSecId = mapSection.Element("nextsec")?.Value;
                        }
                        else
                        {
                            nextSecId = mapSection.Element("prevsec")?.Value;
                        }


                        td.UpdateTrainData(train.name, t =>
                        {
                            //t.currentPosition = currentPosition;
                            t.nextPosition = nextSecId;
                            t.reverse = reverse;
                            t.speed = speed;
                            t.finalPosition = final;
                            t.mapOrientation = orientation;
                            t.move = 2;
                        });

                    }
                }
            }
        }

        public static IEnumerable<XElement> GetFromElements(Trains train)
        {
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
            string[] validPositions = { "Beroun", "Karlstejn", "Lhota" };
            if (!(validPositions.Contains(train.finalPosition)) && train.finalPosition != null && train.circuit == 0)
            {
                var matchingToElements = xdoc.Descendants("to")
                .Where(t => t.Element("id").Value == train.finalPosition
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
        /// Metoda, ktera najde moznou finalni stanici dle aktualni stanice
        /// </summary>
        /// <param name="train">konkretni vlak, ktery si vybere uzivatel</param>
        /// <param name="wantedName">Pozadovana kolej ve vybrane stanici</param>
        /// <returns></returns>
        public static IEnumerable<XElement> GetFinalTrack(Trains train, string wantedName)
        {
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
            /*
            return xdoc.Descendants()
                .Where(e => e.Name.LocalName == "prevsec" || e.Name.LocalName == "nextsec" || e.Name.LocalName == "prevsections" || e.Name.LocalName == "nextsections")
                .Where(e => e.Ancestors("section").Any(a => (string)a.Attribute("id") == position))
                .Select(e => (string)e)
                .ToList();
            */

            /*
            return xdoc.Descendants()
                .Where(e => e.Name.LocalName == "prevsec" || e.Name.LocalName == "nextsec" || e.Name.LocalName == "prevsections" || e.Name.LocalName == "nextsections")
                .Where(e => e.Ancestors("section").Any(a => (string)a.Attribute("id") == position))
                .SelectMany(e =>
                {
                    if (e.Name.LocalName == "prevsections" || e.Name.LocalName == "nextsections")
                        return e.Elements().Select(x => (string)x);

                    else
                        return new[] { (string)e };
                })
                .ToList();
            */

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
            /*
            var matchingToElements = xdoc.Descendants("to")
            .Where(t => t.Element("parts")?.Descendants().Select(p => p.Value).Contains(previousPosition) == true // check if previousPosition is present
        && t.Element("parts")?.Descendants().Select(p => p.Value).Contains(currentPosition) == true // check if currentPosition is present
        && t.Element("parts")?.Descendants().Select(p => p.Value).ToList().IndexOf(previousPosition) + 1 == t.Element("parts")?.Descendants().Select(p => p.Value).ToList().IndexOf(currentPosition) // check if previousPosition is earlier than currentPosition and they are next to each other
    )
    .Select(t => t.Element("fromStart").Value)
    .FirstOrDefault();
            */
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
            /*
            var matchingFromStartElements = xdoc.Descendants("fromStartOutside")
        .Where(fs => fs.Descendants("items").Descendants().Select(p => p.Value).Contains(previousPosition)
                    && fs.Descendants("items").Descendants().Select(p => p.Value).Contains(currentPosition)
                    && fs.Descendants("items").Descendants().Select(p => p.Value).ToList().IndexOf(previousPosition) + 1 == fs.Descendants("items").Descendants().Select(p => p.Value).ToList().IndexOf(currentPosition))
        .Select(fs => fs.Attribute("id").Value)
        .FirstOrDefault();
            */

            var matchingFromStartElements = xdoc.Descendants("fromStartOutside")
        .Where(fs => fs.Descendants("items").Descendants().Select(p => p.Value).Contains(previousPosition)
                    && fs.Descendants("items").Descendants().Select(p => p.Value).Contains(currentPosition)
                    && fs.Descendants("items").Descendants().Select(p => p.Value).ToList().IndexOf(previousPosition) + 1 == fs.Descendants("items").Descendants().Select(p => p.Value).ToList().IndexOf(currentPosition))
        .Select(fs => fs.Attribute("id").Value)
        .ToList();

            //var fromStartValue = matchingFromStartElements.FirstOrDefault()?.Attribute("id")?.Value;

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

        protected void OnMyEventLoco(LocomotiveDataSend e)
        {
            try
            {
                LocomotiveDataEvent?.Invoke(this, e);
            }
            catch
            { }

        }

        protected void OnMyEventTurnout(TurnoutsDataSend e)
        {
            try
            {
                TurnoutsDataEvent?.Invoke(this, e);
            }
            catch
            { }

        }
    }

    public class LocomotiveDataSend
    {
        public Locomotive Loco { get; set; }

        public bool Reverze { get; set; }

        public byte Speed { get; set; }

    }

    public class TurnoutsDataSend
    {
        public uint NumberOfUnit { get; set; }
        public byte Turnouts { get; set; }
        public byte Value { get; set; }
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

    public class SwitchesChange
    {
        public uint TrainId { get; set; }
        public uint NumberOfUnit { get; set; }
        public byte Turnouts { get; set; }
        public byte Value { get; set; }
    }

}
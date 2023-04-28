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
using System.IO;

namespace ControlLogic
{
    public class MainLogic
    {
        //event handler na pohyb lokomotiv z logiky rizeni
        public event EventHandler<LocomotiveDataSend> LocomotiveDataEvent;

        //event handler na zmenu polohy vyhybek z logiky rizeni
        public event EventHandler<TurnoutsDataSend> TurnoutsDataEvent;

        //event handler na zobrazeni zpravy z logiky rizeni
        public event EventHandler<InfoMessageSend> InfoMessageEvent;

        //instance ktera je vyvolana z windows form
        private static MainLogic instance = null;

        //zamek na instance aby nedochazelo ke kolizi
        private static readonly object lockingInstance = new object();

        //zamek na odbery proudu aby nedochazelo ke kolizi
        public static readonly object lockingOccupancy = new object();

        //zamek na logiku aby nedochazelo ke kolizi
        private static readonly object lockingLogic = new object();

        //list s daty urcenymi lokomotivam
        public static List<LocomotiveDataSend> dataToSendLoco = new List<LocomotiveDataSend>();

        //list s odbery proudu
        private static List<CurrentDrain> currentDrain = new List<CurrentDrain>();

        //list rezervovanych useku
        public static List<ReservedSections> reservedSections = new List<ReservedSections>();

        //list s daty urcenymi vyhybkam
        public static List<SwitchesChange> switchesChange = new List<SwitchesChange>();

        //list s odbery proudu
        private static List<Section> occupancySections = new List<Section>();

        //list obsahujici nejdulezitejsi data o kazdem vlaku, po kazdem cyklu ukladana
        private static List<Trains> trainsList = new List<Trains>();

        //konfiguracni dokument s namapovanym kolejistem
        private static XDocument xdoc = new XDocument();

        //list, ve kterem jsou uchovavany data vlaku
        private static TrainDataJSON td = new TrainDataJSON();

        //timer, po jake dobe bude probihat kontrola kolejiste
        private static System.Timers.Timer timerCheck;

        //timer, po jake dobe bude zastaven vlak
        private static Dictionary<Trains, System.Timers.Timer> timeToStop = new Dictionary<Trains, System.Timers.Timer>();
        //private static System.Timers.Timer timeToStop;

        //test mereni rychlosti cyklu
        private static List<double> stopwatchValues = new List<double>();


        /// <summary>
        /// metoda pro ulozeni a uchovani instance vyvolane ve windows form
        /// </summary>
        /// <returns>Instance vytvoreni ve windows form</returns>
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

        /// <summary>
        /// inicializace puvodniho nacteni vlaku z JSONu a nacteni konfiguracniho souboru kolejiste
        /// </summary>
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
            //ulozeni instance, aby windows form dokazal chytat eventy
            instance = ml;

            //metoda pro 
            InitTrainListAndXdoc();

            //inicializace pocatecnich podminek
            SetInitialConditions();

            //nastaveni timeru, ktery kontroluje logiku
            StartTimers();

            //pro testovani chovani
            //controlLogic();
        }


        /// <summary>
        /// Metoda pro nastaveni pocatecnich podminek
        /// Vytvori imaginarni odbery proudu pro kontrolu u logiky rizeni
        /// Bude ulozena poloha vlaku a tim do teto polohy nebude mozne zadne vlaky vypustit
        /// </summary>
        public static void SetInitialConditions()
        {
            foreach (Trains train in trainsList)
            {
                //Vlozeni proudoveho odberu soucasneho a minuleho useku:
                AddCurrentDrain(train.id, train.currentPosition);
                AddCurrentDrain(train.id, train.lastPosition);
            }
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

                //nastaveni vsech vlaku priznaku, ze stoji a nemaji jet
                foreach (Trains train in trainsList)
                {
                    train.move = 0;
                }
            }
        }

        /// <summary>
        /// Timer, ktery se vyvola pro odpocet pro zastaveni vlaku
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        /// <param name="train">aktualni vlak</param>
        private static void Timer_Elapsed(object sender, ElapsedEventArgs e, Trains train, int moveValue)
        {
            //Zastaveni timeru pro zastaveni vlaku
            timeToStop[train].Stop();
            //timeToStop.Stop();

            //Nastaveni hodnoty vlaku, ze chce jet
            train.move = moveValue;

            ((System.Timers.Timer)sender).Dispose();


            //Zastaveni vlaku - vyvolani event handleru a ulozeni dat
            Locomotive locomotive = new Locomotive(train.name);

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
            //Mereni vykonavaciho casu jedne otocky logiky rizeni
            Stopwatch sw = new Stopwatch();
            sw.Start();

            lock (lockingLogic)
            {
                //nacti aktualni data o poloze
                TrainDataJSON td = new TrainDataJSON();
                trainsList = td.LoadJson();

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

                        //kontrola zdali nedojde ke kolizi s jinym vlakem v pripade rozjezdu
                        if (!CheckColisionToMove(train))
                            continue;

                        //kontrola zdali vlak nechce vjet na okruh  bez vyhybek, kde jede vlak v opacnem smeru
                        if (!SameCircuitToMove(train))
                            continue;

                        MainLogic ml = GetInstance();

                        //pokud ma vlak rezervovane useky, prehod vyhybky
                        //v teto fazi jiz vlak prosel kontrolou, ze nedojde ke kolizi a muze jet
                        if (reservedSections.Any(rs => rs.TrainIdReserved == train.id))
                        {
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
                        }

                        //rozjed vlak
                        Locomotive locomotive = new Locomotive(train.name);

                        train.move = 1;

                        dataToSendLoco.Add(new LocomotiveDataSend { Loco = locomotive, Reverze = train.reverse, Speed = train.speed });
                        ml.OnMyEventLoco(new LocomotiveDataSend { Loco = locomotive, Reverze = train.reverse, Speed = train.speed });
                    }
                }

                //aktualizuj data o polohe vlaku a vsech datech
                foreach (Trains train in trainsList)
                {
                    //jestli vlak nestoji = nebyla zadna akce, uloz data
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


            //Vypnuti stopek
            sw.Stop();
            double elapsedSeconds = sw.Elapsed.TotalMilliseconds;
            stopwatchValues.Add(elapsedSeconds);
        }

        #region Funkce a metody na testovani jedoucich vlaku (kontrola kolize a proudu, aktualizace polohy, hledani cesty)
        /// <summary>
        /// Testovani a aktualizace odberu proudu jedoucich vlaku - testuji se zde pouze minule a soucasne polohy
        /// Jedna se o useky, ktere jsou ulozene
        /// </summary>
        /// <param name="train">Aktualni vlak</param>
        public static void CheckCurrentDrain(Trains train)
        {
            //list, do ktereho budou vlozeny odbery proudu aktualni
            List<Section> occupancySec;
            lock (lockingOccupancy)
            {
                //vytvoreni noveho listu a ulozeni dat, jelikoz neustale dochazi k odberum proudu
                occupancySec = new List<Section>(ProcessDataFromTCP.GetSavedOccupancySection());
            }
            for (int i = 0; i < currentDrain.Count(); i++)
            {
                //pokud v listu currentDrain s odbery proudu je nalezen aktualni vlak
                if (train.id == currentDrain[i].TrainIdDrain && currentDrain[i].Section != null)
                {
                    for (int j = 0; j < occupancySec.Count(); j++)
                    {
                        //Test na shodu nazvu useku ulozeneho a odberu proudu zmereny usekovou jednotkou
                        if (occupancySec[j].Name == currentDrain[i].Section)
                        {
                            //je odber proudu, pokracuj
                            if (occupancySections[j].current > 0)
                                continue;

                            //neni odber proudu, smaz data
                            else
                            {
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

                    //uloz data
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

                    //zastav vlak po vterine
                    var timer = new System.Timers.Timer(1000);
                    timer.Elapsed += (sender, e) => Timer_Elapsed(sender, e, train, 0);
                    timer.Start();
                    timeToStop[train] = timer;
                }
            }
            else
            {
                //nakopirovani aktualnich dat z odberu proudu
                List<Section> occupancySec;
                lock (lockingOccupancy)
                {
                    occupancySec = new List<Section>(ProcessDataFromTCP.GetSavedOccupancySection());
                }

                //test, zdalivlak neni v nadchazejici pozici
                foreach (Section s in occupancySec)
                {
                    //doslo ke zmene polohy, vlak je v dalsim useku
                    if ((train.nextPosition == s.Name) && (s.current > 5))
                    {
                        //vlozeni do odberu proudu nove pozice
                        AddCurrentDrain(train.id, train.nextPosition);

                        //Doslo ke zmene polohy. Poloha udavajici jako soucasna je jiz minula
                        string last = train.currentPosition;
                        string cur = train.nextPosition;
                        train.lastPosition = last;
                        train.currentPosition = cur;


                        //Zjisteni hodnoty aktualniho okruhu
                        var mapSection = xdoc.Descendants("section")
                                .FirstOrDefault(e => e.Attribute("id")?.Value == cur);
                        int circuit = (int)mapSection.Element("circuit");
                        train.circuit = circuit;

                        //predpripraveny string pro nazev nadchazejiciho useku
                        string nextSecId;

                        //jsou nejake rezervovane sekce?
                        IEnumerable<ReservedSections> rp = reservedSections.Where(x => x.TrainIdReserved == train.id);
                        if (rp.Count() > 0)
                        {
                            //vymazani rezervovanych useku, ve kterych vlak uz byl
                            ReservedSections ress = reservedSections.FirstOrDefault(x => x.TrainIdReserved == train.id && train.lastPosition == x.Section);
                            if (ress != null)
                                reservedSections.Remove(ress);

                            //aktualizace nadchazejiciho useku vlaku
                            ReservedSections rs = reservedSections.FirstOrDefault(x => x.TrainIdReserved == train.id && train.currentPosition != x.Section && train.lastPosition != x.Section);
                            if (rs != null)
                                train.nextPosition = rs.Section;

                        }

                        //pokud je ma poloha (po aktualizaci) finalni, neni zadna dalsi pozice kam jet
                        else if (train.currentPosition == train.finalPosition)
                        {
                            train.nextPosition = null;
                            nextSecId = null;
                        }

                        //ostatni pripady - vlak je na kolejisti mezi stanicemi mimo kriticke oblasti
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
                    }

                    //test, jestli jedouci vlak ma alespon v jednom useku odber proudu, jinak zastavit!
                    bool testCurrent = currentDrain.Any(cd => cd.TrainIdDrain == train.id);
                    if (!testCurrent)
                    {
                        //pokud vlak nikde neodebira polohu, jede mimo ocekavany usek, zastav vlak(y)!
                        MainLogic ml = GetInstance();

                        Locomotive locomotive = new Locomotive(train.name);

                        ml.OnMyEventMessage(new InfoMessageSend { InfoMessage = "Vlak " + train.name + " se nenachazi v ocekavane poloze, zastavuji vsechny vlaky!" });

                        dataToSendLoco.Add(new LocomotiveDataSend { Loco = locomotive, Reverze = train.reverse, Speed = 0 });
                        ml.OnMyEventLoco(new LocomotiveDataSend { Loco = locomotive, Reverze = train.reverse, Speed = 0 });

                        //uloz data vlaku
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

            //zastav okamzite vlak, hrozi kolize!
            MainLogic ml = GetInstance();

            Locomotive locomotive = new Locomotive(train.name);

            dataToSendLoco.Add(new LocomotiveDataSend { Loco = locomotive, Reverze = train.reverse, Speed = 3 });
            ml.OnMyEventLoco(new LocomotiveDataSend { Loco = locomotive, Reverze = train.reverse, Speed = 3 });

            train.move = 2;
        }

        /// <summary>
        /// Testování, jestli jsem v kritickém úseku
        /// </summary>
        /// <param name="train"></param>
        /// <returns></returns>
        public static bool CheckCritical(Trains train)
        {
            //kontrola, jestli vlak vjizdi do kritickeho useku
            var trainInfo = xdoc.Descendants("crit")
                .Where(e => (string)e.Element("last") == train.lastPosition && (string)e.Element("current") == train.currentPosition)
                .ToList();

            int alreadyReserved = reservedSections.Count(cd => cd.TrainIdReserved == train.id);

            List<string> finalStations = SearchLogic.GetAllStationTracks();


            //vjizdim do kritickeho useku? (ANO = v listu jsou nejake hodnoty) - testuju pouze vjezd
            if ((trainInfo.Any() && alreadyReserved < 1) || finalStations.Contains(train.currentPosition))
            {
                //pokud obsahuje cilovou stanici a ne kolej, nebo se vlak nachazi na nadrazi
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
            //kontrola, ze jsou nactena data vlaku
            if (!(trainsList.Count() > 0))
            {
                TrainDataJSON td = new TrainDataJSON();
                trainsList = td.LoadJson();
            }


            //bool testujici volnou cilovou kolej ci ze vlak bude odjizdet
            bool fintrack = false;

            //pokud se ma hledat nejvhodnejsi cesta na nadrazi = neni definovana cilova kolej
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
                            //vytvor rezervovane useky
                            var reserveSections = toElement.Element("parts")
                               .Elements()
                               .Select(e => e.Value)
                               .ToList();

                            //vytvor rezervaci na vyhybky
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

                                //uloz vyhybky pozadovane do listu
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
                            fintrack = true;

                            //splneno, konec cyklu
                            goto EndLoop;
                        }
                    }
                }
            }
            //je zadefinovana chtena cilova kolej
            else
            {
                //vrati vsechny izolovane_rezervovane useky mezi body soucasna poloha a cilova stanice
                var resSections = xdoc.Descendants("from")
                          .Where(f => (string)f.Attribute("id") == train.currentPosition)
                          .Elements("to")
                          .Where(t => (string)t.Attribute("id") == train.finalPosition)
                          .SelectMany(t => t.Element("parts").Elements())
                          .ToList();

                //vrati vsechny vyhybky mezi soucasnou polohou a cilovou stanici
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

                //vytvor rezervaci na vyhybky do listu
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

                //pokud vlak jeste neni v cili
                if (!(trainsList.Any(tl => tl.currentPosition == toValue)) && validPositions.Contains(toFinalValue))
                {
                    //testovani, zdali vlak muze jet a jestli v usecich neni zadny jiny vlak
                    bool hasMatch = trainsList.Any(tl => tl != train && tl.currentPosition != null && parts.Any(p => tl.currentPosition == p));

                    //IEnumerable<Trains> matchingTrains = trainsList.Where(tl => tl != train && parts.Any(p => tl.currentPosition == p));
                    //string theMatch = string.Join(", ", matchingTrains.Select(mt => mt.name));

                    if (!hasMatch)
                    {
                        //pridej rezervovane useky
                        for (int i = 0; i < parts.Count(); i++)
                        {
                            AddReservedSections(train.id, parts[i]);
                            if (i != 0 && parts[i - 1] == train.currentPosition)
                                train.nextPosition = parts[i];
                        }

                        //pridej pozadavky na vyhybky
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

                            //pridej vyhybky do rezervovanych vyhybek
                            for (int i = 0; i < reserveSwitches.Count(); i++)
                            {
                                AddSwitchesReservation(train.id, reserveSwitches[i].UnitNum, reserveSwitches[i].Turnouts, reserveSwitches[i].Value);
                            }

                            //pokud vlak ma jet do cilove stanice, ktera je v danem okruhu, tak se aktualizuje cilova pozice
                            if (toFinalValue == "Beroun" && train.circuit == 0)
                                train.finalPosition = toValue;

                            /*
                            //aktualizace polohy vlaku
                            td.UpdateTrainData(train.name, (t) =>
                            {
                                t.nextPosition = train.nextPosition;
                                t.finalPosition = train.finalPosition;
                            });
                            */
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Metoda, ktera zjisti, jestli vlak muze jet, kdyz vjizdi do kritickem useku
        /// </summary>
        /// <param name="train"></param>
        /// <param name="fintrack"></param>
        public static void InCritical(Trains train, bool fintrack)
        {
            //list obsahujici informace, jestli ma testovany vlak jet pres rezervovany usek, ktery byl rezervovan drive
            var reservationCheck = reservedSections
               .Where(rs => rs.TrainIdReserved != train.id &&
                   reservedSections
                       .Where(p => p.TrainIdReserved == train.id && reservedSections.IndexOf(p) > reservedSections.IndexOf(rs))
                       .Any(p => p.Section == rs.Section))
               .ToList();


            //kontrola kolize, ze vlak muze jet pres rezervovane useky - overi toto, pokud i stoji nejaky vlak v rezervovanych usecich
            var colisionCheck = reservedSections
               .Where(rs => rs.TrainIdReserved == train.id)
               .ToList();

            bool bb = false;
            foreach (var section in colisionCheck)
            {
                bb = currentDrain.Any(cd => cd.Section == section.Section && cd.TrainIdDrain != train.id);
                if (bb)
                    break; // exit the loop if collision is found
            }



            //je neco v rezervovanem useku nebo neni volna cilova kolej?
            if (reservationCheck.Count() != 0 || bb || !fintrack)
            {
                //stop the train
                //critical = 1

                //train.move = "2";

                bool testing = false;
                train.move = 2;

                //pro testovani - zastaveni hned
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
                    /*
                    timeToStop = new System.Timers.Timer(1000);

                    // Set the event handler for the Elapsed event
                    timeToStop.Elapsed += (sender, e) => Timer_Elapsed(sender, e, train);

                    // Start the timer
                    timeToStop.Start();
                    */

                    var timer = new System.Timers.Timer(1000);
                    timer.Elapsed += (sender, e) => Timer_Elapsed(sender, e, train, 2);
                    timer.Start();
                    timeToStop[train] = timer;
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
            //stoji vlak z duvodu, ze je na hranici kritickeho useku?
            var trainInfo = xdoc.Descendants("crit")
                .Where(e => (string)e.Element("last") == train.lastPosition && (string)e.Element("current") == train.currentPosition)
                .ToList();

            //test jestli uz vlak stoji v kritickem useku
            IEnumerable<XElement> critical = SearchLogic.GetCriticalReservedSection(train);


            //ma vlak rezervovane useky?
            if (reservedSections.Any(rs => rs.TrainIdReserved == train.id))
            {
                //pokud vlak stoji jiz v rezervovanem useku, smaz vsechny predchozi useky
                if (reservedSections.Any(rs => rs.TrainIdReserved == train.id && rs.Section == train.currentPosition))
                {
                    while (true)
                    {
                        ReservedSections ress = reservedSections.FirstOrDefault(x => x.TrainIdReserved == train.id);
                        if (ress.Section != train.currentPosition)
                        {
                            reservedSections.Remove(ress);
                        }
                        else
                            break;
                    }
                }

                int countReserved = reservedSections.Count(t => t.TrainIdReserved == train.id);

                bool reservationCheck = true;

                int testCount = 0;

                //cyklus pres vsechny useky, zdali je vlak prvni, ktery ma dane useky rezervovany
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

                        //byly zkontrolovany vsechny useky, konec
                        testCount++;
                        if (testCount == countReserved)
                            break;
                    }
                }

                //je neco v rezervovanem useku od jineho vlaku nebo neni volna cilova kolej?
                if (countReserved != 0 && !reservationCheck)
                {

                    //TODO - vlak jiz stoji v rezervovanem useku

                    //vlak je v useku mezi vyhybkami ve stanici a zaroven neni na nadrazi ci nektere z finalnich koleji
                    List<string> finalStations = SearchLogic.GetAllStationTracks();
                    if (train.circuit == 0 && !finalStations.Contains(train.currentPosition))
                    {
                        //useky, ktere maji shodne rezervovane useky s useky soucasneho vlaku
                        var checkPriority = reservedSections
                            .Where(rs => rs.TrainIdReserved != train.id &&
                                reservedSections
                                    .Where(p => p.TrainIdReserved == train.id)
                                    .Any(p => p.Section == rs.Section))
                            .Where(rs => rs.TrainIdReserved != train.id)
                            .ToList();

                        //kontrola priority - pokud nemaji tyto useky rezervovane vlaky, ktere jedou, tak v poradku a pravo priority pr ovlak, ktery stoji ve vyhybkach
                        for (int i = 0; i < checkPriority.Count(); i++)
                        {
                            //checkPriority
                            int move = trainsList.Where(tl => tl.id == checkPriority[i].TrainIdReserved)
                                .Select(tl => tl.move)
                                .FirstOrDefault();

                            if (move == 1)
                                return false;

                        }

                        //kontrola kolize, ze vlak muze jet pres rezervovane useky - overi toto, pokud i stoji nejaky vlak v rezervovanych usecich
                        var colisionCheck = reservedSections
                           .Where(rs => rs.TrainIdReserved == train.id)
                           .ToList();
                        foreach (var section in colisionCheck)
                        {
                            bool bb = false;
                            bb = currentDrain.Any(cd => cd.Section == section.Section && cd.TrainIdDrain != train.id);
                            if (bb)
                                return false;
                        }
                        return true;

                    }
                    else
                        return false;
                }
                //cilova kolej volna a rezervovane useky ma rezervovany jako prvni
                else
                {
                    return true;
                }
            }
            //najdi cestu v kritickem useku
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
                //vlak nenasel pozadovanou cestu/cesta neni volna, proto vraci false
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
        }

        /// <summary>
        /// kontrola jestli vlak nema jet do useku, ve ktere
        /// </summary>
        /// <param name="train"></param>
        /// <returns></returns>
        public static bool SameCircuitToMove(Trains train)
        {
            int circuit = 0;
            bool check = true;

            //kontrola, jestli jsou rezervovane useky pro vlak, hleda od posledniho useku (ten, ktery je mimo nadrazi / na nadrazi)
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
            //nejsou rezervovane useky
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

        protected void OnMyEventMessage(InfoMessageSend e)
        {
            try
            {
                InfoMessageEvent?.Invoke(this, e);
            }
            catch
            { }

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

    public class InfoMessageSend
    {
        public string InfoMessage { get; set; }
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
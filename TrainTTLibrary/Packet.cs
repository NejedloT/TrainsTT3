using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using static TrainTTLibrary.Packet;
using System.Xml;
using System.Xml.Linq;

namespace TrainTTLibrary
{
    /// <summary>
    /// Zakladni trida a predek nadledujicich trid pro praci s packety
    /// </summary>
    public class Packet
    {
        public List<byte> BytePacket { set; get; } //seznam bytu, ktere jsou prijaty pres seriovy port

        public string TCPPacket { set; get; } //tcp packet

        public string Type { set; get; } //promena znacici typ packetu

        public byte Head { set; get; } //hlavicka

        public List<byte> Adress { set; get; } //adresa - slozena z cisla instrukce (gen R, gen W atd.) a cisla jednotky

        public List<byte> Data { set; get; } //list pro datove byty

        public byte CRC { set; get; } //cyclic redundancy code

        public UInt32 NumberOfUnit { set; get; } // cislo jednotky

        public UInt32 NumberOfAdress { set; get; } //cislo adresy (generator R, zesilovar 2R atd.)

        /// <summary>
        /// MEtoda pro vytvoreni "vlastnosti" z prijatych bytu
        /// </summary>
        /// <param name="bytePacket">seznam bytu</param>
        public void SetPropetiesFromByte(byte[] bytePacket)
        {
            BytePacket = new List<byte>(bytePacket);

            Adress = new List<byte>();

            Data = new List<byte>();

            for (int i = 0; i < 2; i++)
            {
                Adress.Add(bytePacket[i + 1]);
            }

            Head = bytePacket[0];

            for (int i = 0; i < Head; i++)
            {
                Data.Add(bytePacket[i + 3]);
            }

            CRC = bytePacket[bytePacket.Length - 1];

            AdressToNumberOfAdressAndUnit();

            AssingType();

            TCPPacket = (Type + ":");
        }

        /// <summary>
        /// Metoda pro napleneni vlastnosti z prijate tcp zpravy
        /// </summary>
        /// <param name="TCPPacket">prichozi TCP packet ve forme stringu</param>
        protected void SetPropetiesFromTCP(string TCPPacket)
        {
            BytePacket = new List<byte>();

            Adress = new List<byte>();

            Data = new List<byte>();

            this.TCPPacket = TCPPacket;

            string[] s = TCPPacket.Split(':');

            Type = (s[0] + ":");

            if (RecognizeTCPType(Type) == dataType.unknow)
            {
                this.TCPPacket = UnknowPacket(this.TCPPacket);
            }

        }

        /// <summary>
        /// Metoda, ktera na zaklade cisla adresy priradi konkretni typ
        /// </summary>
        protected void AssingType()
        {

            switch (NumberOfAdress)
            {
                case (int)unitAdress.emergency_W: //nouzovy signal

                    Type = dataType.emergency.ToString();
                    break;

                case (int)unitAdress.generator_DCC_W: //zprava pro generator, pohyby lokomotiv

                    if ((Data[1] >= 0x01) && (Data[1] <= 0x7f))// (0x00) = broadcast, (0x01 až 0x7f) = lokomotivní dekodéry, (0x80 až 0xbf) = dekodéry pro příslušenství
                    {
                        Type = dataType.train_move.ToString();
                    }
                    else if ((Data[1] >= 0x80) && (Data[1] <= 0xbf))
                    {
                        Type = dataType.train_function.ToString();
                    }
                    else
                    {
                        Type = dataType.unknow.ToString();
                    }
                    break;

                case (int)unitAdress.generator_DCC_R: //zprava od generatoru

                    if ((Head == 0x01) && (Data[0] == 0x55))
                    {
                        Type = dataType.watchdog.ToString(); // watchdog message: 01 20 20 55 88
                        break;
                    }
                    else
                    {
                        Type = dataType.unknow.ToString(); //neznama zprava
                        break;
                    }

                case (int)unitAdress.zesilovac_DCC_1_R: //zprava od usekove jednotky - odbery proudu

                    if (Head == 0x08)
                    {
                        Type = dataType.occupancy_section.ToString(); //odbery proudu
                    }
                    else
                    {
                        Type = dataType.unknow.ToString();  //neznama zprava
                    }

                    break;

                case (int)unitAdress.zesilovac_DCC_2_R: //zprava od usekove jednotky - aktualni stav ci chyba

                    if (Head == 0x04)
                    {
                        Type = dataType.unit_info.ToString(); //instrukce od usekove jednotky
                    }
                    else
                    {
                        Type = dataType.unknow.ToString(); //neznama zprava
                    }
                    break;

                case (int)unitAdress.zesilovac_DCC_1_W: //zprava usekove jednotce - konfigurace usekove jednotky
                    if (Head == 0x04)
                    {
                        Type = dataType.unit_instruction.ToString(); //konfiguracni data
                    }
                    else
                    {
                        Type = dataType.unknow.ToString(); //neznama zprava
                    }
                    break;

                case (int)unitAdress.zesilovac_DCC_2_W: //nevyuzito

                    Type = dataType.unknow.ToString(); //neznama zprava

                    break;

                case (int)unitAdress.vyhybky_W: //zprava jednotce vyhybek
                    if (Head == 0x08)
                        Type = dataType.turnout_instruction.ToString();

                    else
                        Type = dataType.unknow.ToString();

                    break;

                case (int)unitAdress.vyhybky_R: //zprava od jednotky vyhybek
                    if (Head == 0x08)
                        Type = dataType.turnout_info.ToString();

                    else
                        Type = dataType.unknow.ToString();

                    break;

                case (int)unitAdress.navestidla_W: //momentalne nevyuzito

                    Type = dataType.unknow.ToString();
                    break;

                case (int)unitAdress.tocna_W: //momentalne nevyuzito

                    Type = dataType.unknow.ToString();
                    break;

                case (int)unitAdress.tocna_R: //momentalne nevyuzito

                    Type = dataType.unknow.ToString();
                    break;

                default:

                    Type = dataType.unknow.ToString();
                    break;
            }

        }

        /// <summary>
        /// MEtoda pro zjisteni typu zpravy na zaklade prijatych dat
        /// </summary>
        /// <param name="str">Vstupni string TCP zpravy</param>
        /// <returns>Dany typ (navestidla_W, turnout_instruction atd.)</returns>
        public static dataType RecognizeTCPType(String str)
        {

            string[] s = str.Split(':');

            dataType type;

            if (!Enum.TryParse(s[0], out type))
            {
                return dataType.unknow;
            }

            return type;

        }

        /// <summary>
        /// Metoda, ktery vypocte cyklicky redundantni kod
        /// </summary>
        /// <param name="paket">Vstupni list bytu packetu</param>
        /// <returns>vypoctene CRC</returns>
        public static byte VypocetCRC(List<byte> paket)
        {
            byte crc = 0;
            byte icrc, ucrc;

            byte CRC_POLYNOM = 0xd8;
            byte TOP_BIT = 0x80;

            for (ucrc = 0; ucrc < paket.Count; ucrc++)
            {
                crc ^= paket[ucrc];

                for (icrc = 8; icrc > 0; --icrc)
                {


                    if ((crc & TOP_BIT) != 0)
                    {
                        crc = (byte)(crc << 1);
                        crc = (byte)(crc ^ CRC_POLYNOM);
                    }
                    else
                    {
                        crc <<= 1;
                    }
                }
            }

            return (byte)crc;
        }

        /// <summary>
        /// Metoda pro vypocet adresy jednotky a cisla jednotka z HEX adresy
        /// </summary>
        protected void AdressToNumberOfAdressAndUnit()
        {

            UInt32 adr = (uint)((Adress[0] << 8) | Adress[1]);

            adr = adr >> 5;

            NumberOfAdress = adr >> 7;

            NumberOfUnit = adr & ((UInt32)0x7f); // 0x7f = 1111111

        }

        /// <summary>
        /// Metoda pro vypocet HEX adresy z adresy jednotky a cisla jednotky
        /// </summary>
        protected void NumberOfAdressAndUnitToAdress() // z čísla jednotky a adresy jednotky vytvořit HEX adresu pro paket 
        {
            UInt16 i = (UInt16)(NumberOfAdress << 12);

            i = (ushort)(i | ((UInt16)(NumberOfUnit << 5)));

            Adress.Add((byte)(i >> 8));

            Adress.Add((byte)(i));
        }

        /// <summary>
        /// Vytvoreni packetu z hlavicky, adresy, dat a CRC
        /// </summary>
        protected void SetBytePacket()
        {

            BytePacket.Add(Head);

            BytePacket.AddRange(Adress);

            BytePacket.AddRange(Data);

            CRC = VypocetCRC(BytePacket);

            BytePacket.Add(CRC);

        }

        /// <summary>
        /// MEtoda, ktera nahradi neznamy datovy typ typem unknown
        /// </summary>
        /// <param name="tcpPacket">Vstupni TCP packet ve forme stringu</param>
        /// <returns>Hodnotu, ze datovy typ je neznamy</returns>
        static public string UnknowPacket(string tcpPacket)
        {
            string[] s = tcpPacket.Split(':');

            tcpPacket = s[1];

            return dataType.unknow.ToString() + ":" + s[1];
        }

        /// <summary>
        /// Metoda pro upravu nazvu vlaku
        /// Cilem metody je nahradit mezery podtrzitkem
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GapToUnderLine(string str)
        {

            string[] s = str.Split(' ');

            string a = "";

            for (int i = 0; i < s.Length; i++)
            {
                a += (i == (s.Length - 1)) ? s[i] : (s[i] + "_");
            }

            return a;
        }

        /// <summary>
        /// Metoda pro upravu nazvu vlaku
        /// Cilem metody je nahradit podtrzitko mezerami
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UnderLineToGap(string str)
        {
            string[] s = str.Split('_');

            string a = "";

            for (int i = 0; i < s.Length; i++)
            {
                a += (i == (s.Length - 1)) ? s[i] : (s[i] + " ");
            }

            return a;
        }

        /// <summary>
        /// JEdnotlive datove typy a instrukce
        /// </summary>
        public enum dataType
        {
            watchdog, //pro udrzeni aktivniho stavu od generatoru
            occupancy_section, //odbery proudu od usekove jednotky
            unit_info, //chyby a stav usekove jednotky
            turnout_info, //chyby a stav jednotky vyhybek
            // zprávy odesílané řídící jednotkou
            train_move, //pohyb vlaku
            train_function, //svetla vlaku a dalsi funkce
            train_move_to_place, //pohyb vlaku na definovanem useku (definovano i stanicemi)
            unit_instruction, //zprava usekove jednotce - jeji konfigurace
            turnout_instruction, //zprava jednotce vyhybek - nastaveni vyhybek a jine
            // zprávy pro řídící jednotku
            unknow, // neznámý typ zprávy
            oled_info, // textové zprávy mezi TCP uživateli s bíže nespecifikovanými parametry
            emergency,// adresa 0000
            // zprávy pro komunikaci

        };

        /// <summary>
        /// Jednotlive adresy jednotek
        /// </summary>
        public enum unitAdress
        {
            emergency_W,
            generator_DCC_W,    //zpravy generatoru - pohyb vlaku
            generator_DCC_R,    //zprava od generatoru
            zesilovac_DCC_1_R,	//namerene proudy usekove jednotky
            zesilovac_DCC_2_R,	//stavy a chyby usekove jednotky
            zesilovac_DCC_1_W,  //zapis do usekove jednotky
            zesilovac_DCC_2_W,  //nevyuzito
            vyhybky_W,          //prikazy jednotce vyhybek
            vyhybky_R,          //stavy a chyby jednotky vyhybek (od jednotky)
            navestidla_W,       //navestidla (nevyuzito)
            tocna_W,            //tocna (nevyuzito)
            tocna_R,            //tocna (nevyuzito)
        };

        /// <summary>
        /// Seznam jednotlivych instrukci urcenych usekove jednotce a pripadne i jejich adresy
        /// </summary>
        public enum unitInstruction : byte
        {
            restart_jednotky = 0x01,
            prodleva_odesilani_zmerenych_proudu = 0x02,
            restart_H_mustku = 0x03,
            nastaveni_zdroje = 0x04,
            mereni_na_zvolenych_kanalech = 0x05,
            precteni_stavu_jednotky = 0x10,
            err,
        }

        /// <summary>
        /// Seznam jednotlivych instrukci od usekove jednotky klientum a pripadne i jejich adresy
        /// </summary>
        public enum unitInfo : byte
        {
            aktualni_stav = 0x10,
            err,
            chyba = 0xFF,
        }

        /// <summary>
        /// Seznam jednotlivych instrukci urcenych jednotkam vyhybek
        /// </summary>
        public enum turnoutInstruction : byte
        {
            //[EnumMember(Value = "0x00-0xB5")]
            presna_poloha_serv,
            restart_jednotky = 0xB6,
            nastaveni_prodlevy_pred_natocenim = 0xB7,
            precteni_stavu_natoceni = 0xB8,
            nastaveni_dorazu = 0xB9,
            nastaveni_vyhybky = 0xBA,
            err,
        }

        /// <summary>
        /// Seznam jednotlivych instrukci od jednotky vyhybek pro hlaseni stavu a chyb
        /// </summary>
        public enum turnoutInfo : byte
        {
            natoceni_vlevo_vpravo,
            natoceni_ve_stupnich,
            err,
            chyba = 0xFF,
        }
    }

    /// <summary>
    /// Trida pro instrukce urcene usekove jednotce
    /// </summary>
    public class UnitInstructionPacket : Packet
    {
        public unitInstruction UnitInstruction { set; get; } //typ instrukce urceny usekove jednotce

        /// <summary>
        /// MEtoda pro vytvoreni instance z prijatych vlastnosti
        /// </summary>
        /// <param name="type">Typ instrukce</param>
        /// <param name="numberOfUnit">Cislo jednotky</param>
        /// <param name="data">Prijata data</param>
        public UnitInstructionPacket(unitInstruction type, uint numberOfUnit, byte data)
        {

            BytePacket = new List<byte>();

            Adress = new List<byte>();

            Data = new List<byte>();

            UnitInstruction = type;

            NumberOfAdress = (uint)unitAdress.zesilovac_DCC_1_W;

            NumberOfUnit = numberOfUnit;

            Head = 0x04; //hlavicka - unit instruction ji ma 4 byty

            AssingType(); //zjistit konkretni typ

            NumberOfAdressAndUnitToAdress(); //vytvoreni 2 bytove HEX adresy z adresy jednotky a cisla jednotky

            //Dle typu instrukce jsou vlozena data z dokumentace (viz. Albrecht Patrik - inovace elektroniky)
            if (type == unitInstruction.restart_jednotky)
            {
                Data.Add(0x01);

                Data.Add(0x01);
            }

            else if (type == unitInstruction.prodleva_odesilani_zmerenych_proudu)
            {
                Data.Add(0x02);

                if (data <= 0xFA || data == 0xFF)
                    Data.Add(data);

                else
                    Data.Add(0x32);
            }

            else if (type == unitInstruction.restart_H_mustku)
            {
                Data.Add(0x03);

                Data.Add(0x01);
            }

            else if (type == unitInstruction.nastaveni_zdroje)
            {
                Data.Add(0x04);

                if (data == 0x01 || data == 0x00)
                    Data.Add(data);

                else
                    Data.Add(0x00);
            }

            else if (type == unitInstruction.mereni_na_zvolenych_kanalech)
            {
                Data.Add(0x05);

                Data.Add(data);
            }

            else if (type == unitInstruction.precteni_stavu_jednotky)
            {
                Data.Add(0x10);

                Data.Add(0x01);

            }
            else
            {
                //chyba
                TCPPacket += ("Chyba v UnitInstructionPacket (unitInstruction type,uint numberOfUnit,byte data)\n");
                return;
            }

            for (int i = 0; i < 2; i++)
                Data.Add(0);

            SetBytePacket();

            TCPPacket = (Type + ":" + UnitInstruction.ToString() + "," + NumberOfUnit + "," + Data[0] + "," + Data[1] + "\n");

        }

        /// <summary>
        /// MEtoda pro vytvoreni instance z prijatych bytu
        /// </summary>
        /// <param name="bytePacket">List prijatych bytu</param>
        public UnitInstructionPacket(byte[] bytePacket)
        {
            SetPropetiesFromByte(bytePacket);

            if (Enum.IsDefined(typeof(unitInfo), Data[0]))
            {
                UnitInstruction = (unitInstruction)Data[0];
            }
            else
            {
                UnitInstruction = unitInstruction.err;
            }
            //UnitInstruction = (unitInstruction)Enum.ToObject(typeof(unitInstruction),Data[0]);                        
            TCPPacket += (UnitInstruction.ToString() + "," + NumberOfUnit + "," + Data[0] + "," + Data[1] + "\n");
        }

        //pro zpravu z TCP terminalu

        /// <summary>
        /// MEtoda pro vytvoreni instance z prijate tcp zpravy
        /// packet tvoren nejprve typem zpravy ; data,data ...
        /// </summary>
        /// <param name="tcpPacket"></param>
        public UnitInstructionPacket(string tcpPacket)
        {
            SetPropetiesFromTCP(tcpPacket);

            string[] s = tcpPacket.Split(':'); //oddeleni typu zpravu a ostatnich dat

            tcpPacket = s[1];

            s = tcpPacket.Split(','); //rozdeleni jednotlivych dat dle stredniku

            UnitInstruction = stringToUnitInstruction(s[0]);

            if (UnitInstruction == unitInstruction.err) //v pripade chyby skonci
            {
                TCPPacket = UnknowPacket(TCPPacket);
                Type = dataType.unknow.ToString();
                return;
            }

            NumberOfAdress = (uint)unitAdress.zesilovac_DCC_1_W; //adresa jednotky je zapis do jednotky

            uint a;

            //Zkus zjistit cislo jednotky
            if (!uint.TryParse(s[1], out a))
            {
                TCPPacket = UnknowPacket(TCPPacket);
                Type = dataType.unknow.ToString();
                return;
            }

            NumberOfUnit = a;

            Head = 0x04;

            Data.Add((byte)UnitInstruction);

            NumberOfAdressAndUnitToAdress(); //vytvoreni 2 bytove (11 bitu) HEX adresy z adresy jednotky a cisla jednotky

            //data - definovany 4 byty, ale vyuzity vzdy jen 2
            for (int i = 1; i < 2; i++)
            {
                byte b;

                //jedna data jsou cislo jednotky
                if (!byte.TryParse(s[2 + i], out b))
                {
                    TCPPacket = UnknowPacket(TCPPacket);
                    Type = dataType.unknow.ToString();
                    return;
                }
                Data.Add(b);
            }

            for (int i = 0; i < 2; i++) //zbytek doplnen nulami
                Data.Add(0);

            SetBytePacket();
        }

        /// <summary>
        /// MEtoda pro zjisteni typu unitInstruction
        /// </summary>
        /// <param name="str">Vstupni string z TCP zpravy</param>
        /// <returns>Konkretni typ unitInstruction</returns>
        public static unitInstruction stringToUnitInstruction(string str)
        {
            unitInstruction type;

            if (!Enum.TryParse(str, out type))
            {
                return unitInstruction.err;
            }

            return type;

        }

    }

    /// <summary>
    /// Trida pro zpravy odeslane usekovymi jednotkami klientum
    /// </summary>
    public class UnitInfoPacket : Packet
    {
        public unitInfo UnitInfo { set; get; } //typ instrukce odeslany usekovou jednotkou klientum

        /// <summary>
        /// Metoda pro zpracovani dat od usekove jednotky v podobe bytovych dat
        /// </summary>
        /// <param name="bytePacket"></param>
        public UnitInfoPacket(byte[] bytePacket)
        {
            SetPropetiesFromByte(bytePacket);

            if (Enum.IsDefined(typeof(unitInfo), Data[0]))
            {
                UnitInfo = (unitInfo)Data[0]; //typ unitInfo
            }
            else
            {
                UnitInfo = unitInfo.err; //pokud neni enum definovan, tak se jedna o error
            }

            TCPPacket += (UnitInfo.ToString() + "," + NumberOfUnit + "," + Data[0] + "," + Data[1] + "," + Data[2] + "," + Data[3] + "\n");

        }

        /// <summary>
        /// Metoda pro zpracovani dat od usekove jednotky v podobe prijate TCP zpravy
        /// </summary>
        /// <param name="tcpPacket">Prichozi TCP zprava</param>
        public UnitInfoPacket(string tcpPacket)
        {
            SetPropetiesFromTCP(tcpPacket);

            string[] s = tcpPacket.Split(':'); //rozdeleni typu zpravy a dat

            tcpPacket = s[1];

            s = tcpPacket.Split(','); //rozdeleni jednotlivych dat

            UnitInfo = stringToUnitInfo(s[0]);

            if (UnitInfo == unitInfo.err) //pokud unitInfo je error, tak se jedna o neznamy datovy typ a skonci
            {
                TCPPacket = UnknowPacket(TCPPacket);

                Type = dataType.unknow.ToString();

                return;
            }

            NumberOfAdress = (uint)unitAdress.zesilovac_DCC_2_R;

            uint a;

            if (!uint.TryParse(s[1], out a)) //vyextrahuj cislo jednotky z prijate TCP zpravy
            {
                TCPPacket = UnknowPacket(TCPPacket);

                Type = dataType.unknow.ToString();

                return;
            }

            NumberOfUnit = a;

            Data.Add((byte)UnitInfo);

            Head = 0x04;

            NumberOfAdressAndUnitToAdress(); //vytvoreni HEX adresy z adresy jednotky a cisla jednotky

            for (int i = 1; i < 4; i++)
            {
                byte b;

                if (!byte.TryParse(s[2 + i], out b)) //na nulove adrese typ zpravy, zbytek jsou data a prida je do listu
                {
                    TCPPacket = UnknowPacket(TCPPacket);

                    Type = dataType.unknow.ToString();

                    return;
                }
                Data.Add(b);
            }

            SetBytePacket();
        }

        /// <summary>
        /// Metoda pro vytvoreni instance z prijatych dat
        /// (kdyby bylo potreba do budoucna)
        /// </summary>
        /// <param name="type">Typ zpravy</param>
        /// <param name="numberOfUnit">Cislo jednotky</param>
        /// <param name="data1">Data</param>
        /// <param name="data2">Data</param>
        /// <param name="data3">Data</param>
        /// <param name="data4">Data</param>
        public UnitInfoPacket(unitInfo type, uint numberOfUnit, byte data1, byte data2, byte data3, byte data4)
        {

            BytePacket = new List<byte>();

            Adress = new List<byte>();

            Data = new List<byte>();

            UnitInfo = type;

            NumberOfAdress = (uint)unitAdress.zesilovac_DCC_2_R;

            NumberOfUnit = numberOfUnit;

            Head = 0x04;

            AssingType();

            NumberOfAdressAndUnitToAdress();

            Data.Add(data1);

            Data.Add(data2);

            Data.Add(data3);

            Data.Add(data4);

            SetBytePacket();

            TCPPacket = (Type + ":" + UnitInfo.ToString() + "," + NumberOfUnit + "," + Data[0] + "," + Data[1] + "," + Data[2] + "," + Data[3] + "\n");
        }

        /// <summary>
        /// Metoda pro ziskani typu unitInfo z prijateho stringu v tcp zprave
        /// </summary>
        /// <param name="str">Prijata data v podobě stringu</param>
        /// <returns>Vraci typ zpravy</returns>
        public static unitInfo stringToUnitInfo(string str)
        {
            unitInfo type;

            if (!Enum.TryParse(str, out type))
            {
                return unitInfo.err;
            }

            return type;

        }
    }

    /// <summary>
    /// Trida pro zasilani instrukce jednotce vyhybek
    /// </summary>
    public class TurnoutInstructionPacket : Packet
    {
        public turnoutInstruction TurnoutInstruction { set; get; } //typ instrukce odeslany jednotce vyhybek

        /// <summary>
        /// Metoda pro vytvoreni isntance z prijate TCP zpravy
        /// </summary>
        /// <param name="tcpPacket">Prijata TCP zprava ve forme stringu</param>
        public TurnoutInstructionPacket(string tcpPacket)
        {
            SetPropetiesFromTCP(tcpPacket);

            string[] s = tcpPacket.Split(':'); //rozdeleni typu zpravy a dat

            tcpPacket = s[1]; //data

            s = tcpPacket.Split(','); //rozdeleni jednotlivych dat

            TurnoutInstruction = stringToTurnoutInstruction(s[0]);

            if (TurnoutInstruction == turnoutInstruction.err) //v pripade erroru skonci
            {
                TCPPacket = UnknowPacket(TCPPacket);
                Type = dataType.unknow.ToString();
                return;
            }

            NumberOfAdress = (uint)unitAdress.vyhybky_W; //adresa jednotky

            uint a;
            if (!uint.TryParse(s[1], out a)) //zkus ziskat cislo jednotky
            {
                TCPPacket = UnknowPacket(TCPPacket);
                Type = dataType.unknow.ToString();
                return;
            }
            NumberOfUnit = a;

            //pocet prvku
            int numberOfValues = s.Length;

            NumberOfAdressAndUnitToAdress();

            Head = 0x08;

            //presne nastaveni vyhybek
            if (TurnoutInstruction == turnoutInstruction.presna_poloha_serv)
            {
                for (int i = 0; i < 8; i++)
                {
                    byte b;

                    if (!byte.TryParse(s[2 + i], out b))
                    {
                        TCPPacket = UnknowPacket(TCPPacket);
                        Type = dataType.unknow.ToString();
                        return;
                    }

                    //limity, kterych mohou hodnoty dosahovat
                    if (b <= 0xB4)
                        Data.Add(b);

                    else
                        Data.Add(0xB5);
                }
            }

            //nastaveni vyhybek vlevo ci vpravo
            else if (TurnoutInstruction == turnoutInstruction.nastaveni_vyhybky)
            {
                Data.Add((byte)TurnoutInstruction);

                byte b;

                if (!byte.TryParse(s[2], out b)) //vyber vyhybek
                {
                    TCPPacket = UnknowPacket(TCPPacket);
                    Type = dataType.unknow.ToString();
                    return;
                }

                Data.Add(b);

                byte c;

                if (!byte.TryParse(s[3], out c)) //nastaveni do spravne polohy
                {
                    TCPPacket = UnknowPacket(TCPPacket);
                    Type = dataType.unknow.ToString();
                    return;
                }

                Data.Add(c);

                for (int i = 0; i < 6; i++)
                    Data.Add(0);
            }

            //restart jednotky
            else if (TurnoutInstruction == turnoutInstruction.restart_jednotky)
            {
                Data.Add((byte)TurnoutInstruction);

                byte b;

                if (!byte.TryParse(s[2], out b))
                {
                    TCPPacket = UnknowPacket(TCPPacket);
                    Type = dataType.unknow.ToString();
                    return;
                }
                Data.Add(b);

                for (int i = 0; i < 6; i++)
                    Data.Add(0);
            }

            //nastaveni prodlevy pred otocenim
            else if (TurnoutInstruction == turnoutInstruction.nastaveni_prodlevy_pred_natocenim)
            {
                Data.Add((byte)TurnoutInstruction); //0xB7

                byte b;

                if (!byte.TryParse(s[2], out b))
                {
                    TCPPacket = UnknowPacket(TCPPacket);
                    Type = dataType.unknow.ToString();
                    return;
                }

                if (b > 0xFA)
                    Data.Add(0xFA);
                else
                    Data.Add(b);

                for (int i = 0; i < 6; i++)
                    Data.Add(0);
            }

            //ziskani aktualniho nastaveni a stavu jednotky
            else if (TurnoutInstruction == turnoutInstruction.precteni_stavu_natoceni)
            {
                Data.Add((byte)TurnoutInstruction); //0xB8

                byte b;

                if (!byte.TryParse(s[2], out b))
                {
                    TCPPacket = UnknowPacket(TCPPacket);
                    Type = dataType.unknow.ToString();
                    return;
                }

                if (b == 0x02)
                    Data.Add(b);

                else
                    Data.Add(0x01);

                for (int i = 0; i < 6; i++)
                    Data.Add(0);
            }

            //nastaveni softwarovych dorazu
            else if (TurnoutInstruction == turnoutInstruction.nastaveni_dorazu)
            {
                Data.Add((byte)TurnoutInstruction); //0xB9

                byte b;

                if (!byte.TryParse(s[2], out b))
                {
                    TCPPacket = UnknowPacket(TCPPacket);
                    Type = dataType.unknow.ToString();
                    return;
                }

                if (b >= 0x00 && b <= 0x07)
                    Data.Add(b);
                else
                    Data.Add(0xAA);

                byte c;

                if (!byte.TryParse(s[3], out c))
                {
                    TCPPacket = UnknowPacket(TCPPacket);
                    Type = dataType.unknow.ToString();
                    return;
                }

                if (c >= 0x00 && c <= 0xB4)
                    Data.Add(c);
                else
                    Data.Add(0x5A);

                byte d;

                if (!byte.TryParse(s[4], out d))
                {
                    TCPPacket = UnknowPacket(TCPPacket);
                    Type = dataType.unknow.ToString();
                    return;
                }

                if (d >= 0x00 && d <= 0xB4)
                    Data.Add(d);
                else
                    Data.Add(0x6E);

                for (int i = 0; i < 4; i++)
                    Data.Add(0);
            }
            else
            {
                TCPPacket = UnknowPacket(TCPPacket);
                Type = dataType.unknow.ToString();
                return;
            }

            SetBytePacket();

            //TCPPacket = (Type + ":" + TurnoutInstruction.ToString() + "," + NumberOfUnit + "," + Data[0] + "," + Data[1] + "\n");
        }

        /// <summary>
        /// Metoda pro vytvoreni instance z prijatych bytu dat
        /// </summary>
        /// <param name="bytePacket">Seznam prijatych bytu</param>
        public TurnoutInstructionPacket(byte[] bytePacket)
        {
            SetPropetiesFromByte(bytePacket);

            if (Enum.IsDefined(typeof(turnoutInstruction), Data[0])) //pokud je definovana hodnota shodna s prijatou
            {
                TurnoutInstruction = (turnoutInstruction)Data[0];
            }
            else if (Data[0] >= 0x00 && Data[0] <= 0xB5) //pokud se jedna o presne nastaveni polohy servomotoru
            {
                TurnoutInstruction = turnoutInstruction.presna_poloha_serv;
            }
            else //error
            {
                TurnoutInstruction = turnoutInstruction.err;
            }

            TCPPacket += (TurnoutInstruction.ToString() + "," + NumberOfUnit + TurnoutInstruction + "\n");
        }

        /// <summary>
        /// presne polohovani mikro servo pohonu (v praxi bude vyuzivano natoceni vlevo a vpravo)
        /// </summary>
        /// <param name="type">Typ instrukce</param>
        /// <param name="numberOfUnit">Cislo jednotky</param>
        /// <param name="data0">Presne natoceni vyhybky 1</param>
        /// <param name="data1">Presne natoceni vyhybky 2</param>
        /// <param name="data2">Presne natoceni vyhybky 3</param>
        /// <param name="data3">Presne natoceni vyhybky 4</param>
        /// <param name="data4">Presne natoceni vyhybky 5</param>
        /// <param name="data5">Presne natoceni vyhybky 6</param>
        /// <param name="data6">Presne natoceni vyhybky 7</param>
        /// <param name="data7">Presne natoceni vyhybky 8</param>
        public TurnoutInstructionPacket(turnoutInstruction type, uint numberOfUnit, byte data0, byte data1, byte data2, byte data3, byte data4, byte data5, byte data6, byte data7)
        {
            BytePacket = new List<byte>();

            Adress = new List<byte>();

            Data = new List<byte>();

            TurnoutInstruction = type;

            NumberOfAdress = (uint)unitAdress.vyhybky_W;

            NumberOfUnit = numberOfUnit;

            Head = 0x08;

            AssingType();

            NumberOfAdressAndUnitToAdress();

            for (int i = 0; i < 8; i++)
            {
                byte data = (byte)(typeof(turnoutInstruction).GetField("data" + i).GetValue(this));

                if (data <= 0xB4)
                    Data.Add(data);

                else
                    Data.Add(0xB5);
            }

            SetBytePacket();

            TCPPacket = (Type + ":" + TurnoutInstruction.ToString() + "," + NumberOfUnit + "," + Data[0] + "," + Data[1] + "," + Data[2] + "," + Data[3] + "," + Data[4] + "," + Data[5] + "," + Data[6] + "," + Data[7] + "\n");
        }

        /// <summary>
        /// Nastaveni vybranych vyhybek vlevo ci vpravo
        /// </summary>
        /// <param name="type">Typ instrukce</param>
        /// <param name="numberOfUnit">Cislo jednotky</param>
        /// <param name="data1">Vyber vyhybek, ktere se maji pretocit</param>
        /// <param name="data2">Nastaveni vyhybek do pozadovane pozice</param>
        public TurnoutInstructionPacket(turnoutInstruction type, uint numberOfUnit, byte data1, byte data2)
        {
            BytePacket = new List<byte>();

            Adress = new List<byte>();

            Data = new List<byte>();

            TurnoutInstruction = type;

            NumberOfAdress = (uint)unitAdress.vyhybky_W;

            NumberOfUnit = numberOfUnit;

            Head = 0x08;

            AssingType();

            NumberOfAdressAndUnitToAdress();

            Data.Add(0xBA);

            Data.Add(data1); //vyber vyhybek k pretoceni

            Data.Add(data2); //zvoleni konkretnich hodnot

            for (int i = 0; i < 5; i++)
                Data.Add(0);

            SetBytePacket();

            TCPPacket = (Type + ":" + TurnoutInstruction.ToString() + "," + NumberOfUnit + "," + Data[1] + "," + Data[2] + "\n");

        }

        /// <summary>
        /// Restart jednotky, nastaveni prodlevy a precteni stavu natoceni
        /// </summary>
        /// <param name="type">Typ instrukce</param>
        /// <param name="numberOfUnit">Cislo jednotky</param>
        /// <param name="data">Data (jsou-li potreba)</param>
        public TurnoutInstructionPacket(turnoutInstruction type, uint numberOfUnit, byte data)
        {
            BytePacket = new List<byte>();

            Adress = new List<byte>();

            Data = new List<byte>();

            TurnoutInstruction = type;

            NumberOfAdress = (uint)unitAdress.vyhybky_W;

            NumberOfUnit = numberOfUnit;

            Head = 0x08;

            AssingType();

            NumberOfAdressAndUnitToAdress();

            if (type == turnoutInstruction.restart_jednotky)
            {
                Data.Add(0xB6);

                Data.Add(0x01);
            }

            else if (type == turnoutInstruction.nastaveni_prodlevy_pred_natocenim)
            {
                Data.Add(0xB7);

                //omezeni meznich hodnot
                if (data > 0xFA)
                    Data.Add(0xFA);
                else
                    Data.Add(data);
            }

            else if (type == turnoutInstruction.precteni_stavu_natoceni)
            {
                Data.Add(0xB8);

                //omezeni meznich hodnot
                if (data == 0x02)
                    Data.Add(0x02);
                else
                    Data.Add(0x01);
            }

            for (int i = 0; i < 6; i++) //zbytek doplnen nulami
                Data.Add(0);

            SetBytePacket();

            TCPPacket = (Type + ":" + TurnoutInstruction.ToString() + "," + NumberOfUnit + "," + Data[1] + "\n");

        }

        /// <summary>
        /// Nastaveni softwarovych dorazu
        /// </summary>
        /// <param name="type">Typ instrukce</param>
        /// <param name="numberOfUnit">Cislo jednotky</param>
        /// <param name="data1">Vyber servopohonu</param>
        /// <param name="data2">Nastaveni leveho dorazu</param>
        /// <param name="data3">Nastaveni praveho dorazu</param>
        public TurnoutInstructionPacket(turnoutInstruction type, uint numberOfUnit, byte data1, byte data2, byte data3)
        {
            BytePacket = new List<byte>();

            Adress = new List<byte>();

            Data = new List<byte>();

            TurnoutInstruction = type;

            NumberOfAdress = (uint)unitAdress.vyhybky_W;

            NumberOfUnit = numberOfUnit;

            Head = 0x08;

            AssingType();

            NumberOfAdressAndUnitToAdress();

            Data.Add(0xB9);

            if (data1 >= 0x00 && data1 <= 0x07)
                Data.Add(data1);
            else
                Data.Add(0xAA); //vsechny servopohony

            if (data2 >= 0x00 && data2 <= 0xB4)
                Data.Add(data2);
            else
                Data.Add(0x5A);

            if (data3 >= 0x00 && data3 <= 0xB4)
                Data.Add(data3);
            else
                Data.Add(0x6E);

            for (int i = 0; i < 4; i++)
                Data.Add(0);

            SetBytePacket();

            TCPPacket = (Type + ":" + TurnoutInstruction.ToString() + "," + NumberOfUnit + "," + Data[1] + "," + Data[2] + "," + Data[3] + "\n");
        }

        /// <summary>
        /// Metoda pro urceni typu instrukce
        /// </summary>
        /// <param name="str">Vstupni string odpovidajici typu instrukce</param>
        /// <returns></returns>
        public static turnoutInstruction stringToTurnoutInstruction(string str)
        {
            turnoutInstruction type;

            if (!Enum.TryParse(str, out type))
            {
                return turnoutInstruction.err;
            }

            return type;
        }
    }

    /// <summary>
    /// Trida pro instrukce od jednotek vyhybek
    /// </summary>
    public class TurnoutInfoPacket : Packet
    {
        public turnoutInfo TurnoutInfo { set; get; } //typ instrukce od jednotky vyhybek odeslany klientum

        /// <summary>
        /// Metoda pro vytvoreni instance z priajteho TCP packetu
        /// </summary>
        /// <param name="tcpPacket">Prijata TCP zprava</param>
        public TurnoutInfoPacket(string tcpPacket)
        {
            SetPropetiesFromTCP(tcpPacket);

            //rozdeleni na typ zpravu a data
            string[] s = tcpPacket.Split(':');

            tcpPacket = s[1];

            //rozdeleni jednotlivych dat dle ","
            s = tcpPacket.Split(',');

            TurnoutInfo = stringToTurnoutInfo(s[0]);

            //kontrola, ze to neni error
            if (TurnoutInfo == turnoutInfo.err)
            {
                TCPPacket = UnknowPacket(TCPPacket);
                Type = dataType.unknow.ToString();
                return;
            }

            NumberOfAdress = (uint)unitAdress.vyhybky_R;

            //pocet prvku
            int numberOfValues = s.Length;

            //cislo jednotky
            uint a;
            if (!uint.TryParse(s[1], out a))
            {
                TCPPacket = UnknowPacket(TCPPacket);
                Type = dataType.unknow.ToString();
                return;
            }
            NumberOfUnit = a;

            NumberOfAdressAndUnitToAdress();

            Head = 0x08;

            //jedna se o data znaciti natoceni vlevo a vpravo
            if (TurnoutInfo == turnoutInfo.natoceni_vlevo_vpravo)
            {
                for (int i = 0; i < numberOfValues; i++)
                {
                    byte b;

                    if (!byte.TryParse(s[2 + i], out b))
                    {
                        TCPPacket = UnknowPacket(TCPPacket);
                        Type = dataType.unknow.ToString();
                        return;
                    }

                    //kontrola hodnot
                    if (b == 0xF0 || b == 0xF1 || b == 0xF2 || b == 0xF3)
                    {
                        Data.Add(b);
                    }

                    else
                    {
                        TCPPacket = UnknowPacket(TCPPacket);
                        Type = dataType.unknow.ToString();
                        return;
                    }
                }
            }

            else if (TurnoutInfo == turnoutInfo.natoceni_ve_stupnich)
            {
                for (int i = 0; i < numberOfValues; i++)
                {
                    byte b;

                    if (!byte.TryParse(s[2 + i], out b))
                    {
                        TCPPacket = UnknowPacket(TCPPacket);
                        Type = dataType.unknow.ToString();
                        return;
                    }

                    if (b >= 0x00 && b <= 0xB5)
                    {
                        Data.Add(b);
                    }
                    else
                    {
                        TCPPacket = UnknowPacket(TCPPacket);
                        Type = dataType.unknow.ToString();
                        return;
                    }

                }
            }

            else if (TurnoutInfo == turnoutInfo.chyba)
            {
                Data.Add((byte)TurnoutInfo);

                byte b;
                if (!byte.TryParse(s[1], out b))
                {
                    TCPPacket = UnknowPacket(TCPPacket);
                    Type = dataType.unknow.ToString();
                    return;
                }

                Data.Add(b);

                Data.Add(0xFF);

                for (int i = 0; i < 5; i++)
                    Data.Add(0);
            }

            else
            {
                TCPPacket = UnknowPacket(TCPPacket);
                Type = dataType.unknow.ToString();
                return;
            }

            SetBytePacket();
        }

        /// <summary>
        /// Metoda pro vytvoreni instance z prijatych bytu
        /// </summary>
        /// <param name="bytePacket"></param>
        public TurnoutInfoPacket(byte[] bytePacket)
        {
            SetPropetiesFromByte(bytePacket);

            if (Enum.IsDefined(typeof(turnoutInfo), Data[0]))
            {
                TurnoutInfo = (turnoutInfo)Data[0]; //pokud je definovano, tak prirad typ
            }
            else if (Data[0] >= 0xF0 && Data[0] <= 0xF3) //jedna se o info o natoceni vlevo/vpravo
            {
                TurnoutInfo = turnoutInfo.natoceni_vlevo_vpravo;
            }
            else if (Data[0] >= 0x00 && Data[0] <= 0xB5) //jedna se o info o natoceni ve stupnich
            {
                TurnoutInfo = turnoutInfo.natoceni_ve_stupnich;
            }
            else
            {
                TurnoutInfo = turnoutInfo.err; //chyba
            }

            TCPPacket = (TurnoutInfo.ToString() + ", Jednotka:" + NumberOfUnit + ", " + TurnoutInfo + "\n");
        }

        //metoda pro zjisteni konkretniho typu instrukce z prijateho stringu
        public static turnoutInfo stringToTurnoutInfo(string str)
        {
            turnoutInfo type;

            if (!Enum.TryParse(str, out type))
            {
                return turnoutInfo.err;
            }

            return type;
        }
    }

    /// <summary>
    /// Trida pro uvedeni vlaku do pohybu
    /// </summary>
    public class TrainMotionPacket : Packet
    {
        public string Name { set; get; } //jmeno vlaku

        public bool Reverse { set; get; } //ma jet pozpatku?

        public byte Speed { set; get; } //rychlost vlaku

        public uint ID { set; get; } //ID vlaku

        /// <summary>
        /// Metoda pro vytvoreni instance z priajtych dat
        /// </summary>
        /// <param name="locomotive">Vybrana lokomotiva</param>
        /// <param name="reverse">Informace, zdali ma jet pozpatku</param>
        /// <param name="speed">Rychlost vlaku</param>
        public TrainMotionPacket(Locomotive locomotive, bool reverse, byte speed)
        {
            BytePacket = new List<byte>();

            Adress = new List<byte>();

            Data = new List<byte>();

            ID = locomotive.ID;

            Name = locomotive.Name;

            Speed = speed;

            Reverse = reverse;

            ComposeByte(); //sloz packet

            AssingType(); //prirad adresu jednotky

            TCPPacket = (Type + ":" + Name + "," + Speed + "," + ((Reverse) ? "reverse\n" : "ahead\n"));


        }

        /// <summary>
        /// Metoda pro vytvoreni instance z prijate TCP zpravy
        /// </summary>
        /// <param name="tcpPacket">Prijata TCP zprava</param>
        public TrainMotionPacket(string tcpPacket)
        {
            SetPropetiesFromTCP(tcpPacket);

            string[] s = tcpPacket.Split(':'); //rozdeleni typu zpravy a dat

            tcpPacket = s[1];

            s = tcpPacket.Split(','); //rozdeleni jednotlivych dat

            Name = s[0];

            ID = LocomotiveInfo.NameToID(Name); //ID lokomotivy ze jmena lokomotivy

            byte a;

            if (!byte.TryParse(s[1], out a)) //pokud o zjisteni rychlosti
            {
                TCPPacket = UnknowPacket(TCPPacket);
                Type = dataType.unknow.ToString();
                return;
            }

            Speed = a;

            bool unknowPacket = false;

            if (s[2] == "reverse")
            {
                Reverse = true;
            }
            else if (s[2] == "ahead")
            {
                Reverse = false;
            }
            else
            {
                unknowPacket = true;
            }

            if ((ID == 0) || (Speed > 31) || (Speed < 0) || (unknowPacket))
            {
                TCPPacket = UnknowPacket(TCPPacket);
                Type = dataType.unknow.ToString();
            }

            ComposeByte();
        }

        /// <summary>
        /// MEtoda pro vytvoreni isntance z prijatych bytu
        /// </summary>
        /// <param name="bytePacket"></param>
        public TrainMotionPacket(byte[] bytePacket)
        {
            SetPropetiesFromByte(bytePacket);

            Name = LocomotiveInfo.IDToName(Data[0]);

            ID = Data[0];

            Speed = (byte)(Data[1] & 0x1f);

            Reverse = (((Data[1] >> 5) & 0x1) == 1) ? false : true;

            TCPPacket += (Name + "," + Speed + "," + ((Reverse) ? "reverse\n" : "ahead\n"));

        }

        /// <summary>
        /// Metoda pro slozeni datovych bytu k odeslani
        /// </summary>
        private void ComposeByte()
        {
            NumberOfAdress = (uint)unitAdress.generator_DCC_W;

            NumberOfUnit = 1;

            Head = 0x08;

            NumberOfAdressAndUnitToAdress();

            Data.Add((byte)ID);

            Data.Add(SecondByte());

            for (int i = 0; i < 6; i++)
            {
                Data.Add(0);
            }

            SetBytePacket();

        }

        /// <summary>
        /// Metoda pro vypocet druheho datoveho bytu slozeneho z rychlosti a informaci o tom, zdali ma jet pozpatku
        /// </summary>
        /// <returns></returns>
        private byte SecondByte()
        {

            string secondDataByte = "01";

            if (Reverse)
            {
                secondDataByte += "0";
            }
            else
            {
                secondDataByte += "1";
            }

            string s = Convert.ToString(Speed, 2);

            while (s.Length < 5)
            {
                s = "0" + s;
            }

            secondDataByte += s;

            return (byte)Convert.ToUInt32(secondDataByte, 2);
        }

    }

    /// <summary>
    /// Trida pro vytvoreni specifickeho pohybu dle jizdniho radu (neni vyuzito, nechato v pripade potreby pro budouci vyuziti)
    /// </summary>
    public class TrainMotionInstructionPacket : Packet
    {
        public TrainMotionPacket TrainMoveInfo { set; get; } //Data shodna s TrainMotionPacket (Name, ID, Reverse, Speed)

        public Section Section { set; get; } //nazev ciloveho useku

        public uint WaitTime { set; get; } //doba prejezdu po dosazeni ciloveho useku

        /// <summary>
        /// Metoda pro vytvoreni instance z prijate TCP zpravy
        /// </summary>
        /// <param name="tcpPacket">Prichozi TCP zprava</param>
        public TrainMotionInstructionPacket(string tcpPacket)
        {
            SetPropetiesFromTCP(tcpPacket);

            TrainMoveInfo = new TrainMotionPacket(TCPPacket);

            if (TrainMoveInfo.Type == dataType.unknow.ToString())
            {
                TCPPacket = UnknowPacket(TCPPacket);
                Type = dataType.unknow.ToString();
            }

            if (TrainMoveInfo.Speed < 4)
            {
                TCPPacket = UnknowPacket(TCPPacket);
                Type = dataType.unknow.ToString();
            }

            string[] s = tcpPacket.Split(':');

            tcpPacket = s[1];

            s = tcpPacket.Split(',');

            foreach (Section section in SectionInfo.listOfSection)
            {
                if (section.Name == s[3])
                {
                    Section = section;
                }
            }

            if (Section == null)
            {
                TCPPacket = UnknowPacket(TCPPacket);
                Type = dataType.unknow.ToString();
                return;
            }

            uint a;

            if (!uint.TryParse(s[4], out a))
            {
                TCPPacket = UnknowPacket(TCPPacket);
                Type = dataType.unknow.ToString();
                return;
            }

            WaitTime = a;
        }

        /// <summary>
        /// Metoda pro vytvoreni instance z prijatych vlastnosti
        /// </summary>
        /// <param name="locomotive">Vybrana Lokomotiva</param>
        /// <param name="reverse">Ma jet pozpatku?</param>
        /// <param name="speed">Rychlost</param>
        /// <param name="section">Cilovy usek</param>
        /// <param name="waitTime">Doba prejezdu</param>
        public TrainMotionInstructionPacket(Locomotive locomotive, bool reverse, byte speed, Section section, uint waitTime)
        {
            TrainMoveInfo = new TrainMotionPacket(locomotive, reverse, speed);

            Section = section;

            WaitTime = waitTime;

            Type = dataType.train_move_to_place.ToString();

            TCPPacket = TrainMoveInfo.TCPPacket.TrimEnd() + "," + Section + "," + WaitTime + "\n";

            string[] s = TCPPacket.Split(':');

            TCPPacket = Type + ":" + s[1];
        }
    }

    /// <summary>
    /// Trida pro funkce lokomotiv
    /// </summary>
    public class TrainFunctionPacket : Packet
    {
        public bool Lights { set; get; } //ma rozsvitit svetla?

        public string Name { set; get; } //jmeno lokomotivy

        public uint ID { set; get; } //id lokomotivy

        /// <summary>
        /// Metoda pro vytvoreni instance z prijatych bytu
        /// </summary>
        /// <param name="bytePacket"></param>
        public TrainFunctionPacket(byte[] bytePacket)
        {
            SetPropetiesFromByte(bytePacket);

            ID = Data[0];

            Name = LocomotiveInfo.IDToName(ID);

            TCPPacket += (Name + ",");

            Lights = (((Data[1] >> 4) & 0x1) == 1) ? true : false;

            TCPPacket += ((Lights) ? "on\n" : "off\n");

        }

        /// <summary>
        /// Metoda pro vytvoreni instance z prijatych vlastnosti
        /// </summary>
        /// <param name="locomotive">Vybrana lokomotiva</param>
        /// <param name="lights">Zdali ma rozsvitit svetla</param>
        public TrainFunctionPacket(Locomotive locomotive, bool lights)
        {
            BytePacket = new List<byte>();

            Adress = new List<byte>();

            Data = new List<byte>();

            ID = locomotive.ID;

            Name = locomotive.Name;

            Lights = lights;

            ComposeByte();

            AssingType();

            TCPPacket += (Type + ":" + Name + "," + ((Lights) ? "on\n" : "off\n"));
        }

        /// <summary>
        /// Metoda pro vytvoreni instance z prijate TCP zpravy
        /// </summary>
        /// <param name="tcpPacket">Prichozi TCP zprava</param>
        public TrainFunctionPacket(string tcpPacket)
        {
            SetPropetiesFromTCP(tcpPacket);

            string[] s = tcpPacket.Split(':');

            tcpPacket = s[1];

            s = tcpPacket.Split(',');

            Name = s[0];

            ID = LocomotiveInfo.NameToID(Name);

            bool unknowPacket = false;

            if (s[1] == "on") //ma rozsvitit?
            {
                Lights = true;
            }
            else if (s[1] == "off")
            {
                Lights = false;
            }
            else
            {
                unknowPacket = true;
            }

            if ((ID == 0) || (unknowPacket))
            {
                TCPPacket = UnknowPacket(TCPPacket);
            }

            ComposeByte(); //slozit byte
        }

        /// <summary>
        /// Metoda pro slozeny bytoveho packetu
        /// </summary>
        private void ComposeByte()
        {
            NumberOfAdress = (uint)unitAdress.generator_DCC_W;

            NumberOfUnit = 1;

            Head = 0x08;

            NumberOfAdressAndUnitToAdress();

            Data.Add((byte)ID);

            Data.Add(SecondByte());

            for (int i = 0; i < 6; i++)
            {
                Data.Add(0);
            }

            SetBytePacket();
        }

        /// <summary>
        /// MEtoda pro vytvoreni druheho datoveho bytu
        /// </summary>
        /// <returns></returns>
        private byte SecondByte()
        {

            string secondDataByte = "100";

            if (Lights)
            {
                secondDataByte += "1";
            }
            else
            {
                secondDataByte += "0";
            }

            secondDataByte += "0000";

            return (byte)Convert.ToUInt32(secondDataByte, 2);
        }
    }


    /// <summary>
    /// Trida pro komunikaci s mikrokontrolerem s LED diplejem (neni vyuzito)
    /// </summary>
    public class OLEDInformationPacket : Packet
    {
        public List<string> Messages { set; get; }

        public OLEDInformationPacket(string tcpPacket)
        {

            TCPPacket = tcpPacket;

            string[] s = TCPPacket.Split(':');

            Type = (s[0] + ":");

            if (RecognizeTCPType(Type) == dataType.unknow)
            {
                this.TCPPacket = UnknowPacket(this.TCPPacket);
            }

            string str = tcpPacket.Substring(8);

            s = str.Split('*');

            Messages = new List<string>();

            for (int i = 0; i < s.Length; i++)
            {
                Messages.Add(s[i]);
            }

        }

        public OLEDInformationPacket(List<string> messages)
        {

            Messages = messages;

            Type = dataType.oled_info.ToString();

            TCPPacket = (Type + ":");

            for (int i = 0; i < Messages.Count - 1; i++)
            {
                TCPPacket += (Messages[i] + "*");
            }

            TCPPacket += (Messages[Messages.Count - 1]);

            TCPPacket += "\n";

        }
    }

    /// <summary>
    /// Trida pro zpracovani dat o obsazenosti useku od usekove jednotky
    /// </summary>
    public class OccupancySectionPacket : Packet
    {
        public List<Section> Sections { set; get; } //list, do ktereho jsou ukladana data

        /// <summary>
        /// Metoda pro vytvoreni instance z prijateho bytoveho packetu
        /// </summary>
        /// <param name="bytePacket">Prijate byty</param>
        public OccupancySectionPacket(byte[] bytePacket)
        {
            Sections = new List<Section>();

            SetPropetiesFromByte(bytePacket); //zjisteni vlastnosti z prijatych bytu

            RecognizeUnit(); //zjisteni cisla jednotky, ktera zaslala data

            foreach (Section s in Sections) //ulozeni odberu proudu prijatych sekci
            {
                s.Current = Data[Sections.IndexOf(s)];
                TCPPacket += (s.Name + "=" + s.Current + ",");
            }

            TCPPacket = TCPPacket.Substring(0, TCPPacket.Length - 1);

            TCPPacket += "\n";
        }

        /// <summary>
        /// MEtoda pro vytvoreni instance z prijate TCP zpravy
        /// </summary>
        /// <param name="tcpPacket">Prichozi string ve forme TCP zpravy</param>
        public OccupancySectionPacket(string tcpPacket)
        {
            Sections = new List<Section>();

            SetPropetiesFromTCP(tcpPacket);

            NumberOfAdress = (uint)unitAdress.zesilovac_DCC_1_R;

            RecognizeSection(tcpPacket); //zjisteni konkretni sekce

            NumberOfAdressAndUnitToAdress(); //vytvoreni hex adresy z cisla jednoky a adresy

            Head = 0x08;

            foreach (Section section in Sections)
            {
                Data.Add((byte)section.Current);
            }

        }

        /// <summary>
        /// Metoda pro zjisteni cisla jednotky a prirazeni odberu proudu jednotlivym usekum
        /// </summary>
        /// <param name="tcpPacket">TCP zprava ve forme stringu</param>
        private void RecognizeSection(string tcpPacket)
        {
            string[] s = tcpPacket.Split(':');

            tcpPacket = s[1];

            string[] sec = tcpPacket.Split(',');

            string[] sectio = sec[0].Split('=');

            foreach (Section section in SectionInfo.listOfSection)
            {
                if (section.Name == sectio[0])
                {
                    NumberOfUnit = section.NumberOfUnit;
                    break;
                }
            }

            int i = 0;
            foreach (string sect in sec)
            {
                string[] secti = sect.Split('=');

                Sections.Add(new Section(secti[0], NumberOfUnit, (uint)i, uint.Parse(secti[1])));
                i++;
            }
        }

        /// <summary>
        /// Metoda pro rozliseni sekci dle cisla jednotky
        /// </summary>
        private void RecognizeUnit()
        {
            switch (NumberOfUnit)
            {
                case 1:
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            Sections.Add(SectionInfo.listOfSection[i]);
                        }
                        break;
                    }
                case 2:
                    {
                        for (int i = 8; i < 16; i++)
                        {
                            Sections.Add(SectionInfo.listOfSection[i]);
                        }
                        break;
                    }

                case 3:
                    {
                        for (int i = 16; i < 24; i++)
                        {
                            Sections.Add(SectionInfo.listOfSection[i]);
                        }
                        break;
                    }
                case 4:
                    {

                        for (int i = 24; i < 32; i++)
                        {
                            Sections.Add(SectionInfo.listOfSection[i]);
                        }
                        break;

                    }
                case 5:
                    {

                        for (int i = 32; i < 40; i++)
                        {
                            Sections.Add(SectionInfo.listOfSection[i]);
                        }
                        break;

                    }
                case 6:
                    {

                        for (int i = 40; i < 48; i++)
                        {
                            Sections.Add(SectionInfo.listOfSection[i]);
                        }
                        break;

                    }
                case 7:
                    {

                        for (int i = 48; i < 56; i++)
                        {
                            Sections.Add(SectionInfo.listOfSection[i]);
                        }
                        break;

                    }
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Trida definujici jednotlive izolovane useky kolejiste
    /// </summary>
    public class Section
    {
        public string Name { set; get; } //jmeno useku
        public uint NumberOfUnit { set; get; } //cislo jednotky, ktera ma dany usek
        public uint ModulePosition { set; get; } //pozice kanalu ktery zmeril odber proudu

        public uint current; //hodnota odberu proudu

        /// <summary>
        /// Metoda predstavujici zmereny proud v jendotlivych usecich
        /// </summary>
        public uint Current
        {
            set
            {
                current = value;
                OccupancySection();

            }
            get
            {
                return current;
            }
        }

        /// <summary>
        /// MEtoda pro obsazenost úseku
        /// </summary>
        public bool Occupancy { set; get; }

        /// <summary>
        /// Metoda pro prirazeni jednotlivych useku odpovidajici usekum z konfiguracniho souboru
        /// </summary>
        /// <param name="name">Jmeno useku</param>
        /// <param name="numberOfUnit">Cislo jednotky</param>
        /// <param name="modulePosition">Merici kanal</param>
        public Section(string name, uint numberOfUnit, uint modulePosition)
        {
            Name = name;
            NumberOfUnit = numberOfUnit;
            ModulePosition = modulePosition;
            Current = 0;
            Occupancy = false;
            //ModulePosition = modulePosition;
        }

        /// <summary>
        /// Pro pridani odberu proudu a dat z TCP komunikace
        /// </summary>
        /// <param name="name"></param>
        /// <param name="numberOfUnit"></param>
        /// <param name="modulePosition"></param>
        /// <param name="current"></param>
        public Section(string name, uint numberOfUnit, uint modulePosition, uint current)
        {
            Name = name;
            NumberOfUnit = numberOfUnit;
            ModulePosition = modulePosition;
            Current = current;
            OccupancySection();

        }

        /// <summary>
        /// Metoda pro zjisteni vlastnosti z nazvu useku
        /// </summary>
        /// <param name="name">Jmeno useku</param>
        public Section(string name)
        {
            Name = name;

            foreach (Section section in SectionInfo.listOfSection)
            {
                if (section.Name == name)
                {
                    NumberOfUnit = section.NumberOfUnit;
                    ModulePosition = section.ModulePosition;
                    break;
                }
            }
            Current = 0;
            OccupancySection();

        }

        /// <summary>
        /// Metoda pro ulozeni odberu proudu a obsazenosti dle odberu proudu
        /// </summary>
        private void OccupancySection()
        {
            Occupancy = (Current > 40) ? true : false;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// Metoda pro nacteni izolovanych useku z konfiguracniho souboru
    /// </summary>
    public class SectionInfo
    {
        public static List<Section> listOfSection = InitSections();

        /// <summary>
        /// Metoda pro inicializace kolejovych useku
        /// </summary>
        /// <returns>Vraci seznam useku</returns>
        public static List<Section> InitSections()
        {
            if (listOfSection != null) //pokud useky byly nacteny, tak je vrat
            {
                return listOfSection;
            }

            List<Section> list = new List<Section>();

            XmlDocument doc = new XmlDocument();

            doc.Load("C:\\Users\\Tomáš\\Documents\\ZCU_FEL\\v1_diplomka\\TestDesign\\TestDesignTT\\ControlLogic\\conf_kolejiste.xml");

            XmlNodeList sectionNodes = doc.SelectNodes("//section"); //najdi jednotlive useky v konfiguracnim souboru

            foreach (XmlNode sectionNode in sectionNodes)
            {
                string name = sectionNode.Attributes["id"].Value; //jmeno useku

                uint numberOfUnit = uint.Parse(sectionNode.SelectSingleNode("moduleid").InnerText); //cislo jednotky

                uint modulePosition = uint.Parse(sectionNode.SelectSingleNode("moduleposition").InnerText); //merici kanal

                list.Add(new Section(name, numberOfUnit, modulePosition)); //pridani do listu useku
            }

            list.Sort((a, b) => //setrideni listu nejprve od nejnizsiho cisla jednotky a pote dle nejnizsiho kanalu
            {
                if (a.NumberOfUnit == b.NumberOfUnit)
                {
                    return a.ModulePosition.CompareTo(b.ModulePosition);
                }
                else
                {
                    return a.NumberOfUnit.CompareTo(b.NumberOfUnit);
                }
            });

            listOfSection = list; //ulozeni nacteneho listu do seznamu useku

            return listOfSection;
        }
    }

    /// <summary>
    /// Trida pro nacteni seznamu lokomotiv z konfiguracniho souboru
    /// </summary>
    public class LocomotiveInfo
    {
        public static List<Locomotive> listOfLocomotives = Initloco();

        private static List<Locomotive> Initloco()
        {
            if (listOfLocomotives != null)
            {
                return listOfLocomotives; //pokud jiz jsou nacteny, tak vrat lokomotivy
            }

            List<Locomotive> list = new List<Locomotive>(); //predpripraveni listu na lokomotivy

            ConfigItem configItems = new ConfigItem("C:\\Users\\Tomáš\\Documents\\ZCU_FEL\\v1_diplomka\\TestDesign\\TestDesignTT\\TrainTTLibrary\\locomotives.xml");

            foreach (Item item in configItems.Items) //nasteni jednotlivych lokomotiv
            {
                list.Add(new Locomotive(item.Str, item.Num));
            }
            listOfLocomotives = list;

            return listOfLocomotives;
        }

        /// <summary>
        /// Metoda pro zjisteni jmena lokomotivy
        /// </summary>
        /// <param name="id">ID lokomotivy</param>
        /// <returns>Jmeno lokomotivy</returns>
        public static string IDToName(uint id)
        {

            foreach (Locomotive locomotive in LocomotiveInfo.listOfLocomotives)
            {

                if (locomotive.ID == id)
                {
                    return locomotive.Name;
                }
            }
            return "Error";
        }

        /// <summary>
        /// Metoda pro zjisteni ID lokomotivy ze jmena
        /// </summary>
        /// <param name="name">Jmeno lokomotivy</param>
        /// <returns>ID lokomotivy</returns>
        public static uint NameToID(string name)
        {
            foreach (Locomotive locomotive in LocomotiveInfo.listOfLocomotives)
            {
                if (locomotive.Name == name)
                {
                    return locomotive.ID;
                }
            }
            return 0;
        }
    }

    /// <summary>
    /// Trida definujici lokomotivy
    /// </summary>
    public class Locomotive
    {
        public string Name { set; get; } //jmeno lokomotivy
        public uint ID { set; get; } //ID lokomotivy

        /// <summary>
        /// Metoda pro prirazeni lokomotivy ze jmena
        /// </summary>
        /// <param name="name">Jmeno lokomotivy</param>
        public Locomotive(string name)
        {
            Name = name;
            ID = LocomotiveInfo.NameToID(name);

        }

        /// <summary>
        /// Metoda pro prirazeni lokomotivy dle jejiho ID
        /// </summary>
        /// <param name="id">ID lokomotivy</param>
        public Locomotive(uint id)
        {
            ID = id;
            Name = LocomotiveInfo.IDToName(id);
        }

        /// <summary>
        /// Metoda pro lokomotivu dle jejiho jmena a id
        /// </summary>
        /// <param name="name">Jmeno lokomotivy</param>
        /// <param name="id">ID lokomotivy</param>
        public Locomotive(string name, uint id)
        {
            ID = id;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

    }

    /// <summary>
    /// Pomocna trida pro nacteni lokomotiv
    /// </summary>
    public class Item
    {
        public string Str { get; set; }
        public uint Num { get; set; }

    }

    /// <summary>
    /// Trida pro nacteni lokomotiv
    /// </summary>
    public class ConfigItem
    {
        public List<Item> Items = new List<Item>();

        public ConfigItem(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    XmlSerializer serializer = new XmlSerializer(Items.GetType());
                    using (StreamReader sr = new StreamReader(fileName))
                    {
                        Items = (List<Item>)serializer.Deserialize(sr);
                    }
                }
                else throw new FileNotFoundException("File not found");
            }
            catch (Exception ex)
            {

            }
        }
    }
}
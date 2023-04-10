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
    public class Packet
    {
        public List<byte> BytePacket { set; get; }

        public string TCPPacket { set; get; }

        public string Type { set; get; }

        public byte Head { set; get; }

        public List<byte> Adress { set; get; }

        public List<byte> Data { set; get; }

        public byte CRC { set; get; }

        public UInt32 NumberOfUnit { set; get; }

        public UInt32 NumberOfAdress { set; get; }

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

        protected void AssingType()
        {

            switch (NumberOfAdress)
            {
                case (int)unitAdress.emergency_W:

                    Type = dataType.emergency.ToString();
                    break;

                case (int)unitAdress.generator_DCC_W:

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

                case (int)unitAdress.generator_DCC_R: // message from generator DCC

                    if ((Head == 0x01) && (Data[0] == 0x55)) // watchdog message: 01 20 20 55 88
                    {
                        Type = dataType.watchdog.ToString();
                        break;
                    }
                    else
                    {
                        Type = dataType.unknow.ToString();
                        break;
                    }

                case (int)unitAdress.zesilovac_DCC_1_R: // message from DCC amplifier - odbery - DCC 1 R

                    if (Head == 0x08)
                    {
                        Type = dataType.occupancy_section.ToString(); //zprava od zesilovace - odbery proudu
                    }
                    else
                    {
                        Type = dataType.unknow.ToString();
                    }

                    break;

                case (int)unitAdress.zesilovac_DCC_2_R: // message from DCC amplifier - stav/chyba - DCC 2 R

                    if (Head == 0x04)
                    {
                        Type = dataType.unit_info.ToString(); //zprava od ridiciho zesilovace - aktualni stav ci chyba
                    }
                    else
                    {
                        Type = dataType.unknow.ToString();
                    }
                    break;

                case (int)unitAdress.zesilovac_DCC_1_W:
                    if (Head == 0x04)
                    {
                        Type = dataType.unit_instruction.ToString(); //konfiguraci data zesilovaci
                    }
                    else
                    {
                        Type = dataType.unknow.ToString();
                    }
                    break;

                case (int)unitAdress.zesilovac_DCC_2_W:

                    Type = dataType.unknow.ToString();

                    break;

                case (int)unitAdress.vyhybky_W:
                    if (Head == 0x08)
                        Type = dataType.turnout_instruction.ToString();

                    else
                        Type = dataType.unknow.ToString();

                    break;

                case (int)unitAdress.vyhybky_R:
                    if (Head == 0x08)
                        Type = dataType.turnout_info.ToString();

                    else
                        Type = dataType.unknow.ToString();

                    break;

                case (int)unitAdress.navestidla_W:

                    Type = dataType.unknow.ToString();
                    break;

                case (int)unitAdress.tocna_W:

                    Type = dataType.unknow.ToString();
                    break;

                case (int)unitAdress.tocna_R:

                    Type = dataType.unknow.ToString();
                    break;

                default:

                    Type = dataType.unknow.ToString();
                    break;
            }

        }

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

        protected void AdressToNumberOfAdressAndUnit() // z HEX adresy vyčíst číslo jednotky a adresu jednotky 
        {

            UInt32 adr = (uint)((Adress[0] << 8) | Adress[1]);

            adr = adr >> 5;

            NumberOfAdress = adr >> 7;

            NumberOfUnit = adr & ((UInt32)0x7f); // 0x7f = 1111111

        }

        protected void NumberOfAdressAndUnitToAdress() // z čísla jednotky a adresy jednotky vytvořit HEX adresu pro paket 
        {
            //UInt32 i = ((NumberOfAdress << 12) | (NumberOfUnit << 5));

            UInt16 i = (UInt16)(NumberOfAdress << 12);
            i = (ushort)(i | ((UInt16)(NumberOfUnit << 5)));

            //uint i = ((NumberOfAdress << 12) | (NumberOfUnit << 5));

            Adress.Add((byte)(i >> 8));

            Adress.Add((byte)(i));

            //int u = 0;

            //Adress.Add((byte)(i & 0xff));

        }

        protected void SetBytePacket()
        {

            BytePacket.Add(Head);

            BytePacket.AddRange(Adress);

            BytePacket.AddRange(Data);

            CRC = VypocetCRC(BytePacket);

            BytePacket.Add(CRC);

        }

        static public string UnknowPacket(string tcpPacket)
        {
            string[] s = tcpPacket.Split(':');

            tcpPacket = s[1];

            return dataType.unknow.ToString() + ":" + s[1];
        }

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

        public enum dataType
        {
            watchdog,
            occupancy_section,
            unit_info,
            turnout_info,
            // zprávy odesílané řídící jednotkou
            train_move,
            train_function,
            train_move_to_place,
            unit_instruction,
            turnout_instruction,
            // zprávy pro řídící jednotku
            unknow, // neznámý typ zprávy
            oled_info, // textové zprávy mezi TCP uživateli s bíže nespecifikovanými parametry
            emergency,// adresa 0000
            // zprávy pro komunikaci

        };

        public enum unitAdress
        {
            emergency_W,
            generator_DCC_W,
            generator_DCC_R,
            zesilovac_DCC_1_R,	//namerene proudy
            zesilovac_DCC_2_R,	//unit info
            zesilovac_DCC_1_W,
            zesilovac_DCC_2_W,
            vyhybky_W,
            vyhybky_R,
            navestidla_W,
            tocna_W,
            tocna_R,
        };

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

        public enum unitInfo : byte
        {
            aktualni_stav = 0x10,
            err,
            chyba = 0xFF,
        }

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

        public enum turnoutInfo : byte
        {
            natoceni_vlevo_vpravo,
            natoceni_ve_stupnich,
            err,
            chyba = 0xFF,
        }
    }

    public class UnitInstructionPacket : Packet
    {
        unitInstruction UnitInstruction { set; get; }

        public UnitInstructionPacket(unitInstruction type, uint numberOfUnit, byte data)
        {

            BytePacket = new List<byte>();

            Adress = new List<byte>();

            Data = new List<byte>();

            UnitInstruction = type;

            NumberOfAdress = (uint)unitAdress.zesilovac_DCC_1_W;

            NumberOfUnit = numberOfUnit;

            Head = 0x04;

            AssingType();

            NumberOfAdressAndUnitToAdress();

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
        public UnitInstructionPacket(string tcpPacket)
        {
            SetPropetiesFromTCP(tcpPacket);

            string[] s = tcpPacket.Split(':');

            tcpPacket = s[1];

            s = tcpPacket.Split(',');

            UnitInstruction = stringToUnitInstruction(s[0]);

            if (UnitInstruction == unitInstruction.err)
            {
                TCPPacket = UnknowPacket(TCPPacket);
                Type = dataType.unknow.ToString();
                return;
            }

            NumberOfAdress = (uint)unitAdress.zesilovac_DCC_1_W;

            uint a;

            //number of unit
            if (!uint.TryParse(s[1], out a))
            {
                TCPPacket = UnknowPacket(TCPPacket);
                Type = dataType.unknow.ToString();
                return;
            }

            NumberOfUnit = a;

            Head = 0x04;

            Data.Add((byte)UnitInstruction);

            NumberOfAdressAndUnitToAdress();

            //data - definovany 4 byty, ale vyuzity vzdy jen 2
            for (int i = 1; i < 2; i++)
            {
                byte b;

                //data0 (unit instruction je znama z vyse)
                if (!byte.TryParse(s[2 + i], out b))
                {
                    TCPPacket = UnknowPacket(TCPPacket);
                    Type = dataType.unknow.ToString();
                    return;
                }
                Data.Add(b);
            }

            for (int i = 0; i < 2; i++)
                Data.Add(0);

            SetBytePacket();
        }

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

    public class UnitInfoPacket : Packet
    {
        public unitInfo UnitInfo { set; get; }

        public UnitInfoPacket(byte[] bytePacket)
        {
            SetPropetiesFromByte(bytePacket);

            if (Enum.IsDefined(typeof(unitInfo), Data[0]))
            {
                UnitInfo = (unitInfo)Data[0];
            }
            else
            {
                UnitInfo = unitInfo.err;
            }

            TCPPacket += (UnitInfo.ToString() + "," + NumberOfUnit + "," + Data[0] + "," + Data[1] + "," + Data[2] + "," + Data[3] + "\n");

        }


        public UnitInfoPacket(string tcpPacket)
        {
            SetPropetiesFromTCP(tcpPacket);

            string[] s = tcpPacket.Split(':');

            tcpPacket = s[1];

            s = tcpPacket.Split(',');

            UnitInfo = stringToUnitInfo(s[0]);

            if (UnitInfo == unitInfo.err)
            {
                TCPPacket = UnknowPacket(TCPPacket);

                Type = dataType.unknow.ToString();

                return;
            }

            NumberOfAdress = (uint)unitAdress.zesilovac_DCC_2_R;

            uint a;

            if (!uint.TryParse(s[1], out a))
            {
                TCPPacket = UnknowPacket(TCPPacket);

                Type = dataType.unknow.ToString();

                return;
            }

            NumberOfUnit = a;

            Data.Add((byte)UnitInfo);

            Head = 0x04;

            NumberOfAdressAndUnitToAdress();

            for (int i = 1; i < 4; i++)
            {
                byte b;

                if (!byte.TryParse(s[2 + i], out b))
                {
                    TCPPacket = UnknowPacket(TCPPacket);

                    Type = dataType.unknow.ToString();

                    return;
                }
                Data.Add(b);
            }

            SetBytePacket();
        }

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

    public class TurnoutInstructionPacket : Packet
    {
        turnoutInstruction TurnoutInstruction { set; get; }

        //TODO or Change
        public TurnoutInstructionPacket(string tcpPacket)
        {
            SetPropetiesFromTCP(tcpPacket);

            string[] s = tcpPacket.Split(':');

            tcpPacket = s[1];

            s = tcpPacket.Split(',');

            TurnoutInstruction = stringToTurnoutInstruction(s[0]);

            if (TurnoutInstruction == turnoutInstruction.err)
            {
                TCPPacket = UnknowPacket(TCPPacket);
                Type = dataType.unknow.ToString();
                return;
            }

            NumberOfAdress = (uint)unitAdress.vyhybky_W;

            uint a;
            if (!uint.TryParse(s[1], out a))
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

                    if (b <= 0xB4)
                        Data.Add(b);

                    else
                        Data.Add(0xB5);
                }
            }

            else if (TurnoutInstruction == turnoutInstruction.nastaveni_vyhybky)
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

                byte c;

                if (!byte.TryParse(s[3], out c))
                {
                    TCPPacket = UnknowPacket(TCPPacket);
                    Type = dataType.unknow.ToString();
                    return;
                }

                Data.Add(c);

                for (int i = 0; i < 6; i++)
                    Data.Add(0);
            }

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

            TCPPacket = (Type + ":" + TurnoutInstruction.ToString() + "," + NumberOfUnit + "," + Data[0] + "," + Data[1] + "\n");
        }

        public TurnoutInstructionPacket(byte[] bytePacket)
        {
            SetPropetiesFromByte(bytePacket);

            if (Enum.IsDefined(typeof(turnoutInstruction), Data[0]))
            {
                TurnoutInstruction = (turnoutInstruction)Data[0];
            }
            else if (Data[0] >= 0x00 && Data[0] <= 0xB5)
            {
                TurnoutInstruction = turnoutInstruction.presna_poloha_serv;
            }
            else
            {
                TurnoutInstruction = turnoutInstruction.err;
            }

            TCPPacket = (TurnoutInstruction.ToString() + "," + NumberOfUnit + TurnoutInstruction + "\n");
        }

        //presne polohovani mikro servo pohonu
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

        //nastaveni vyhybek
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

            Data.Add(data1);

            Data.Add(data2);

            for (int i = 0; i < 5; i++)
                Data.Add(0);

            SetBytePacket();

            TCPPacket = (Type + ":" + TurnoutInstruction.ToString() + "," + NumberOfUnit + "," + Data[1] + "," + Data[2] + "\n");

        }

        //restart jednotky, nastaveni prodlevy a precteni stavu natoceni
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

                if (data > 0xFA)
                    Data.Add(0xFA);
                else
                    Data.Add(data);
            }

            else if (type == turnoutInstruction.precteni_stavu_natoceni)
            {
                Data.Add(0xB8);

                if (data == 0x02)
                    Data.Add(0x02);

                else
                    Data.Add(0x01);
            }

            for (int i = 0; i < 6; i++)
                Data.Add(0);

            SetBytePacket();

            TCPPacket = (Type + ":" + TurnoutInstruction.ToString() + "," + NumberOfUnit + "," + Data[0] + "," + Data[1] + "\n");

        }

        //nastaveni dorazu
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
                Data.Add(0xAA);

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

            TCPPacket = (Type + ":" + TurnoutInstruction.ToString() + "," + NumberOfUnit + "," + Data[0] + "," + Data[1] + "," + Data[2] + "," + Data[3] + "\n");
        }

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

    public class TurnoutInfoPacket : Packet
    {
        turnoutInfo TurnoutInfo { set; get; }

        public TurnoutInfoPacket(string tcpPacket)
        {
            SetPropetiesFromTCP(tcpPacket);

            string[] s = tcpPacket.Split(':');

            tcpPacket = s[1];

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

        public TurnoutInfoPacket(byte[] bytePacket)
        {
            SetPropetiesFromByte(bytePacket);

            if (Enum.IsDefined(typeof(turnoutInfo), Data[0]))
            {
                TurnoutInfo = (turnoutInfo)Data[0];
            }
            else if (Data[0] >= 0xF0 && Data[0] <= 0xF3)
            {
                TurnoutInfo = turnoutInfo.natoceni_vlevo_vpravo;
            }
            else if (Data[0] >= 0x00 && Data[0] <= 0xB5)
            {
                TurnoutInfo = turnoutInfo.natoceni_ve_stupnich;
            }
            else
            {
                TurnoutInfo = turnoutInfo.err;
            }

            TCPPacket = (TurnoutInfo.ToString() + "," + NumberOfUnit + TurnoutInfo + "\n");
        }

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

    public class TrainMotionPacket : Packet
    {
        public string Name { set; get; }

        public bool Reverse { set; get; }

        public byte Speed { set; get; }

        public uint ID { set; get; }

        public TrainMotionPacket(byte[] bytePacket)
        {
            SetPropetiesFromByte(bytePacket);

            Name = LocomotiveInfo.IDToName(Data[0]);

            ID = Data[0];

            Speed = (byte)(Data[1] & 0x1f);

            Reverse = (((Data[1] >> 5) & 0x1) == 1) ? false : true;

            TCPPacket += (Name + ","  + Speed + "," + ((Reverse) ? "reverse\n" : "ahead\n"));

        }

        public TrainMotionPacket(Locomotive locomotive, bool reverse, byte speed)
        {
            BytePacket = new List<byte>();

            Adress = new List<byte>();

            Data = new List<byte>();

            ID = locomotive.ID;

            Name = locomotive.Name;

            Speed = speed;

            Reverse = reverse;

            ComposeByte();

            AssingType();

            TCPPacket = (Type + ":" + Name + "," +  Speed + "," + ((Reverse) ? "reverse\n" : "ahead\n"));


        }

        public TrainMotionPacket(string tcpPacket)
        {
            SetPropetiesFromTCP(tcpPacket);

            string[] s = tcpPacket.Split(':');

            tcpPacket = s[1];

            s = tcpPacket.Split(',');

            Name = s[0];

            ID = LocomotiveInfo.NameToID(Name);

            byte a;

            if (!byte.TryParse(s[1], out a))
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

    public class TrainMotionInstructionPacket : Packet
    {
        public TrainMotionPacket TrainMoveInfo { set; get; }

        public Section Section { set; get; }

        public uint WaitTime { set; get; }

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

    public class TrainFunctionPacket : Packet
    {

        public bool Lights { set; get; }

        public string Name { set; get; }

        public uint ID { set; get; }

        public TrainFunctionPacket(byte[] bytePacket)
        {
            SetPropetiesFromByte(bytePacket);

            ID = Data[0];

            Name = LocomotiveInfo.IDToName(ID);

            TCPPacket += (Name + ",");

            Lights = (((Data[1] >> 4) & 0x1) == 1) ? true : false;

            TCPPacket += ((Lights) ? "on\n" : "off\n");

        }

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

        public TrainFunctionPacket(string tcpPacket)
        {
            SetPropetiesFromTCP(tcpPacket);

            string[] s = tcpPacket.Split(':');

            tcpPacket = s[1];

            s = tcpPacket.Split(',');

            Name = s[0];

            ID = LocomotiveInfo.NameToID(Name);

            bool unknowPacket = false;

            if (s[1] == "on")
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

            ComposeByte();
        }

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

    public class OccupancySectionPacket : Packet
    {
        public List<Section> Sections { set; get; }

        public OccupancySectionPacket(byte[] bytePacket)
        {
            Sections = new List<Section>();

            SetPropetiesFromByte(bytePacket);

            RecognizeUnit();

            foreach (Section s in Sections)
            {
                s.Current = Data[Sections.IndexOf(s)];
                TCPPacket += (s.Name + "=" + s.Current + ",");
            }

            TCPPacket = TCPPacket.Substring(0, TCPPacket.Length - 1);

            TCPPacket += "\n";
        }

        public OccupancySectionPacket(string tcpPacket)
        {
            Sections = new List<Section>();

            SetPropetiesFromTCP(tcpPacket);

            NumberOfAdress = (uint)unitAdress.zesilovac_DCC_1_R;

            RecognizeSection(tcpPacket);

            NumberOfAdressAndUnitToAdress();

            Head = 0x08;

            foreach (Section section in Sections)
            {
                Data.Add((byte)section.Current);
            }

        }

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

            foreach (string sect in sec)
            {
                string[] secti = sect.Split('=');

                Sections.Add(new Section(secti[0], NumberOfUnit, uint.Parse(secti[1])));
            }
        }

        private void RecognizeUnit()
        {
            switch (NumberOfUnit)
            {
                case 2:
                    
                     //TODO dodelat sekce!!
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            Sections.Add(SectionInfo.listOfSection[i]);
                        }
                        break;
                    }
                    
                case 3:
                    {
                        for (int i = 8; i < 16; i++)
                        {
                            Sections.Add(SectionInfo.listOfSection[i]);
                        }
                        break;
                    }
                    // pouze ukázka, až se zde přidá další úseková jednotka, např. num. 4, připíšou se do konfiguráku section další názvy úseků 
                    /*                
                    case 4: 
                      {
                          for (int i = 8; i < 16; i++)
                          {
                              Sections.Add(SectionInfo.listOfSection[i]);
                          }
                          break;
                      }
                      */
            }
        }
    }

    public class Section
    {
        public string Name { set; get; }
        public uint NumberOfUnit { set; get; }
        public uint ModulePosition { set; get; }

        public uint current;
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
        public bool Occupancy { set; get; }

        public Section(string name, uint numberOfUnit, uint modulePosition)
        {
            Name = name;
            NumberOfUnit = numberOfUnit;
            ModulePosition = modulePosition;
            Current = 0;
            Occupancy = false;
            //ModulePosition = modulePosition;
        }

        public Section(string name, uint numberOfUnit, uint modulePosition, uint current)
        {
            Name = name;
            NumberOfUnit = numberOfUnit;
            ModulePosition = modulePosition;
            Current = current;
            OccupancySection();

        }

        public Section(string name)
        {
            Name = name;

            foreach (Section section in SectionInfo.listOfSection)
            {
                if (section.Name == name)
                {
                    NumberOfUnit = section.NumberOfUnit;
                    break;
                }
            }
            Current = 0;
            OccupancySection();

        }

        private void OccupancySection()
        {
            Occupancy = (Current > 40) ? true : false;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class SectionInfo
    {
        public static List<Section> listOfSection = InitSections();

        public static List<Section> InitSections()
        {
            if (listOfSection != null)
            {
                return listOfSection;
            }

            List<Section> list = new List<Section>();

            XmlDocument doc = new XmlDocument();

            //doc.Load("C:\\Train_2.0\\TrainTTLibrary\\sections.xml");
            doc.Load("C:\\Users\\Tomáš\\Documents\\ZCU_FEL\\v1_diplomka\\TestDesign\\TestDesignTT\\ControlLogic\\conf_kolejiste.xml");

            XmlNodeList sectionNodes = doc.SelectNodes("//section");

            foreach (XmlNode sectionNode in sectionNodes)
            {
                string name = sectionNode.Attributes["id"].Value;

                uint numberOfUnit = uint.Parse(sectionNode.SelectSingleNode("moduleid").InnerText);

                uint modulePosition = uint.Parse(sectionNode.SelectSingleNode("moduleposition").InnerText);

                list.Add(new Section(name, numberOfUnit, modulePosition));
            }

            list.Sort((a, b) =>
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

            listOfSection = list;

            return listOfSection;
        }
    }

    public class LocomotiveInfo
    {
        public static List<Locomotive> listOfLocomotives = Initloco();

        private static List<Locomotive> Initloco()
        {
            if (listOfLocomotives != null)
            {
                return listOfLocomotives;
            }

            List<Locomotive> list = new List<Locomotive>();

            //C:\Users\Tomáš\Documents\ZCU_FEL\v1_diplomka\TestDesign\TestDesignTT\TrainTTLibrary

            ConfigItem configItems = new ConfigItem("C:\\Users\\Tomáš\\Documents\\ZCU_FEL\\v1_diplomka\\TestDesign\\TestDesignTT\\TrainTTLibrary\\locomotives.xml");

            foreach (Item item in configItems.Items)
            {
                list.Add(new Locomotive(item.Str, item.Num));
            }
            listOfLocomotives = list;

            return listOfLocomotives;
        }

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

    public class Locomotive
    {
        public string Name { set; get; }
        public uint ID { set; get; }

        public Locomotive(string name)
        {
            Name = name;
            ID = LocomotiveInfo.NameToID(name);

        }

        public Locomotive(uint id)
        {
            ID = id;
            Name = LocomotiveInfo.IDToName(id);
        }

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

    public class Item
    {
        public string Str { get; set; }
        public uint Num { get; set; }

    }

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
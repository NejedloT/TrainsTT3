using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TrainTTLibrary;

namespace ControlLogic
{
    public class ProcessDataFromTCP
    {
        public static List<Section> occupancySections = new List<Section>();

        public static List<String> tcpData = new List<String>();


        /// <summary>
        /// zpracovani dat TCP komunikace
        /// </summary>
        /// <param name="e">Event na prichozi data vyvolat v prislusne windows form </param>
        public static void ProcessData(TCPReceivedEventArgs e)
        {

            if (e.data is String)
            {
                String s = e.data as String;

                string time = DateTime.Now.ToString();

                //zjisteni, zdali se jedna o zpravu o obsazenosti useku
                if (Packet.RecognizeTCPType(s) == Packet.dataType.occupancy_section)
                {
                    //packet obsahujici data o usecich
                    OccupancySectionPacket occupancySectionPacket = new OccupancySectionPacket(s);

                    SaveOccupancySection(occupancySectionPacket.Sections);

                    tcpData.Add(time + " " + occupancySectionPacket.TCPPacket);
                }
                if (Packet.RecognizeTCPType(s) == Packet.dataType.unit_info)
                {
                    UnitInfoPacket unitInfoPacket = new UnitInfoPacket(s);

                    tcpData.Add(time + " " + unitInfoPacket.TCPPacket);
                }

                if (Packet.RecognizeTCPType(s) == Packet.dataType.turnout_info)
                {
                    TurnoutInfoPacket turnoutInfoPacket = new TurnoutInfoPacket(s);
                    
                    tcpData.Add(time + " " + turnoutInfoPacket.TCPPacket);
                }
            }
        }

        private static void SaveOccupancySection(List<Section> newOccupancySection)
        {
            bool alreadyInPackets = false;

            for (int j = 0; j < newOccupancySection.Count; j++)
            {
                for (int i = 0; i < occupancySections.Count; i++)
                {

                    if (occupancySections[i].Name == newOccupancySection[j].Name)
                    {
                        occupancySections[i] = newOccupancySection[j];

                        alreadyInPackets = true;

                        break;
                    }
                }

                if (!alreadyInPackets)
                {
                    occupancySections.Add(newOccupancySection[j]);
                }
            }
        }

        public static List<String> GetSavedTCPData()
        {
            return tcpData;
        }

        public static List<Section> GetSavedOccupancySection()
        {
            return occupancySections;
        }
    }
}

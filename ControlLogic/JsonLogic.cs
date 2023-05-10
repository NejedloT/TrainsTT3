using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Xml.Linq;

namespace ControlLogic
{
    public class Trains
    {
        public uint id { get; set; }
        public string name { get; set; }
        public string currentPosition { get; set; }
        public string lastPosition { get; set; }
        public string nextPosition { get; set; }
        public int move { get; set; }
        public byte speed { get; set; }
        public bool reverse { get; set; }
        public string mapOrientation { get; set; }
        public int circuit { get; set; }
        public string startPosition { get; set; }
        public string finalPosition { get; set; }


        /////////////////////////////////////////////
        ///

    }

    public class TrainData
    {
        public List<Trains> data { get; set; }
    }

	
	public class TrainDataJSON
	{
		private readonly string jsonPath = "C:\\Users\\Tomáš\\Documents\\ZCU_FEL\\v1_diplomka\\TestDesign\\TestDesignTT\\ControlLogic\\train_data.json";
		
		public static object lockObject = new object();

		public List<Trains> LoadJson()
		{
			lock (lockObject)
			{
				string jsonData = File.ReadAllText(jsonPath);
				var jsonOptions = new JsonSerializerOptions
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
					Converters = { new JsonStringEnumConverter() }
				};
				var trainData = JsonSerializer.Deserialize<TrainData>(jsonData, jsonOptions);
				return trainData.data;
			}
		}

		public void SaveJson(List<Trains> data)
		{
			lock (lockObject)
			{
				var trainData = new TrainData { data = data };
				var jsonOptions = new JsonSerializerOptions
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
					Converters = { new JsonStringEnumConverter() },
					WriteIndented = true // optional, for pretty formatting
				};
				string jsonData = JsonSerializer.Serialize(trainData, jsonOptions);
				File.WriteAllText(jsonPath, jsonData);
			}
		}

        public void UpdateTrainData(string trainName, Action<Trains> updateAction)
        {
            lock (lockObject)
            {
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters = { new JsonStringEnumConverter() }
                };
                string jsonData = File.ReadAllText(jsonPath);
                var trainData = JsonSerializer.Deserialize<TrainData>(jsonData, jsonOptions);

                // Find the train data where the name is "Taurus_EVB".
                var trainToUpdate = trainData.data.FirstOrDefault(t => t.name == trainName);
                if (trainToUpdate != null)
                {
                    // Update the required properties of the found train data.
                    updateAction(trainToUpdate);

                    // Serialize the updated List<Trains> object back to JSON and write it to the file.
                    jsonData = JsonSerializer.Serialize(trainData, jsonOptions);
                    File.WriteAllText(jsonPath, jsonData);
                }
            }
        }
    }


    //For future change
    /*
    public class ConfigurationSections
    {
        public string Id { get; set; }
        public List<string> Prevsecs { get; set; }
        public List<string> Nextsecs { get; set; }
        public int Circuit { get; set; }
        public MapCoordinates Mapcoordinates { get; set; }
        public int Moduleid { get; set; }
        public int Moduleposition { get; set; }
    }

    public class MapCoordinates
    {
        public int Xpos { get; set; }
        public int Ypos { get; set; }
    }

    public class ConfigurationRoutes
    {
        public string FromId { get; set; }
        public List<To> To { get; set; }
    }

    public class Switches
    {
        public int Id { get; set; }
        public int Turnouts { get; set; }
        public int Value { get; set; }
    }

    public class To
    {
        public string Id { get; set; }
        public List<string> Parts { get; set; }
        public List<Switches> Switches { get; set; }
        public string ToFinal { get; set; }
        public string FromStart { get; set; }
        public int Circuit { get; set; }
    }

    public class ConfigurationCircuits
    {
        public int Id { get; set; }
        public List<FromStartOutside> FromStartOutside { get; set; }
    }

    public class FromStartOutside
    {
        public string Name { get; set; }
        public List<string> Items { get; set; }
        public string ToFinalPosition { get; set; }
    }

    public class ConfigurationCriticalRoute
    {
        public string Last { get; set; }
        public string Current { get; set; }
    }

    public class CircuitToFinalItem
    {
        public string Name { get; set; }
        public List<string> Items { get; set; }
    }

    public class ConfigurationCircuitToFinal
    {
        public int Id { get; set; }
        public List<CircuitToFinalItem> ToCircuit { get; set; }
    }

    public class ConfigurationStationTrack
    {
        public string Name { get; set; }
        public List<string> Items { get; set; }
        public int Circuit { get; set; }
    }


    public class ConfigurationTurnoutStopDefinition
    {
        public int unit { get; set; }
        public int pos { get; set; }
        public int leftStop { get; set; }
        public int rightStop { get; set; }
    }

    public class ModelSection
    {
        public List<ConfigurationSections> sections { get; set; }
        public List<ConfigurationRoutes> routes { get; set; }
        public List<ConfigurationCriticalRoute> ciritcalRoutes { get; set; }
        public List<ConfigurationCircuits> circuits { get; set; }
        public List<ConfigurationCircuitToFinal> circuitToFinal { get; set; }
        public List<ConfigurationStationTrack> stationTracks { get; set; }
        public List<ConfigurationTurnoutStopDefinition> turnoutStopDefinitions { get; set; }
    }


    public class ConfigurationFile
    {
        public static object lockConfiguration = new object();

        private readonly string jsonPath = "C:\\Users\\Tomáš\\Documents\\ZCU_FEL\\v1_diplomka\\TestDesign\\TestDesignTT\\ControlLogic\\conf_kolejiste.json";


        public List<ConfigurationSections> LoadSections()
        {
            lock(lockConfiguration)
            {
                string jsonData = File.ReadAllText(jsonPath);
                var json = JObject.Parse(jsonData);
                var sections = json["model"]["sections"].ToObject<List<ConfigurationSections>>();
                return sections;
            }
        }

        public List<ConfigurationRoutes> LoadRoutes()
        {
            lock (lockConfiguration)
            {
                string jsonData = File.ReadAllText(jsonPath);
                var json = JObject.Parse(jsonData);
                //var routes = json["model"]["routes"].ToObject<List<ConfigurationRoutes>>();
                var routes = json["model"]["routes"]["from"].ToObject<List<ConfigurationRoutes>>();
                return routes;
            }
        }

        public List<ConfigurationCriticalRoute> LoadCriticalRoutes()
        {
            lock (lockConfiguration)
            {
                string jsonData = File.ReadAllText(jsonPath);
                var json = JObject.Parse(jsonData);
                var criticalroutes = json["model"]["criticalroutes"].ToObject<List<ConfigurationCriticalRoute>>();
                return criticalroutes;
            }
        }

        public List<ConfigurationCircuits> LoadCircuits()
        {
            lock (lockConfiguration)
            {
                string jsonData = File.ReadAllText(jsonPath);
                var json = JObject.Parse(jsonData);
                var circuits = json["model"]["circuits"].ToObject<List<ConfigurationCircuits>>();
                return circuits;
            }
        }

        public List<ConfigurationCircuitToFinal> LoadCircuitToFinal()
        {
            lock (lockConfiguration)
            {
                string jsonData = File.ReadAllText(jsonPath);
                var json = JObject.Parse(jsonData);
                var circuitsToFinal = json["model"]["circuitsToFinal"]["fromCircuit"].ToObject<List<ConfigurationCircuitToFinal>>();
                return circuitsToFinal;
            }
        }

        public List<ConfigurationStationTrack> LoadStationTracks()
        {
            lock (lockConfiguration)
            {
                string jsonData = File.ReadAllText(jsonPath);
                var json = JObject.Parse(jsonData);
                var stationTracks = json["model"]["stationTracks"]["nameOfStation"].ToObject<List<ConfigurationStationTrack>>();
                return stationTracks;
            }
        }

        public List<ConfigurationTurnoutStopDefinition> LoadTurnoutStopsDefinition()
        {
            lock (lockConfiguration)
            {
                string jsonData = File.ReadAllText(jsonPath);
                var json = JObject.Parse(jsonData);
                var stationTracks = json["model"]["turnoutStopDefinitions"]["Items"].ToObject<List<ConfigurationTurnoutStopDefinition>>();
                return stationTracks;
            }
        }
    }
    */
}

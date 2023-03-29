using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

namespace ControlLogic
{
    
    public class Trains
    {
        public string id { get; set; }
        public string name { get; set; }
        public string currentPosition { get; set; }
        public string lastPosition { get; set; }
        public string nextPosition { get; set; }
        public string move { get; set; }
        public string speed { get; set; }
        public string direction { get; set; }
        public string mapOrientation { get; set; }
        public string circuit { get; set; }
        public string critical { get; set; }
        public string startPosition { get; set; }
        public string finalPosition { get; set; }


        /////////////////////////////////////////////
        ///

    }

    public class TrainData
    {
        public List<Trains> data { get; set; }
    }

    public class LoadJson
    {
        public List<Trains> data { get; set; }
        public LoadJson()
        {
            string jsonPath = "C:\\Users\\Tomáš\\Documents\\ZCU_FEL\\v1_diplomka\\TestDesign\\TestDesignTT\\ControlLogic\\train_data.json";
            string jsonData = File.ReadAllText(jsonPath);
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            };
            var trainData = JsonSerializer.Deserialize<TrainData>(jsonData, jsonOptions);
            data = trainData.data;
        }
    }

    public class StoreJson
    {
        private readonly string jsonPath = "C:\\Users\\Tomáš\\Documents\\ZCU_FEL\\v1_diplomka\\TestDesign\\TestDesignTT\\ControlLogic\\train_data.json";

        public void SaveJson(List<Trains> data)
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
}

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace JeanPascaline
{
    class DataStorage
    {
        public static Dictionary<string, string> pairs = new Dictionary<string, string>();

        static DataStorage()
        {
            if (!ValidatDataStorage("DataStorage.json")) return;
            string json = File.ReadAllText("DataStorage.json");
            pairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        private static bool ValidatDataStorage(string file)
        {
            if (!File.Exists(file))
            {
                File.WriteAllText(file, "");
                SaveData();
                return false;
            }
            return true;
        }

        public static void SaveData()
        {
            string json = JsonConvert.SerializeObject(pairs, Formatting.Indented);
            File.WriteAllText("DataStorage.json", json);
        }

        public static void AddPairsToData(string key, string value)
        {
            pairs.Add(key, value);
            SaveData();
        }   
    }
}

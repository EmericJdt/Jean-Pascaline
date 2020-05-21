using JeanPascaline.Core.AccountSystem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace JeanPascaline.Core
{
    public static class DataStorage
    {

        public static void SaveUserAccounts(IEnumerable<UserAccount> accounts, string filePath)
        {
            string json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static IEnumerable<UserAccount> LoadUserAccounts(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<UserAccount>>(json);
        }

        public static void SaveGuildList(IEnumerable<GuildAccount> accounts, string filePath)
        {
            string json = JsonConvert.SerializeObject(accounts, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            });
            File.WriteAllText(filePath, json);
        }

        public static IEnumerable<GuildAccount> LoadGuildList(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<GuildAccount>>(json);
        }

        public static bool SaveExiste(string filePath) { return File.Exists(filePath); }

        public static void CreateFile(string filePath)
        {
            FileStream file = File.Create(filePath);
            file.Close();
            Console.WriteLine("File Created");
        }
    }
}

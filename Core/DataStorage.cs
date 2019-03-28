using System;
using System.Collections.Generic;
using System.Text;
using JeanPascaline.Core.AccountSystem;
using System.IO;
using Newtonsoft.Json;

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

        public static bool SaveExiste(string filePath) { return File.Exists(filePath); }

    }
}

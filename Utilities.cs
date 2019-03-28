using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace JeanPascaline
{
    class Utilities
    {
        private static Dictionary<string, string> alerts;

        public static void Utilites()
        {

            string json = File.ReadAllText(@"SystemLang/alerts.json", Encoding.GetEncoding(28591));
            var data = JsonConvert.DeserializeObject<dynamic>(json);
            alerts = data.ToObject<Dictionary<string, string>>();
        }

        public static string GetAlert(string key)
        {
            if (alerts.ContainsKey(key)) return alerts[key];
            return "";
        }

        public static string GetFormattedAlert(string key, object param)
        {
            return GetFormattedAlert(key, new object[] { param });
        }

        public static string GetFormattedAlert(string key, params object[] param)
        {
            if (alerts.ContainsKey(key))
            {
                return string.Format(alerts[key], param);
            }
            return "";
        }
    }
}

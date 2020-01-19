using Newtonsoft.Json;
using System.IO;

namespace JeanPascaline
{
    class Config
    {
        private const string configFolder = "Ressources/Config";
        private const string configFile = "config.json";

        // Get all the necessary data to proceed connexion.

        public static BotConfig bot;
        static Config()
        {
            if (!Directory.Exists(configFolder))
                Directory.CreateDirectory(configFolder);

            if (!File.Exists(configFolder + "/" + configFile))
            {
                bot = new BotConfig();
                string json = JsonConvert.SerializeObject(bot, Formatting.Indented);
                File.WriteAllText(configFolder + "/" + configFile, json);
            }
            else
            {
                string json = File.ReadAllText(configFolder + "/" + configFile);
                bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }

        public struct BotConfig
        {
#pragma warning disable CS0649 // Le champ 'Config.BotConfig.Token' n'est jamais assigné et aura toujours sa valeur par défaut null
            public string Token;
#pragma warning restore CS0649 // Le champ 'Config.BotConfig.Token' n'est jamais assigné et aura toujours sa valeur par défaut null
#pragma warning disable CS0649 // Le champ 'Config.BotConfig.NyahGifCount' n'est jamais assigné et aura toujours sa valeur par défaut 0
            public int NyahGifCount;
#pragma warning restore CS0649 // Le champ 'Config.BotConfig.NyahGifCount' n'est jamais assigné et aura toujours sa valeur par défaut 0
#pragma warning disable CS0649 // Le champ 'Config.BotConfig.HugGifCount' n'est jamais assigné et aura toujours sa valeur par défaut 0
            public int HugGifCount;
#pragma warning restore CS0649 // Le champ 'Config.BotConfig.HugGifCount' n'est jamais assigné et aura toujours sa valeur par défaut 0
#pragma warning disable CS0649 // Le champ 'Config.BotConfig.PatPatGifCount' n'est jamais assigné et aura toujours sa valeur par défaut 0
            public int PatPatGifCount;
#pragma warning restore CS0649 // Le champ 'Config.BotConfig.PatPatGifCount' n'est jamais assigné et aura toujours sa valeur par défaut 0
#pragma warning disable CS0649 // Le champ 'Config.BotConfig.QuoteCount' n'est jamais assigné et aura toujours sa valeur par défaut 0
            public int QuoteCount;
#pragma warning restore CS0649 // Le champ 'Config.BotConfig.QuoteCount' n'est jamais assigné et aura toujours sa valeur par défaut 0
        }
    }
}

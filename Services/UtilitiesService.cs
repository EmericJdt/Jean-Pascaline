using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using JeanPascaline.Core.AccountSystem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace JeanPascaline.Services
{
    public class UtilitiesService : InteractiveBase<SocketCommandContext>
    {
        private static Dictionary<string, string> alerts = new Dictionary<string, string>();

        private static readonly Dictionary<string, Dictionary<string, string>> TabAlerts = new Dictionary<string, Dictionary<string, string>>();

        private static readonly Random rnd = new Random();

        public static void InitializeAlerts()
        {
            string[] Files = Directory.GetFiles(@"SystemLang/");
            foreach (string FilePath in Files)
            {
                string json = File.ReadAllText(FilePath, Encoding.UTF8/*Encoding.GetEncoding(28591)*/);
                dynamic data = JsonConvert.DeserializeObject<dynamic>(json);
                alerts = data.ToObject<Dictionary<string, string>>();
                TabAlerts.Add(Path.GetFileNameWithoutExtension(FilePath), alerts);
            }
        }

        public static string GetAlert(GuildAccount GuildData, string key, object param = null)
        {
            return GetAlert(GuildData, key, new object[] { param });
        }

        public static string GetAlert(GuildAccount GuildData, string key, params object[] param)
        {
            alerts = TabAlerts[GuildData.Language];
            if (alerts.ContainsKey(key)) return string.Format(alerts[key], param);
            else return "";
        }


        public static async Task SendErrorAsync(SocketCommandContext Context, Exception ex)
        {
            EmbedBuilder ErrorEmbed = CreateEmbed(Color.Red, ex.Message);
            await Context.Channel.SendMessageAsync(embed: ErrorEmbed.Build());
        }

        public static EmbedBuilder CreateEmbed(EmbedAuthorBuilder Author, Color Color, List<EmbedFieldBuilder> Fields, string ThumbnailUrl = null, string Description = null, EmbedFooterBuilder Footer = null)
        {
            EmbedBuilder Embed = new EmbedBuilder()
            {
                // Mandatory
                Author = Author,
                Color = Color,
                Fields = Fields,

                // Non-Mandatory
                Description = Description,
                Timestamp = DateTime.Now,
                ThumbnailUrl = ThumbnailUrl,
                Footer = Footer,

                // Never Used
                ImageUrl = null,
                Title = null,
                Url = null
            };
            return Embed;
        }

        public static EmbedBuilder CreateEmbed(Color Color, string Description, string Title = null)
        {
            EmbedBuilder Embed = new EmbedBuilder()
            {
                Color = Color,
                Description = Description,
                Title = Title,
                Timestamp = DateTime.Now
            };
            return Embed;
        }

        public static EmbedAuthorBuilder CreateAuthorEmbed(string Name, string IconUrl, string Url = null)
        {
            EmbedAuthorBuilder Author = new EmbedAuthorBuilder()
            {
                IconUrl = IconUrl,
                Name = Name,
                Url = Url
            };
            return Author;
        }

        public static List<EmbedFieldBuilder> CreateListFields(Dictionary<object, object> FieldsData, bool IsInline = false)
        {
            List<EmbedFieldBuilder> ListFields = new List<EmbedFieldBuilder>();
            foreach (KeyValuePair<object, object> FieldData in FieldsData)
            {
                ListFields.Add(CreateFieldBuilder(Convert.ToString(FieldData.Key), FieldData.Value, IsInline));
            }
            return ListFields;
        }

        private static EmbedFieldBuilder CreateFieldBuilder(string Name, object Value, bool IsInline = false)
        {
            EmbedFieldBuilder Field = new EmbedFieldBuilder()
            {
                Name = Name,
                Value = Value,
                IsInline = IsInline
            };
            return Field;
        }

        public static EmbedFooterBuilder CreateFooterBuilder(string IconUrl, string Text)
        {
            EmbedFooterBuilder Footer = new EmbedFooterBuilder()
            {
                IconUrl = IconUrl,
                Text = Text
            };
            return Footer;
        }

        public static Dictionary<object, object> GetCommandsInfoDictionnary(List<CommandInfo> Commands)
        {
            Dictionary<object, object> CommandsInfo = new Dictionary<object, object>();
            foreach (CommandInfo command in Commands)
            {
                string embedFieldText = command.Summary ?? "No description available\n";
                string parameters = "";
                foreach (ParameterInfo parameter in command.Parameters)
                {
                    if (parameter.IsOptional) parameters += $"({parameter.Name}) ";
                    else parameters += $"[{parameter.Name}] ";
                }
                CommandsInfo.Add($"!{command.Name} {parameters}", embedFieldText);
            }
            return CommandsInfo;
        }

        public static int GetRandomNumber(int min, int max)
        {
            return rnd.Next(min, max);
        }
        public static string GetYukueByID(int randomNumber)
        {
            switch (randomNumber)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    return "Dzeta";

                case 4:
                case 5:
                case 6:
                    return "Alpha";

                case 7:
                case 8:
                case 9:
                    return "Tau";

                case 10:
                case 11:
                case 12:
                    return "Omega";

                case 13:
                case 14:
                case 15:
                    return "Epsilon";

                case 16:
                case 17:
                case 18:
                    return "Sigma";

                case 19: return "Omicron";
                case 20: return "Teta";
                case 21: return "Gamma";
                case 22: return "######";
                case 23: return "SabrineEpsilon";
                case 24: return "AlphaDzeta";
                case 25: return "Tau2";
                case 26: return "Omega2";
                case 27: return "GammaTeta";
                case 28: return "AlphaDzeta2";
                case 29: return "SigmaTau";

                default: return null;
            }
        }
    }
}

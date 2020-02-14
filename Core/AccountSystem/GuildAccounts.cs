using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;

namespace JeanPascaline.Core.AccountSystem
{
    public static class GuildAccounts
    {
        private static readonly List<GuildAccount> GuildsList;

        private static readonly string GuildsFile = @"Ressources/guildslist.json";

        static GuildAccounts()
        {
            if (!DataStorage.SaveExiste(GuildsFile))
            {
                DataStorage.CreateFile(GuildsFile);
                GuildsList = new List<GuildAccount>();
                SaveGuilds();
            }
            else
            {
                GuildsList = DataStorage.LoadGuildList(GuildsFile).ToList();
            }
        }

        public static void SaveGuilds()
        {
            DataStorage.SaveGuildList(GuildsList, GuildsFile);
        }

        public static GuildAccount GetAccount(SocketGuild Guild)
        {
            return GetOrCreateAccount(Guild);
        }

        private static GuildAccount GetOrCreateAccount(SocketGuild Guild)
        {
            IEnumerable<GuildAccount> result = from a in GuildsList
                                               where a.ID == Guild.Id
                                               select a;
            GuildAccount account = result.FirstOrDefault();
            return account ?? CreateGuildAccount(Guild);
        }

        private static GuildAccount CreateGuildAccount(SocketGuild Guild)
        {
            GuildAccount newGuild = new GuildAccount()
            {
                ID = Guild.Id,
                AnnoucementChannelID = Guild.DefaultChannel.Id,
                DefaultRoleID = Guild.Roles.LastOrDefault().Id,
                ModeratorRoleID = Guild.Roles.FirstOrDefault(x => x.Position == 1).Id,
                MutedRoleID = Guild.Roles.FirstOrDefault(x => x.Name == "Muted").Id,
                MusicChannelID = Guild.DefaultChannel.Id,
                LogChannelID = Guild.DefaultChannel.Id,
                MaxLevel = 70,
                IsAnnoncementMessagesEnabled = false,
                WarningsThreshold = 5,
                Language = "Default",
                Rewards = new Dictionary<ulong, uint>(),
                ForbiddenWords = new HashSet<string>(),
            };
            GuildsList.Add(newGuild);
            SaveGuilds();
            return newGuild;
        }
    }
}
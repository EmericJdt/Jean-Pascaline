using Discord.WebSocket;
using System;
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
            var result = from a in GuildsList
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
                AnnoucementChannelID = 0,
                DefaultRoleID = 0,
                ModeratorRoleID = 0,
                MusicChannelID = 0,
                LogChannelID = 0,
                MaxLevel = 70,
                IsAnnoncementMessagesEnabled = false,
                WarningsThreshold = 5,
                Language = "Default",
                Rewards = new Dictionary<ulong, uint>(),
            };
            GuildsList.Add(newGuild);
            SaveGuilds();
            return newGuild;
        }
    }
}
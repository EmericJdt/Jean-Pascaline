using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JeanPascaline.Core.AccountSystem
{
    public static class UserAccounts
    {

        private static List<UserAccount> ListAccount;

        private static readonly Dictionary<ulong, List<UserAccount>> GuildsList = new Dictionary<ulong, List<UserAccount>>();

        static UserAccounts()
        {
            foreach (SocketGuild Guild in Program._client.Guilds)
            {
                if (DataStorage.SaveExiste(@"Ressources\Guilds\" + Guild.Id + ".json"))
                {
                    ListAccount = DataStorage.LoadUserAccounts("Ressources/Guilds/" + Guild.Id + ".json").ToList();
                    GuildsList.Add(Guild.Id, ListAccount);
                }
                else
                {
                    DataStorage.CreateFile(@"Ressources\Guilds\" + Guild.Id + ".json");
                    ListAccount = new List<UserAccount>();
                    GuildsList.Add(Guild.Id, ListAccount);
                    DataStorage.SaveUserAccounts(ListAccount, @"Ressources\Guilds\" + Guild.Id + ".json");
                }
            }
        }

        public static void SaveAccounts(ulong GuildID)
        {
            DataStorage.SaveUserAccounts(GuildsList[GuildID], @"Ressources\Guilds\" + GuildID + ".json");
        }

        public static UserAccount GetAccount(SocketUser user, ulong GuildID)
        {
            return GetOrCreateAccount(user.Id, user.ToString(), GuildID);
        }

        private static UserAccount GetOrCreateAccount(ulong id, string hashcode, ulong GuildID)
        {
            ListAccount = GuildsList[GuildID];
            var result = from a in ListAccount
                         where a.ID == id
                         select a;
            UserAccount account = result.FirstOrDefault();
            if (account == null) account = CreateUserAccount(id, hashcode, GuildID);
            return account;
        }

        private static UserAccount CreateUserAccount(ulong id, string hashcode, ulong GuildID)
        {
            UserAccount newAccount = new UserAccount()
            {
                ID = id,
                Hashcode = hashcode,
                Pronouns = "***",
                Description = "***",
                LastMessage = DateTime.UtcNow,
                NbWarnings = 0,
                XP = 0,
                Warns = new Dictionary<string, string>(),
                Roles = new List<ulong>(),
            };
            ListAccount.Add(newAccount);
            SaveAccounts(GuildID);
            return newAccount;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord.WebSocket;

namespace JeanPascaline.Core.AccountSystem
{
    public static class UserAccounts
    {

        private static List<UserAccount> Accounts;

        private static string AccountsFile = "Ressources/accounts.json";

        static UserAccounts()
        {
            if (DataStorage.SaveExiste(AccountsFile))
            {
                Accounts = DataStorage.LoadUserAccounts(AccountsFile).ToList();
            }
            else
            {
                Accounts = new List<UserAccount>();
                SaveAccounts();
            }
        }

        public static void SaveAccounts()
        {
            DataStorage.SaveUserAccounts(Accounts, AccountsFile);
        }

        public static UserAccount GetAccount(SocketUser user)
        {
            return GetOrCreateAccount(user.Id, user.ToString());
            
        }

        private static UserAccount GetOrCreateAccount(ulong id, string hashcode)
        {
            var result = from a in Accounts
                         where a.ID == id
                         select a;
            var account = result.FirstOrDefault();
            if(account == null) account = CreateUserAccount(id, hashcode);
            return account;
        }

        private static UserAccount CreateUserAccount(ulong id, string hashcode)
        {
            var newAccount = new UserAccount()
            {
                ID = id,
                Hashcode = hashcode,
                LastMessage = DateTime.UtcNow,
                NbWarnings = 0,
                XP = 0
            };
            Accounts.Add(newAccount);
            SaveAccounts();
            return newAccount;
        }
    }
}

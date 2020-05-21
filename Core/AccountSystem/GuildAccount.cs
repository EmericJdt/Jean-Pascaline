using System.Collections.Generic;

namespace JeanPascaline.Core.AccountSystem
{
    public class GuildAccount
    {
        public ulong ID { get; set; }

        public bool IsDevGuild
        {
            get
            {
                if (ID == 161284551199424513 || ID == 471357240549310477) return true;
                else return false;
            }
        }

        public ulong DefaultRoleID { get; set; }

        public ulong ModeratorRoleID { get; set; }

        public ulong MutedRoleID { get; set; }

        public ulong AnnoucementChannelID { get; set; }

        public ulong MusicChannelID { get; set; }

        public ulong LogChannelID { get; set; }

        public uint MaxLevel { get; set; }

        public bool IsAnnoncementMessagesEnabled { get; set; }

        public uint WarningsThreshold { get; set; }

        public string Language { get; set; }

        public Dictionary<ulong, uint> Rewards { get; set; }

        public HashSet<string> ForbiddenWords { get; set; }
    };
}
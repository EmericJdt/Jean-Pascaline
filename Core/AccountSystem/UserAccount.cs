using System;
using System.Collections.Generic;

namespace JeanPascaline.Core.AccountSystem
{
    public class UserAccount
    {
        public ulong ID { get; set; }

        public string Hashcode { get; set; }

        public string Pronouns { get; set; }

        public string Description { get; set; }

        public uint XP { get; set; }

        public uint Level
        {
            get
            {
                return (uint)Math.Sqrt(XP / 50);
            }
        }

        public DateTime LastMessage { get; set; }

        public uint NbWarnings { get; set; }

        public Dictionary<string, string> Warns { get; set; }

        public ICollection<ulong> Roles { get; set; }
    }
}

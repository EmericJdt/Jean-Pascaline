using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.WebSocket;
using JeanPascaline.Core.AccountSystem;
using System.Threading.Tasks;


namespace JeanPascaline.Core.LevelSystem
{
    internal static class Leveling
    {
        internal static async Task UserSentMessageAsync(SocketGuildUser user, IDMChannel DM)
        {
            var userAccount = UserAccounts.GetAccount(user);
            if (userAccount.LastMessage == null) { userAccount.LastMessage = DateTime.UtcNow; }
            if (DateTime.UtcNow <= userAccount.LastMessage.AddMinutes(1)) { return; }
            else
            {
                uint oldLevel = userAccount.Level;
                Random rnd = new Random();
                userAccount.XP += (uint)rnd.Next(15, 30);
                userAccount.LastMessage = DateTime.UtcNow;
                UserAccounts.SaveAccounts();
                uint newLevel = userAccount.Level;
                if (oldLevel != newLevel)
                {
                    await LevelUpSystem.LevelUp(user, newLevel);

                    var Embed = new EmbedBuilder();
                    var Embed1 = new EmbedAuthorBuilder();
                    Embed.WithColor(Color.Teal);
                    Embed1.Name = user.Guild.ToString();
                    Embed1.IconUrl = user.Guild.IconUrl;
                    Embed.WithAuthor(Embed1);
                    Embed.WithTitle(Utilities.GetAlert("LEVEL_UP_TITLE"));
                    Embed.AddInlineField("Niveau actuel", newLevel);
                    Embed.AddInlineField("Points d'expérience", userAccount.XP);


                    await DM.SendMessageAsync("", embed: Embed);
                }
            }
        }
    }

    public static class LevelUpSystem {

        public static async Task LevelUp(SocketGuildUser user, uint newLevel)
        {
            switch (newLevel)
            {
                case 8:
                    await user.AddRolesAsync(new List<IRole> { user.Guild.GetRole(Convert.ToUInt64(Utilities.GetAlert("RANK8_ROLE1"))), user.Guild.GetRole(Convert.ToUInt64(Utilities.GetAlert("RANK8_ROLE2"))) });
                    await user.RemoveRoleAsync(user.Guild.GetRole(Convert.ToUInt64(Utilities.GetAlert("RANK0_ROLE1"))));
                    break;

                case 16:
                    await user.AddRolesAsync(new List<IRole> { user.Guild.GetRole(Convert.ToUInt64(Utilities.GetAlert("RANK16_ROLE1"))), user.Guild.GetRole(Convert.ToUInt64(Utilities.GetAlert("RANK16_ROLE2"))) });
                    await user.RemoveRolesAsync(new List<IRole> { user.Guild.GetRole(Convert.ToUInt64(Utilities.GetAlert("RANK8_ROLE1"))), user.Guild.GetRole(Convert.ToUInt64(Utilities.GetAlert("RANK8_ROLE2"))) });
                    break;

                case 24:
                    await user.AddRoleAsync(user.Guild.GetRole(Convert.ToUInt64(Utilities.GetAlert("RANK24_ROLE1"))));
                    await user.RemoveRolesAsync(new List<IRole> { user.Guild.GetRole(Convert.ToUInt64(Utilities.GetAlert("RANK16_ROLE1"))), user.Guild.GetRole(Convert.ToUInt64(Utilities.GetAlert("RANK16_ROLE2"))) });
                    break;

                case 33:
                    await user.AddRolesAsync(new List<IRole> { user.Guild.GetRole(Convert.ToUInt64(Utilities.GetAlert("RANK33_ROLE1"))), user.Guild.GetRole(Convert.ToUInt64(Utilities.GetAlert("RANK33_ROLE2"))) });
                    await user.RemoveRoleAsync(user.Guild.GetRole(Convert.ToUInt64(Utilities.GetAlert("RANK24_ROLE1"))));
                    break;

                case 44:
                    await user.RemoveRolesAsync(new List<IRole> { user.Guild.GetRole(Convert.ToUInt64(Utilities.GetAlert("RANK33_ROLE1"))), user.Guild.GetRole(Convert.ToUInt64(Utilities.GetAlert("RANK33_ROLE2"))) });
                    await user.AddRoleAsync(user.Guild.GetRole(Convert.ToUInt64(Utilities.GetAlert("RANK44_ROLE1"))));
                    break;

                case 55:
                    await user.RemoveRoleAsync(user.Guild.GetRole(Convert.ToUInt64(Utilities.GetAlert("RANK44_ROLE1"))));
                    await user.AddRolesAsync(new List<IRole> { user.Guild.GetRole(Convert.ToUInt64(Utilities.GetAlert("RANK55_ROLE1"))), user.Guild.GetRole(Convert.ToUInt64(Utilities.GetAlert("RANK55_ROLE2"))) });
                    break;

                case 60:
                    await user.AddRolesAsync(new List<IRole> { user.Guild.GetRole(Convert.ToUInt64(Utilities.GetAlert("RANK60_ROLE1"))), user.Guild.GetRole(Convert.ToUInt64(Utilities.GetAlert("RANK60_ROLE2"))) });
                    await user.RemoveRolesAsync(new List<IRole> { user.Guild.GetRole(Convert.ToUInt64(Utilities.GetAlert("RANK55_ROLE1"))), user.Guild.GetRole(Convert.ToUInt64(Utilities.GetAlert("RANK55_ROLE2"))) });
                    break;

                case 70:
                    await user.AddRolesAsync(new List<IRole> { user.Guild.GetRole(Convert.ToUInt64(Utilities.GetAlert("RANK70_ROLE1"))), user.Guild.GetRole(Convert.ToUInt64(Utilities.GetAlert("RANK70_ROLE2"))) });
                    await user.RemoveRolesAsync(new List<IRole> { user.Guild.GetRole(Convert.ToUInt64(Utilities.GetAlert("RANK60_ROLE1"))), user.Guild.GetRole(Convert.ToUInt64(Utilities.GetAlert("RANK60_ROLE2"))) });

                    break;

                default:
                    break;
            }
        }
    }
}

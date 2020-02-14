using Discord;
using Discord.Commands;
using Discord.WebSocket;
using JeanPascaline.Core.AccountSystem;
using JeanPascaline.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JeanPascaline.Core.LevelSystem
{
    internal static class Leveling
    {
        internal static async Task UserSentMessageAsync(SocketUser User, IDMChannel DM, SocketCommandContext Context)
        {

            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            UserAccount UserData = UserAccounts.GetAccount(User, GuildData.ID);
            if (DateTime.UtcNow <= UserData.LastMessage.AddMinutes(1)) return;

            uint oldLevel = UserData.Level;

            if (UserData.Level < GuildData.MaxLevel)
            {
                UserData.XP += (uint)UtilitiesService.GetRandomNumber(10, 25);
            }

            UserData.LastMessage = DateTime.UtcNow;
            UserAccounts.SaveAccounts(GuildData.ID);

            if (oldLevel != UserData.Level)
            {
                try
                {
                    EmbedAuthorBuilder LevelingAuthor = UtilitiesService.CreateAuthorEmbed(UtilitiesService.GetAlert(GuildData, "LEVELUPCARDTITLE", Context.Guild.ToString()), Context.Guild.IconUrl);
                    Dictionary<object, object> FieldsData = new Dictionary<object, object>()
                    {
                        { UtilitiesService.GetAlert(GuildData,"LEVELUPCARDLEVEL"), UserData.Level},
                        { UtilitiesService.GetAlert(GuildData, "LEVELUPCARDXP"), UserData.XP }
                    };

                    EmbedBuilder LevelingEmbed = UtilitiesService.CreateEmbed(LevelingAuthor, Color.Green, UtilitiesService.CreateListFields(FieldsData));

                    await DM.SendMessageAsync(embed: LevelingEmbed.Build());
                }
                catch (Exception ex)
                {
                    await UtilitiesService.SendErrorAsync(Context, ex);
                }
                if (GuildData.Rewards.Values.Contains(UserData.Level))
                {
                    foreach (KeyValuePair<ulong, uint> Reward in GuildData.Rewards.Where(x => x.Value == UserData.Level))
                    {
                        await ((IGuildUser)User).AddRoleAsync(Context.Guild.GetRole(Reward.Key));
                    }
                }
            }
        }
    }
}


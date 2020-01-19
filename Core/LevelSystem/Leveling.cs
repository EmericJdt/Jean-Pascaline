using Discord;
using Discord.Commands;
using Discord.WebSocket;
using JeanPascaline.Core.AccountSystem;
using JeanPascaline.Services;
using System;
using System.Collections.Generic;
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
                UserData.XP += (uint)Utilities.GetRandomNumber(15, 30);
            }

            UserData.LastMessage = DateTime.UtcNow;
            UserAccounts.SaveAccounts(GuildData.ID);

            if (oldLevel != UserData.Level)
            {
                try
                {
                    EmbedAuthorBuilder LevelingAuthor = Utilities.CreateAuthorEmbed(Utilities.GetAlert(GuildData, "LEVELUPCARDTITLE", Context.Guild.ToString()), Context.Guild.IconUrl);
                    Dictionary<object, object> FieldsData = new Dictionary<object, object>() 
                    {
                        { Utilities.GetAlert(GuildData,"LEVELUPCARDLEVEL"), UserData.Level},
                        { Utilities.GetAlert(GuildData, "LEVELUPCARDXP"), UserData.XP }
                    };

                    EmbedBuilder LevelingEmbed = Utilities.CreateEmbed(LevelingAuthor, Color.Green, Utilities.CreateListFields(FieldsData));

                    await DM.SendMessageAsync(embed: LevelingEmbed.Build());
                }
                catch (Exception ex)
                {
                    await Utilities.SendErrorAsync(Context, ex);
                }
            }
        }
    }
}


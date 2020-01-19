using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using JeanPascaline.Core.AccountSystem;
using JeanPascaline.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JeanPascaline.Modules
{
    [RequireUserPermission(GuildPermission.Administrator)]
    public class Administration : InteractiveBase<SocketCommandContext>
    {

        [Command("givexp"), Summary("Donne un nombre défini de point d'expériences à un membre. — Give a certain number of experience point to someone.")]
        [Alias("gxp"), Remarks("adm")]
        public async Task GiveXP(SocketUser member, uint xp)
        {
            await Context.Message.DeleteAsync();
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            UserAccount UserData = UserAccounts.GetAccount(member, Context.Guild.Id);

            if (UserData.Level >= GuildData.MaxLevel)
            {
                await ReplyAsync(Utilities.GetAlert(GuildData, "UNSUCESSFULGIVE", Context.User.Mention, member.Mention));                return;
            }
            else
            {
                UserData.XP += xp;
                UserAccounts.SaveAccounts(Context.Guild.Id);
                await ReplyAsync(Utilities.GetAlert(GuildData, "GIVEXP", xp, member.Mention));
            }
        }

        [Command("takexp"), Summary("Retire un certain nombre de points d'expérience à un membre. — Retrieve a certain amount of experience point to a member.")]
        [Alias("txp"), Remarks("adm")]
        public async Task TakeXP(SocketUser member, uint xp)
        {
            await Context.Message.DeleteAsync();
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            UserAccount UserData = UserAccounts.GetAccount(member, Context.Guild.Id);
            
            if ((UserData.XP - xp) < 0)
            {
                await ReplyAsync(Utilities.GetAlert(GuildData, "UNSUCESSFULTAKE", Context.User.Mention, member.Mention));
            }
            else
            {
                UserData.XP -= xp;
                UserAccounts.SaveAccounts(Context.Guild.Id);
                await ReplyAsync(Utilities.GetAlert(GuildData, "TAKEXP", xp, member.Mention));
            }

        }

        [Command("setchannel")]
        [Alias("sc")]
        public async Task SetChannel(ITextChannel Channel = null, string Channelcode = null)
        {
            await Context.Message.DeleteAsync();
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            switch (Channelcode)
            {
                case string DefaultChannel when DefaultChannel.ToLower() == "dc":
                    GuildData.DefaultRoleID = Channel.Id;
                    break;
                case string LogChannel when LogChannel.ToLower() == "lc":
                    GuildData.LogChannelID = Channel.Id;
                    break;
                case string MusicChannel when MusicChannel.ToLower() == "mc":
                    GuildData.MusicChannelID = Channel.Id;
                    break;
                default:

                    EmbedBuilder ChannelEmbed = new EmbedBuilder()
                    {
                        Color = new Color(255, 0, 0),
                        Title = Utilities.GetAlert(GuildData, "CODELISTTITLE"),
                        Description = "· DC — Default Channel\n" +
                        "· LC — Log Channel\n" +
                        "· MC — Music Channel",
                        Timestamp = DateTime.Now,
                        Footer = new EmbedFooterBuilder()
                        {
                            Text = Utilities.GetAlert(GuildData, "PARAMEMBEDFOOTER", "setchannel (Channelcode)")
                        }
                    };
                    await ReplyAsync(embed: ChannelEmbed.Build());

                    return;
            }
            GuildAccounts.SaveGuilds();
            await ReplyAndDeleteAsync(Utilities.GetAlert(GuildData, "CHANGESSAVED", Context.User.Mention));
        }

        [Command("setrole")]
        [Alias("sr")]
        public async Task SetRole(IRole Role = null, string Rolecode = null)
        {
            await Context.Message.DeleteAsync();
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            switch (Rolecode)
            {
                case string DefaultRole when DefaultRole.ToLower() == "dr":
                    GuildData.DefaultRoleID = Role.Id;
                    break;
                case string ModeratorRole when ModeratorRole.ToLower() == "mr":
                    GuildData.ModeratorRoleID = Role.Id;
                    break;
                default:
                    EmbedBuilder RoleEmbed = new EmbedBuilder()
                    {
                        Color = new Color(0, 255, 0),
                        Title = Utilities.GetAlert(GuildData, "CODELISTTITLE"),
                        Description = "· DR — Default Role\n" +
                        "· MR — Moderator Role",
                        Timestamp = DateTime.Now,
                        Footer = new EmbedFooterBuilder()
                        {
                            Text = Utilities.GetAlert(GuildData, "PARAMEMBEDFOOTER", "setrole (rolecode)")
                        }
                    };
                    await ReplyAsync(embed: RoleEmbed.Build());
                    return;
            }
            GuildAccounts.SaveGuilds();
            await ReplyAndDeleteAsync(Utilities.GetAlert(GuildData, "CHANGESSAVED", Context.User.Mention));
        }

        [Command("setvalue")]
        [Alias("sv")]
        public async Task SetValue(uint Value = 0, string Valuecode = null)
        {
            await Context.Message.DeleteAsync();
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            if (Value > 500) Value = 500;
            switch (Valuecode)
            {
                case string MaxLevel when MaxLevel.ToLower() == "ml":
                    GuildData.MaxLevel = Value;
                    break;
                case string WarningThreshold when WarningThreshold.ToLower() == "wt":
                    GuildData.WarningsThreshold = Value;
                    break;
                default:
                    EmbedBuilder RoleEmbed = new EmbedBuilder()
                    {
                        Color = new Color(0, 0, 255),
                        Title = Utilities.GetAlert(GuildData, "CODELISTTITLE"),
                        Description = "· ML — Max Level\n" +
                        "· WT — Warnings Threshold",
                        Timestamp = DateTime.Now,
                        Footer = new EmbedFooterBuilder()
                        {
                            Text = Utilities.GetAlert(GuildData, "PARAMEMBEDFOOTER", "setvalue (valuecode)")
                        }
                    };
                    await ReplyAsync(embed: RoleEmbed.Build());
                    return;
            }
            GuildAccounts.SaveGuilds();
            await ReplyAndDeleteAsync(Utilities.GetAlert(GuildData, "CHANGESSAVED", Context.User.Mention));
        }

        [Command("addreward", RunMode = RunMode.Async)]
        [Alias("arw")]
        public async Task AddReward(IRole RewardRole, uint Level)
        {
            await Context.Message.DeleteAsync();
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            GuildData.Rewards.TryAdd(RewardRole.Id, Level);
            await ReplyAndDeleteAsync(Utilities.GetAlert(GuildData, "REWARDADDED", Context.User.Mention));
        }

        [Command("removereward", RunMode = RunMode.Async)]
        [Alias("rrw")]
        public async Task RemoveReward(IRole RewardRole)
        {
            await Context.Message.DeleteAsync();
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            try { 
                GuildData.Rewards.Remove(RewardRole.Id);
                await ReplyAndDeleteAsync(Utilities.GetAlert(GuildData, "REWARDREMOVED", Context.User.Mention));
            }
            catch (Exception ex)
            {
                await Utilities.SendErrorAsync(Context, ex);
            }
        }

        [Command("joinalert"), Remarks("adm")]
        [Alias("ja"), Summary("Active/Désactive les messages d'accueil et de départ. — Activate/Deactivate welcome and leaving alerts.")]
        public async Task SwitchWelcomeMessage()
        {
            await Context.Message.DeleteAsync();
            
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            if (GuildData.IsAnnoncementMessagesEnabled == true)
            {
                GuildData.IsAnnoncementMessagesEnabled = false;
                await ReplyAndDeleteAsync(Utilities.GetAlert(GuildData, "SETWELCOMEMESSAGE", Context.User.Mention));
            }
            else
            {
                GuildData.IsAnnoncementMessagesEnabled = true;
                await ReplyAndDeleteAsync(Utilities.GetAlert(GuildData, "REMOVEWELCOMEMESSAGE", Context.User.Mention));
            }
            GuildAccounts.SaveGuilds();
        }

        [Command("language", RunMode = RunMode.Async), Remarks("adm")]
        [Alias("lang"), Summary("Change la langue du bot. — Change the bot language.")]
        public async Task SetLanguage(string langcode = null)
        {
            await Context.Message.DeleteAsync();
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            
            if(langcode == null)
            {
                EmbedBuilder LangEmbed = new EmbedBuilder()
                {
                    Color = new Color(0, 0, 128),
                    Title = Utilities.GetAlert(GuildData, "LANGUAGELISTTITLE"),
                    Description = "- FR — Français\n" +
                    "- EN — English",
                    Timestamp = DateTime.Now,
                    Footer = new EmbedFooterBuilder()
                    {
                        Text = Utilities.GetAlert(GuildData, "PARAMEMBEDFOOTER","language (Langcode)")
                    }
                };
                await ReplyAsync(embed: LangEmbed.Build());
            }

            else { 
                switch (langcode)
                {
                    case "FR":
                        GuildData.Language = "Default";
                        break;
                    case "EN":
                        GuildData.Language = "English";
                        break;
                    default:
                        await ReplyAndDeleteAsync(Utilities.GetAlert(GuildData, "UNKNOWNLANGUAGE"));
                        return;
                }
                GuildAccounts.SaveGuilds();
                await ReplyAndDeleteAsync(Utilities.GetAlert(GuildData, "SETLANGUAGE", Context.User.Mention));
            }
        }
    }
}

using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using JeanPascaline.Core.AccountSystem;
using JeanPascaline.Services;
using System;
using System.Threading.Tasks;

namespace JeanPascaline.Modules
{
    [RequireUserPermission(GuildPermission.Administrator)]
    public class AdministrationModule : InteractiveBase<SocketCommandContext>
    {

        [Command("givexp")]
        [Alias("gxp"), Remarks("adm")]
        [Summary("Donne un nombre défini de point d'expériences à un'e membre. — Give a certain number of experience point to someone.")]
        public async Task GiveXP(SocketUser member, uint xp)
        {
            
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            UserAccount UserData = UserAccounts.GetAccount(member, Context.Guild.Id);

            if (UserData.Level >= GuildData.MaxLevel)
            {
                EmbedBuilder Embed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildData, "UNSUCESSFULGIVE", member.Mention));
                await ReplyAsync(embed: Embed.Build());
            }
            else
            {
                UserData.XP += xp;
                UserAccounts.SaveAccounts(Context.Guild.Id);
                EmbedBuilder Embed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildData, "GIVEXP", xp, member.Mention));
                await ReplyAsync(embed: Embed.Build());
            }
        }

        [Command("takexp")]
        [Alias("txp"), Remarks("adm")]
        [Summary("Retire un certain nombre de points d'expérience à un'e membre. — Retrieve a certain amount of experience point to a member.")]
        public async Task TakeXP(SocketUser member, uint xp)
        {
            
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            UserAccount UserData = UserAccounts.GetAccount(member, Context.Guild.Id);

            if ((UserData.XP - xp) < 0)
            {
                EmbedBuilder Embed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildData, "UNSUCESSFULTAKE", member.Mention));
                await ReplyAsync(embed: Embed.Build());
            }
            else
            {
                UserData.XP -= xp;
                UserAccounts.SaveAccounts(Context.Guild.Id);
                EmbedBuilder Embed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildData, "TAKEXP", xp, member.Mention));
                await ReplyAsync(embed: Embed.Build());
            }

        }

        [Command("setchannel")]
        [Alias("sc"), Remarks("adm")]
        [Summary("Définit l'utilité d'un salon écrit précis. (Salon musical, salon d'annonce, salon logs.) — Defines the subject of a specific written channel. (Music channel, Announcement channel, Log channel.)")]
        public async Task SetChannel(ITextChannel Channel = null, string Channelcode = null)
        {
            
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
                        Title = UtilitiesService.GetAlert(GuildData, "CODELISTTITLE"),
                        Description = "· DC — Default Channel\n" +
                        "· LC — Log Channel\n" +
                        "· MC — Music Channel",
                        Timestamp = DateTime.Now,
                        Footer = new EmbedFooterBuilder()
                        {
                            Text = UtilitiesService.GetAlert(GuildData, "PARAMEMBEDFOOTER", "setchannel (Channelcode)")
                        }
                    };
                    await ReplyAsync(embed: ChannelEmbed.Build());

                    return;
            }
            GuildAccounts.SaveGuilds();
            EmbedBuilder Embed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildData, "CHANGESSAVED", Context.User.Mention));
            await ReplyAndDeleteAsync("", embed: Embed.Build());
        }

        [Command("setrole")]
        [Alias("sr"), Remarks("adm")]
        [Summary("Définit l'utilité d'un rôle précis. (Rôle des modérateurs, rôle par défaut, ...) — Defines a precise role. (Moderator, Default role, ...)")]
        public async Task SetRole(IRole Role = null, string Rolecode = null)
        {
            
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
                        Title = UtilitiesService.GetAlert(GuildData, "CODELISTTITLE"),
                        Description = "· DR — Default Role\n" +
                        "· MR — Moderator Role",
                        Timestamp = DateTime.Now,
                        Footer = new EmbedFooterBuilder()
                        {
                            Text = UtilitiesService.GetAlert(GuildData, "PARAMEMBEDFOOTER", "setrole (rolecode)")
                        }
                    };
                    await ReplyAsync(embed: RoleEmbed.Build());
                    return;
            }
            GuildAccounts.SaveGuilds();
            EmbedBuilder Embed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildData, "CHANGESSAVED", Context.User.Mention));
            await ReplyAndDeleteAsync("", embed: Embed.Build());
        }

        [Command("setvalue")]
        [Alias("sv"), Remarks("adm")]
        [Summary("Change la valeur d'un paramètre du bot. (Niveau maximal sur le serveur, nombre d'avertissements) — Changes the value of a bot setting. (Maximum level, number of warnings)")]
        public async Task SetValue(uint Value = 0, string Valuecode = null)
        {
            
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
                        Title = UtilitiesService.GetAlert(GuildData, "CODELISTTITLE"),
                        Description = "· ML — Max Level\n" +
                        "· WT — Warnings Threshold",
                        Timestamp = DateTime.Now,
                        Footer = new EmbedFooterBuilder()
                        {
                            Text = UtilitiesService.GetAlert(GuildData, "PARAMEMBEDFOOTER", "setvalue (valuecode)")
                        }
                    };
                    await ReplyAsync(embed: RoleEmbed.Build());
                    return;
            }
            GuildAccounts.SaveGuilds();
            EmbedBuilder Embed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildData, "CHANGESSAVED", Context.User.Mention));
            await ReplyAndDeleteAsync("", embed: Embed.Build());
        }

        [Command("addreward", RunMode = RunMode.Async)]
        [Alias("arw"), Remarks("adm")]
        [Summary("Ajoute une récompense à la liste des récompenses pour un niveau précis. — Adds a reward to the reward list for a specific level.")]
        public async Task AddReward(IRole RewardRole, uint Level)
        {
            
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            GuildData.Rewards.TryAdd(RewardRole.Id, Level);
            GuildAccounts.SaveGuilds();
            EmbedBuilder Embed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildData, "REWARDADDED", Context.User.Mention));
            await ReplyAndDeleteAsync("", embed: Embed.Build());
        }

        [Command("removereward", RunMode = RunMode.Async)]
        [Alias("rrw"), Remarks("adm")]
        [Summary("Retire une récompense de la liste des récompenses pour un niveau précis. — Removes a reward from the reward list for a specific level.")]
        public async Task RemoveReward(IRole RewardRole)
        {
            
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            try
            {
                GuildData.Rewards.Remove(RewardRole.Id);
                GuildAccounts.SaveGuilds();
                EmbedBuilder Embed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildData, "REWARDREMOVED", Context.User.Mention));
                await ReplyAndDeleteAsync("", embed: Embed.Build());
            }
            catch (Exception ex)
            {
                await UtilitiesService.SendErrorAsync(Context, ex);
            }
        }

        [Command("addword", RunMode = RunMode.Async)]
        [Alias("aw"), Remarks("adm")]
        [Summary("Ajoute un mot à la liste des mots interdits. — Adds a word to the banned list.")]
        public async Task AddWord([Remainder]string Word)
        {
            
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            GuildData.ForbiddenWords.Add(Word);
            GuildAccounts.SaveGuilds();
            EmbedBuilder Embed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildData, "WORDADDED", Context.User.Mention));
            await ReplyAndDeleteAsync("", embed: Embed.Build());
        }

        [Command("removeword", RunMode = RunMode.Async)]
        [Alias("rw"), Remarks("adm")]
        [Summary("Retire un mot de la liste des mots interdits. — Removes a word from the banned list.")]
        public async Task RemoveWord([Remainder]string Word)
        {
            
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            GuildData.ForbiddenWords.Remove(Word);
            GuildAccounts.SaveGuilds();
            EmbedBuilder Embed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildData, "WORDREMOVED", Context.User.Mention));
            await ReplyAndDeleteAsync("", embed: Embed.Build());
        }

        [Command("joinalert"), Remarks("adm")]
        [Alias("ja")]
        [Summary("Active/Désactive les messages d'accueil et de départ. — Activate/Deactivate welcome and leaving alerts.")]
        public async Task SwitchWelcomeMessage()
        {
            

            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            if (GuildData.IsAnnoncementMessagesEnabled == true)
            {
                GuildData.IsAnnoncementMessagesEnabled = false;
                EmbedBuilder Embed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildData, "REMOVEWELCOMEMESSAGE", Context.User.Mention));
                await ReplyAndDeleteAsync("", embed: Embed.Build());
            }
            else
            {
                GuildData.IsAnnoncementMessagesEnabled = true;
                EmbedBuilder Embed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildData, "SETWELCOMEMESSAGE", Context.User.Mention));
                await ReplyAndDeleteAsync("", embed: Embed.Build());
            }
            GuildAccounts.SaveGuilds();
        }

        [Command("language", RunMode = RunMode.Async), Remarks("adm")]
        [Alias("lang"), Summary("Change la langue du bot. — Change the bot language.")]
        public async Task SetLanguage(string langcode = null)
        {

            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);

            switch (langcode)
            {
                case "FR":
                    GuildData.Language = "Default";
                    break;

                case "EN":
                    GuildData.Language = "English";
                    break;

                default:
                    EmbedBuilder LangEmbed = UtilitiesService.CreateEmbed(Color.Blue, "· FR — Français\n· EN — English", UtilitiesService.GetAlert(GuildData, "LANGUAGELISTTITLE"));
                    LangEmbed.Footer = new EmbedFooterBuilder()
                    {
                        Text = UtilitiesService.GetAlert(GuildData, "PARAMEMBEDFOOTER", "language (Langcode)")
                    };
                    await ReplyAsync(embed: LangEmbed.Build());
                    return;
            }
            GuildAccounts.SaveGuilds();
            EmbedBuilder Embed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildData, "SETLANGUAGE", Context.User.Mention));
            await ReplyAndDeleteAsync("", embed: Embed.Build());

        }
    }
}

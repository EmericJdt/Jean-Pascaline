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
    public class InfoModule : InteractiveBase<SocketCommandContext>
    {
        // Provides a list of all the commands avaliable depending on the user's permissions'.

        [Command("index", RunMode = RunMode.Async), Summary("Obtenir une liste de toutes les commandes. — Get a list of all commands.")]
        [Alias("help")]
        public async Task Index(string Module = null)
        {
            await Context.Message.DeleteAsync();

            List<CommandInfo> _commands = Program._commands.Commands.Take(Program._commands.Commands.Count() / 2).ToList();
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            GuildPermissions UserPermissions = ((SocketGuildUser)Context.User).GuildPermissions;
            EmbedBuilder CommandEmbed;
            switch (Module)
            {
                case string Info when Info.ToLower() == "info":
                    EmbedAuthorBuilder InfoModuleAuthor = Utilities.CreateAuthorEmbed($"{Context.Guild.Name} — {Utilities.GetAlert(GuildData, "INDEXCARDTITLE")}", Context.Guild.IconUrl);
                    CommandEmbed = Utilities.CreateEmbed(InfoModuleAuthor, Color.Teal, Utilities.CreateListFields(Utilities.GetCommandsInfoDictionnary(_commands.Where(x => x.Remarks == null).ToList())));
                    break;
                case string Audio when Audio.ToLower() == "audio":
                    EmbedAuthorBuilder AudioCommandsAuthor = Utilities.CreateAuthorEmbed($"{Context.Guild.Name} — {Utilities.GetAlert(GuildData, "INDEXAUDIOCARDTITLE")}", Context.Guild.IconUrl);
                    CommandEmbed = Utilities.CreateEmbed(AudioCommandsAuthor, Color.Purple, Utilities.CreateListFields(Utilities.GetCommandsInfoDictionnary(_commands.Where(x => x.Module.Group == "audio").ToList())));
                    break;
                case string Mod when Mod.ToLower() == "mod" || Mod.ToLower() == "moderation":
                    EmbedAuthorBuilder ModerationCommandsAuthor = Utilities.CreateAuthorEmbed($"{Context.Guild.Name} — {Utilities.GetAlert(GuildData, "INDEXMODCARDTITLE")}", Context.Guild.IconUrl);
                    CommandEmbed = Utilities.CreateEmbed(ModerationCommandsAuthor, Color.Blue, Utilities.CreateListFields(Utilities.GetCommandsInfoDictionnary(_commands.Where(x => x.Module.Group == "mod").ToList())));
                    break;
                case string Admin when Admin.ToLower() == "adm" || Admin.ToLower() == "admin":
                    EmbedAuthorBuilder AdminCommandsAuthor = Utilities.CreateAuthorEmbed($"{Context.Guild.Name} — {Utilities.GetAlert(GuildData, "INDEXADMCARDTITLE")}", Context.Guild.IconUrl);
                    CommandEmbed = Utilities.CreateEmbed(AdminCommandsAuthor, Color.Red, Utilities.CreateListFields(Utilities.GetCommandsInfoDictionnary(_commands.Where(x => x.Module.Group == "adm").ToList())));
                    break;
                case string Dev when Dev.ToLower() == "dev":
                    EmbedAuthorBuilder DevModuleAuthor = Utilities.CreateAuthorEmbed($"{Context.Guild.Name} — {Utilities.GetAlert(GuildData, "INDEXDEVCARDTITLE")}", Context.Guild.IconUrl);
                    CommandEmbed = Utilities.CreateEmbed(DevModuleAuthor, Color.Teal, Utilities.CreateListFields(Utilities.GetCommandsInfoDictionnary(_commands.Where(x => x.Remarks == "Owner").ToList())));
                    break;
                default:
                    EmbedBuilder ModuleEmbed = new EmbedBuilder()
                    {
                        Color = Color.Teal,
                        Title = Utilities.GetAlert(GuildData, "MODULELISTTITLE"),
                        Description =
                        "· Info — Information\n" +
                        "· Audio — Music\n" +
                        "· Mod — Moderation\n" +
                        "· Adm — Administration\n" +
                        "· Dev — Specials",
                        Timestamp = DateTime.Now,
                        Footer = new EmbedFooterBuilder()
                        {
                            Text = Utilities.GetAlert(GuildData, "MODULEFOOTEREMBED", "index (module)")
                        }
                    };
                    await ReplyAsync(embed: ModuleEmbed.Build());
                    return;
            }
            await Context.User.SendMessageAsync(embed: CommandEmbed.Build());
        }

        // Provides informations about users displayed on a profile.

        [Command("profile", RunMode = RunMode.Async), Summary("Obtenir le niveau et les points d'expérience d'un utilisateur — Get the level and the experience of a user")]
        public async Task Profile(IGuildUser member = null)
        {
            await Context.Message.DeleteAsync();
            
            member ??= Context.User as IGuildUser;
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            UserAccount UserData = UserAccounts.GetAccount((SocketUser)member, GuildData.ID);

            var Author = Utilities.CreateAuthorEmbed($"{Context.Guild.Name} — {member.ToString()}", Context.Guild.IconUrl);
            Dictionary<object, object> FieldsData = new Dictionary<object, object>()
            {
                { Utilities.GetAlert(GuildData, "PROFILECARDNAME"), member.Mention.ToString() },
                { Utilities.GetAlert(GuildData, "PROFILECARDPRONOUNS"), UserData.Pronouns },
                { Utilities.GetAlert(GuildData, "PROFILECARDLEVEL"), UserData.Level },
                { Utilities.GetAlert(GuildData, "PROFILECARDDESCRIPTION"), UserData.Description },
                { Utilities.GetAlert(GuildData, "PROFILECARDJOINEDAT"), $"{member.JoinedAt.Value.Day}/{member.JoinedAt.Value.Month}/{member.JoinedAt.Value.Year}" },
                { Utilities.GetAlert(GuildData, "PROFILECARDWARNCOUNT"), $"{UserData.NbWarnings}/{GuildData.WarningsThreshold}"}
            };

            EmbedBuilder embed = Utilities.CreateEmbed(Author, new Color(245, 200, 7),Utilities.CreateListFields(FieldsData, true), member.GetAvatarUrl());

            if (UserData.NbWarnings != 0)
            {
                string list = "";
                foreach (KeyValuePair<string, string> key in UserData.Warns)
                {
                    list += $"{key.Key} — {key.Value}\n";
                }
                embed.AddField(Utilities.GetAlert(GuildData, "PROFILECARDWARNLIST"), list);
            }

            try
            {
                await Context.User.SendMessageAsync("", false, embed.Build());
            }
            catch (Exception ex)
            {
                await Utilities.SendErrorAsync(Context, ex);
            }
        }

        // Changes the pronouns displayed on the user's profile.

        [Command("pronouns", RunMode = RunMode.Async), Summary("Modifie les pronoms affichés sur votre profil. — Modify the pronouns displayed on your profile.")]
        public async Task Pronouns([Remainder]string pronouns)
        {
            await Context.Message.DeleteAsync();
            UserAccounts.GetAccount(Context.User, Context.Guild.Id).Pronouns = pronouns;
            UserAccounts.SaveAccounts(Context.Guild.Id);
            await ReplyAndDeleteAsync(Utilities.GetAlert(GuildAccounts.GetAccount(Context.Guild), "PROFILECHANGEPRONOUNS", Context.User.Mention));
        }

        // Get a list of all rewards on the server

        [Command("rewards", RunMode = RunMode.Async)]
        public async Task Rewards()
        {
            await Context.Message.DeleteAsync();
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            UserAccount UserData = UserAccounts.GetAccount(Context.User, GuildData.ID);
            List<EmbedFieldBuilder> RewardsFields = new List<EmbedFieldBuilder>();

            foreach (KeyValuePair<ulong, uint> RoleData in GuildData.Rewards.OrderBy(x => x.Value))
            {
                if (RoleData.Value > UserData.Level)
                {
                    RewardsFields.Add(
                    new EmbedFieldBuilder()
                    {
                        Name= $"Lvl {RoleData.Value}", 
                        Value = Context.Guild.GetRole(RoleData.Key).Mention
                    }
                    );
                }
            }

            if (RewardsFields.Count == 0)
            {
                await ReplyAndDeleteAsync(Utilities.GetAlert(GuildData, "NOREWARDS", Context.User.Mention));
            }
            else
            {
                EmbedAuthorBuilder RewardsAuthor = Utilities.CreateAuthorEmbed(Utilities.GetAlert(GuildData, "REWARDLISTTITLE"), Context.Guild.IconUrl);
                EmbedBuilder RewardsEmbed = Utilities.CreateEmbed(RewardsAuthor, new Color(Utilities.GetRandomNumber(0,255), Utilities.GetRandomNumber(0, 255), Utilities.GetRandomNumber(0, 255)), RewardsFields);
                await ReplyAsync(Utilities.GetAlert(GuildData, "REWARDSRESPONSE", Context.User.Mention, UserData.Level), false, RewardsEmbed.Build());
            }
        }

        // Changes the description displayed on the user's profile.

        [Command("description", RunMode = RunMode.Async), Summary("Modifie la description affiché sur votre profil. — Modify the description displayed on your profile.")]
        public async Task Description([Remainder]string description)
        {
            await Context.Message.DeleteAsync();
            UserAccounts.GetAccount(Context.User, Context.Guild.Id).Description = description;
            UserAccounts.SaveAccounts(Context.Guild.Id);
            await ReplyAndDeleteAsync(Utilities.GetAlert(GuildAccounts.GetAccount(Context.Guild), "PROFILECHANGEDESCRIPTION", Context.User.Mention));
        }

        // Sends a random gif of a anime girl doing a cat pose.

        [Command("nyah", RunMode = RunMode.Async), Summary("ฅ^•ﻌ•^ฅ")]
        public async Task Nyah(IGuildUser member = null)
        {
            await Context.Message.DeleteAsync();
            if(member == null) {
                await Context.Channel.SendFileAsync(@"Images/nyah/" + Utilities.GetRandomNumber(1, Config.bot.NyahGifCount) + ".gif", Context.User.Mention);
            }
            else
            {
                await Context.Channel.SendFileAsync(@"Images/nyah/" + Utilities.GetRandomNumber(1, Config.bot.NyahGifCount) + ".gif",
                $"{Utilities.GetAlert(GuildAccounts.GetAccount(Context.Guild),"NYAH", Context.User.Mention, member.Mention)} ฅ^•ﻌ•^ฅ");
            }
        }

        // Sends a random gif of a hug.

        [Command("hug", RunMode = RunMode.Async), Summary("Parce qu'on à tous besoin d'un gros calin en cas de problèmes. — Because we all need a big hug in case of emergency.")]
        public async Task Hug(IGuildUser member = null)
        {
            await Context.Message.DeleteAsync();
            if (member == null)
            {
                await Context.Channel.SendFileAsync(@"Images/hug/" + Utilities.GetRandomNumber(1, Config.bot.HugGifCount) + ".gif", Context.User.Mention);
            }
            else {
                await Context.Channel.SendFileAsync(@"Images/hug/" + Utilities.GetRandomNumber(1, Config.bot.HugGifCount) + ".gif",
                Utilities.GetAlert(GuildAccounts.GetAccount(Context.Guild), "HUG", Context.User.Mention, member.Mention));
            }
        }

        // Sends a random gif of a anime girl being pat on the head.

        [Command("patpat", RunMode = RunMode.Async), Summary("*pat pat*")]
        public async Task PatPat(IGuildUser member = null)
        {
            await Context.Message.DeleteAsync();
            if(member == null) {
                await Context.Channel.SendFileAsync(@"Images/patpat/" + Utilities.GetRandomNumber(1, Config.bot.PatPatGifCount) + ".gif", Context.User.Mention);
            }
            else {
                await Context.Channel.SendFileAsync(@"Images/patpat/" + Utilities.GetRandomNumber(1, Config.bot.PatPatGifCount) + ".gif",
                Utilities.GetAlert(GuildAccounts.GetAccount(Context.Guild), "PATPAT", Context.User.Mention, member.Mention));
            }
        }

        // Sends a motivation quote.

        [Command("motivation", RunMode = RunMode.Async), Summary("Envoie une citation motivante. — Send a motivation quote (French Only).")]
        public async Task Motivation(IGuildUser member = null)
        {
            await Context.Message.DeleteAsync();
            member ??= (IGuildUser)Context.User;
            if(GuildAccounts.GetAccount(Context.Guild).Language != "English" || GuildAccounts.GetAccount(Context.Guild).Language != "Default")
            { 
                await ReplyAsync(Utilities.GetAlert(GuildAccounts.GetAccount(Context.Guild), "MOT" + Utilities.GetRandomNumber(1, Config.bot.QuoteCount)) + member.Mention);
            }
        }

        // Creates a invite of the announcement channel and send the to the user.

        [Command("invite", RunMode = RunMode.Async), Summary("Crée et envoie une invitation du serveur à usage unique. — Create and send a disposable guild link.")]
        public async Task Invite()
        {
            await Context.Message.DeleteAsync();
            try
            {
                var invite = Context.Guild.GetTextChannel(Context.Guild.DefaultChannel.Id).CreateInviteAsync(3600, 1, true, true);
                IDMChannel DM = await Context.User.GetOrCreateDMChannelAsync();
                await DM.SendMessageAsync(Utilities.GetAlert(GuildAccounts.GetAccount(Context.Guild), "INVITEREQUEST" ,invite.Result));
            }
            catch (Exception ex)
            {
                await Utilities.SendErrorAsync(Context, ex);
            }
        }
    }
}
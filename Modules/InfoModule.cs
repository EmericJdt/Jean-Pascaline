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
            

            List<CommandInfo> _commands = Program._commands.Commands.Take(Program._commands.Commands.Count()).ToList();
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            GuildPermissions UserPermissions = ((SocketGuildUser)Context.User).GuildPermissions;
            EmbedBuilder CommandEmbed;
            switch (Module)
            {
                case string Info when Info.ToLower() == "info":
                    EmbedAuthorBuilder InfoModuleAuthor = UtilitiesService.CreateAuthorEmbed($"{Context.Guild.Name} — {UtilitiesService.GetAlert(GuildData, "INDEXCARDTITLE")}", Context.Guild.IconUrl);
                    CommandEmbed = UtilitiesService.CreateEmbed(InfoModuleAuthor, Color.Teal, UtilitiesService.CreateListFields(UtilitiesService.GetCommandsInfoDictionnary(_commands.Where(x => x.Remarks == null).ToList())));
                    break;
                case string Audio when Audio.ToLower() == "audio":
                    EmbedAuthorBuilder AudioCommandsAuthor = UtilitiesService.CreateAuthorEmbed($"{Context.Guild.Name} — {UtilitiesService.GetAlert(GuildData, "INDEXAUDIOCARDTITLE")}", Context.Guild.IconUrl);
                    CommandEmbed = UtilitiesService.CreateEmbed(AudioCommandsAuthor, Color.Purple, UtilitiesService.CreateListFields(UtilitiesService.GetCommandsInfoDictionnary(_commands.Where(x => x.Remarks == "Audio").ToList())));
                    break;
                case string Mod when Mod.ToLower() == "mod" || Mod.ToLower() == "moderation":
                    EmbedAuthorBuilder ModerationCommandsAuthor = UtilitiesService.CreateAuthorEmbed($"{Context.Guild.Name} — {UtilitiesService.GetAlert(GuildData, "INDEXMODCARDTITLE")}", Context.Guild.IconUrl);
                    CommandEmbed = UtilitiesService.CreateEmbed(ModerationCommandsAuthor, Color.Blue, UtilitiesService.CreateListFields(UtilitiesService.GetCommandsInfoDictionnary(_commands.Where(x => x.Remarks == "mod").ToList())));
                    break;
                case string Admin when Admin.ToLower() == "adm" || Admin.ToLower() == "admin":
                    EmbedAuthorBuilder AdminCommandsAuthor = UtilitiesService.CreateAuthorEmbed($"{Context.Guild.Name} — {UtilitiesService.GetAlert(GuildData, "INDEXADMCARDTITLE")}", Context.Guild.IconUrl);
                    CommandEmbed = UtilitiesService.CreateEmbed(AdminCommandsAuthor, Color.Red, UtilitiesService.CreateListFields(UtilitiesService.GetCommandsInfoDictionnary(_commands.Where(x => x.Remarks == "adm").ToList())));
                    break;
                case string Dev when Dev.ToLower() == "dev":
                    EmbedAuthorBuilder DevModuleAuthor = UtilitiesService.CreateAuthorEmbed($"{Context.Guild.Name} — {UtilitiesService.GetAlert(GuildData, "INDEXDEVCARDTITLE")}", Context.Guild.IconUrl);
                    CommandEmbed = UtilitiesService.CreateEmbed(DevModuleAuthor, Color.Teal, UtilitiesService.CreateListFields(UtilitiesService.GetCommandsInfoDictionnary(_commands.Where(x => x.Remarks == "Owner").ToList())));
                    break;
                default:
                    EmbedBuilder ModuleEmbed = UtilitiesService.CreateEmbed(Color.Teal, "· Info — Information\n" +
                        "· Audio — Music\n" +
                        "· Mod — Moderation\n" +
                        "· Adm — Administration\n" +
                        "· Dev — Specials", UtilitiesService.GetAlert(GuildData, "MODULELISTTITLE"));
                    ModuleEmbed.Timestamp = DateTime.Now;
                    ModuleEmbed.Footer = new EmbedFooterBuilder()
                    {
                        Text = UtilitiesService.GetAlert(GuildData, "MODULEFOOTEREMBED", "index (module)")
                    };
                    await ReplyAsync(embed: ModuleEmbed.Build());
                    return;
            }
            await Context.User.SendMessageAsync(embed: CommandEmbed.Build());
        }

        // Provides informations about users displayed on a profile.

        [Command("profile", RunMode = RunMode.Async), Summary("Affiche le profil d'un' utilisateur'ice — Shows a user's profil.")]
        public async Task Profile(IGuildUser member = null)
        {
            

            member ??= Context.User as IGuildUser;
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            UserAccount UserData = UserAccounts.GetAccount((SocketUser)member, GuildData.ID);

            EmbedAuthorBuilder Author = UtilitiesService.CreateAuthorEmbed($"{Context.Guild.Name} — {member.ToString()}", Context.Guild.IconUrl);
            Dictionary<object, object> FieldsData = new Dictionary<object, object>()
            {
                { UtilitiesService.GetAlert(GuildData, "PROFILECARDNAME"), member.Mention.ToString() },
                { UtilitiesService.GetAlert(GuildData, "PROFILECARDPRONOUNS"), UserData.Pronouns },
                { UtilitiesService.GetAlert(GuildData, "PROFILECARDDESCRIPTION"), UserData.Description },
                { UtilitiesService.GetAlert(GuildData, "PROFILECARDLEVEL"), UserData.Level },
                { UtilitiesService.GetAlert(GuildData, "PROFILECARDXP"), UserData.XP },
                { UtilitiesService.GetAlert(GuildData, "PROFILECARDJOINEDAT"), $"{member.JoinedAt.Value.Day}/{member.JoinedAt.Value.Month}/{member.JoinedAt.Value.Year}" },
                { UtilitiesService.GetAlert(GuildData, "PROFILECARDWARNCOUNT"), $"{UserData.NbWarnings}/{GuildData.WarningsThreshold}"}
            };

            EmbedBuilder ResponseEmbed = UtilitiesService.CreateEmbed(Author, new Color(255, 255, 0), UtilitiesService.CreateListFields(FieldsData, true), member.GetAvatarUrl());

            if (UserData.NbWarnings != 0)
            {
                string list = "";
                foreach (KeyValuePair<string, string> key in UserData.Warns)
                {
                    list += $"{key.Key} — {key.Value}\n";
                }
                ResponseEmbed.AddField(UtilitiesService.GetAlert(GuildData, "PROFILECARDWARNLIST"), list);
            }

            try
            {
                await Context.User.SendMessageAsync("", false, ResponseEmbed.Build());
            }
            catch (Exception ex)
            {
                await UtilitiesService.SendErrorAsync(Context, ex);
            }
        }

        // Get a list of all rewards on the server

        [Command("rewards", RunMode = RunMode.Async)]
        [Summary("Affiche une liste des récompenses disponibles sur le serveur. — Shows the list of rewards available on the server.")]
        public async Task Rewards()
        {
            
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
                        Name = UtilitiesService.GetAlert(GuildData, "REWARDSCARDLEVEL", RoleData.Value),
                        Value = Context.Guild.GetRole(RoleData.Key).Mention
                    }
                    );
                }
            }

            if (RewardsFields.Count == 0)
            {
                EmbedBuilder ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildData, "NOREWARDS", Context.User.Mention));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
            else
            {
                EmbedAuthorBuilder RewardsAuthor = UtilitiesService.CreateAuthorEmbed(UtilitiesService.GetAlert(GuildData, "REWARDLISTTITLE"), Context.Guild.IconUrl);
                EmbedBuilder RewardsEmbed = UtilitiesService.CreateEmbed(RewardsAuthor, new Color(UtilitiesService.GetRandomNumber(0, 255), UtilitiesService.GetRandomNumber(0, 255), UtilitiesService.GetRandomNumber(0, 255)), RewardsFields);
                await ReplyAsync(UtilitiesService.GetAlert(GuildData, "REWARDSRESPONSE", Context.User.Mention, UserData.Level), false, RewardsEmbed.Build());
            }
        }

        // Changes the pronouns displayed on the user's profile.

        [Command("pronouns", RunMode = RunMode.Async)]
        [Summary("Modifie les pronoms affichés sur votre profil. — Modify the pronouns displayed on your profile.")]
        public async Task Pronouns([Remainder]string pronouns)
        {
            
            UserAccounts.GetAccount(Context.User, Context.Guild.Id).Pronouns = pronouns;
            UserAccounts.SaveAccounts(Context.Guild.Id);
            EmbedBuilder Embed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildAccounts.GetAccount(Context.Guild), "PROFILECHANGEPRONOUNS", Context.User.Mention));
            await ReplyAndDeleteAsync("", embed: Embed.Build());
        }

        // Changes the description displayed on the user's profile.

        [Command("description", RunMode = RunMode.Async)]
        [Summary("Modifie la description affiché sur votre profil. — Modify the description displayed on your profile.")]
        public async Task Description([Remainder]string description)
        {
            
            UserAccounts.GetAccount(Context.User, Context.Guild.Id).Description = description;
            UserAccounts.SaveAccounts(Context.Guild.Id);
            EmbedBuilder ResponseEmbed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildAccounts.GetAccount(Context.Guild), "PROFILECHANGEDESCRIPTION", Context.User.Mention));
            await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
        }

        // Sends a random gif of a anime girl doing a cat pose.

        [Command("nyah", RunMode = RunMode.Async), Summary("ฅ^•ﻌ•^ฅ")]
        public async Task Nyah(IGuildUser member = null)
        {
            if (member == null)
            {
                await Context.Channel.SendFileAsync(@"Images/nyah/" + UtilitiesService.GetRandomNumber(1, Config.bot.NyahGifCount) + ".gif", Context.User.Mention);
            }
            else
            {
                await Context.Channel.SendFileAsync(@"Images/nyah/" + UtilitiesService.GetRandomNumber(1, Config.bot.NyahGifCount) + ".gif",
                $"{UtilitiesService.GetAlert(GuildAccounts.GetAccount(Context.Guild), "NYAH", Context.User.Mention, member.Mention)} ฅ^•ﻌ•^ฅ");
            }
        }

        // Sends a random gif of a hug.

        [Command("hug", RunMode = RunMode.Async)]
        [Summary("Parce qu'on à tous besoin d'un gros calin en cas de problèmes. — Because we all need a big hug in case of emergency.")]
        public async Task Hug(IGuildUser member = null)
        {
            
            if (member == null)
            {
                await Context.Channel.SendFileAsync(@"Images/hug/" + UtilitiesService.GetRandomNumber(1, Config.bot.HugGifCount) + ".gif", Context.User.Mention);
            }
            else
            {
                await Context.Channel.SendFileAsync(@"Images/hug/" + UtilitiesService.GetRandomNumber(1, Config.bot.HugGifCount) + ".gif",
                UtilitiesService.GetAlert(GuildAccounts.GetAccount(Context.Guild), "HUG", Context.User.Mention, member.Mention));
            }
        }

        // Sends a random gif of a anime girl being pat on the head.

        [Command("patpat", RunMode = RunMode.Async), Summary("*pat pat*")]
        public async Task PatPat(IGuildUser member = null)
        {
            if (member == null)
            {
                await Context.Channel.SendFileAsync(@"Images/patpat/" + UtilitiesService.GetRandomNumber(1, Config.bot.PatPatGifCount) + ".gif", Context.User.Mention);
            }
            else
            {
                await Context.Channel.SendFileAsync(@"Images/patpat/" + UtilitiesService.GetRandomNumber(1, Config.bot.PatPatGifCount) + ".gif",
                UtilitiesService.GetAlert(GuildAccounts.GetAccount(Context.Guild), "PATPAT", Context.User.Mention, member.Mention));
            }
        }

        // Sends a motivation quote.

        [Command("motivation", RunMode = RunMode.Async)]
        [Summary("Envoie une citation motivante. — Sends a motivational quote.")]
        public async Task Motivation(IGuildUser member = null)
        {
            
            member ??= (IGuildUser)Context.User;
            if (GuildAccounts.GetAccount(Context.Guild).Language != "English" || GuildAccounts.GetAccount(Context.Guild).Language != "Default")
            {
                await ReplyAsync(UtilitiesService.GetAlert(GuildAccounts.GetAccount(Context.Guild), "MOT" + UtilitiesService.GetRandomNumber(1, Config.bot.QuoteCount)) + member.Mention);
            }
        }

        // Creates a invite of the announcement channel and send the to the user.

        [Command("invite", RunMode = RunMode.Async)]
        [Summary("Crée et envoie une invitation du serveur à usage unique. — Provides a disposable guild link.")]
        public async Task Invite()
        {
            
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            try
            {
                Task<IInviteMetadata> invite = Context.Guild.GetTextChannel(Context.Guild.DefaultChannel.Id).CreateInviteAsync(3600, 1, true, true);
                IDMChannel DM = await Context.User.GetOrCreateDMChannelAsync();
                await DM.SendMessageAsync(UtilitiesService.GetAlert(GuildData, "INVITEREQUEST", invite.Result));

                EmbedAuthorBuilder InviteLogAuthor = UtilitiesService.CreateAuthorEmbed(UtilitiesService.GetAlert(GuildData, "INVITEREQUESTCARDTITLE"), Context.User.GetAvatarUrl());

                Dictionary<object, object> FieldsData = new Dictionary<object, object>()
                {
                    { UtilitiesService.GetAlert(GuildData,"CARDUSERFIELD"), Context.User.Mention  },
                    { "Code : ", invite.Result.Code },
                };

                EmbedBuilder InviteLogEmbed = UtilitiesService.CreateEmbed(InviteLogAuthor, Color.Blue, UtilitiesService.CreateListFields(FieldsData, true));

                await Context.Guild.GetTextChannel(GuildData.LogChannelID).SendMessageAsync(embed: InviteLogEmbed.Build());
            }
            catch (Exception ex)
            {
                await UtilitiesService.SendErrorAsync(Context, ex);
            }
        }
    }
}
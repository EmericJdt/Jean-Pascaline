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

    // All commands of this module (except "clean") sends a log message in the GuildData Log Channel.

    public class ModerationModule : InteractiveBase<SocketCommandContext>
    {

        // Ban a user from the Guild, calls another method to do the work.

        [Command("ban"), Summary("Bannit un'e utilisateur'ice du serveur. — Bans a member from the guild."), Remarks("mod")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Ban(IGuildUser member, [Remainder]string reason = "Non spécifiée.")
        {
            
            IGuildUser User = Context.User as IGuildUser;
            EmbedBuilder ResponseEmbed;
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            if (!User.RoleIds.Contains(GuildData.ModeratorRoleID) && !User.GuildPermissions.Administrator && User.Id != Context.Guild.OwnerId)
            {
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildAccounts.GetAccount(Context.Guild), "DENIED"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
            else if (member.RoleIds.Contains(GuildData.ModeratorRoleID))
            {
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildAccounts.GetAccount(Context.Guild), "COMMANDONMOD"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
            else
            {
                await BanAsync(member, reason, Context);
            }
        }

        // Unban a user from the Guild

        [Command("unban"), Summary("Révoque le bannissement d'un'e utilisateur'ice — Unbans a member from the guild."), Remarks("mod")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Unban(IUser user)
        {
            
            IGuildUser User = Context.User as IGuildUser;
            EmbedBuilder ResponseEmbed;
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            if (!User.RoleIds.Contains(GuildData.ModeratorRoleID) && !User.GuildPermissions.Administrator && User.Id != Context.Guild.OwnerId)
            {
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildAccounts.GetAccount(Context.Guild), "DENIED"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
            else
            {
                await UnbanAsync(user, Context);
            }
        }

        // Expells a member from the Guild, they can come back with an invitation.

        [Command("kick"), Summary("Expulse un'e utilisateur'ice du serveur. — Kicks out a member from the guild."), Remarks("mod")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task Kick(IGuildUser member, [Remainder]string reason = "Non spécifiée.")
        {
            
            IGuildUser User = Context.User as IGuildUser;
            EmbedBuilder ResponseEmbed;
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            if (!User.RoleIds.Contains(GuildData.ModeratorRoleID) && !User.GuildPermissions.Administrator && User.Id != Context.Guild.OwnerId)
            {
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildAccounts.GetAccount(Context.Guild), "DENIED"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
            else if (member.RoleIds.Contains(GuildData.ModeratorRoleID))
            {
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildAccounts.GetAccount(Context.Guild), "COMMANDONMOD"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
            else
            {
                await KickAsync(member, reason, Context);
            }
        }

        // Add muted role to a specific member of the guild, preventing them to talk in the chat.

        [Command("mute"), Summary("Rend muet un'e utilisateur'ice. — Prevents a member from sending messages."), Remarks("mod")]
        [RequireBotPermission(GuildPermission.MuteMembers)]
        public async Task Mute(IGuildUser member, [Remainder]string reason = "Non spécifiée.")
        {
            
            IGuildUser User = Context.User as IGuildUser;
            EmbedBuilder ResponseEmbed;
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            if (!User.RoleIds.Contains(GuildData.ModeratorRoleID) && !User.GuildPermissions.Administrator && User.Id != Context.Guild.OwnerId)
            {
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildAccounts.GetAccount(Context.Guild), "DENIED"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
            else if (member.RoleIds.Contains(GuildData.ModeratorRoleID))
            {
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildAccounts.GetAccount(Context.Guild), "COMMANDONMOD"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
            else
            {
                await MuteAsync(member, reason, Context);
            }
        }

        // Removes muted role to a specific member of the guild

        [Command("unmute"), Summary("Rend la parole à un'e utilisateur'ice — Give back the permission to send message to a member"), Remarks("mod")]
        [RequireBotPermission(GuildPermission.MuteMembers)]
        public async Task Unmute(IGuildUser member)
        {
            
            IGuildUser User = Context.User as IGuildUser;
            EmbedBuilder ResponseEmbed;
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            if (!User.RoleIds.Contains(GuildData.ModeratorRoleID) && !User.GuildPermissions.Administrator && User.Id != Context.Guild.OwnerId)
            {
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildAccounts.GetAccount(Context.Guild), "DENIED"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
            else if (member.RoleIds.Contains(GuildData.ModeratorRoleID))
            {
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildAccounts.GetAccount(Context.Guild), "COMMANDONMOD"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
            else
            {
                await UnmuteAsync(member, Context);
            }
        }

        // Warn a user, add a warn to UserAccount Data

        [Command("warn"), Summary("Avertir un'e utilisateur'ice — Warns a member."), Remarks("mod")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task Warn(IGuildUser member, [Remainder]string reason = "Non spécifiée.")
        {
            
            IGuildUser User = Context.User as IGuildUser;
            EmbedBuilder ResponseEmbed;
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            if (!User.RoleIds.Contains(GuildData.ModeratorRoleID) && !User.GuildPermissions.Administrator && User.Id != Context.Guild.OwnerId)
            {
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildAccounts.GetAccount(Context.Guild), "DENIED"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
            else if (member.RoleIds.Contains(GuildData.ModeratorRoleID))
            {
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildAccounts.GetAccount(Context.Guild), "COMMANDONMOD"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
            else
            {
                await WarnAsync(member, reason, Context);
            }
        }

        // Removes warns from users

        [Command("unwarn"), Summary("Retire les avertissements d'un'e utilisateur'ice. — Retrieve warnings of a user."), Remarks("mod")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task Unwarn(IGuildUser member)
        {
            
            IGuildUser User = Context.User as IGuildUser;
            EmbedBuilder ResponseEmbed;
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            if (!User.RoleIds.Contains(GuildData.ModeratorRoleID) && !User.GuildPermissions.Administrator && User.Id != Context.Guild.OwnerId)
            {
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildAccounts.GetAccount(Context.Guild), "DENIED"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
            else if (member.RoleIds.Contains(GuildData.ModeratorRoleID))
            {
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildAccounts.GetAccount(Context.Guild), "COMMANDONMOD"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
            else
            {
                await UnwarnAsync(member, Context);
            }
        }

        // Cleans a certain amount of messages in the Context Channel.

        [Command("clean"), Summary("Supprime le nombre de messages d'un salon. — Delete an defined number of messages in a specific channel."), Remarks("mod")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task Clean(uint count)
        {
            
            IGuildUser User = Context.User as IGuildUser;
            EmbedBuilder ResponseEmbed;
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            if (!User.RoleIds.Contains(GuildData.ModeratorRoleID) && !User.GuildPermissions.Administrator && User.Id != Context.Guild.OwnerId)
            {
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildAccounts.GetAccount(Context.Guild), "DENIED"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
            else
            {
                await CleanAsync(count, Context);
            }
        }

        // Remove the description and pronouns a user's profile.

        [Command("clearprofile"), Alias("cp"), Summary("Supprime la description et les pronoms d'un profil. — Delete description and pronouns from a profile."), Remarks("mod")]
        public async Task ClearProfile(IGuildUser member, [Remainder]string reason)
        {
            
            IGuildUser User = Context.User as IGuildUser;
            EmbedBuilder ResponseEmbed;
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            if (!User.RoleIds.Contains(GuildData.ModeratorRoleID) || !User.GuildPermissions.Administrator || User.Id != Context.Guild.OwnerId)
            {
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildAccounts.GetAccount(Context.Guild), "DENIED"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
            else if (member.RoleIds.Contains(GuildData.ModeratorRoleID))
            {
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildAccounts.GetAccount(Context.Guild), "COMMANDONMOD"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
            else
            {
                await ClearProfileAsync(member, reason, Context);
            }
        }

        // Private function to ban someone, used in Ban and Warn commands.

        private async Task BanAsync(IGuildUser member, string reason, SocketCommandContext Context)
        {

            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            IMessageChannel LogChannel = Context.Guild.GetTextChannel(GuildData.LogChannelID);

            EmbedAuthorBuilder BanAuthor = UtilitiesService.CreateAuthorEmbed($"{UtilitiesService.GetAlert(GuildData, "BANCARDTITLE")} — {member.ToString()}", member.GetAvatarUrl());

            Dictionary<object, object> FieldsData = new Dictionary<object, object>()
            {
                { UtilitiesService.GetAlert(GuildData,"CARDUSERFIELD"), member.Mention },
                { UtilitiesService.GetAlert(GuildData, "CARDMODFIELD"), Context.User.Mention },
                { UtilitiesService.GetAlert(GuildData, "CARDREASONFIELD"), reason}
            };

            EmbedBuilder BanEmbed = UtilitiesService.CreateEmbed(BanAuthor, Color.Red, UtilitiesService.CreateListFields(FieldsData, true));

            EmbedBuilder Embed = UtilitiesService.CreateEmbed(Color.DarkRed, UtilitiesService.GetAlert(GuildData, "ANNONCEBAN", member.Mention.ToString(), reason));
            await ReplyAsync(embed: Embed.Build());
            await LogChannel.SendMessageAsync(embed: BanEmbed.Build());
            await Context.Guild.AddBanAsync(member, 0, reason);
        }

        // Private function to unban someone.
        private async Task UnbanAsync(IUser user, SocketCommandContext Context)
        {
            
            await Context.Guild.RemoveBanAsync(user);

            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            IMessageChannel LogChannel = Context.Guild.GetTextChannel(GuildData.LogChannelID);

            EmbedAuthorBuilder UnbanAuthor = UtilitiesService.CreateAuthorEmbed($"{UtilitiesService.GetAlert(GuildData, "UNBANCARDTITLE")} — {user.ToString()}", user.GetAvatarUrl());
            Dictionary<object, object> FieldsData = new Dictionary<object, object>()
            {
                { UtilitiesService.GetAlert(GuildData, "CARDUSERFIELD"), user.Mention},
                { UtilitiesService.GetAlert(GuildData, "CARDMODFIELD"), Context.User.Mention }
            };

            EmbedBuilder UnbanEmbed = UtilitiesService.CreateEmbed(UnbanAuthor, Color.Magenta, UtilitiesService.CreateListFields(FieldsData, true));

            EmbedBuilder AnnonceEmbed = UtilitiesService.CreateEmbed(new Color(0, 0, 0), UtilitiesService.GetAlert(GuildData, "ANNONCEUNBAN", user.Mention.ToString()));
            await ReplyAsync(embed: AnnonceEmbed.Build());
            await LogChannel.SendMessageAsync(embed: UnbanEmbed.Build());
        }

        // Private function to Kick someone.
        private async Task KickAsync(IGuildUser member, string reason, SocketCommandContext Context)
        {
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);

            IMessageChannel LogChannel = Context.Guild.GetTextChannel(GuildData.LogChannelID);
            EmbedAuthorBuilder KickAuthor = UtilitiesService.CreateAuthorEmbed($"{UtilitiesService.GetAlert(GuildData, "KICKCARDTITLE")} — {member.ToString()}", member.GetAvatarUrl());

            Dictionary<object, object> FieldsData = new Dictionary<object, object>()
            {
                { UtilitiesService.GetAlert(GuildData, "CARDUSERFIELD"), member.Mention },
                { UtilitiesService.GetAlert(GuildData, "CARDMODFIELD"), Context.User.Mention },
                { UtilitiesService.GetAlert(GuildData, "CARDREASONFIELD"), reason}
            };

            EmbedBuilder KickEmbed = UtilitiesService.CreateEmbed(KickAuthor, Color.Red, UtilitiesService.CreateListFields(FieldsData, true));

            EmbedBuilder AnnonceEmbed = UtilitiesService.CreateEmbed(new Color(0, 0, 0), UtilitiesService.GetAlert(GuildData, "ANNONCEKICK", member.Mention.ToString(), reason));
            await ReplyAsync(embed: AnnonceEmbed.Build());
            await LogChannel.SendMessageAsync(embed: KickEmbed.Build());
            await member.KickAsync(reason);
        }

        // Private function to mute someone.
        private async Task MuteAsync(IGuildUser member, string reason, SocketCommandContext Context)
        {
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            await member.AddRoleAsync(Context.Guild.GetRole(GuildData.MutedRoleID));

            IMessageChannel LogChannel = Context.Guild.GetTextChannel(GuildData.LogChannelID);

            EmbedAuthorBuilder MuteAuthor = UtilitiesService.CreateAuthorEmbed($"{UtilitiesService.GetAlert(GuildData, "MUTECARDTITLE")} — {member.ToString()}", member.GetAvatarUrl());

            Dictionary<object, object> FieldsData = new Dictionary<object, object>()
            {
                { UtilitiesService.GetAlert(GuildData, "CARDUSERFIELD"), member.Mention },
                { UtilitiesService.GetAlert(GuildData, "CARDMODFIELD"), Context.User.Mention },
                { UtilitiesService.GetAlert(GuildData, "CARDREASONFIELD"), reason }
            };

            EmbedBuilder MuteEmbed = UtilitiesService.CreateEmbed(MuteAuthor, Color.Orange, UtilitiesService.CreateListFields(FieldsData, true));

            EmbedBuilder AnnonceEmbed = UtilitiesService.CreateEmbed(Color.DarkRed, UtilitiesService.GetAlert(GuildData, "ANNONCEMUTE", member.Mention, reason));
            await ReplyAsync(embed: AnnonceEmbed.Build());
            await LogChannel.SendMessageAsync(embed: MuteEmbed.Build());
        }

        // Private function to unmute someone.
        private async Task UnmuteAsync(IGuildUser member, SocketCommandContext Context)
        {
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            await member.RemoveRoleAsync(Context.Guild.GetRole(GuildData.MutedRoleID));

            IMessageChannel LogChannel = Context.Guild.GetTextChannel(GuildData.LogChannelID);

            EmbedAuthorBuilder UnmuteAuthor = UtilitiesService.CreateAuthorEmbed($"{UtilitiesService.GetAlert(GuildData, "UNMUTECARDTITLE")} — {member.ToString()}", member.GetAvatarUrl());

            Dictionary<object, object> FieldsData = new Dictionary<object, object>()
            {
                { UtilitiesService.GetAlert(GuildData,"CARDUSERFIELD"), member.Mention },
                { UtilitiesService.GetAlert(GuildData, "CARDMODFIELD"), Context.User.Mention }
            };

            EmbedBuilder UnmuteEmbed = UtilitiesService.CreateEmbed(UnmuteAuthor, Color.Green, UtilitiesService.CreateListFields(FieldsData, true));

            EmbedBuilder AnnonceEmbed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildData, "ANNONCEUNMUTE", member.Mention));
            await ReplyAsync(embed: AnnonceEmbed.Build());
            await LogChannel.SendMessageAsync(embed: UnmuteEmbed.Build());
        }

        // Private function to warn someone.
        private async Task WarnAsync(IGuildUser member, string reason, SocketCommandContext Context)
        {
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);

            UserAccount UserData = UserAccounts.GetAccount((SocketUser)member, Context.Guild.Id);
            UserData.NbWarnings++;
            UserData.Warns.Add(DateTime.UtcNow.ToUniversalTime().ToString(), reason);
            UserAccounts.SaveAccounts(Context.Guild.Id);

            IMessageChannel LogChannel = Context.Guild.GetTextChannel(GuildData.LogChannelID);

            EmbedAuthorBuilder WarnAuthor = UtilitiesService.CreateAuthorEmbed($"{UtilitiesService.GetAlert(GuildData, "WARNCARDTITLE")} — {member.ToString()}", member.GetAvatarUrl());

            Dictionary<object, object> FieldsData = new Dictionary<object, object>()
                {
                    { UtilitiesService.GetAlert(GuildData,"CARDUSERFIELD"), member.Mention },
                    { UtilitiesService.GetAlert(GuildData, "CARDMODFIELD"), Context.User.Mention },
                    { UtilitiesService.GetAlert(GuildData, "CARDREASONFIELD"), reason },
                    { UtilitiesService.GetAlert(GuildData, "WARNCARDCOUNTFIELD"), $"{UserData.NbWarnings}/{GuildData.WarningsThreshold}"}
                };

            EmbedBuilder WarnEmbed = UtilitiesService.CreateEmbed(WarnAuthor, Color.Gold, UtilitiesService.CreateListFields(FieldsData, true));

            EmbedBuilder AnnonceEmbed = UtilitiesService.CreateEmbed(Color.DarkRed, UtilitiesService.GetAlert(GuildData, "ANNONCEWARN", member.Mention, reason));
            await ReplyAsync(embed: AnnonceEmbed.Build());
            await LogChannel.SendMessageAsync(embed: WarnEmbed.Build());

            if (UserData.NbWarnings >= GuildData.WarningsThreshold) await BanAsync(member, "Trop d'avertissements", Context);
        }

        // Private function to unwarn someone.
        private async Task UnwarnAsync(IGuildUser member, SocketCommandContext Context)
        {
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            UserAccount user = UserAccounts.GetAccount((SocketUser)member, Context.Guild.Id);
            user.Warns.Clear();
            user.NbWarnings = 0;
            UserAccounts.SaveAccounts(Context.Guild.Id);

            IMessageChannel LogChannel = Context.Guild.GetTextChannel(GuildData.LogChannelID);

            EmbedAuthorBuilder UnwarnAuthor = UtilitiesService.CreateAuthorEmbed(UtilitiesService.GetAlert(GuildData, "UNWARNCARDTITLE", member.ToString()), member.GetAvatarUrl());

            Dictionary<object, object> FieldsData = new Dictionary<object, object>()
            {
                { UtilitiesService.GetAlert(GuildData,"CARDUSERFIELD"), member.Mention},
                { UtilitiesService.GetAlert(GuildData, "CARDMODFIELD"), Context.User.Mention }
            };

            EmbedBuilder UnwarnEmbed = UtilitiesService.CreateEmbed(UnwarnAuthor, Color.Green, UtilitiesService.CreateListFields(FieldsData));

            EmbedBuilder AnnonceEmbed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildData, "RETRAITWARNS", member.Mention));
            await ReplyAsync(embed: AnnonceEmbed.Build());
            await LogChannel.SendMessageAsync(embed: UnwarnEmbed.Build());
        }

        // Private function to clean messages.
        private async Task CleanAsync(uint count, SocketCommandContext Context)
        {
            IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync((int)count).FlattenAsync();
            await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);

            EmbedBuilder Embed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildAccounts.GetAccount(Context.Guild), "CLEANMESSAGES", messages.Count()));
            await ReplyAndDeleteAsync("", embed: Embed.Build());
        }

        // Private function to clear a user's profile.
        private async Task ClearProfileAsync(IGuildUser member, string reason, SocketCommandContext Context)
        {
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            UserAccount UserData = UserAccounts.GetAccount(member as SocketUser, GuildData.ID);
            UserData.Description = "***";
            UserData.Pronouns = "***";
            IMessageChannel LogChannel = Context.Guild.GetTextChannel(GuildData.LogChannelID);

            EmbedAuthorBuilder ClearProfileAuthor = UtilitiesService.CreateAuthorEmbed(UtilitiesService.GetAlert(GuildData, "CLEARPROFILECARDTITLE", member.ToString()), member.GetAvatarUrl());

            Dictionary<object, object> FieldsData = new Dictionary<object, object>()
            {
                { UtilitiesService.GetAlert(GuildData,"CARDUSERFIELD"), member.Mention},
                { UtilitiesService.GetAlert(GuildData, "CARDMODFIELD"), Context.User.Mention },
                { UtilitiesService.GetAlert(GuildData, "CARDREASONFIELD"), reason }
            };

            EmbedBuilder ClearProfileEmbed = UtilitiesService.CreateEmbed(ClearProfileAuthor, Color.Green, UtilitiesService.CreateListFields(FieldsData));

            EmbedBuilder Embed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildAccounts.GetAccount(Context.Guild), "CLEARPROFILE", member.Mention, reason));
            await ReplyAndDeleteAsync("", embed: Embed.Build());
            await LogChannel.SendMessageAsync(embed: ClearProfileEmbed.Build());
        }
    }
}
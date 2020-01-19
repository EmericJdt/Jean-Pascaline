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

    [Group("mod")]
    [Alias("moderation", "m")]    
    public class Moderation : InteractiveBase<SocketCommandContext>
    {

        // Ban a user from the Guild, calls another method to do the work.

        [Command("ban"), Summary("Bannir un utilisateur du serveur. (Un message est envoyé à l'utilisateur.) — Bans a member from the guild. (A DM is sent to the member.)"), Remarks("Moderation")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Ban(IGuildUser member, [Remainder]string raison = "Non spécifiée.")
        {
            await Context.Message.DeleteAsync();
            await BanAsync(member, raison, Context);
        }

        // Unban a user from the Guild

        [Command("unban"), Summary("Révoque le bannissement d'un utilisateur — Unbans a member from the guild."), Remarks("Moderation")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Unban(IUser user)
        {
            await Context.Message.DeleteAsync();
            await Context.Guild.RemoveBanAsync(user);
            
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            IMessageChannel LogChannel = Context.Guild.GetTextChannel(GuildData.LogChannelID);

            EmbedAuthorBuilder UnbanAuthor = Utilities.CreateAuthorEmbed($"{Utilities.GetAlert(GuildData, "UNBANCARDTITLE")} — {user.ToString()}", user.GetAvatarUrl());
            Dictionary<object, object> FieldsData = new Dictionary<object, object>()
            {
                { Utilities.GetAlert(GuildData, "CARDUSERFIELD"), user.Mention},
                { Utilities.GetAlert(GuildData, "CARDMODFIELD"), Context.User.Mention }
            };

            EmbedBuilder UnbanEmbed = Utilities.CreateEmbed(UnbanAuthor, Color.Magenta, Utilities.CreateListFields(FieldsData, true));

            await ReplyAsync(Utilities.GetAlert(GuildData, "ANNONCEUNBAN", user.Mention.ToString()));
            await LogChannel.SendMessageAsync(embed: UnbanEmbed.Build());
        }

        // Expells a member from the Guild, they can come back with an invitation.

        [Command("kick"), Summary("Expulse un utilisateur du serveur (Un message est envoyé à l'utilisateur) — Kicks a member from the guild. (A DM is sent to the member.)"), Remarks("Moderation")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task Kick(IGuildUser member, [Remainder]string reason = "Non spécifiée.")
        {
            await Context.Message.DeleteAsync();
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);

            if (member.RoleIds.Contains(GuildData.ModeratorRoleID))
            {
                await ReplyAsync(Utilities.GetAlert(GuildAccounts.GetAccount(Context.Guild), "UNABLE"));
                return;
            }

            else 
            {
                IMessageChannel LogChannel = Context.Guild.GetTextChannel(GuildData.LogChannelID);
                EmbedAuthorBuilder KickAuthor = Utilities.CreateAuthorEmbed($"{Utilities.GetAlert(GuildData, "KICKCARDTITLE")} — {member.ToString()}", member.GetAvatarUrl());
                
                Dictionary<object, object> FieldsData = new Dictionary<object, object>()
                {
                    { Utilities.GetAlert(GuildData, "CARDUSERFIELD"), member.Mention },
                    { Utilities.GetAlert(GuildData, "CARDMODFIELD"), Context.User.Mention },
                    { Utilities.GetAlert(GuildData, "CARDREASONFIELD"), reason}
                };
                
                EmbedBuilder KickEmbed = Utilities.CreateEmbed(KickAuthor, Color.Red, Utilities.CreateListFields(FieldsData, true));

                await ReplyAsync(Utilities.GetAlert(GuildData, "ANNONCEKICK", member.Mention.ToString(), reason));
                await LogChannel.SendMessageAsync(embed: KickEmbed.Build());
                await member.KickAsync(reason);
            }
        }

        // Add muted role to a specific member of the guild, preventing them to talk in the chat.

        [Command("mute"), Summary("Rend muet un utilisateur. — Prevents a member from sending messages."), Remarks("Moderation")]
        [RequireBotPermission(GuildPermission.MuteMembers)]
        public async Task Mute(IGuildUser member, [Remainder]string reason = "Non spécifiée.")
        {
            await Context.Message.DeleteAsync();
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);

            if (member.RoleIds.Contains(GuildData.ModeratorRoleID))
            {
                await ReplyAsync(Utilities.GetAlert(GuildAccounts.GetAccount(Context.Guild), "UNABLE"));
                return;
            }

            else
            {
                await member.AddRoleAsync(Context.Guild.Roles.FirstOrDefault(x => x.Name == "Muted"));

                IMessageChannel LogChannel = Context.Guild.GetTextChannel(GuildData.LogChannelID);

                EmbedAuthorBuilder MuteAuthor = Utilities.CreateAuthorEmbed($"{Utilities.GetAlert(GuildData, "MUTECARDTITLE")} — {member.ToString()}", member.GetAvatarUrl());

                Dictionary<object, object> FieldsData = new Dictionary<object, object>() 
                {
                    { Utilities.GetAlert(GuildData, "CARDUSERFIELD"), member.Mention },
                    { Utilities.GetAlert(GuildData, "CARDMODFIELD"), Context.User.Mention },
                    { Utilities.GetAlert(GuildData, "CARDREASONFIELD"), reason }
                };

                EmbedBuilder MuteEmbed = Utilities.CreateEmbed(MuteAuthor, Color.Orange, Utilities.CreateListFields(FieldsData, true));

                await ReplyAsync(Utilities.GetAlert(GuildData, "ANNONCEMUTE", member.Mention, reason));
                await LogChannel.SendMessageAsync(embed: MuteEmbed.Build());
            }
        }

        // Removes muted role to a specific member of the guild

        [Command("unmute"), Summary("Rend la parole à un utilisateur — Give back the permission to send message to a member"), Remarks("Moderation")]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        [RequireBotPermission(GuildPermission.MuteMembers)]
        public async Task Unmute(IGuildUser member)
        {
            await Context.Message.DeleteAsync();

            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            if (member.RoleIds.Contains(GuildData.ModeratorRoleID))
            {
                await ReplyAsync(Utilities.GetAlert(GuildAccounts.GetAccount(Context.Guild), "UNABLE"));
                return;
            }

            else
            {
                await member.RemoveRoleAsync(Context.Guild.Roles.FirstOrDefault(x => x.Name == "Muted"));

                IMessageChannel LogChannel = Context.Guild.GetTextChannel(GuildData.LogChannelID);

                EmbedAuthorBuilder UnmuteAuthor = Utilities.CreateAuthorEmbed($"{Utilities.GetAlert(GuildData, "UNMUTECARDTITLE")} — {member.ToString()}", member.GetAvatarUrl());

                Dictionary<object, object> FieldsData = new Dictionary<object, object>()
                {
                    { Utilities.GetAlert(GuildData,"CARDUSERFIELD"), member.Mention },
                    { Utilities.GetAlert(GuildData, "CARDMODFIELD"), Context.User.Mention }
                };

                EmbedBuilder UnmuteEmbed = Utilities.CreateEmbed(UnmuteAuthor, Color.Green, Utilities.CreateListFields(FieldsData, true));

                await ReplyAsync(Utilities.GetAlert(GuildData, "ANNONCEUNMUTE", member.Mention));
                await LogChannel.SendMessageAsync(embed: UnmuteEmbed.Build());
            }
        }

        // Warn a user, add a warn to UserAccount Data

        [Command("warn"), Summary("Avertir un utilisateur — Warns a member."), Remarks("Moderation")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task Warn(IGuildUser member, [Remainder]string reason = "Non spécifiée.")
        {
            await Context.Message.DeleteAsync();
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);

            if (member.RoleIds.Contains(GuildData.ModeratorRoleID))
            {
                await ReplyAsync(Utilities.GetAlert(GuildAccounts.GetAccount(Context.Guild), "UNABLE"));
                return;
            }

            else
            {
                UserAccount UserData = UserAccounts.GetAccount((SocketUser)member, Context.Guild.Id);
                UserData.NbWarnings++;
                UserData.Warns.Add(DateTime.UtcNow.ToUniversalTime().ToString(), reason);
                UserAccounts.SaveAccounts(Context.Guild.Id);

                IMessageChannel LogChannel = Context.Guild.GetTextChannel(GuildData.LogChannelID);

                EmbedAuthorBuilder WarnAuthor = Utilities.CreateAuthorEmbed($"{Utilities.GetAlert(GuildData, "WARNCARDTITLE")} — {member.ToString()}", member.GetAvatarUrl());

                Dictionary<object, object> FieldsData = new Dictionary<object, object>()
                {
                    { Utilities.GetAlert(GuildData,"CARDUSERFIELD"), member.Mention },
                    { Utilities.GetAlert(GuildData, "CARDMODFIELD"), Context.User.Mention },
                    { Utilities.GetAlert(GuildData, "CARDREASONFIELD"), reason },
                    { Utilities.GetAlert(GuildData, "WARNCARDCOUNTFIELD"), $"{UserData.NbWarnings}/{GuildData.WarningsThreshold}"}
                };

                EmbedBuilder WarnEmbed = Utilities.CreateEmbed(WarnAuthor, Color.Gold, Utilities.CreateListFields(FieldsData, true));

                await ReplyAsync(Utilities.GetAlert(GuildData, "ANNONCEWARN", member.Mention, reason));
                await LogChannel.SendMessageAsync(embed: WarnEmbed.Build());

                if (UserData.NbWarnings >= GuildData.WarningsThreshold) await BanAsync(member, "Trop d'avertissements", Context);
            }
        }

        // Removes warns from users

        [Command("unwarn"), Summary("Retire les avertissements d'un utilisateur. — Retrieve warnings of a user."), Remarks("Moderation")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task Unwarn(IGuildUser member)
        {
            await Context.Message.DeleteAsync();
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);

            if (member.RoleIds.Contains(GuildData.ModeratorRoleID))
            {
                await ReplyAsync(Utilities.GetAlert(GuildAccounts.GetAccount(Context.Guild), "UNABLE"));
                return;
            }
            else
            {
                UserAccount user = UserAccounts.GetAccount((SocketUser)member, Context.Guild.Id);
                user.NbWarnings = 0;
                user.Warns.Clear();
                UserAccounts.SaveAccounts(Context.Guild.Id);

                IMessageChannel LogChannel = Context.Guild.GetTextChannel(GuildData.LogChannelID);

                EmbedAuthorBuilder UnwarnAuthor = Utilities.CreateAuthorEmbed(Utilities.GetAlert(GuildData, "UNWARNCARDTITLE", member.ToString()), member.GetAvatarUrl());

                Dictionary<object, object> FieldsData = new Dictionary<object, object>()
                {
                    { Utilities.GetAlert(GuildData,"CARDUSERFIELD"), member.Mention},
                    { Utilities.GetAlert(GuildData, "CARDMODFIELD"), Context.User.Mention }
                };

                EmbedBuilder UnwarnEmbed = Utilities.CreateEmbed(UnwarnAuthor, Color.Green, Utilities.CreateListFields(FieldsData));


                await ReplyAsync(Utilities.GetAlert(GuildData, "RETRAITWARNS", member.Mention));
                await LogChannel.SendMessageAsync(embed: UnwarnEmbed.Build());
            }
        }

        // Cleans a certain amount of messages in the Context Channel.
        
        [Command("clean"), Summary("Supprime le nombre de messages d'un salon. — Delete an defined number of messages in a specific channel."), Remarks("Moderation")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task Clean(uint count)
        {
            await Context.Message.DeleteAsync();
            IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync((int)count).FlattenAsync();
            await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);
            await ReplyAndDeleteAsync(Utilities.GetAlert(GuildAccounts.GetAccount(Context.Guild), "CLEANMESSAGES", messages.Count()));
        }

        [Command("clearprofile")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task RemoveDescription(IGuildUser User)
        {
            await Context.Message.DeleteAsync();
            UserAccount UserData = UserAccounts.GetAccount(User as SocketUser, Context.Guild.Id);
            UserData.Description = "***";
            UserData.Pronouns = "***";
            await ReplyAndDeleteAsync(Utilities.GetAlert(GuildAccounts.GetAccount(Context.Guild), "", User.Mention));
        }

        // Private function to ban someone, used in Ban and Warn commands.

        private async Task BanAsync(IGuildUser member, string reason, SocketCommandContext Context)
        {

            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            if (member.RoleIds.Contains(GuildData.ModeratorRoleID))
            {
                await ReplyAsync(Utilities.GetAlert(GuildAccounts.GetAccount(Context.Guild), "UNABLE"));
                return;
            }

            else
            {
                IMessageChannel LogChannel = Context.Guild.GetTextChannel(GuildData.LogChannelID);

                EmbedAuthorBuilder BanAuthor = Utilities.CreateAuthorEmbed($"{Utilities.GetAlert(GuildData, "BANCARDTITLE")} — {member.ToString()}", member.GetAvatarUrl());

                Dictionary<object, object> FieldsData = new Dictionary<object, object>()
                {
                    { Utilities.GetAlert(GuildData,"CARDUSERFIELD"), member.Mention },
                    { Utilities.GetAlert(GuildData, "CARDMODFIELD"), Context.User.Mention },
                    { Utilities.GetAlert(GuildData, "CARDREASONFIELD"), reason}
                };

                EmbedBuilder BanEmbed = Utilities.CreateEmbed(BanAuthor, Color.Red, Utilities.CreateListFields(FieldsData, true));

                await ReplyAsync(Utilities.GetAlert(GuildData, "ANNONCEBAN", member.Mention.ToString(), reason));
                await LogChannel.SendMessageAsync(embed: BanEmbed.Build());
                await Context.Guild.AddBanAsync(member, 0, reason);
            }
        }
    }
}
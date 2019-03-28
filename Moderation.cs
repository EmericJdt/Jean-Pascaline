using System;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using JeanPascaline.Core.AccountSystem;
using JeanPascaline.Core.LevelSystem;

namespace JeanPascaline
{

    [Group("moderation")]
    [Alias("mod", "m")]
    public class Moderation : ModuleBase
    {

        [Command("ban"), Summary("Bannir un utilisateur")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Ban(IGuildUser target, string raison = "Non spécifiée.")
        {

            await Context.Message.DeleteAsync();

            if (target.GuildPermissions.Has(GuildPermission.BanMembers))
            {
                await ReplyAsync(Utilities.GetAlert("UNSUCESSFUL_MOD"));
                return;
            }

            else
            {
                await Context.Guild.AddBanAsync(target, 0, raison);

                IMessageChannel Log = (IMessageChannel)await Context.Guild.GetChannelAsync(434680967584808971);
                var Embed = new EmbedBuilder();
                var Embed1 = new EmbedAuthorBuilder();
                Embed.WithColor(Color.Red);
                Embed1.Name = $"[BANNISSEMENT] — {target.ToString()}";
                Embed1.IconUrl = target.GetAvatarUrl();
                Embed.WithAuthor(Embed1);
                Embed.AddField("Utilisateur :", target.Mention);
                Embed.AddField("Modérateur", Context.User.Mention);
                Embed.AddField("Raison :", raison);

                var DM = await target.GetOrCreateDMChannelAsync();

                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("ANNONCE_BAN", target.Mention.ToString(), raison));
                await DM.SendMessageAsync(Utilities.GetFormattedAlert("DM_BAN", Context.Guild.Name, raison));
            }

        }

        [Command("unban"), Summary("Révoquer le bannissement d'un utilisateur")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Unban(IGuildUser target)
        {
            await Context.Message.DeleteAsync();

            if (target.GuildPermissions.Has(GuildPermission.BanMembers))
            {
                await ReplyAsync(Utilities.GetAlert("UNSUCESSFUL_MOD"));
                return;
            }

            else
            {
                await Context.Guild.RemoveBanAsync(target);

                IMessageChannel Log = (IMessageChannel)await Context.Guild.GetChannelAsync(434680967584808971);
                var Embed = new EmbedBuilder();
                var Embed1 = new EmbedAuthorBuilder();
                Embed.WithColor(Color.Red);
                Embed1.Name = $"[REVOCATION DU BANNISSEMENT] — {target.ToString()}";
                Embed1.IconUrl = target.GetAvatarUrl();
                Embed.WithAuthor(Embed1);
                Embed.AddField("Utilisateur :", target.Mention);
                Embed.AddField("Modérateur", Context.User.Mention);

                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("ANNONCE_UBNAN", target.Mention.ToString()));
                await Log.SendMessageAsync("", embed: Embed);
            }

        }

        [Command("kick"), Summary("Expulser un utilisateur")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task Kick(IGuildUser target, string raison = "Non spécifiée.")
        {

            await Context.Message.DeleteAsync();

            if (target.GuildPermissions.Has(GuildPermission.KickMembers))
            {
                await ReplyAsync(Utilities.GetAlert("UNSUCESSFUL_MOD"));
                return;
            }

            else
            {

                await target.KickAsync(raison);

                IMessageChannel Log = (IMessageChannel)await Context.Guild.GetChannelAsync(434680967584808971);
                var Embed = new EmbedBuilder();
                var Embed1 = new EmbedAuthorBuilder();
                Embed.WithColor(Color.Red);
                Embed1.Name = $"[EXPULSION] — {target.ToString()}";
                Embed1.IconUrl = target.GetAvatarUrl();
                Embed.WithAuthor(Embed1);
                Embed.AddField("Utilisateur :", target.Mention);
                Embed.AddField("Modérateur", Context.User.Mention);
                Embed.AddField("Raison :", raison);

                var DM = await target.GetOrCreateDMChannelAsync();

                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("ANNONCE_KICK", target.Mention.ToString(), raison));
                await DM.SendMessageAsync(Utilities.GetFormattedAlert("DM_KICK", Context.Guild.Name, raison));
                await Log.SendMessageAsync("", embed: Embed);
            }

        }

        [Command("mute"), Summary("Rendre muet un utilisateur")]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        [RequireBotPermission(GuildPermission.MuteMembers)]
        public async Task Mute(IGuildUser target, string raison = "Non spécifiée.")
        {
            await Context.Message.DeleteAsync();

            if (target.GuildPermissions.Has(GuildPermission.MuteMembers))
            {
                await ReplyAsync(Utilities.GetAlert("UNSUCESSFUL_MOD"));
                return;
            }

            else
            {

                await target.AddRoleAsync(Context.Guild.Roles.FirstOrDefault(x => x.Name == "Muted"));

                IMessageChannel Log = (IMessageChannel)await Context.Guild.GetChannelAsync(434680967584808971);
                var Embed = new EmbedBuilder();
                var Embed1 = new EmbedAuthorBuilder();
                Embed.WithColor(Color.Orange);
                Embed1.Name = $"[MUTE] — {target.ToString()}";
                Embed1.IconUrl = target.GetAvatarUrl();
                Embed.WithAuthor(Embed1);
                Embed.AddField("Utilisateur :", target.Mention);
                Embed.AddField("Modérateur", Context.User.Mention);
                Embed.AddField("Raison :", raison);

                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("ANNONCE_MUTE", target.Mention, raison));
                await Log.SendMessageAsync("", embed: Embed);
            }

        }

        [Command("unmute"), Summary("Rendre la parole à un utilisateur")]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        [RequireBotPermission(GuildPermission.MuteMembers)]
        public async Task Unmute(IGuildUser target)
        {
            await Context.Message.DeleteAsync();

            if (target.GuildPermissions.Has(GuildPermission.MuteMembers))
            {
                await ReplyAsync(Utilities.GetAlert("UNSUCESSFUL_MOD"));
                return;
            }

            else
            {
                await target.RemoveRoleAsync(Context.Guild.Roles.FirstOrDefault(x => x.Name == "Muted"));

                IMessageChannel Log = (IMessageChannel)await Context.Guild.GetChannelAsync(434680967584808971);
                var Embed = new EmbedBuilder();
                var Embed1 = new EmbedAuthorBuilder();
                Embed.WithColor(Color.Green);
                Embed1.Name = $"[UNMUTE] — {target.ToString()}";
                Embed1.IconUrl = target.GetAvatarUrl();
                Embed.WithAuthor(Embed1);
                Embed.AddField("Utilisateur :", target.Mention);
                Embed.AddField("Modérateur", Context.User.Mention);

                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("ANNONCE_UNMUTE", target.Mention));
                await Log.SendMessageAsync("", embed: Embed);
            }
        }

        [Command("warn"), Summary("Avertir un utilisateur")]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        [RequireBotPermission(GuildPermission.MuteMembers)]
        public async Task Warn(IGuildUser target, string raison = "Non spécifiée.")
        {
            await Context.Message.DeleteAsync();

            if (target.GuildPermissions.Has(GuildPermission.MuteMembers))
            {
                await ReplyAsync(Utilities.GetAlert("UNSUCESSFUL_MOD"));
                return;
            }

            else
            {
                var user = UserAccounts.GetAccount((SocketUser)target);
                user.NbWarnings++;
                UserAccounts.SaveAccounts();

                IMessageChannel Log = (IMessageChannel) await Context.Guild.GetChannelAsync(434680967584808971);
                var Embed = new EmbedBuilder();
                var Embed1 = new EmbedAuthorBuilder();
                Embed.WithColor(Color.Gold);
                Embed1.Name = $"[AVERTISSEMENT] — {target.ToString()}";
                Embed1.IconUrl = target.GetAvatarUrl();
                Embed.WithAuthor(Embed1);
                Embed.AddField("Utilisateur :", target.Mention);
                Embed.AddInlineField("Modérateur", Context.User.Mention);
                Embed.AddField("Nombre de warns", $"{user.NbWarnings}/5");
                Embed.AddInlineField("Raison", raison);

                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("ANNONCE_WARN", target.Mention, raison));
                await Log.SendMessageAsync("", embed: Embed);

                if (user.NbWarnings >= 5)
                {
                    await target.KickAsync("Trop d'avertissements");
                    Embed.WithColor(Color.Red);
                    Embed1.Name = $"[EXPULSION] — {target.ToString()}";
                    Embed1.IconUrl = target.GetAvatarUrl();
                    Embed.WithAuthor(Embed1);
                    Embed.AddField("Utilisateur :", target.Mention);
                    Embed.AddField("Modérateur", Context.User.Mention);
                    Embed.AddField("Raison :", raison);

                    var DM = await target.GetOrCreateDMChannelAsync();

                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("ANNONCE_KICK", target.Mention.ToString(), raison));
                    await DM.SendMessageAsync(Utilities.GetFormattedAlert("DM_KICK", Context.Guild.Name, raison));
                    await Log.SendMessageAsync("", embed: Embed);
                }
            }
        }

        [Command("clear"), Summary("Retire les avertissements d'un utilisateur")]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        [RequireBotPermission(GuildPermission.MuteMembers)]
        public async Task Clear(IGuildUser target)
        {
            await Context.Message.DeleteAsync();

            if (target.GuildPermissions.Has(GuildPermission.MuteMembers))
            {
                await Context.Channel.SendMessageAsync(Utilities.GetAlert("UNSUCESSFUL_MOD"));
                return;
            }
            else
            {
                var user = UserAccounts.GetAccount((SocketUser)target);
                user.NbWarnings = 0;
                UserAccounts.SaveAccounts();

                IMessageChannel Log = (IMessageChannel)await Context.Guild.GetChannelAsync(434680967584808971);
                var Embed = new EmbedBuilder();
                var Embed1 = new EmbedAuthorBuilder();
                Embed.WithColor(Color.Green);
                Embed1.Name = $"[RETRAIT D'AVERTISSEMENTS] — {target.ToString()}";
                Embed1.IconUrl = target.GetAvatarUrl();
                Embed.WithAuthor(Embed1);
                Embed.AddField("Utilisateur :", target.Mention);
                Embed.AddField("Modérateur", Context.User.Mention);

                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("RETRAIT_WARNS", target.Mention));
                await Log.SendMessageAsync("", embed: Embed);
                
            }
        }
    }
}
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using JeanPascaline.Core.AccountSystem;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JeanPascaline.Services
{
    public class EventService : InteractiveBase<SocketCommandContext>
    {

        // Triggered when the bot joins a new Guild. Create a file for the guild and enter all of the users data in it.

        internal static async Task BotJoinedGuild(SocketGuild Guild)
        {
            if(!File.Exists(@"Ressources\Guilds\" + Guild.Id + ".json"))
            {
                await File.WriteAllTextAsync(@"Ressources\Guilds\" + Guild.Id + ".json", "[]");
            }
            
            if (!Guild.Roles.Contains(Guild.Roles.FirstOrDefault(x => x.Name == "Muted")))
            {
                IRole mutedRole = await Guild.CreateRoleAsync("Muted");
            }

            foreach(IGuildUser user in Guild.Users)
            {
                if (!user.IsBot) {
                    var target = UserAccounts.GetAccount((SocketUser)user, user.Guild.Id);
                    target.Roles = user.RoleIds.ToList();
                    target.Roles.Remove(user.Guild.Id);
                }
            }
            UserAccounts.SaveAccounts(Guild.Id);
            GuildAccount GuildData = GuildAccounts.GetAccount(Guild);
        }

        // Triggered when a user leaves the Guild, Send a message if the option has been enabled.

        internal static async Task UserLeft(SocketGuildUser User)
        {
            if (User.IsBot) return;
            GuildAccount GuildData = GuildAccounts.GetAccount(User.Guild);
            if (GuildData.IsAnnoncementMessagesEnabled.Equals(false)) return;

            SocketTextChannel channel = Program._client.GetChannel(GuildData.AnnoucementChannelID) as SocketTextChannel;
            if (GuildData.IsDevGuild.Equals(true))
            {
                await channel.SendMessageAsync(Utilities.GetAlert(GuildData, "DEVLEAVINGMESSAGE", User.ToString()));
            }
            else await channel.SendMessageAsync(Utilities.GetAlert(GuildData, "LEAVINGMESSAGE", User.ToString()));
        }

        // Triggered when a role is deleted, remove all referencies of this role in UserAccount data.

        internal static Task RoleDeleted(SocketRole arg)
        {
            ulong GuildID = arg.Guild.Id;
            ulong RoleID = arg.Id;

            foreach (SocketUser user in arg.Guild.Users)
            {
                UserAccount UserData = UserAccounts.GetAccount(user, GuildID);
                if(UserData.Roles.Contains(RoleID)) UserData.Roles.Remove(RoleID);
            }
            return Task.CompletedTask;
        }

        // Triggered when a Guild Member has been updated, Clear and readd all their roles in UserAccount Data.
        // Flaw : It triggers when a Nickname has been changed.

        internal static Task GuildMemberUpdated(SocketGuildUser arg1, SocketGuildUser arg2)
        {
            if (arg1.IsBot) return Task.CompletedTask;
            UserAccount UserData = UserAccounts.GetAccount(arg2, arg1.Guild.Id);
            UserData.Roles.Clear();
            UserData.Roles = ((IGuildUser)arg2).RoleIds.ToList();
            UserData.Roles.Remove(arg1.Guild.Id);
            return Task.CompletedTask;
        }

        // Triggred when someone joins the Guild. Send a message if the option has been enabled and add roles if user was already in the guild.

        internal static async Task UserJoin(SocketGuildUser User)
        {
            SocketTextChannel channel = Program._client.GetChannel(GuildAccounts.GetAccount(User.Guild).AnnoucementChannelID) as SocketTextChannel;
            UserAccount NewUser = UserAccounts.GetAccount(User, User.Guild.Id);
            GuildAccount Server = GuildAccounts.GetAccount(User.Guild);

            if (Server.IsDevGuild.Equals(true) && Server.IsAnnoncementMessagesEnabled.Equals(true))
                await channel.SendMessageAsync(Utilities.GetAlert(Server, "DEVWELCOMEMESSAGE", User.Mention));
            else if (Server.IsAnnoncementMessagesEnabled.Equals(true))
                await channel.SendMessageAsync(Utilities.GetAlert(Server, "WELCOMEMESSAGE", User.Mention));

            if (NewUser.Roles.Count > 0)
            {
                foreach (ulong RoleID in NewUser.Roles)
                {
                    await User.AddRoleAsync(User.Guild.GetRole(RoleID));
                }
            }
            else await User.AddRoleAsync(User.Guild.GetRole(Server.DefaultRoleID));
        }
    }
}

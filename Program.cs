using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Audio;
using Discord.Net;


using JeanPascaline.Core.AccountSystem;
using JeanPascaline.Core.LevelSystem;

namespace JeanPascaline
{

    public class Program
    {
        public static CommandService _commands;
        public static DiscordSocketClient _client;
        public IServiceProvider _services;
        
    
        public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();
        public async Task MainAsync()
        {
            if (Config.bot.token == "" || Config.bot.token == null) return;
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });
            _commands = new CommandService();
            _client.Log += Log;

            _services = new ServiceCollection()
                    .BuildServiceProvider();


            Utilities.Utilites();
            await InstallCommands();
            await _client.LoginAsync(TokenType.Bot, Config.bot.token);
            await _client.StartAsync();
            await _client.SetGameAsync("Chercher sa soeur");


            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message);
            return Task.CompletedTask;
        }

        public async Task InstallCommands()
        {
            _client.MessageReceived += HandleCommand;
            _client.UserJoined += AnnounceJoinedUser;
            _client.UserLeft += AnnonceLeftUser;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());

        }

        private async Task AnnonceLeftUser(SocketGuildUser user)
        {
            var channel = _client.GetChannel(250709034108321792) as SocketTextChannel;
            await channel.SendMessageAsync(Utilities.GetFormattedAlert("BYE_USER", user.ToString()));
        }

        private async Task AnnounceJoinedUser(SocketGuildUser user)
        {
            var channel = _client.GetChannel(250709034108321792) as SocketTextChannel;
            IDMChannel DM = await user.GetOrCreateDMChannelAsync();
            
            await channel.SendMessageAsync(Utilities.GetFormattedAlert("WELCOME_USER", user.Mention));
            await DM.SendMessageAsync(Utilities.GetFormattedAlert("WELCOME_DM_USER", user.Guild.Name));
            switch (UserAccounts.GetAccount(user).Level)
            {
                case uint lvl when (lvl >= 8 && lvl < 16):
                    await user.AddRoleAsync(channel.Guild.Roles.FirstOrDefault(x => x.Name == "Théoriste"));
                    await user.AddRoleAsync(channel.Guild.Roles.FirstOrDefault(x => x.Name == "Croyant.e.s du Dimanche"));
                    break;
                case uint lvl when (lvl >= 16 && lvl < 24):
                    await user.AddRoleAsync(channel.Guild.Roles.FirstOrDefault(x => x.Name == "Théoriste Supérieur"));
                    await user.AddRoleAsync(channel.Guild.Roles.FirstOrDefault(x => x.Name == "Croyant.e.s"));
                    break;
                case uint lvl when (lvl >= 24 && lvl < 33):
                    await user.AddRoleAsync(channel.Guild.Roles.FirstOrDefault(x => x.Name == "Théoriste Certifié.e.s"));
                    await user.AddRoleAsync(channel.Guild.Roles.FirstOrDefault(x => x.Name == "Croyant.e.s"));
                    break;
                case uint lvl when (lvl >= 33 && lvl < 44):
                    await user.AddRoleAsync(channel.Guild.Roles.FirstOrDefault(x => x.Name == "Théoricien.ne.s Novice"));
                    await user.AddRoleAsync(channel.Guild.Roles.FirstOrDefault(x => x.Name == "Croyant.e.s Confirmé.e.s"));
                    break;
                case uint lvl when (lvl >= 44 && lvl < 55):
                    await user.AddRoleAsync(channel.Guild.Roles.FirstOrDefault(x => x.Name == "Théoricien.ne.s"));
                    await user.AddRoleAsync(channel.Guild.Roles.FirstOrDefault(x => x.Name == "Croyant.e.s Confirmé.e.s"));
                    break;
                case uint lvl when (lvl >= 55 && lvl < 60):
                    await user.AddRoleAsync(channel.Guild.Roles.FirstOrDefault(x => x.Name == "Théoricien.ne.s Confirmé.e.s"));
                    await user.AddRoleAsync(channel.Guild.Roles.FirstOrDefault(x => x.Name == "Évêque du Jean-Pascalinisime"));
                    break;
                case uint lvl when (lvl >= 60 && lvl < 70):
                    await user.AddRoleAsync(channel.Guild.Roles.FirstOrDefault(x => x.Name == "Théoricien.ne.s du Complot"));
                    await user.AddRoleAsync(channel.Guild.Roles.FirstOrDefault(x => x.Name == "Archévêque du Jean-Pascalinisime"));
                    break;
                case uint lvl when (lvl >= 70):
                    await user.AddRoleAsync(channel.Guild.Roles.FirstOrDefault(x => x.Name == "Théoricien.ne.s Suprême"));
                    await user.AddRoleAsync(channel.Guild.Roles.FirstOrDefault(x => x.Name == "Cardinal du Jean-Pascalinisme"));
                    break;
                case uint lvl when (lvl <= 7):
                    await user.AddRoleAsync(channel.Guild.Roles.FirstOrDefault(x => x.Name == "Théoriste en Carton"));
                    break;
                default:
                    await user.AddRoleAsync(channel.Guild.Roles.FirstOrDefault(x => x.Name == "Théoriste en Carton"));
                    break;
            }
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
            SocketUserMessage Message = messageParam as SocketUserMessage;
            if (Message == null) return;

            var context = new SocketCommandContext(_client, Message);

            if (context.User.IsBot == true || context.User.IsWebhook == true) return;
          
            
            if(UserAccounts.GetAccount(context.User).Level < 70) await Leveling.UserSentMessageAsync((SocketGuildUser)context.User, await context.User.GetOrCreateDMChannelAsync());
            int argPos = 0;
            if ((Message.HasStringPrefix(Config.bot.cmdPrefix, ref argPos)) || (Message.HasMentionPrefix(_client.CurrentUser, ref argPos))) {
                var result = await _commands.ExecuteAsync(context, argPos, _services);

                /*if (!result.IsSuccess)
                    await context.Channel.SendMessageAsync(result.ErrorReason);*/

            }
            else if (Message.Content.Contains("Salut") || Message.Content.Contains("Bonjour") || Message.Content.Contains("Hey"))
            {
                if (DateTime.UtcNow.Hour >= 6 && DateTime.UtcNow.Hour <= 18)
                {
                    await context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("BONJOUR", context.User.Mention));
                }
                else
                {
                    await context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("BONSOIR", context.User.Mention));
                }
            }
        }
    }

    public class InfoModule : ModuleBase<SocketCommandContext>
    {
       /* [Command("salut"), Summary("Dire Bonjour")]
        public async Task Salut()
        {
            if (DateTime.UtcNow.Hour >= 6 && DateTime.UtcNow.Hour <= 18)
            {
                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("BONJOUR", Context.User.Mention)); 
            }
            else
            {
                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("BONSOIR", Context.User.Mention));
            }
        }*/

        [Command("rank"), Summary("Déterminer son niveau")]
        public async Task Rank(IGuildUser target = null)
        {
            await Context.Message.DeleteAsync();
            var DM = await Context.User.GetOrCreateDMChannelAsync();
            SocketUser m_target;
            if (target == null)
            {
                m_target = Context.User;
            }
            else
            {
                m_target = (SocketUser)target;
            }

            UserAccount rank = UserAccounts.GetAccount(m_target);

            var Embed = new EmbedBuilder();
            var Author = new EmbedAuthorBuilder
            {
                Name = $"{Context.Guild.Name} — {m_target.ToString()}",
                IconUrl = Context.Guild.IconUrl
            };
            Embed.WithAuthor(Author);
            Embed.WithColor(Color.Blue);
            Embed.ThumbnailUrl = m_target.GetAvatarUrl();
            Embed.AddField("Niveau actuel :", rank.Level);
            Embed.AddInlineField("Points d'expérience :", rank.XP);
            Embed.AddInlineField("Nombre d'avertissements :", $"{rank.NbWarnings}/5");

            await DM.SendMessageAsync("", embed: Embed);
        }

        [Command("nyah"), Summary("")]
        public async Task Nyah()
        {
            await Context.Channel.SendFileAsync(@"Images/nyah.gif", Context.User.Mention);
        }

        [Command("invite"), Summary("")]
        public async Task Invite()
        {
            await Context.Message.DeleteAsync();

            var DM = await Context.User.GetOrCreateDMChannelAsync();
            await DM.SendMessageAsync(Utilities.GetAlert("DEMANDE_INVITE"));
        }

        [Command("hug"), Summary("*câlin*")]
        public async Task Hug()
        {
            await Context.Channel.SendFileAsync(@"Images/giphy.gif", Context.User.Mention);
        }

        [Command("acces"), Summary("Accéder aux salons déprime")]
        public async Task Acces()
        {
            await Context.Message.DeleteAsync();
            await ((IGuildUser)Context.User).AddRoleAsync(Context.Guild.Roles.FirstOrDefault(x => x.Name == "Accès"));
            var DM = await Context.User.GetOrCreateDMChannelAsync();

            if (!((SocketGuildUser)Context.User).Roles.Contains(((IGuildUser)Context.User).Guild.Roles.FirstOrDefault(x => x.Name == "Accès")))
            {
                await ((IGuildUser)Context.User).AddRoleAsync(Context.Guild.Roles.FirstOrDefault(x => x.Name == "Accès"));
                await DM.SendMessageAsync(Utilities.GetAlert("ACCES_GRANTED"));
            }
            else
            {
                await ((IGuildUser)Context.User).RemoveRoleAsync(Context.Guild.Roles.FirstOrDefault(x => x.Name == "Accès"));
                await DM.SendMessageAsync(Utilities.GetAlert("ACCES_DENIED"));
            }
        }

        [Command("index"), Summary("Index des commandes du serveur")]
        public async Task Index(uint Id = 0)
        {
            await Context.Message.DeleteAsync();
            Random rnd = new Random();

            var DM = await Context.User.GetOrCreateDMChannelAsync();
            if (Id == 0) Id = (uint)rnd.Next(1, 15);
            switch (Id)
            {
                case 15:
                    await DM.SendFileAsync(@"Images/Index.jpg");
                    break;
                default:
                    if (((SocketGuildUser)Context.User).Roles.Contains(Context.Guild.Roles.FirstOrDefault(x => x.Name == "Connards (Modo)"))) await DM.SendMessageAsync($"```apache\n{Utilities.GetAlert("INDEX")}\n\n{Utilities.GetAlert("JPVITCH_INDEX")}\n\n{Utilities.GetAlert("MOD_INDEX")}```");
                    else if (((SocketGuildUser)Context.User).Roles.Contains(Context.Guild.Roles.FirstOrDefault(x => x.Name == "Jean-Pascalinovitch"))) await DM.SendMessageAsync($"```apache\n{Utilities.GetAlert("INDEX")}\n\n{Utilities.GetAlert("JPVITCH_INDEX")}```");
                    else await DM.SendMessageAsync($"```apache\n{Utilities.GetAlert("INDEX")}```");
                    break;
            }
        }
        [Command("yukue"),Summary("Non, pas Yukue mais Yukue.")]
        public async Task Yukue()
        {

            Random rnd = new Random();
            switch(rnd.Next(1, 9))
            {
                case 1:
                    // Dzéta
                    await Context.Channel.SendFileAsync(@"Images/Dzeta.png", " Bah ça se voit non ? Je joue au Scrabble. - Dzéta");
                    break;
                case 2:
                    // Alpha
                    await Context.Channel.SendFileAsync(@"Images/Alpha.png", "Très drôle. - Alpha");
                    break;
                case 3:
                    // Tau
                    await Context.Channel.SendFileAsync(@"Images/Tau.png", "Ah, tu as cru qu'on vivait dans un monde de SF ? - Tau");
                    break;
                case 4:
                    // Oméga
                    await Context.Channel.SendFileAsync(@"Images/Omega.png", "Oh, t'es tombé sur un sacré poisson Omi-chi... - Oméga");
                    break;
                case 5:
                    // Epsilon
                    await Context.Channel.SendFileAsync(@"Images/Epsilon.png", "Je te laisse dix secondes pour décliner ton identité, passé ce délai ce toit risque de changer de couleur. - Epsilon");
                    break;
                case 6:
                    // Sigma
                    await Context.Channel.SendFileAsync(@"Images/Sigma.png", "Chut... Laisse-là moi un moment... - Sigma");
                    break;
                case 7:
                    // Omicron
                    await Context.Channel.SendFileAsync(@"Images/Omicron.png", "Je savais que vous êtiez assez limité chez Azrom, mais de là à être ami avec une épée, franchement. - Omicron");
                    break;
                case 8:
                    // Teta
                    await Context.Channel.SendFileAsync(@"Images/Teta.png", "Soit tu la fermes, soit tu crèves. - Têta");
                    break;
                case 9:
                    // Teta
                    await Context.Channel.SendMessageAsync("Attends. Tu parles de Yukue, Yukue, Yukue ou Yukue ?!");
                    break;
            }
        }
    }
}

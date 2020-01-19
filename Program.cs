using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using JeanPascaline.Core.AccountSystem;
using JeanPascaline.Core.LevelSystem;
using JeanPascaline.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Victoria;

namespace JeanPascaline
{
    public class Program
    {

        public static CommandService _commands;
        public static DiscordSocketClient _client;
        public IServiceProvider _services;

        public static void Main() => new Program().MainAsync().GetAwaiter().GetResult();
        public async Task MainAsync()
        {
            if (Config.bot.Token == null) return;
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });
            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, Config.bot.Token);
            await _client.StartAsync();
            await _client.SetGameAsync("Chercher sa soeur - !index");

            _commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Debug,
                CaseSensitiveCommands = false

            });

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton<InteractiveService>()
                .AddSingleton<LavaConfig>()
                .AddSingleton<LavaNode>()
                .AddSingleton<MusicService>()
                .BuildServiceProvider();

            await _services.GetRequiredService<MusicService>().InitializeAsync();
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            Utilities.InitializeAlerts();
            await InstalEventHandler();
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message);
            return Task.CompletedTask;
        }

        public async Task InstalEventHandler()
        {
            _client.MessageReceived += HandleCommand;
            _client.UserJoined += EventService.UserJoin;
            _client.UserLeft += EventService.UserLeft;
            _client.JoinedGuild += EventService.BotJoinedGuild;
            _client.RoleDeleted += EventService.RoleDeleted;
            _client.GuildMemberUpdated += EventService.GuildMemberUpdated;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
            if (!(messageParam is SocketUserMessage Message)) return;

            SocketCommandContext context = new SocketCommandContext(_client, Message);
            if (context.User.IsBot == true || context.User.IsWebhook == true) return;

            // Prevents a muted user to speak if permissions has not been made by the guild owner.

            if ((context.User as SocketGuildUser).Roles.Contains(context.Guild.Roles.FirstOrDefault(x => x.Name == "Muted")))
            {
                await context.Message.DeleteAsync();
                return;
            }

            // Getting Guild Data for further actions

            GuildAccount GuildData = GuildAccounts.GetAccount(context.Guild);

            // Check if message is a command

            int argPos = 0;
            if (Message.HasStringPrefix("!", ref argPos))
            {
                IResult result = await _commands.ExecuteAsync(context, argPos, _services, MultiMatchHandling.Best);
                if (!result.IsSuccess)
                {
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                }
                return;
            }

            // Trigger the leveling system if message is not a command

            await Leveling.UserSentMessageAsync(context.User, await context.User.GetOrCreateDMChannelAsync(), context);

            // Check for certain message case and provide an answer to them

            switch (context.Message.Content.ToLower())
            {
                case string UserMessage when UserMessage.StartsWith("salut") || UserMessage.StartsWith("hey"):
                    await context.Channel.SendMessageAsync(Utilities.GetAlert(GuildData, "SALUT", context.User.Mention, Emote.Parse("<:AyanoBongoCat:629041865680748544>")));
                    break;

                case string UserMessage when UserMessage.Contains("peeposalute"):
                    await context.Channel.SendMessageAsync($"<:PeepoSalute:613327585203453962> {context.User.Mention}");
                    break;

                default:
                    break;
            }
        }
    }
}
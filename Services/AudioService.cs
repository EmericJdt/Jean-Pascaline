using Discord;
using Discord.WebSocket;
using JeanPascaline.Core.AccountSystem;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Victoria;
using Victoria.EventArgs;

namespace JeanPascaline.Services
{
    public sealed class AudioService
    {
        private readonly LavaNode _lavaNode;
        private readonly LavaConfig _lavaConfig;
        private readonly DiscordSocketClient _client;
        private readonly ILogger _Logger;
        public readonly List<LavaTrack> JPQueue = new List<LavaTrack>();
        public readonly HashSet<ulong> VoteQueue = new HashSet<ulong>();
        public bool repeatState;

        public AudioService(DiscordSocketClient client, LavaNode lavaNode, LavaConfig lavaConfig, ILogger<AudioService> logger)
        {
            _client = client;
            _lavaNode = lavaNode;
            _lavaConfig = lavaConfig;
            _Logger = logger;

            _client.Ready += ClientReadyAsync;
            _client.UserVoiceStateUpdated += BotLeaveChannel;
            _client.LoggedOut += ClientLoggedOutAsync;

            _lavaNode.OnLog += LogAsync;
            _lavaNode.OnTrackEnded += TrackFinished;
            _lavaNode.OnTrackStuck += OnTrackStuck;
            _lavaNode.OnWebSocketClosed += OnWebSocketClosed;
            _lavaNode.OnTrackException += OnTrackException;
        }

        private async Task ClientLoggedOutAsync()
        {
            await _lavaNode.DisconnectAsync();
            await _lavaNode.DisposeAsync();
        }

        public async Task ClientReadyAsync()
        {
            await _lavaNode.ConnectAsync();
            _lavaConfig.LogSeverity = LogSeverity.Debug;
        }

        private async Task TrackFinished(TrackEndedEventArgs arg)
        {
            if (!arg.Reason.ShouldPlayNext())
                return;

            if (!arg.Player.Queue.TryDequeue(out global::Victoria.Interfaces.IQueueable item) || !(item is LavaTrack nextTrack))
            {
                if (repeatState == true)
                {
                    await arg.Player.PlayAsync(arg.Track);
                    return;
                }
                else
                {
                    GuildAccount GuildData = GuildAccounts.GetAccount((SocketGuild)arg.Player.VoiceChannel.Guild);
                    EmbedBuilder Embed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildData, "NOMORETRACKS"));
                    await arg.Player.TextChannel.SendMessageAsync(embed: Embed.Build());
                    JPQueue.Clear();
                    return;
                }
            }
            await arg.Player.PlayAsync(nextTrack);
            JPQueue.Remove(nextTrack);
        }

        private async Task BotLeaveChannel(SocketUser arg1, SocketVoiceState arg2, SocketVoiceState arg3)
        {
            if (arg2.VoiceChannel.Users.Count == 1)
            {
                SocketGuild Guild = arg2.VoiceChannel.Guild;
                LavaPlayer _player = _lavaNode.GetPlayer(Guild);
                EmbedBuilder Embed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildAccounts.GetAccount(Guild), "BOTHASLEAVED", arg2.VoiceChannel.Name));
                await _player.TextChannel.SendMessageAsync(embed: Embed.Build());
                await _player.StopAsync();
                _player.Queue.Clear();
                JPQueue.Clear();
                await _lavaNode.LeaveAsync(arg2.VoiceChannel);
            }
        }

        private Task OnTrackException(TrackExceptionEventArgs arg)
        {
            _Logger.Log(LogLevel.Debug, $"Track exception received for {arg.Track.Title} - \"{arg.ErrorMessage}\".");
            return Task.CompletedTask;
        }

        private Task OnTrackStuck(TrackStuckEventArgs arg)
        {
            _Logger.Log(LogLevel.Debug, $"Track stuck received for {arg.Track.Title}.");
            return Task.CompletedTask;
        }

        private Task OnWebSocketClosed(WebSocketClosedEventArgs arg)
        {
            _Logger.Log(LogLevel.Debug, $"Discord WebSocket connection closed with following reason: {arg.Reason} - \"{arg.Reason}\".");
            return Task.CompletedTask;
        }

        private Task LogAsync(LogMessage logMessage)
        {
            System.Console.WriteLine(logMessage.Message);
            return Task.CompletedTask;
        }
    }
}

using Discord;
using Discord.WebSocket;
using JeanPascaline.Core.AccountSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;

namespace JeanPascaline.Services
{
    public class MusicService
    {
        private readonly LavaNode _lavaNode;
        private readonly DiscordSocketClient _client;

        public MusicService(DiscordSocketClient client, LavaNode lavaNode)
        {
            _client = client;
            _lavaNode = lavaNode;
            _client.Ready += ClientReadyAsync;
            _client.UserVoiceStateUpdated += BotLeaveChannel;
        }

        public Task InitializeAsync()
        {
            _client.Ready += ClientReadyAsync;
            _lavaNode.OnLog += LogAsync;
            _lavaNode.OnTrackEnded += TrackFinished;
            return Task.CompletedTask;
        }

        public async Task ConnectAsync(SocketVoiceChannel voiceChannel, ITextChannel textChannel)
            => await _lavaNode.JoinAsync(voiceChannel, textChannel);

        public async Task LeaveAsync(SocketVoiceChannel voiceChannel)
            => await _lavaNode.LeaveAsync(voiceChannel);

        public async Task<List<LavaTrack>> SearchAsync(string query)
        {
            var results = await _lavaNode.SearchYouTubeAsync(query);
            if (results.LoadType == LoadType.NoMatches || results.LoadType == LoadType.LoadFailed)
            {
                return null;
            }
            else return results.Tracks.Take(5).ToList();
        }

        public async Task<List<LavaTrack>> QueueAsync(IGuild guild)
        {
            var _player = _lavaNode.GetPlayer(guild);
            if (_player == null)
                return null;
            else
            {
                await Task.Delay(0);
                return _player.Queue.Items.ToList();
            }
        }

        public async Task<string> PlayAsync(IGuild guild, LavaTrack track, GuildAccount guildAccount)
        {
            var _player = _lavaNode.GetPlayer(guild);

            if (_player.PlayerState.Equals(PlayerState.Playing))
            {
                _player.Queue.Enqueue(track);
                return Utilities.GetAlert(guildAccount, "TRACKADDEDTOQUEUE", track.Title);
            }
            else
            {
                await _player.PlayAsync(track);
                return Utilities.GetAlert(guildAccount, "TRACKISPLAYING", track.Title);
            }
        }

        public async Task<string> StopAsync(IGuild guild, GuildAccount guildAccount)
        {
            var _player = _lavaNode.GetPlayer(guild);
            if (_player is null)
                return Utilities.GetAlert(guildAccount, "PLAYERWASNOTPLAYING");
            await _player.StopAsync();
            _player.Queue.Clear();
            return Utilities.GetAlert(guildAccount, "TRACKISSTOPPED");
        }

        public async Task<string> SkipAsync(IGuild guild, GuildAccount guildAccount)
        {
            var _player = _lavaNode.GetPlayer(guild);
            if (_player is null)
                return Utilities.GetAlert(guildAccount, "NOTHINGINQUEUE");
            else if(_player.Queue.Count == 0)
            {
                return await StopAsync(guild, guildAccount);
            }
            var oldTrack = _player.Track;
            await _player.SkipAsync();
            return Utilities.GetAlert(guildAccount, "TRACKISSKIPPED", oldTrack.Title, _player.Track.Title);
        }

        public async Task<string> PauseOrResumeAsync(IGuild guild, GuildAccount guildAccount)
        {
            var _player = _lavaNode.GetPlayer(guild);
            if (_player is null)
                return Utilities.GetAlert(guildAccount, "PLAYERWASNOTPLAYING");

            if (!_player.PlayerState.Equals(PlayerState.Paused))
            {
                await _player.PauseAsync();
                return Utilities.GetAlert(guildAccount, "PLAYERPAUSED");
            }
            else
            {
                await _player.ResumeAsync();
                return Utilities.GetAlert(guildAccount, "PLAYERRESUMED");
            }
        }

        private async Task ClientReadyAsync()
        {
            await _lavaNode.ConnectAsync();
        }

        private async Task TrackFinished(TrackEndedEventArgs arg)
        {
            if (!arg.Reason.ShouldPlayNext())
                return;

            if (!arg.Player.Queue.TryDequeue(out var item) || !(item is LavaTrack nextTrack))
            {
                await arg.Player.TextChannel.SendMessageAsync(
                    Utilities.GetAlert(GuildAccounts.GetAccount((SocketGuild)arg.Player.VoiceChannel.Guild),"NOMORETRACKS"));
                return;
            }
            await arg.Player.PlayAsync(nextTrack);
        }

        private async Task BotLeaveChannel(SocketUser arg1, SocketVoiceState arg2, SocketVoiceState arg3)
        {
            if(arg2.VoiceChannel.Users.Count == 1)
            {
                await _lavaNode.LeaveAsync(arg2.VoiceChannel);
            }
        }

        private Task LogAsync(LogMessage logMessage)
        {
            Console.WriteLine(logMessage.Message);
            return Task.CompletedTask;
        }
    }
}

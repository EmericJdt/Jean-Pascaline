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
using Victoria;
using Victoria.Enums;

namespace JeanPascaline.Modules
{
    public class AudioModule : InteractiveBase<SocketCommandContext>
    {
        private readonly LavaNode _lavaNode;
        private readonly AudioService _audioService;

        public AudioModule(LavaNode lavaNode, AudioService audioService)
        {
            _lavaNode = lavaNode;
            _audioService = audioService;
        }

        [Command("play", RunMode = RunMode.Async), Remarks("Audio"), Alias("SPlay")]
        [Summary("Recherche et joue une musique sur Youtube. (Utilise !SPlay pour chercher une musique sur SoundCloud.) — Search and play a song on Youtube. (Use !SPlay to search on SoundCloud.)")]
        public async Task Play([Remainder]string query = null)
        {
            try
            {
                GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
                SocketGuildUser user = Context.User as SocketGuildUser;
                EmbedBuilder ResponseEmbed;

                if (Context.Channel.Id != GuildData.MusicChannelID) return;

                

                if (user.VoiceChannel is null)
                {
                    ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildData, "USERISNOTINVOICECHANNEL"));
                    await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
                }

                else if (query == null)
                {
                    await _lavaNode.JoinAsync(user.VoiceChannel, Context.Channel as ITextChannel);
                }

                else
                {
                    Victoria.Responses.Rest.SearchResponse results;

                    if (Context.Message.Content.ToLower().Contains("splay"))
                    {
                        results = await _lavaNode.SearchSoundCloudAsync(query);
                    }
                    else
                    {
                        results = await _lavaNode.SearchYouTubeAsync(query);
                    }


                    if (results.LoadStatus == LoadStatus.NoMatches || results.LoadStatus == LoadStatus.LoadFailed)
                    {
                        ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildData, "NOMATCHESFOUND"));
                        await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
                    }
                    else
                    {
                        List<LavaTrack> FindedTracks = results.Tracks.Take(5).ToList();
                        ResponseEmbed = UtilitiesService.CreateEmbed(Color.Green, $"__{UtilitiesService.GetAlert(GuildData, "SEARCHINGRESULT")}__\n\n");

                        foreach (LavaTrack FindedTrack in FindedTracks)
                        {
                            ResponseEmbed.Description += $"**[{FindedTracks.FindIndex(x => x.Id == FindedTrack.Id) + 1}] — {FindedTrack.Title}\n** by {FindedTrack.Author} [{FindedTrack.Duration}]\n\n";
                        }

                        IUserMessage Message = await ReplyAsync(embed: ResponseEmbed.Build());
                        SocketMessage response = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));

                        await Message.DeleteAsync();
                        await response.DeleteAsync();

                        if (response == null) return;

                        if (Convert.ToInt32(response.Content) < 6 && Convert.ToInt32(response.Content) > 0)
                        {
                            await _lavaNode.JoinAsync(user.VoiceChannel, Context.Channel as ITextChannel);
                            LavaTrack Track = FindedTracks.ElementAt(Convert.ToInt32(response.Content) - 1);
                            _lavaNode.TryGetPlayer(Context.Guild, out LavaPlayer _player);

                            if (_player.PlayerState.Equals(PlayerState.Playing))
                            {
                                _player.Queue.Enqueue(Track);
                                _audioService.JPQueue.Add(Track);
                                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildData, "TRACKADDEDTOQUEUE", Track.Title));
                                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
                            }

                            else
                            {
                                await _player.PlayAsync(Track);
                                _audioService.repeatState = false;
                                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildData, "TRACKISPLAYING", Track.Title));
                                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                EmbedBuilder ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, ex.Message);
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
        }

        [Command("leave"), Remarks("Audio")]
        [Summary("Fait partir le bot du salon vocal. — Bot leaves your vocal channel.")]
        public async Task Leave()
        {
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            SocketGuildUser user = Context.User as SocketGuildUser;
            EmbedBuilder ResponseEmbed;
            if (Context.Channel.Id != GuildData.MusicChannelID) return;

            

            if (user.VoiceChannel is null)
            {
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildData, "USERISNOTINVOICECHANNEL"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
            else
            {
                await _lavaNode.LeaveAsync(user.VoiceChannel);
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildData, "BOTHASLEAVED", user.VoiceChannel.Name));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
        }

        [Command("stop"), Remarks("Audio")]
        [Summary("Arrête la musique et vide la liste de lecture. — Stops music and delete the current playlist.")]
        public async Task Stop()
        {
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            EmbedBuilder ResponseEmbed;
            if (Context.Channel.Id != GuildData.MusicChannelID) return;

            

            if (!_lavaNode.TryGetPlayer(Context.Guild, out LavaPlayer _player))
            {
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildData, "NOTHINGINQUEUE"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
            else
            {
                await _player.StopAsync();
                _player.Queue.Clear();
                _audioService.JPQueue.Clear();
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildData, "TRACKISSTOPPED"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
        }

        [Command("skip"), Remarks("Audio"), Alias("Voteskip")]
        [Summary("Passe à la prochaine musique lorsque 50% des utilisateurs présents utilise cette commande. — Skips to the next song when 50 percent of users use this command.")]
        public async Task Skip()
        {
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            EmbedBuilder ResponseEmbed;
            if (Context.Channel.Id != GuildData.MusicChannelID) return;

            

            if (!_lavaNode.TryGetPlayer(Context.Guild, out LavaPlayer _player))
            {
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildData, "NOTHINGINQUEUE"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }

            SocketGuildUser[] voiceChannelUsers = (_player.VoiceChannel as SocketVoiceChannel).Users.Where(x => !x.IsBot).ToArray();
            if (_audioService.VoteQueue.Contains(Context.User.Id))
            {
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildData, "ONLYONEVOTE"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
                return;
            }

            _audioService.VoteQueue.Add(Context.User.Id);
            int percentage = _audioService.VoteQueue.Count / voiceChannelUsers.Length * 100;
            if ((percentage < 50 && Context.User != Context.Guild.Owner) || Context.User.Id != 331435611116404737)
            {
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildData, "LOWPERCENTAGE"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
                return;
            }

            else if (_player.Queue.Count == 0)
            {
                await _player.StopAsync();
                _player.Queue.Clear();
                _audioService.VoteQueue.Clear();
                _audioService.JPQueue.Clear();
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildData, "TRACKISSTOPPED"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }

            else
            {
                LavaTrack oldTrack = _player.Track;
                await _player.SkipAsync();
                _audioService.JPQueue.Remove(_player.Track);
                _audioService.VoteQueue.Clear();
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildData, "TRACKISSKIPPED", oldTrack.Title, _player.Track.Title));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
        }

        [Command("pause"), Remarks("Audio"), Alias("Resume")]
        [Summary("Met en pause ou relance la musique actuelle. — Pause or resume the current track.")]
        public async Task Pause()
        {
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            EmbedBuilder ResponseEmbed;
            if (Context.Channel.Id != GuildData.MusicChannelID) return;

            

            if (!_lavaNode.TryGetPlayer(Context.Guild, out LavaPlayer _player))
            {
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildData, "PLAYERWASNOTPLAYING"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }

            else switch (_player.PlayerState)
                {
                    case PlayerState.Paused:
                        await _player.ResumeAsync();
                        ResponseEmbed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildData, "PLAYERRESUMED"));
                        await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
                        break;

                    case PlayerState.Playing:
                        await _player.PauseAsync();
                        ResponseEmbed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildData, "PLAYERPAUSED"));
                        await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
                        break;

                    case PlayerState.Stopped:
                        ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildData, "PLAYERWASNOTPLAYING"));
                        await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
                        break;

                    default:
                        return;

                }
        }

        [Command("volume"), Remarks("Audio")]
        [Summary("Ajuste le volume de la musique. (0 - 150) — Ajust music volume. (0 - 150)")]
        public async Task Volume(ushort volume = 100)
        {
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            EmbedBuilder ResponseEmbed;
            if (Context.Channel.Id != GuildData.MusicChannelID || volume > 150) return;

            

            if (!_lavaNode.TryGetPlayer(Context.Guild, out LavaPlayer _player))
            {
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildData, "PLAYERWASNOTPLAYING"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
            else
            {
                await _player.UpdateVolumeAsync(volume);
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildData, "PLAYERVOLUMEUPDATED", volume));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
        }

        [Command("repeat"), Remarks("Audio")]
        [Summary("Répète la musique actuelle. — Loops the current song.")]
        public async Task Repeat()
        {
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            EmbedBuilder ResponseEmbed;
            if (Context.Channel.Id != GuildData.MusicChannelID) return;

            
            if (!_lavaNode.TryGetPlayer(Context.Guild, out _))
            {
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildData, "PLAYERWASNOTPLAYING"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
            else
            {
                _audioService.repeatState = !_audioService.repeatState;
                ResponseEmbed = UtilitiesService.CreateEmbed(Color.Green, UtilitiesService.GetAlert(GuildData, "REPEATSTATECHANGED"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
        }

        [Command("queue"), Remarks("Audio")]
        [Summary("Affiche le nom des musiques en liste d'attente. — Shows music in the queue.")]
        public async Task Queue()
        {
            
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);


            if (_audioService.JPQueue.Count == 0)
            {
                EmbedBuilder ResponseEmbed = UtilitiesService.CreateEmbed(Color.Red, UtilitiesService.GetAlert(GuildData, "NOTRACKSONQUEUE"));
                await ReplyAndDeleteAsync("", embed: ResponseEmbed.Build());
            }
            else
            {
                List<LavaTrack> QueueList = _audioService.JPQueue;
                EmbedBuilder QueueEmbed = UtilitiesService.CreateEmbed(Color.Green, "", UtilitiesService.GetAlert(GuildData, "QUEUECARDTITLE"));

                foreach (LavaTrack Track in QueueList)
                {
                    QueueEmbed.Description += $"**[{QueueList.FindIndex(x => x.Id == Track.Id) + 1}] — {Track.Title}**\nby {Track.Author} [{Track.Duration}]\n";
                }
                await ReplyAsync(embed: QueueEmbed.Build());
            }
        }
    }
}








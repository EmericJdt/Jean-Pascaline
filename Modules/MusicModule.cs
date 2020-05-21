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

namespace JeanPascaline.Modules
{
    public class MusicModule : InteractiveBase<SocketCommandContext>
    {
        public static MusicService _musicService;

        public MusicModule(MusicService musicService)
        {
            _musicService = musicService;
        }

        [Command("play", RunMode = RunMode.Async), Remarks("Audio")]
        [Summary("Recherche et joue une musique sur Youtube. — Search and play a song on Youtube.")]
        public async Task Play([Remainder]string query = null)
        {
            if (Context.Channel.Id != GuildAccounts.GetAccount(Context.Guild).MusicChannelID) return;
            var user = Context.User as SocketGuildUser;
            if (user.VoiceChannel is null)
            {
                await ReplyAndDeleteAsync(Utilities.GetAlert(GuildAccounts.GetAccount(Context.Guild),"USERISNOTINVOICECHANNEL"));
                return;
            }

            await _musicService.ConnectAsync(user.VoiceChannel, Context.Channel as ITextChannel);
            if (query == null) return;

            List<LavaTrack> FindedTracks = await _musicService.SearchAsync(query);
            if(FindedTracks == null)
            {
                await ReplyAndDeleteAsync(Utilities.GetAlert(GuildAccounts.GetAccount(Context.Guild), "NOMATCHESFOUND"));
            }
            else
            {
                EmbedAuthorBuilder MusicAuthor = Utilities.CreateAuthorEmbed(Utilities.GetAlert(GuildAccounts.GetAccount(Context.Guild), "SEARCHINGRESULT"), Context.Guild.IconUrl);
                Dictionary<object, object> FieldsData = new Dictionary<object, object>();

                foreach (LavaTrack FindedTrack in FindedTracks)
                {
                    FieldsData.Add($"**{FindedTracks.FindIndex(x => x.Id == FindedTrack.Id) + 1} - {FindedTrack.Title}**", $"{FindedTrack.Author} - {FindedTrack.Duration}");
                }

                EmbedBuilder MusicEmbed = Utilities.CreateEmbed(MusicAuthor, Color.Magenta, Utilities.CreateListFields(FieldsData));

                var Message = await ReplyAsync(embed: MusicEmbed.Build());
                var response = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));
                if (response == null) return;

                GuildAccount guildAccount = GuildAccounts.GetAccount(Context.Guild);

                if (Convert.ToInt32(response.Content) <= 6 && Convert.ToInt32(response.Content) > 0)
                {
                    await ReplyAsync(await _musicService.PlayAsync(Context.Guild, FindedTracks.ElementAt(Convert.ToInt32(response.Content) - 1), guildAccount));
                }
                await Message.DeleteAsync();
                await response.DeleteAsync();
            }
        }

        [Command("leave"), Remarks("Audio")]
        [Summary("Le bot pars du salon vocal. — Bot leaves your vocal channel.")]
        public async Task Leave()
        {
            if (Context.Channel.Id != GuildAccounts.GetAccount(Context.Guild).MusicChannelID) return;
            var user = Context.User as SocketGuildUser;
            if (user.VoiceChannel is null)
            {
                await ReplyAndDeleteAsync(Utilities.GetAlert(GuildAccounts.GetAccount(Context.Guild),
                "USERISNOTINVOICECHANNEL"));
            }
            else
            {
                await _musicService.LeaveAsync(user.VoiceChannel);
                await ReplyAndDeleteAsync(Utilities.GetAlert(GuildAccounts.GetAccount(Context.Guild),
                "BOTHASLEAVED",
                 user.VoiceChannel.Name));
            }
        }

        [Command("stop"), Remarks("Audio")]
        [Summary("Arrête la musique et vide la liste de lecture. — Stops music and delete the current playlist.")]
        public async Task Stop()
        {
            if (Context.Channel.Id != GuildAccounts.GetAccount(Context.Guild).MusicChannelID) return;
            await ReplyAndDeleteAsync(await _musicService.StopAsync(Context.Guild, GuildAccounts.GetAccount(Context.Guild)));
        }

        [Command("skip"), Remarks("Audio")]
        [Summary("Passe la musique actuelle de la liste de lecture. — Skip the current track from the playlist.")]
        public async Task Skip()
        {
            if (Context.Channel.Id != GuildAccounts.GetAccount(Context.Guild).MusicChannelID) return;
            await ReplyAsync(await _musicService.SkipAsync(Context.Guild, GuildAccounts.GetAccount(Context.Guild)));
        }

        [Command("pause"), Remarks("Audio"), Alias("resume")]
        [Summary("Met en pause ou relance la musique actuelle. — Pause or resume the current track.")]
        public async Task Pause()
        {
            if (Context.Channel.Id != GuildAccounts.GetAccount(Context.Guild).MusicChannelID) return;
            await ReplyAsync(await _musicService.PauseOrResumeAsync(Context.Guild, GuildAccounts.GetAccount(Context.Guild)));
        }

        [Command("Queue"), Remarks("Audio")]
        [Summary("Affiche une liste des musiques qui seront joués — ")]
        public async Task Queue()
        {
            await Context.Message.DeleteAsync();
            string Message = "";
            List<LavaTrack> QueueList = await _musicService.QueueAsync(Context.Guild);
            if (QueueList is null) return;
            foreach(LavaTrack Track in QueueList)
            {
                Message += $"**{QueueList.FindIndex(x => x.Id == Track.Id) + 1} — {Track.Title}**\n>[{Track.Duration}] — {Track.Author} <{Track.Url}>\n\n";
            }
            await ReplyAsync(Message);
        }
    }
}
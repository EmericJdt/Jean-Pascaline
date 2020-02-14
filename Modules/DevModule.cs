using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using JeanPascaline.Core.AccountSystem;
using JeanPascaline.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JeanPascaline.Modules
{
    public class DevModule : InteractiveBase<SocketCommandContext>
    {
        //|—————————————————————————————————————————————— DEV GUILDS COMMANDS ——————————————————————————————————————————————|\\

        [Command("acces", RunMode = RunMode.Async), Summary("Accéder aux salons déprime."), Remarks("Owner")]
        public async Task Acces()
        {
            if (GuildAccounts.GetAccount(Context.Guild).ID != 161284551199424513) return;
            await Context.Message.DeleteAsync();
            SocketGuildUser User = Context.User as SocketGuildUser;

            if (!User.Roles.Contains(((IGuildUser)User).Guild.Roles.FirstOrDefault(x => x.Id == 512027757031456768)))
            {
                await User.AddRoleAsync(Context.Guild.Roles.FirstOrDefault(x => x.Id == 512027757031456768));
            }
            else
            {
                await User.RemoveRoleAsync(Context.Guild.Roles.FirstOrDefault(x => x.Id == 512027757031456768));
            }
        }

        [Command("yukue", RunMode = RunMode.Async), Summary("Non, pas Yukue mais Yukue !"), Remarks("Owner")]
        public async Task Yukue()
        {
            GuildAccount GuildData = GuildAccounts.GetAccount(Context.Guild);
            if (GuildData.IsDevGuild == false) return;

            int RandomNumber = UtilitiesService.GetRandomNumber(0, 31);
            if (RandomNumber == 30) await ReplyAsync(UtilitiesService.GetAlert(GuildData, "YUKUE" + RandomNumber));
            else await Context.Channel.SendFileAsync(@"Images/Yukue/" + UtilitiesService.GetYukueByID(RandomNumber) + ".png", UtilitiesService.GetAlert(GuildData, "YUKUE" + RandomNumber));
        }
    }
}

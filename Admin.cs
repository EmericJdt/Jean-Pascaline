using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using JeanPascaline.Core.AccountSystem;
using JeanPascaline.Core.LevelSystem;

namespace JeanPascaline
{

    [Group("admin")]
    [Alias("admin", "adm")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class Administration : ModuleBase
    {

        [Command("AddXP"), Summary("Ajouter de l'expérience.")]
        [Alias("axp")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddXP(SocketUser target, uint xp)
        {

            await Context.Message.DeleteAsync();

            var user = UserAccounts.GetAccount(target);
            if(user.Level >= 70)
            {
                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("UNSUCESSFUL_ADD", target.Mention));
                return;
            }

            else
            { 
                user.XP += xp;
                UserAccounts.SaveAccounts();
                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("ADD_XP", xp, target.Mention));
            }

        }

        [Command("WithdrawXP"), Summary("Retirer de l'expérience.")]
        [Alias("wxp")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task WithdrawXP(SocketUser target, uint xp)
        {
            await Context.Message.DeleteAsync();

            var user = UserAccounts.GetAccount(target);
            if ((user.XP - xp) < 0)
            {
                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("UNSUCESSFUL_WD", target.Mention));
                return;
            }

            else
            { 
                user.XP -= xp;
                UserAccounts.SaveAccounts();
                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("WD_XP", xp, target.Mention));
            }

        }
    }
}

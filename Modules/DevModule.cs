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
            if (GuildAccounts.GetAccount(Context.Guild).IsDevGuild == false) return;
            switch (Utilities.GetRandomNumber(0, 37))
            {
                // Dzéta
                case int result when result == 0 || result == 10 || result == 20:
                    await Context.Channel.SendFileAsync(@"Images/Yukue/Dzeta.png", "Si le sort m'était favorable je n'aurais jamais commencé ce jeu... — Dzéta");
                    break;

                // Alpha
                case int result when result == 1 || result == 11 || result == 21:
                    await Context.Channel.SendFileAsync(@"Images/Yukue/Alpha.png", "Très drôle. — Alpha");
                    break;

                // Tau
                case int result when result == 2 || result == 12 || result == 22:
                    await Context.Channel.SendFileAsync(@"Images/Yukue/Tau.png", "Ah... Tu crois qu'on vit dans un monde de SF ? — Tau");
                    break;

                // Oméga
                case int result when result == 3 || result == 13 || result == 23:
                    await Context.Channel.SendFileAsync(@"Images/Yukue/Omega.png", "Oh ! T'es tombée sur un sacré poisson Omi-chi... — Oméga");
                    break;

                // Epsilon
                case int result when result == 4 || result == 14 || result == 24:
                    await Context.Channel.SendFileAsync(@"Images/Yukue/Epsilon.png", "Je te laisse dix secondes pour décliner ton identité, passé ce délai ce toit risque de changer de couleur. — Epsilon");
                    break;

                // Sigma
                case int result when result == 5 || result == 15 || result == 25:
                    await Context.Channel.SendFileAsync(@"Images/Yukue/Sigma.png", "Chut... Laisse-la moi un moment~ — Sigma");
                    break;

                // Omicron
                case int result when result == 6 || result == 16 || result == 26:
                    await Context.Channel.SendFileAsync(@"Images/Yukue/Omicron.png", "Je savais que vous êtiez assez limité chez Azrom, mais de là à être ami avec une épée, franchement. — Omicron");
                    break;

                // Teta
                case int result when result == 7 || result == 17 || result == 27:
                    await Context.Channel.SendFileAsync(@"Images/Yukue/Teta.png", "Soit tu la fermes, soit tu crèves. — Têta");
                    break;

                // Gamma
                case int result when result == 8 || result == 18 || result == 28:
                    await Context.Channel.SendFileAsync(@"Images/Yukue/Gamma.png", "Tu n'as jamais été démembré vivant ? Écoute... y'a un début à tout. — Gamma");
                    break;

                // Lambda
                case int result when result == 9 || result == 19:
                    await Context.Channel.SendFileAsync(@"Images/Yukue/######.png", "... - ######", isSpoiler: true);
                    break;

                // Easter Egg
                case int result when result == 10 || result == 20:
                    await ReplyAsync("Attends. Tu parles de Yukue, Yukue, Yukue ou Yukue ?!");
                    break;

                // Epsilon - Sabrine
                case int result when result == 29 || result == 30:
                    await Context.Channel.SendFileAsync(@"Images/Yukue/SabrineEpsilon.png", "Sabrine : \"On va mourir\" ?\nYukue: \"Oui.\"");
                    break;

                // Alpha - Dzéta
                case 31:
                    await Context.Channel.SendFileAsync(@"Images/Yukue/AlphaDzeta.png", "Dzéta : \"Attends ! Tu vas passer par la porte ?! Tu peux voyager à travers les timelines et t'es même pas foutue de te téléporter à deux mètres ?!\"\nAlpha : \"Oh ! J'ai pas le budget... puis de toute façon j'ai un double des clés.\"");
                    break;

                // Tau Bed
                case 32:
                    await Context.Channel.SendFileAsync(@"Images/Yukue/Tau2.png", "Si tu veux m'aider, pourquoi tu portes un masque toi aussi ? — Tau");
                    break;

                // Omega Rooftop
                case 33:
                    await Context.Channel.SendFileAsync(@"Images/Yukue/Omega2.png", "T'es mignonne mais laisse-moi te rappeler quelque chose... Tu n'as pas le niveau pour pouvoir ne serais-ce que me menacer. — Oméga");
                    break;

                // Gamma - Têta
                case 34:
                    await Context.Channel.SendFileAsync(@"Images/Yukue/GammaTeta.png", "Têta : \"Celle qui en tue le moins paye toutes les tournées !!!\"\nGamma :\"J'espère que tu te diriges vers le bar là !!\"");
                    break;

                // Alpha - Dzéta 2
                case 35:
                    await Context.Channel.SendFileAsync(@"Images/Yukue/AlphaDzeta2.png", "Alpha : \"Dzéta qu'est-ce que tu branles ...?\"\nDzéta : \"Bah ça se voit non ? Je joue au scrabble.\"");
                    break;

                // Tau - Sigma
                case 36:
                    await Context.Channel.SendFileAsync(@"Images/Yukue/SigmaTau.png", "Tau : \"Dis... toi tu ne mentiras jamais hein ?\"\nSigma : \"Si tu fermes les yeux en disant ça c'est que tu connais déjà la réponse.\"");
                    break;
            }
        }
    }
}

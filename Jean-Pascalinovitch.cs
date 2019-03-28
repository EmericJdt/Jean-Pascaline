using System;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace JeanPascaline
{
    [Group("Jpvitch")]
    public class JPvitch : ModuleBase
    {
        // Xemox - Clément

        [Command("Xemox"), Summary("Xemox est un pédo")]
        [Alias("Clement")]
        public async Task Xemox()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Utilities.GetAlert("CLEMENT"));
        }

        // Iseety - Alix

        [Command("Iseety"), Summary("Va lire tes Mp connasse")]
        [Alias("Alix")]
        public async Task Iseety()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Utilities.GetAlert("ALIX"));
        }

        // Bibi - Luana

        [Command("Bibi"), Summary("Salooooooope")]
        [Alias("Salooooooope", "Luana")]
        public async Task Bibi()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Utilities.GetAlert("LUANA"));
        }

        // Goty - Adame

        [Command("Goty"), Summary("DEUS VULT")]
        [Alias("Adame")]
        public async Task Goty()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Utilities.GetAlert("ADAME"));
        }

        // KevZ - Kevenn

        [Command("KevZ"), Summary("Ah")]
        [Alias("Kevenn","Chaton","Kevouille")]
        public async Task KevZ()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Utilities.GetAlert("KEVENN"));
        }

        // Doc - Thomas

        [Command("Doc"), Summary("wink")]
        [Alias("Thomas")]
        public async Task Doc()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Utilities.GetAlert("THOMAS"));
        }

        // Nalomy - Mona

        [Command("Mona"), Summary("Non")]
        [Alias("Nalomy")]
        public async Task Mona()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Utilities.GetAlert("MONA"));
        }

        // Nata - Natalia

        [Command("Nata"), Summary("\"Toilettes\" c'est plus simple.")]
        [Alias("Natalia")]
        public async Task Nata()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Utilities.GetAlert("NATALIA"));
        }

        // Yukamatchi - Thilda

        [Command("Yuka"), Summary("OOOOOOHHH")]
        [Alias("Thilda", "Yukamatchi")]
        public async Task Yuka()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Utilities.GetAlert("THILDA"));
        }

        // JeanLol - Sara

        [Command("Sara"), Summary("LOLICE")]
        [Alias("JeanLol")]
        public async Task Sara()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Utilities.GetAlert("SARA"));
        }

        // Niama - Amina

        [Command("Niama"), Summary("Saucisson")]
        [Alias("Amina")]
        public async Task Niama()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Utilities.GetAlert("AMINA"));
        }

        // Utérus - Lisa

        [Command("Uterus"), Summary("Saucisson")]
        [Alias("Lisa","Roger")]
        public async Task Uterus()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Utilities.GetAlert("LISA"));
        }

        // Nwasette - Killian

        [Command("Nwasette"), Summary("Agrougrou")]
        [Alias("Killian")]
        public async Task Nwasette()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Utilities.GetAlert("KILLIAN"));
        }

        // Kitsune/Squigly - Klement

        [Command("Kitsune"), Summary("Renard")]
        [Alias("Klement", "Squigly")]
        public async Task Kitsune()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Utilities.GetAlert("KLEMENT"));
        }

        // Blue - Soraya

        [Command("Blue"), Summary("Hein ?")]
        [Alias("Soraya")]
        public async Task Blue()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Utilities.GetAlert("SORAYA"));
        }

        // Jeanbada - Jean

        [Command("Jean"), Summary("Phillipe")]
        [Alias("jeanbada")]
        public async Task Jean()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Utilities.GetAlert("JEAN"));
        }

        // Nender - Aurélien

        [Command("Nender"), Summary("Ta gueule")]
        [Alias("Aurelien")]
        public async Task Nender()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Utilities.GetAlert("AURELIEN"));
        }

        // Connor - Matt

        [Command("Matt"), Summary("TU TU TUUUUUUULUUUUUUU")]
        [Alias("Connor")]
        public async Task Matt()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Utilities.GetAlert("MATT"));
        }

        // Raika - Eulalie

        [Command("Raika"), Summary("eeeeeee")]
        [Alias("Eulalie")]
        public async Task Raika()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Utilities.GetAlert("EULALIE"));
        }

        // Blurp - Max

        [Command("Blurp"), Summary("Blurp")]
        [Alias("Max")]
        public async Task Blurp()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Utilities.GetAlert("MAX"));
        }

        // Saphira - Sophie

        [Command("Saphira"), Summary("RP ?")]
        [Alias("Sophie")]
        public async Task Saphira()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Utilities.GetAlert("SOPHIE"));
        }

        // Oort - Étienne

        [Command("Oort"), Summary("C'est petit")]
        [Alias("Etienne")]
        public async Task Oort()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Utilities.GetAlert("ETIENNE"));
        }

        // Slaymizuki - Baptiste

        [Command("Slay"), Summary("LG")]
        [Alias("Baptiste", "Slaymizuki")]
        public async Task Slay()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Utilities.GetAlert("BAPTISTE"));
        }

        // Miss - Alénor

        [Command("Miss"), Summary("300 warns.")]
        [Alias("Alenor")]
        public async Task Miss()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Utilities.GetAlert("ALENOR"));
        }

        // Sabrine - Sabrine

        [Command("Sabrine"), Summary("300 warns.")]
        public async Task Sabrine()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Utilities.GetAlert("SABRINE"));
        }



        // Unter - Émeric

        [Command("Unter"), Summary("La meilleure commande")]
        [Alias("Emeric")]
        public async Task Unter()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Utilities.GetAlert("EMERIC"));
        }
    }
}

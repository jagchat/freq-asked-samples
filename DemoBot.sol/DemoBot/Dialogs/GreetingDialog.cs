using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace DemoBot.Dialogs
{
    [Serializable]
    public class GreetingDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            //await context.PostAsync("Hi I'm Personify Bot");
            await context.SayAsync("Hi I'm Personify Bot", "Hi I'm Personify Bot");
            await Respond(context);

            var userName = String.Empty;
            context.UserData.TryGetValue<string>("Name", out userName);
            if (string.IsNullOrEmpty(userName))
            {
                context.Wait(MessageReceivedAsync);
            }
            else
            {
                context.Done(userName);
            }
        }
        private async Task Respond(IDialogContext context)
        {
            var userName = String.Empty;
            context.UserData.TryGetValue<string>("Name", out userName);
            if (string.IsNullOrEmpty(userName))
            {
                //await context.PostAsync("What is your name?");
                await context.SayAsync("What is your name?", "What is your name?");
                context.UserData.SetValue<bool>("GetName", true);
            }
            else
            {
                //await context.PostAsync($"Hi {userName}.  How can I help you today?");
                await context.SayAsync($"Hi {userName}.  How can I help you today?", $"Hi {userName}.  How can I help you today?");
            }
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            var userName = String.Empty;
            var getName = false;
            context.UserData.TryGetValue<string>("Name", out userName);
            context.UserData.TryGetValue<bool>("GetName", out getName);

            if (getName)
            {
                userName = message.Text;
                context.UserData.SetValue<string>("Name", userName);
                context.UserData.SetValue<bool>("GetName", false);
            }


            await Respond(context);
            context.Done(message);
        }
    }
}
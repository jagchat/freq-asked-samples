using System;
using System.Threading.Tasks;
using DemoBot.Repository;
using DemoBot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace DemoBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var resultMsg = string.Empty;
            var activity = await result as Activity;
            var text2parse = activity.Text;
            var parseResult = await (new MyNluParser()).ParseText(text2parse); //TODO: DI for NLP provider
            var dbResult = (new DbRepository()).ExecuteOperation(parseResult); //TODO: DI for data provider

            if (dbResult.GetType().IsArray)
            {
                resultMsg = "array";
            }
            else if (dbResult.GetType().IsValueType)
            {
                resultMsg = dbResult.ToString();
            }
            else
            {
                resultMsg = dbResult.GetType().ToString();
            }


            // return our reply to the user
            await context.PostAsync($"{resultMsg}");

            context.Wait(MessageReceivedAsync);
        }
    }
}
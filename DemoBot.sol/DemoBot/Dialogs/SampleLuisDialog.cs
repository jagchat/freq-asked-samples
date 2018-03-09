using System;
using System.Threading.Tasks;
using DemoBot.Repository;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System.Linq;
using System.Web.Services.Description;
using Microsoft.Bot.Builder.FormFlow;
using DemoBot.Models;
using NLog;

namespace DemoBot.Dialogs
{
	//TODO: this is fake..need to replace on your own
    [LuisModel("04030643-42c7-4ef1-9djb-f14f5b8ab1e6", "8f94520f0b5743cjba42988567c5cc9b")]
    [Serializable]
    public class SampleLuisDialog : LuisDialog<object>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [Serializable]
        public class PartialMessage
        {
            public string Text { set; get; }
        }

        private PartialMessage message;

        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var msg = await item;
            this.message = new PartialMessage { Text = msg.Text };
            await base.MessageReceived(context, item);
        }

        public readonly BuildFormDelegate<EmployeeCreation> CreateEmployee;

        public SampleLuisDialog(BuildFormDelegate<EmployeeCreation> createEmployee)
        {
            this.CreateEmployee = createEmployee;
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            logger.Trace("Invoking Luis intent 'None' - Text: " + message.Text);
            var msg = "Welcome to Personify Bot!";
            if (!string.IsNullOrEmpty(message.Text)) msg = "I'm sorry I don't know what you mean.";
            //await context.PostAsync(msg);
            await context.SayAsync(msg, msg);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            logger.Trace("Invoking Luis intent 'Greeting'..");
            context.Call(new GreetingDialog(), Callback);
        }

        private async Task Callback(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceived);
        }

        [LuisIntent("CountEmployees")]
        public async Task CountEmployees(IDialogContext context, LuisResult result)
        {
            logger.Trace("Invoking Luis intent 'CountEmployees'..");
            var dbResult = (new DbRepository()).GetEmpCount(); //TODO: Refactor, DI for data provider
            //await context.PostAsync($"There are {dbResult} employees as of now!");
            await context.SayAsync($"There are {dbResult} employees as of now!", $"There are {dbResult} employees as of now!");
            context.Wait(MessageReceived);
            return;
        }

        [LuisIntent("FindEmployee")]
        public async Task FindEmployee(IDialogContext context, LuisResult result)
        {
            logger.Trace("Invoking Luis intent 'FindEmployee'..");

            var entity = result.Entities.FirstOrDefault(e => e.Type == "Empno");
            var value = entity.Entity;

            var dbResult = (new DbRepository()).GetEmpById(value); //TODO: Refactor, DI for data provider
            if (dbResult != null)
            {
                //await context.PostAsync($"{dbResult.GetFormattedOuptut()}");
                await context.SayAsync($"{dbResult.GetFormattedOuptut()}", $"{dbResult.GetFormattedOuptut()}");
            }
            else
            {
                //await context.PostAsync($"oops..Can't find that employee!");
                await context.SayAsync($"I am sorry. Can't find that employee!", $"I am sorry. Can't find that employee!");
            }
            context.Wait(MessageReceived);
            return;
        }

        [LuisIntent("FindDept")]
        public async Task FindDept(IDialogContext context, LuisResult result)
        {
            logger.Trace("Invoking Luis intent 'FindDept'..");

            var entity = result.Entities.FirstOrDefault(e => e.Type == "Deptno");
            var value = entity.Entity;

            var dbResult = (new DbRepository()).GetDeptById(value); //TODO: Refactor, DI for data provider

            if (dbResult != null)
            {
                //await context.PostAsync($"{dbResult.GetFormattedOuptut()}");
                await context.SayAsync($"{dbResult.GetFormattedOuptut()}", $"{dbResult.GetFormattedOuptut()}");
            }
            else
            {
                //await context.PostAsync($"I am sorry. Can't find that department!");
                await context.SayAsync($"I am sorry. Can't find that department!", $"I am sorry. Can't find that department!");
            }

            context.Wait(MessageReceived);
            return;
        }

        [LuisIntent("CreateEmployee")]
        public async Task AddEmployee(IDialogContext context, LuisResult result)
        {
            logger.Trace("Invoking Luis intent 'CreateEmployee'..");

            var empForm = new FormDialog<EmployeeCreation>(new EmployeeCreation(), this.CreateEmployee, FormOptions.PromptInStart);
            context.Call<EmployeeCreation>(empForm, Callback);
        }

    }
}
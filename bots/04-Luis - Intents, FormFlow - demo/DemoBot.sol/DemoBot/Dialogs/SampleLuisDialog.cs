using System;
using System.Threading.Tasks;
using DemoBot.Repository;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System.Linq;
using Microsoft.Bot.Builder.FormFlow;
using DemoBot.Models;

namespace DemoBot.Dialogs
{
    [LuisModel("04030643-42c7-4ef1-9dab-f14f5b8ab1e6", "8f94520f0b5743caba42988567c5cc9b")]
    [Serializable]
    public class SampleLuisDialog : LuisDialog<object>
    {
        public readonly BuildFormDelegate<EmployeeCreation> CreateEmployee;

        public SampleLuisDialog(BuildFormDelegate<EmployeeCreation> createEmployee) {
            this.CreateEmployee = createEmployee;
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result) {
            await context.PostAsync("I'm sorry I don't know what you mean.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            context.Call(new GreetingDialog(), Callback);
        }

        private async Task Callback(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceived);
        }

        [LuisIntent("CountEmployees")]
        public async Task CountEmployees(IDialogContext context, LuisResult result)
        {
            var dbResult = (new DbRepository()).GetEmpCount(); //TODO: Refactor, DI for data provider
            await context.PostAsync($"There are {dbResult} employees as of now!");
            context.Wait(MessageReceived);
            return;
        }

        [LuisIntent("FindEmployee")]
        public async Task FindEmployee(IDialogContext context, LuisResult result)
        {
            var entity = result.Entities.FirstOrDefault(e => e.Type == "Empno");
            var value = entity.Entity;

            var dbResult = (new DbRepository()).GetEmpById(value); //TODO: Refactor, DI for data provider
            if (dbResult != null)
                await context.PostAsync($"{dbResult.GetFormattedOuptut()}");
            else
                await context.PostAsync($"oops..Employee not found!");
            context.Wait(MessageReceived);
            return;
        }

        [LuisIntent("FindDept")]
        public async Task FindDept(IDialogContext context, LuisResult result)
        {
            var entity = result.Entities.FirstOrDefault(e => e.Type == "Deptno");
            var value = entity.Entity;

            var dbResult = (new DbRepository()).GetDeptById(value); //TODO: Refactor, DI for data provider

            if (dbResult != null)
                await context.PostAsync($"{dbResult.GetFormattedOuptut()}");
            else
                await context.PostAsync($"oops..Department not found!");

            context.Wait(MessageReceived);
            return;
        }

        [LuisIntent("CreateEmployee")]
        public async Task AddEmployee(IDialogContext context, LuisResult result)
        {
            var empForm = new FormDialog<EmployeeCreation>(new EmployeeCreation(), this.CreateEmployee, FormOptions.PromptInStart);
            context.Call<EmployeeCreation>(empForm, Callback);
        }

    }
}
using DemoBot.Repository;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DemoBot.Models
{
    public enum DeptOptions {

        Accounting = 10,

        Sales = 20,

        IT = 30,

        Production = 40
    }

    [Serializable]
    public class EmployeeCreation
    {
        [Prompt("Employee id?")]
        public int? Empno;

        [Prompt("What's the Name?")]
        public string Ename;

        [Prompt("Approximate Salary?")]
        public double? Sal;

        public DeptOptions? DeptOption;

        public static IForm<EmployeeCreation> BuildForm() {
            return new FormBuilder<EmployeeCreation>()
                .Message("Welcome to Employee Creating Bot!")
                .OnCompletion(OnCreate)
                .Build();
        }

        private static async Task OnCreate(IDialogContext context, EmployeeCreation state) {
            var emp = new Employee() { Empno = state.Empno.Value, Ename = state.Ename, Sal = state.Sal.Value };
            emp.Deptno = (int) state.DeptOption;
            (new DbRepository()).CreateEmp(emp);
            //await context.PostAsync($"employee {state.Empno} succesfully created!");
            await context.SayAsync($"employee {state.Empno} succesfully created!", $"employee {state.Empno} succesfully created!");
        }
    }
}
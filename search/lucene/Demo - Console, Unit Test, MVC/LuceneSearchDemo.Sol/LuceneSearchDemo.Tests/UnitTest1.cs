using System;
using LuceneSearchDemo.Service.Library.Employees;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuceneSearchDemo.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void PerformEmployeeSearchTest()
        {
            var results = (new EmployeeSearch()).PerformSearch("Chat");
            Assert.IsTrue(results.Count > 0 && results[0].Ename == "chat");
        }
    }
}

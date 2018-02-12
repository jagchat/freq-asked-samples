using DemoBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DemoBot.Services
{
    public interface IParser
    {
         Task<ParseResult> ParseText(string text);
    }
}
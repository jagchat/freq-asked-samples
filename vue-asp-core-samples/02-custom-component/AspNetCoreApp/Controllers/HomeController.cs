using Microsoft.AspNetCore.Mvc;

namespace AspNetVueWidgetDemo
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}

using Microsoft.AspNetCore.Mvc;

namespace Temp.UI.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

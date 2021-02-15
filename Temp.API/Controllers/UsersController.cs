using Microsoft.AspNetCore.Mvc;

namespace Temp.API.Controllers
{
    public class UsersController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Temp.Application.Empolyees;
using Temp.Database;

namespace Temp.UI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeeController : Controller
    {
        public readonly ApplicationDbContext _ctx;

        public EmployeeController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public IActionResult Index()
        {
            return View(new GetEmployees(_ctx).Do());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployee.Request request)
        {
            if(ModelState.IsValid)
            {
                var response = await new CreateEmployee(_ctx).Do(request);

                if(response.Status)
                {
                    TempData["success_message"] = response.Message;
                    return RedirectToAction("Create");
                }
            }
            return RedirectToAction("Create");
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            return View(new GetEmployee(_ctx).Do(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateEmployee.Request request)
        {
            if(ModelState.IsValid)
            {
                var response = await new UpdateEmployee(_ctx).Do(request);

                if(response.Status)
                {
                    TempData["success_message"] = response.Message;
                    return RedirectToAction("Edit", response.Id);
                }
 
            }

            return View("Edit", request.Id);
        }



    }
}

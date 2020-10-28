using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Temp.Application.EmploymentStatuses;
using Temp.Database;

namespace Temp.UI.Controllers
{
    public class EmploymentStatusController : Controller
    {

        private readonly ApplicationDbContext _ctx;

        public EmploymentStatusController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public IActionResult Index()
        {
            return View(new GetEmploymentStatuses(_ctx).Do());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEmploymentStatus.Request request)
        {
            if (ModelState.IsValid)
            {
                var response = await new CreateEmploymentStatus(_ctx).Do(request); 
                
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
            var response = new GetEmploymentStatus(_ctx).Do(id);

            if(response == null)
            {
                return RedirectToAction("Index","Error");
            }

            return View(response);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(UpdateEmploymentStatus.Request request)
        {
            if(ModelState.IsValid)
            {
                var response = await new UpdateEmploymentStatus(_ctx).Do(request);

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

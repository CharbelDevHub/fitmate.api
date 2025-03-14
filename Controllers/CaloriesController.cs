using Microsoft.AspNetCore.Mvc;

namespace fitmate.api.Controllers
{
    public class CaloriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

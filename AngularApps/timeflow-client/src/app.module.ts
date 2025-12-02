using Microsoft.AspNetCore.Mvc;

namespace TimeFlow.AngularApps.timeflow_client.src
{
    public class app : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

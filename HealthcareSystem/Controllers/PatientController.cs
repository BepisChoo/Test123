using Microsoft.AspNetCore.Mvc;

namespace HealthcareSystem.Controllers
{
    public class PatientController : Controller
    {
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(userId) || role != "Patient")
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.FullName = HttpContext.Session.GetString("FullName");
            return View();
        }
    }
}
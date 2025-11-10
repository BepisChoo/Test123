using Microsoft.AspNetCore.Mvc;

namespace HealthcareSystem.Controllers
{
    public class DoctorController : Controller
    {
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(userId) || role != "Doctor")
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.FullName = HttpContext.Session.GetString("FullName");
            return View();
        }
    }
}
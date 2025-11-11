using HealthcareSystem.Data; // For ApplicationDbContext
using HealthcareSystem.Models; // For PatientProfile
using Microsoft.AspNetCore.Authorization; // For [Authorize]
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // For EF queries
using System.Security.Claims; // For user info

namespace HealthcareSystem.Controllers
{
    [Authorize]
    public class PatientController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Patient/Index
        public IActionResult Index()
        {
            ViewBag.FullName = User.FindFirstValue(ClaimTypes.Name) ?? User.Identity?.Name;

            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }

            return View();
        }

        // GET: /Patient/Profile
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(); // if user id isn't a valid int
            }

            var patientProfile = await _context.PatientProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patientProfile == null)
            {
                patientProfile = new PatientProfile { UserId = userId };
            }

            return View(patientProfile);
        }

        // POST: /Patient/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(PatientProfile profile)
        {
            if (ModelState.IsValid)
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!int.TryParse(userIdString, out int userId))
                {
                    return Unauthorized();
                }

                // Ensure users only update their own profile
                if (profile.UserId != userId)
                {
                    return Forbid();
                }

                var existingProfile = await _context.PatientProfiles.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                if (existingProfile != null)
                {
                    _context.PatientProfiles.Update(profile);
                }
                else
                {
                    _context.PatientProfiles.Add(profile);
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(profile);
        }
    }
}

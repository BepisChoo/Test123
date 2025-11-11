using HealthcareSystem.Data; // For ApplicationDbContext
using HealthcareSystem.Models; // For PatientProfile, Appointment
using Microsoft.AspNetCore.Authorization; // For [Authorize]
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // For EF queries
using System.Security.Claims; // For user info
using System.Linq;
using System.Threading.Tasks; // Needed for async Task

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

        // Helper method to safely get the int UserId
        private bool TryGetUserId(out int userId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdString, out userId);
        }

   
        // FEATURE 2: VIEW UPCOMING APPOINTMENTS (Dashboard)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!TryGetUserId(out int userId))
            {
                return Unauthorized(); // Cannot find a valid ID
            }

            // Get FullName for display
            ViewBag.FullName = User.FindFirstValue(ClaimTypes.Name) ?? User.Identity?.Name;

            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }

            // Fetch upcoming appointments for this patient
            var upcomingAppointments = await _context.Appointments
                .Where(a => a.PatientId == userId && a.AppointmentTime > DateTime.UtcNow)
                .Include(a => a.Doctor) // Include doctor details if needed
                .OrderBy(a => a.AppointmentTime)
                .ToListAsync();

            return View(upcomingAppointments);
        }

        // ------------------------------------------------------------------
        // FEATURE 1: UPDATE CONTACT INFO (Profile)
        // ------------------------------------------------------------------

        // GET: /Patient/Profile
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if (!TryGetUserId(out int userId))
            {
                return Unauthorized();
            }

            // Query using int userId
            var patientProfile = await _context.PatientProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patientProfile == null)
            {
                // If no profile exists, create a new one
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
                if (!TryGetUserId(out int userId))
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

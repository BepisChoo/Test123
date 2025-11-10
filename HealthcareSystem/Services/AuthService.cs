using HealthcareSystem.Data;
using HealthcareSystem.Models;
using HealthcareSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace HealthcareSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message, User? User)> RegisterAsync(RegisterViewModel model)
        {
            try
            {
                // Check if username already exists
                if (await _context.Users.AnyAsync(u => u.Username == model.Username))
                {
                    return (false, "Username already exists", null);
                }

                // Check if email already exists
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    return (false, "Email already exists", null);
                }

                // Validate role
                var validRoles = new[] { "Admin", "Doctor", "Patient", "ClinicStaff" };
                if (!validRoles.Contains(model.Role))
                {
                    return (false, "Invalid role", null);
                }

                // Create user
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Role = model.Role,
                    PhoneNumber = model.PhoneNumber,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Create role-specific profile
                if (model.Role == "Doctor")
                {
                    var doctorProfile = new DoctorProfile
                    {
                        UserId = user.UserId,
                        Specialization = model.Specialization ?? "General Practice",
                        LicenseNumber = model.LicenseNumber,
                        YearsOfExperience = 0
                    };
                    _context.DoctorProfiles.Add(doctorProfile);
                }
                else if (model.Role == "Patient")
                {
                    var patientProfile = new PatientProfile
                    {
                        UserId = user.UserId,
                        DateOfBirth = model.DateOfBirth,
                        Gender = model.Gender,
                        BloodType = model.BloodType
                    };
                    _context.PatientProfiles.Add(patientProfile);
                }

                await _context.SaveChangesAsync();

                return (true, "Registration successful", user);
            }
            catch (Exception ex)
            {
                return (false, $"Registration failed: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, User? User)> LoginAsync(LoginViewModel model)
        {
            try
            {
                // Find user by username or email
                var user = await _context.Users
                    .Include(u => u.DoctorProfile)
                    .Include(u => u.PatientProfile)
                    .FirstOrDefaultAsync(u =>
                        u.Username == model.UsernameOrEmail ||
                        u.Email == model.UsernameOrEmail);

                if (user == null)
                {
                    return (false, "Invalid username/email or password", null);
                }

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    return (false, "Invalid username/email or password", null);
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    return (false, "Account is deactivated", null);
                }

                // Update last login
                await UpdateLastLoginAsync(user.UserId);

                return (true, "Login successful", user);
            }
            catch (Exception ex)
            {
                return (false, $"Login failed: {ex.Message}", null);
            }
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.DoctorProfile)
                .Include(u => u.PatientProfile)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<bool> UpdateLastLoginAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.LastLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
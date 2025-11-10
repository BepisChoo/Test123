using HealthcareSystem.Models;
using HealthcareSystem.ViewModels;

namespace HealthcareSystem.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, User? User)> RegisterAsync(RegisterViewModel model);
        Task<(bool Success, string Message, User? User)> LoginAsync(LoginViewModel model);
        Task<User?> GetUserByIdAsync(int userId);
        Task<bool> UpdateLastLoginAsync(int userId);
    }
}
using HR.Models;
using HR.ViewModel;

namespace HR.serviec
{
    public interface IAuthServies
    {
        Task<AuthModel> RegisterAsync(RegisterModel model);
        Task<AuthModel> LoginAsync(Login model);
        Task<List<ApplictionUsers>> GetAllUsersAsync();

    }
}

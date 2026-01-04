using URLShortener.Models;

namespace URLShortener.Services;

public interface IAccountService
{
    Task<bool> LoginAsync(LoginViewModel model, CancellationToken cancellationToken);
    Task LogoutAsync(CancellationToken cancellationToken);
}
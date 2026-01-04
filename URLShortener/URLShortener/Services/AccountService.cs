using Microsoft.AspNetCore.Identity;
using URLShortener.Models;

namespace URLShortener.Services;

public class AccountService : IAccountService
{
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountService(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<bool> LoginAsync(LoginViewModel model, CancellationToken cancellationToken)
    {
        // "false" for rememberMe, "false" for lockoutOnFailure
        var result = await _signInManager.PasswordSignInAsync(
            model.Email, 
            model.Password, 
            isPersistent: false, 
            lockoutOnFailure: false);

        return result.Succeeded;
    }

    public async Task LogoutAsync(CancellationToken cancellationToken)
    {
        await _signInManager.SignOutAsync();
    }
}
using Microsoft.AspNetCore.Mvc;
using URLShortener.Models;
using URLShortener.Services;

namespace URLShortener.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var success = await _accountService.LoginAsync(model, cancellationToken);
            if (success)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await _accountService.LogoutAsync(cancellationToken);
        return RedirectToAction("Login", "Account");
    }
}
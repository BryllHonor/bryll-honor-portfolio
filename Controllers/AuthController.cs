using System.Security.Claims;
using BryllHonorPortfolio.Models;
using BryllHonorPortfolio.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BryllHonorPortfolio.Controllers;

public class AuthController : Controller
{
    private readonly UserService _users;

    public AuthController(UserService users)
    {
        _users = users;
    }

    [HttpGet("/login")]
    public IActionResult LoginForm(string? redirect)
    {
        if (User.Identity?.IsAuthenticated == true) return Redirect("/");

        return View("Login", new LoginViewModel
        {
            RedirectTo = IsSafeRedirect(redirect) ? redirect! : "/"
        });
    }

    [HttpPost("/login")]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var safeRedirect = IsSafeRedirect(model.RedirectTo) ? model.RedirectTo : "/";

        if (!_users.Verify(model.Username, model.Password))
        {
            TempData["FlashError"] = "Incorrect username or password.";
            return Redirect($"/login?redirect={Uri.EscapeDataString(safeRedirect)}");
        }

        var identity = new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.Name, model.Username) },
            CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        TempData["FlashSuccess"] = $"Welcome back, {model.Username}.";
        return Redirect(safeRedirect);
    }

    [HttpPost("/logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }

    private static bool IsSafeRedirect(string? redirect) =>
        !string.IsNullOrEmpty(redirect) && redirect.StartsWith('/') && !redirect.StartsWith("//");
}

using System.Security.Claims;
using ControlVehicular.Web.Models;
using ControlVehicular.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlVehicular.Web.Controllers;

public sealed class AccountController : Controller
{
    private readonly IAuthApiClient _authApi;

    public AccountController(IAuthApiClient authApi)
    {
        _authApi = authApi;
    }

    [AllowAnonymous]
    [HttpGet("/")]
    public IActionResult Login()
    {
        if (IsSignedIn)
        {
            return Redirect("/app");
        }

        return View(new LoginViewModel());
    }

    [AllowAnonymous]
    [HttpGet("/login")]
    public IActionResult LoginAlias() => Redirect("/");

    [AllowAnonymous]
    [HttpPost("/")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (IsSignedIn)
        {
            return Redirect("/app");
        }

        if (!ModelState.IsValid)
        {
            model.Password = string.Empty;
            model.ErrorMessage = "Revisa el correo y la contraseña";
            return View(model);
        }

        try
        {
            var session = await _authApi.LoginAsync(model.Email.Trim(), model.Password, cancellationToken);
            var displayName = string.IsNullOrWhiteSpace(session.User.Name)
                ? session.User.Email
                : session.User.Name;
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, string.IsNullOrWhiteSpace(session.User.Id) ? session.User.Email : session.User.Id),
                new(ClaimTypes.Email, session.User.Email),
                new(ClaimTypes.Name, displayName),
                new("access_token", session.AccessToken),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = false });

            return Redirect("/app");
        }
        catch (AuthApiException ex)
        {
            model.Password = string.Empty;
            model.ErrorMessage = ex.Message;
            return View(model);
        }
    }

    [AllowAnonymous]
    [HttpGet("/register")]
    public IActionResult Register()
    {
        if (IsSignedIn)
        {
            return Redirect("/app");
        }

        return View(new RegisterViewModel());
    }

    [AllowAnonymous]
    [HttpPost("/register")]
    [ValidateAntiForgeryToken]
    public IActionResult Register(RegisterViewModel model)
    {
        if (IsSignedIn)
        {
            return Redirect("/app");
        }

        if (!model.Agreed)
        {
            return View(model);
        }

        return Redirect("/");
    }

    [AllowAnonymous]
    [HttpGet("/forgot-password")]
    public IActionResult ForgotPassword()
    {
        if (IsSignedIn)
        {
            return Redirect("/app");
        }

        return View(new ForgotPasswordViewModel());
    }

    [AllowAnonymous]
    [HttpPost("/forgot-password")]
    [ValidateAntiForgeryToken]
    public IActionResult ForgotPassword(ForgotPasswordViewModel model)
    {
        if (IsSignedIn)
        {
            return Redirect("/app");
        }

        return Redirect("/");
    }

    [Authorize]
    [HttpPost("/logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var token = User.FindFirst("access_token")?.Value;
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!string.IsNullOrEmpty(token))
        {
            await _authApi.LogoutAsync(token, cancellationToken);
        }

        return Redirect("/");
    }

    private bool IsSignedIn => User.Identity?.IsAuthenticated == true;
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlVehicular.Web.Controllers;

[Authorize]
public sealed class HomeController : Controller
{
    [HttpGet("/app")]
    public IActionResult Main() => View();

    [HttpGet("/minor")]
    public IActionResult Minor() => View();
}

using Microsoft.AspNetCore.Mvc;

namespace ProjectManagementLite.Controllers;

public class AuthController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}
using Microsoft.AspNetCore.Mvc;
namespace ClinicBooking.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
    public IActionResult Error() => View();
}

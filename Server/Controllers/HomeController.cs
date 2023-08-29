using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Server.Models;

namespace Server.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    [Route("home")]
    public IActionResult Check()
    {        
        Console.WriteLine("HELLO BUDDY");
        Console.WriteLine(Request.Headers["token"]);
        return Ok();
    }
}
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Frontend.Models;
using Frontend.ApiClients;
using Frontend.ApiClients.Interface;

namespace Frontend.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IDoctorApiService _doctorService;

    public HomeController(ILogger<HomeController> logger, IDoctorApiService doctorService)
    {
        _logger = logger;
        _doctorService = doctorService;
    }

    public async Task<IActionResult> Index()
    {
        var doctors = await _doctorService.GetDoctorsAsync();
        return View(doctors);
    }

    public IActionResult Modify()
    {
        return View();
    }

    public IActionResult Delete()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult SetCrudOp(string op)
    {
        // You could validate op here if you want
        HttpContext.Session.SetString("CurrentCrudOp", op);
        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

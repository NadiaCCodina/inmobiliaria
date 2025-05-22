using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;

namespace inmobiliaria.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
       private readonly RepositorioInmueble repoInmueble;
    public HomeController(ILogger<HomeController> logger)
    {
        this.repoInmueble = new RepositorioInmueble();
        _logger = logger;
    }

    public IActionResult Index()
    {	ViewBag.Titulo = "Página de Inicio";
			List<string> inmuebles = repoInmueble.controlFechas(DateTime.Today, DateTime.Today).Select(x => x.Direccion).ToList();
			return View(inmuebles);
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
}

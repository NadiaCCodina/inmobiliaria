using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using inmobiliaria.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;

namespace inmobiliaria.Controllers
{

	public class ContratoController : Controller
	{
		private readonly RepositorioContrato repositorio;
		private readonly RepositorioInquilino repoInquilino;
        private readonly RepositorioInmueble repoInmueble;
		
		public ContratoController()
		{
			this.repositorio = new RepositorioContrato();
			this.repoInmueble = new RepositorioInmueble();
            this.repoInquilino = new RepositorioInquilino();
		}

        public ActionResult Index(int pagina = 1)
		{
			var lista = repositorio.ObtenerTodos();
			if (TempData.ContainsKey("Id"))
				ViewBag.Id = TempData["Id"];
			if (TempData.ContainsKey("Mensaje"))
				ViewBag.Mensaje = TempData["Mensaje"];
			return View(lista);
		}
    }
}

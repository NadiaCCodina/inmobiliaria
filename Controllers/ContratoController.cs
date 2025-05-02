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

        public ActionResult Index(int pagina = 1 )
		{
			var lista = repositorio.ObtenerTodos();
			if (TempData.ContainsKey("Id"))
				ViewBag.Id = TempData["Id"];
			if (TempData.ContainsKey("Mensaje"))
				ViewBag.Mensaje = TempData["Mensaje"];
			return View(lista);
		}

			public ActionResult Create(int id)
		{
			try
			{
				if(id>0){
					
					ViewBag.Inmueble = repoInmueble.ObtenerPorId(id);
					ViewBag.Inquilino = repoInquilino.ObtenerLista();
					
					return View();
				}else{
				ViewBag.Inquilino = repoInquilino.ObtenerLista();
				ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
				
				return View();}
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		// POST: Inmueble/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Contrato entidad)
		{
			
			try
			{
				if (ModelState.IsValid)
				{
					var precio= repoInmueble.ObtenerPorId(entidad.InmuebleId);

					repositorio.Alta(entidad, precio.Precio);
					//TempData["Id"] = entidad.Id;
					return RedirectToAction(nameof(Index));
				}
				else
				{
					return RedirectToAction(nameof(Index));
					// ViewBag.Propietario = repoPropietario.ObtenerLista();
					//return View(entidad);
				}
			}
			catch (Exception ex)
			{
				ViewBag.Error = ex.Message;

				ViewBag.StackTrate = ex.StackTrace;
				ViewBag.Inquilino = repoInquilino.ObtenerLista();
				ViewBag.Inmueble = repoInmueble.ObtenerTodos();
				return View(entidad);
			}
		}

	public ActionResult Edit(int id)
		{
			
			var entidad = repositorio.ObtenerPorId(id);
		
			ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
			ViewBag.Inquilino = repoInquilino.ObtenerLista();
			ViewBag.Inmueble= repoInmueble.ObtenerPorId(entidad.InmuebleId);

			if (TempData.ContainsKey("Mensaje"))
				ViewBag.Mensaje = TempData["Mensaje"];
			if (TempData.ContainsKey("Error"))
				ViewBag.Error = TempData["Error"];
			return View(entidad);
		}

	
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(int id, Contrato entidad)
		{
			try
			{
				var precio= repoInmueble.ObtenerPorId(entidad.InmuebleId);
				entidad.Id = id;
				repositorio.Modificacion(entidad, precio.Precio);
				TempData["Mensaje"] = "Datos guardados correctamente";
				return RedirectToAction(nameof(Index));
			}
			 catch (Exception ex)
			 {
				
			 	ViewBag.Error = ex.Message;
			 	ViewBag.StackTrate = ex.StackTrace;
			 	return View(entidad);
			 }
		}

		public ActionResult Eliminar(int id)
		{
			var entidad = repositorio.ObtenerPorId(id);
			if (TempData.ContainsKey("Mensaje"))
				ViewBag.Mensaje = TempData["Mensaje"];
			if (TempData.ContainsKey("Error"))
				ViewBag.Error = TempData["Error"];
			return View(entidad);
		}
    }
}



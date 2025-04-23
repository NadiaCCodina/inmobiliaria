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

	public class InmuebleController : Controller
	{
		private readonly RepositorioInmueble repositorio;
		private readonly RepositorioPropietario repoPropietario;
		private RepositorioPropietario? repoPropietari;

		public InmuebleController()
		{
			this.repositorio = new RepositorioInmueble();
			this.repoPropietario = new RepositorioPropietario();
		}

		// GET: Inmueble
		public ActionResult Index(int pagina = 1)
		{
			var lista = repositorio.ObtenerTodos();
			ViewBag.Propietario = repoPropietario.ObtenerLista();
			if (TempData.ContainsKey("Id"))
				ViewBag.Id = TempData["Id"];
			if (TempData.ContainsKey("Mensaje"))
				ViewBag.Mensaje = TempData["Mensaje"];
			return View(lista);
		}

		public ActionResult PorPropietario(int id)
		{
			var lista = repositorio.BuscarPorPropietario(id);
			// if (TempData.ContainsKey("Id"))
			// 	ViewBag.Id = TempData["Id"];
			// if (TempData.ContainsKey("Mensaje"))
			// 	ViewBag.Mensaje = TempData["Mensaje"];
			// ViewBag.Id = id;
			//ViewBag.Propietario = repoPropietario.
			ViewBag.Propietario = repoPropietario.ObtenerLista();
			return View("Index", lista);
		}

		public ActionResult Create()
		{
			try
			{
				ViewBag.Propietario = repoPropietario.ObtenerLista();
				//ViewData["Propietarios"] = repoPropietario.ObtenerTodos();
				//ViewData[nameof(Propietario)] = repoPropietario.ObtenerTodos();
				return View();
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		// POST: Inmueble/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Inmueble entidad)
		{
			try
			{
				if (ModelState.IsValid)
				{
					repositorio.Alta(entidad);
					//TempData["Id"] = entidad.Id;
					return RedirectToAction(nameof(Index));
				}
				else
				{
					return RedirectToAction(nameof(Index));
					// ViewBag.Propietario = repoPropietario.ObtenerLista();
					// return View(entidad);
				}
			}
			catch (Exception ex)
			{
				ViewBag.Error = ex.Message;
				ViewBag.StackTrate = ex.StackTrace;
				ViewBag.Propietario = repoPropietario.ObtenerLista();
				return View(entidad);
			}
		}

		public ActionResult Edit(int id)
		{
			var entidad = repositorio.ObtenerPorId(id);
			ViewBag.Propietario = repoPropietario.ObtenerLista();
			if (TempData.ContainsKey("Mensaje"))
				ViewBag.Mensaje = TempData["Mensaje"];
			if (TempData.ContainsKey("Error"))
				ViewBag.Error = TempData["Error"];
			return View(entidad);
		}

		// POST: Inmueble/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(int id, Inmueble entidad)
		{
			try
			{
				entidad.Id = id;
				repositorio.Modificacion(entidad);
				TempData["Mensaje"] = "Datos guardados correctamente";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ViewBag.Propietario = repoPropietario.ObtenerLista();
				ViewBag.Error = ex.Message;
				ViewBag.StackTrate = ex.StackTrace;
				return View(entidad);
			}
		}
// GET: Inmueble/Eliminar/5
		public ActionResult Eliminar(int id)
		{
			var entidad = repositorio.ObtenerPorId(id);
			if (TempData.ContainsKey("Mensaje"))
				ViewBag.Mensaje = TempData["Mensaje"];
			if (TempData.ContainsKey("Error"))
				ViewBag.Error = TempData["Error"];
			return View(entidad);
		}

		// POST: Inmueble/Eliminar/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Eliminar(int id, Inmueble entidad)
		{
			try
			{
				repositorio.Baja(id);
				TempData["Mensaje"] = "Eliminación realizada correctamente";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ViewBag.Error = ex.Message;
				ViewBag.StackTrate = ex.StackTrace;
				return View(entidad);
			}
		}

	}
}
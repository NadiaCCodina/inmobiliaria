﻿using inmobiliaria.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace inmobiliaria.Controllers
{
	[Authorize]
	public class InmuebleController : Controller
	{
		private readonly RepositorioInmueble repositorio;
		private readonly RepositorioPropietario repoPropietario;
		private RepositorioPropietario? repoPropietari;
		private readonly RepositorioContrato repoContrato;
		public InmuebleController()
		{
			this.repositorio = new RepositorioInmueble();
			this.repoPropietario = new RepositorioPropietario();
			this.repoContrato = new RepositorioContrato();
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


		public ActionResult Inmueble(int pagina = 1)
		{
			var lista = repositorio.ObtenerTodos();
			ViewBag.Propietario = repoPropietario.ObtenerLista();
			if (TempData.ContainsKey("Id"))
				ViewBag.Id = TempData["Id"];
			if (TempData.ContainsKey("Mensaje"))
				ViewBag.Mensaje = TempData["Mensaje"];
			return View(lista);
		}
		public ActionResult PorAmbiente(int cantidad)
		{

			var lista = repositorio.BuscarPorAmbientes(cantidad);
			if (lista != null && lista.Count > 0)
			{
				ViewBag.Propietario = repoPropietario.ObtenerLista();
				return View("Index", lista);
			}
			else
			{
				ViewBag.Propietario = repoPropietario.ObtenerLista();
				var listaTodos = repositorio.ObtenerTodos();
				TempData["Mensaje"] = "No se encontraron inmuebles.";
				return View("Index", listaTodos);
			}
		}

		public ActionResult buscarPor(int? cantidad, string tipo, int? propietarioId)
		{

			cantidad = cantidad > 0 ? cantidad : null;
			if (tipo == "")
			{
				tipo = null;
			}
			if (propietarioId < 1)
			{
				propietarioId = null;
			}
			var lista = repositorio.BuscarPor(tipo, cantidad, propietarioId);

			if (lista != null && lista.Count > 0)
			{
				ViewBag.Propietario = repoPropietario.ObtenerLista();
				return View("Index", lista);
			}
			else
			{
				ViewBag.Propietario = repoPropietario.ObtenerLista();
				var listaTodos = repositorio.ObtenerTodos();
				TempData["Mensaje"] = "No se encontraron inmuebles.";
				return View("Index", listaTodos);
			}
		}

		public ActionResult porTipo(String tipo)
		{

			var lista = repositorio.BuscarPorTipo(tipo);

			if (lista != null && lista.Count > 0)
			{
				ViewBag.Propietario = repoPropietario.ObtenerLista();
				return View("Index", lista);
			}
			else
			{
				ViewBag.Propietario = repoPropietario.ObtenerLista();
				var listaTodos = repositorio.ObtenerTodos();
				TempData["Mensaje"] = "No se encontraron inmuebles.";
				return View("Index", listaTodos);
			}

		}

		public ActionResult PorPropietario(int id)
		{
			var lista = repositorio.BuscarPorPropietario(id);
			ViewBag.Propietario = repoPropietario.ObtenerLista();
			return View("Index", lista);
		}

		public ActionResult PorFechaContrato(DateTime fechaInicio, DateTime fechaFin)
		{

			var lista = repositorio.controlFechas(fechaInicio, fechaFin);

			if (lista != null && lista.Count > 0)
			{
				ViewBag.Propietario = repoPropietario.ObtenerLista();
				ViewBag.FechaIni = fechaInicio;
				ViewBag.FechaF = fechaFin;
				return View("Index", lista);
			}
			else
			{
				ViewBag.Propietario = repoPropietario.ObtenerLista();
				var listaTodos = repositorio.ObtenerTodos();
				TempData["Mensaje"] = "No se encontraron inmuebles.";
				return View("Index", listaTodos);
			}
		}

		public ActionResult Create()
		{
			try
			{
				ViewBag.Propietario = repoPropietario.ObtenerLista();

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
					return RedirectToAction(nameof(Index));
				}
				else
				{
					return RedirectToAction(nameof(Index));

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
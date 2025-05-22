using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;

namespace inmobiliaria.Controllers
{
[Authorize]
	public class InquilinoController : Controller
	{

		// Sin inyección de dependencias (crear dentro del ctor)
		//private readonly RepositorioInquilino repositorio;

		// Con inyección de dependencias (pedir en el ctor como parámetro)
		private readonly RepositorioInquilino repositorio;
		private readonly IConfiguration config;

		public InquilinoController()
		{
			// Sin inyección de dependencias y sin usar el config (quitar el parámetro repo del ctor)
			this.repositorio = new RepositorioInquilino();
			// Sin inyección de dependencias y pasando el config (quitar el parámetro repo del ctor)
			//this.repositorio = new RepositorioInquilino(config);
			// Con inyección de dependencias
			//this.repositorio = repo;
			//this.config = config;
		}


		[Route("[controller]/Index/{pagina:int?}")]
		public ActionResult Index(int pagina = 1)
		{
			try
			{
				//var lista = repositorio.ObtenerTodos();
				var lista = repositorio.ObtenerLista(Math.Max(pagina, 1), 5);
				ViewBag.Id = TempData["Id"];
				// TempData es para pasar datos entre acciones
				// ViewBag/Data es para pasar datos del controlador a la vista
				// Si viene alguno valor por el tempdata, lo paso al viewdata/viewbag
				if (TempData.ContainsKey("Mensaje"))
					ViewBag.Mensaje = TempData["Mensaje"];
				return View(lista);
			}
			catch (Exception ex)
			{// Poner breakpoints para detectar errores
				throw;
			}
		}

		// GET: Inquilino/Create
		public ActionResult Create()
		{
			try
			{
				return View();
			}
			catch (Exception ex)
			{//poner breakpoints para detectar errores
				throw;
			}
		}


		[HttpPost]
		public ActionResult Create(Inquilino inquilino)
		{
			try
			{
				if (ModelState.IsValid)// Pregunta si el modelo es válido
				{
					repositorio.Alta(inquilino);
					return RedirectToAction(nameof(Index));
				}
				else
					return View(inquilino);
			}
			catch (Exception ex)
			{//poner breakpoints para detectar errores
				throw;
			}
		}
		// GET: Inquilino/Delete/5
		public ActionResult Eliminar(int id)
		{
			try
			{
				var entidad = repositorio.ObtenerPorId(id);
				return View(entidad);
			}
			catch (Exception ex)
			{//poner breakpoints para detectar errores
				throw;
			}
		}

		[HttpPost]

		public ActionResult Eliminar(int id, Inquilino entidad)
		{
			try
			{
				repositorio.Baja(id);
				TempData["Mensaje"] = "Eliminación realizada correctamente";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{//poner breakpoints para detectar errores
				throw;
			}
		}

	// GET: Inquilino/Edit/5
		public ActionResult Edit(int id)
		{
			try
			{
				var entidad = repositorio.ObtenerPorId(id);
				return View(entidad);//pasa el modelo a la vista
			}
			catch (Exception ex)
			{//poner breakpoints para detectar errores
				throw;
			}
		}

		// POST: Inquilino/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		//public ActionResult Edit(int id, IFormCollection collection)
		public ActionResult Edit(int id, Inquilino entidad)
		{
		
			Inquilino i = null;
			try
			{
				i = repositorio.ObtenerPorId(id);
			
				i.Nombre = entidad.Nombre;
				i.Apellido = entidad.Apellido;
				i.Dni = entidad.Dni;
				i.Email = entidad.Email;
				i.Telefono = entidad.Telefono;
				repositorio.Modificacion(i);
				TempData["Mensaje"] = "Datos guardados correctamente";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{//poner breakpoints para detectar errores
				throw;
			}
		}
		public ActionResult Busqueda()
		{
			try
			{
				return View();
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		[Route("[controller]/Buscar/{q}", Name = "BuscarInquilino")]
		public ActionResult Buscar(string q)
		{
			try
			{
				var res = repositorio.BuscarPorNombre(q);

				return View("Index", res);
			
			}
			catch (Exception ex)
			{
				return Json(new { Error = ex.Message });
			}
		}

       [Route("[controller]/Buscarjson/{q}", Name = "BuscarInquilinoJson")]
		public ActionResult Buscarjson(string q)
		{
			try
			{
				var res = repositorio.BuscarPorNombre(q);

				
				 return Json(new { Datos = res });
			}
			catch (Exception ex)
			{
				return Json(new { Error = ex.Message });
			}
		}

		public ActionResult Details(int id)
		{
			try
			{
				var entidad = repositorio.ObtenerPorId(id);
				return View(entidad);
			}
			catch (Exception ex)
			{
				throw;
			}
		}
	}
}
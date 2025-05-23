using Microsoft.AspNetCore.Mvc;

using inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;

namespace inmobiliaria.Controllers
{
  [Authorize]
	public class PropietarioController : Controller
	{

		// Sin inyección de dependencias (crear dentro del ctor)
		//private readonly RepositorioPropietario repositorio;

		// Con inyección de dependencias (pedir en el ctor como parámetro)
		private readonly RepositorioPropietario repositorio;
		private readonly IConfiguration config;

		public PropietarioController()
		{
			// Sin inyección de dependencias y sin usar el config (quitar el parámetro repo del ctor)
			this.repositorio = new RepositorioPropietario();
			// Sin inyección de dependencias y pasando el config (quitar el parámetro repo del ctor)
			//this.repositorio = new RepositorioPropietario(config);
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
				var lista = repositorio.ObtenerLista(Math.Max(pagina, 1), 10);
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

		// GET: Propietario/Create
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
		public ActionResult Create(Propietario propietario)
		{
			try
			{
				if (ModelState.IsValid)// Pregunta si el modelo es válido
				{
					repositorio.Alta(propietario);
					return RedirectToAction(nameof(Index));
				}
				else
					return View(propietario);
			}
			catch (Exception ex)
			{//poner breakpoints para detectar errores
				throw;
			}
		}
		// GET: Propietario/Delete/5
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

		public ActionResult Eliminar(int id, Propietario entidad)
		{
			try
			{
				repositorio.Baja(id);
				TempData["Mensaje"] = "Eliminación realizada correctamente";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		// GET: Propietario/Edit/5
		public ActionResult Edit(int id)
		{
			try
			{
				var entidad = repositorio.ObtenerPorId(id);
				return View(entidad);//pasa el modelo a la vista
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		// POST: Propietario/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		
		public ActionResult Edit(int id, Propietario entidad)
		{
			
			Propietario p = null;
			try
			{
				p = repositorio.ObtenerPorId(id);
		
				p.Nombre = entidad.Nombre;
				p.Apellido = entidad.Apellido;
				p.Dni = entidad.Dni;
				p.Email = entidad.Email;
				p.Telefono = entidad.Telefono;
				repositorio.Modificacion(p);
				TempData["Mensaje"] = "Datos guardados correctamente";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
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

		[Route("[controller]/Buscar/{q}", Name = "Buscar")]
		public ActionResult Buscar(string q)
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
				return View();
			}
			catch (Exception ex)
			{
				throw;
			}
		}
	}
}
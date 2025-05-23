using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using inmobiliaria.Models;

namespace inmobiliaria.Controllers
{

	[Authorize]
	public class ContratoController : Controller
	{
		private readonly RepositorioContrato repositorio;
		private readonly RepositorioInquilino repoInquilino;
		private readonly RepositorioInmueble repoInmueble;
		private readonly RepositorioPago repoPago;


		public ContratoController()
		{
			this.repositorio = new RepositorioContrato();
			this.repoInmueble = new RepositorioInmueble();
			this.repoInquilino = new RepositorioInquilino();
			this.repoPago = new RepositorioPago();
		}

		public ActionResult Index(int pagina = 1)
		{
			var lista = repositorio.ObtenerTodos();

			if (TempData.ContainsKey("Id"))
				ViewBag.Id = TempData["Id"];
			if (TempData.ContainsKey("Mensaje"))
				ViewBag.Mensaje = TempData["Mensaje"];
			ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
			return View(lista);
		}

		public ActionResult Detalle(int id)
		{
			if (!User.IsInRole("Administrador"))//no soy admin
			{
				var contrato = repositorio.ObtenerPorId(id);
				return View(contrato);
			}
			else
			{
				var contratoAuditoria = repositorio.ObtenerPorIdAuditoria(id);

				if (TempData.ContainsKey("Id"))
					ViewBag.Id = TempData["Id"];
				if (TempData.ContainsKey("Mensaje"))
					ViewBag.Mensaje = TempData["Mensaje"];
				ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
				return View(contratoAuditoria);
			}
		}


		public ActionResult Create(int id, int idInmueble, DateTime fechaInicio, DateTime fechaFin)
		{
			try
			{
				if (id > 0)
				{
					ViewBag.Contrato = repositorio.ObtenerPorId(id);
					return View();
				}
				else if (idInmueble > 0 && fechaInicio != null && fechaFin != null)
				{
					var controlFecha = repoInmueble.controlFechaId(idInmueble, fechaInicio, fechaFin);
					if (controlFecha != null)
					{
						{
							ViewBag.Inmueble = repoInmueble.ObtenerPorId(idInmueble);
							ViewBag.Inquilino = repoInquilino.ObtenerLista();

							return View();
						}
					}
					else
					{
						TempData["Mensaje"] = "Esa fecha no esta disponible";
						ViewBag.Inquilino = repoInquilino.ObtenerLista();
						ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
						return View();
					}
				}
				else
				{
					ViewBag.Inquilino = repoInquilino.ObtenerLista();
					ViewBag.Inmuebles = repoInmueble.ObtenerTodos();

					return View();
				}
			}
			catch (Exception ex)
			{
				throw ex;
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

					int usuarioActualId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

					var controlF = repoInmueble.controlFechaId(entidad.InmuebleId, entidad.FechaInicio, entidad.FechaFin);
					var precio = repoInmueble.ObtenerPorId(entidad.InmuebleId);
					if (controlF != null)
					{
						entidad.UsuarioAltaId = usuarioActualId;

						repositorio.Alta(entidad, precio.Precio);
						//TempData["Id"] = entidad.Id;
						var pendiente = "Pendiente";
						var cantidadMeses = CalcularCantidadMeses(entidad.FechaInicio, entidad.FechaFin);
						for (int i = 1; i < cantidadMeses + 1; i++)
						{
							repoPago.AltaAutomatico(entidad.Id, i, precio.Precio, pendiente);
						}
						return RedirectToAction(nameof(Index));
					}
					else
					{

						ViewBag.Inquilino = repoInquilino.ObtenerLista();
						ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
						TempData["Mensaje"] = "Fecha no disponible";
						return View(entidad);
					}
				}
				else
				{
					return View(entidad);
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
			ViewBag.Inmueble = repoInmueble.ObtenerPorId(entidad.InmuebleId);

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
				var precio = repoInmueble.ObtenerPorId(entidad.InmuebleId);
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

		public ActionResult Finalizar(int id)
		{
			var entidad = repositorio.ObtenerPorId(id);
			if (TempData.ContainsKey("Mensaje"))
				ViewBag.Mensaje = TempData["Mensaje"];
			if (TempData.ContainsKey("Error"))
				ViewBag.Error = TempData["Error"];
			return View(entidad);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Finalizar(int id, Contrato entidad)
		{
			try
			{
				int usuarioActualId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
				entidad.UsuarioBajaId = usuarioActualId;
				repositorio.Finalizar(entidad);
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

		public ActionResult PorInmueble(int id)
		{

			var lista = repositorio.ObtenerTodosPorInmueble(id);

			if (lista != null && lista.Count > 0)
			{
				if (TempData.ContainsKey("Id"))
					ViewBag.Id = TempData["Id"];
				if (TempData.ContainsKey("Mensaje"))
					ViewBag.Mensaje = TempData["Mensaje"];
				ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
				ViewBag.Inmueble = repoInmueble.ObtenerPorId(id);
				return View("Index", lista);
			}
			else
			{

				var listaTodos = repositorio.ObtenerTodos();
				ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
				TempData["Mensaje"] = "No se encontraron Contratos.";
				return View("Index", listaTodos);
			}

		}

		public ActionResult PorFecha(DateTime fechaInicio, DateTime fechaFin)
		{

			var lista = repositorio.ObtenerPorFecha(fechaInicio, fechaFin);

			if (lista != null && lista.Count > 0)
			{
				ViewBag.FechaIni = fechaInicio;
				ViewBag.FechaF = fechaFin;
				return View("Index", lista);
			}
			else
			{
				var listaTodos = repositorio.ObtenerTodos();
				TempData["Mensaje"] = "No se encontraron Contratos.";
				return View("Index", listaTodos);
			}
		}


		private static double CalcularCantidadMeses(DateTime fechaInicio, DateTime fechaFin)
		{
			var fechaInicioConvertida = fechaInicio.ToUniversalTime();
			var fechaFinConvertida = fechaFin.ToUniversalTime();
			if (fechaInicioConvertida > fechaFinConvertida)
			{
				throw new ArgumentOutOfRangeException(nameof(fechaInicio), "Error en las fechas");
			}
			double diasTotales = (fechaFinConvertida - fechaInicioConvertida).TotalDays;
			return diasTotales / (365.2425 / 12);
		}
	}
}



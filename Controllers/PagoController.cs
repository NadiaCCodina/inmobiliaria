using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
	public class PagoController : Controller
	{
		private readonly RepositorioPago repositorio;
		private readonly RepositorioContrato repoContrato;

		public PagoController()
		{
			this.repositorio = new RepositorioPago();
			this.repoContrato = new RepositorioContrato();

		}

		public ActionResult Create(int id)
		{

			try{
			var pago = repositorio.ObtenerPorId(id);
			//if (pago != null)
			{
				ViewBag.Pago = pago;
				ViewBag.Contrato = repoContrato.ObtenerPorId(id);
				return View();
				// }
				// else
				// {

			}
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Pago entidad)
		{
			try
			{
			if (ModelState.IsValid)
			{

				int usuarioActualId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
				entidad.UsuarioAltaId = usuarioActualId;
				repositorio.Modificacion(entidad);
				//TempData["Id"] = entidad.Id;
				return RedirectToAction(nameof(Index), "Contrato");
				//return View();
			}
			else
			{
				return RedirectToAction(nameof(Index), "Contrato");
				
			}
				}
				catch (Exception ex)
				{
					ViewBag.Error = ex.Message;
					ViewBag.StackTrate = ex.StackTrace;
					//ViewBag.Propietario = repoPropietario.ObtenerLista();
					return View(entidad);
				}
		}

		public ActionResult Index(int id)
		{

			if (!User.IsInRole("Administrador"))//no soy admin
			{
				var lista = repositorio.ObtenerTodosPorId(id);
				ViewBag.Contrato = repoContrato.ObtenerPorId(id);
				if (TempData.ContainsKey("Id"))
					ViewBag.Id = TempData["Id"];
				if (TempData.ContainsKey("Mensaje"))
					ViewBag.Mensaje = TempData["Mensaje"];
				return View(lista);
			}
			else
			{
				var lista = repositorio.ObtenerTodosPorIdAuditoria(id);
				ViewBag.Contrato = repoContrato.ObtenerPorId(id);
				if (TempData.ContainsKey("Id"))
					ViewBag.Id = TempData["Id"];
				if (TempData.ContainsKey("Mensaje"))
					ViewBag.Mensaje = TempData["Mensaje"];
				return View(lista);
			}
		}

        }
        



}
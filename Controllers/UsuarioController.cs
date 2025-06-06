using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;

using inmobiliaria.Models;

namespace inmobiliaria.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;
        private readonly RepositorioUsuario repositorio;

        public UsuarioController(IConfiguration configuration, IWebHostEnvironment environment, RepositorioUsuario repositorio)
        {
            this.configuration = configuration;
            this.environment = environment;
            this.repositorio = new RepositorioUsuario();
        }
        [Route("[controller]/Index/")]
        [Authorize(Policy = "Administrador")]
        public ActionResult Index()
        {
            var usuarios = repositorio.ObtenerTodos();
            return View(usuarios);
        }

        // GET: Usuarios/Create
        [Authorize(Policy = "Administrador")]
        public ActionResult Create()
        {
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View(new Usuario());
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Create(Usuario u)
        {
            if (!ModelState.IsValid)
                return View();
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                                password: u.Clave,
                                salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                                prf: KeyDerivationPrf.HMACSHA1,
                                iterationCount: 1000,
                                numBytesRequested: 256 / 8));
                u.Clave = hashed;
                u.Rol = User.IsInRole("Administrador") ? u.Rol : (int)enRoles.Empleado;
                var nbreRnd = Guid.NewGuid();//posible nombre aleatorio
                int res = repositorio.Alta(u);
                if (u.AvatarFile != null && u.Id > 0)
                {
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    //Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir
                    string fileName = "avatar_" + u.Id + Path.GetExtension(u.AvatarFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    u.Avatar = Path.Combine("/Uploads", fileName);
                    // Esta operación guarda la foto en memoria en la ruta que necesitamos
                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        u.AvatarFile.CopyTo(stream);
                    }
                    repositorio.Modificacion(u);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Roles = Usuario.ObtenerRoles();
                return View();
            }
        }
        // GET: Usuarios/Edit/5
        [Authorize]
        public ActionResult Perfil()
        {
            ViewData["Title"] = "Mi perfil";
            var u = repositorio.ObtenerPorEmail(User.Identity.Name);
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View("Edit", u);
        }

        // GET: Usuarios/Edit/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id)
        {
            ViewData["Title"] = "Editar usuario";
            var u = repositorio.ObtenerPorId(id);
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View(u);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(int id, Usuario u)
        {
            var vista = nameof(Edit);//de que vista provengo
            try
            {
                if (!User.IsInRole("Administrador"))//no soy admin
                {
                    vista = nameof(Perfil);//solo puedo ver mi perfil
                    var usuarioActual = repositorio.ObtenerPorEmail(User.Identity.Name);
                    if (usuarioActual.Id != id)
                    {//si no es admin, solo puede modificarse él mismo
                        return RedirectToAction(nameof(Index), "Home");
                    }
                    else
                    {

                        usuarioActual.Nombre = u.Nombre;
                        usuarioActual.Apellido = u.Apellido;
                        usuarioActual.Email = u.Email;

                        if (!string.IsNullOrWhiteSpace(u.Clave))
                        {
                            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                                            password: u.Clave,
                                            salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                                            prf: KeyDerivationPrf.HMACSHA1,
                                            iterationCount: 1000,
                                            numBytesRequested: 256 / 8));


                            usuarioActual.Clave = hashed;
                        }
                        repositorio.Modificacion(usuarioActual);
                        TempData["Mensaje"] = "Datos guardados correctamente";
                        return RedirectToAction(nameof(Index), "Home");

                    }
                }
                else
                {
                    u.Id = id;
                    if (!string.IsNullOrWhiteSpace(u.Clave))
                    {
                        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                                        password: u.Clave,
                                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                                        prf: KeyDerivationPrf.HMACSHA1,
                                        iterationCount: 1000,
                                        numBytesRequested: 256 / 8));


                        u.Clave = hashed;
                    }
                     repositorio.Modificacion(u);
                    TempData["Mensaje"] = "Datos guardados correctamente";
                   return RedirectToAction(nameof(Index), "Home");

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [AllowAnonymous]
        // GET: Usuarios/Login/
        public ActionResult Login(string returnUrl)
        {
            TempData["returnUrl"] = returnUrl;
            return View();
        }

        // POST: Usuarios/Login/
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginView login)
        {
            try
            {
                var returnUrl = String.IsNullOrEmpty(TempData["returnUrl"] as string) ? "/Home" : TempData["returnUrl"].ToString();
                if (ModelState.IsValid)
                {
                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: login.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));

                    var e = repositorio.ObtenerPorEmail(login.Usuario);
                    if (e == null || e.Clave != hashed)
                    {
                        ModelState.AddModelError("", "El email o la clave no son correctos");
                        TempData["returnUrl"] = returnUrl;
                        return View();
                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, e.Email),
                        new Claim("FullName", e.Nombre + " " + e.Apellido),
                        new Claim(ClaimTypes.Role, e.RolNombre),
                        new Claim(ClaimTypes.NameIdentifier, e.Id.ToString()),
                    };

                    var claimsIdentity = new ClaimsIdentity(
                            claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity));
                    TempData.Remove("returnUrl");
                    return Redirect(returnUrl);
                }
                TempData["returnUrl"] = returnUrl;
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        [Route("salir", Name = "logout")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // GET: Usuarios/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Eliminar(int id)

        {
            var entidad = repositorio.ObtenerPorId(id);
            return View(entidad);
        }

        // POST: Usuarios/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Eliminar(int id, Usuario usuario)
        {
            try
            {
                repositorio.Baja(id);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


    }
}


using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplicationGestorTareas.Models;

namespace WebApplicationGestorTareas.Controllers
{
    public class UsuariosController : Controller
    {
        private GestorTareasEntities db = new GestorTareasEntities();


        #region login

        public ActionResult Login()
        {
            ViewBag.Rol_Id = new SelectList(db.Rol, "Id", "Nombre");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Usuario objUser)
        {
            if (ModelState.IsValid)
            {
                using (GestorTareasEntities db = new GestorTareasEntities())
                {
                    var obj = db.Usuario.Where(a => a.Nombre.Equals(objUser.Nombre) && a.Contraseña.Equals(objUser.Contraseña)).FirstOrDefault();
                    if (obj != null)
                    {
                        Session["UserID"] = obj.Id.ToString();
                        Session["UserName"] = obj.Nombre.ToString();
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View(objUser);
        }

        public ActionResult Registro()
        {
            ViewBag.Rol_Id = new SelectList(db.Rol, "Id", "Nombre");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro([Bind(Include = "Id,Nombre,Email,Contraseña,Telefono,Imagen,Rol_Id")] Usuario usuario)
        {
            var existingUser = db.Usuario.FirstOrDefault(a => a.Id == usuario.Id);
            if (existingUser == null)
            {
                if (ModelState.IsValid)
                {
                    usuario.Rol = db.Rol.Find(usuario.Rol_Id);
                    db.Usuario.Add(usuario);
                    db.SaveChanges();
                    Session["UserID"] = usuario.Id.ToString();
                    Session["UserName"] = usuario.Nombre.ToString();
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "El usuario ya existe.");
            }
            ViewBag.Rol_Id = new SelectList(db.Rol, "Id", "Nombre", usuario.Rol_Id);
            return View(usuario);
        }

        public ActionResult Logout()
        {
            Session["UserID"] = null;
            Session["UserName"] = null;
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult RecuperarContraseña()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RecuperarContraseña(string UsuarioCorreo, string NuevaContraseña, string ConfirmarNuevaContraseña)
        {
            if (ModelState.IsValid)
            {
                var usuario = db.Usuario.FirstOrDefault(u => u.Nombre.Equals(UsuarioCorreo) || u.Email.Equals(UsuarioCorreo));

                if (usuario != null && NuevaContraseña == ConfirmarNuevaContraseña)
                {
                    usuario.Contraseña = NuevaContraseña;
                    db.Entry(usuario).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo recuperar la contraseña. Verifica tus datos.");
                    return View();
                }
            }
            return View();
        }

        #endregion


        // GET: ver perfiles
        public ActionResult Index()
        {
            return View(db.Usuario.ToList());
        }

        // GET: ver detalles de un perfil / ver mi perfil --> ver puntos
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // GET: Usuario/EditarPerfil/5
        public ActionResult EditarPerfil(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            UsuarioDto usuarioDto = new UsuarioDto()
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Contraseña = usuario.Contraseña,
                Email = usuario.Email,
                Telefono = usuario.Telefono,
                Puntos = usuario.Puntos,
                Imagen = usuario.Imagen,
                Rol_Id = usuario.Rol_Id
            };
            ViewBag.Roles = db.Rol.ToList();
            ViewBag.Rol_Id = new SelectList(db.Rol, "Id", "Nombre", usuario.Rol_Id);
            return View(usuarioDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarPerfil([Bind(Include = "Id,Nombre,Email,Telefono,Imagen,Rol_Id")] UsuarioDto usuarioDto, HttpPostedFileBase archivoImagen)
        {
            var usuarioExistente = db.Usuario.Find(usuarioDto.Id);

            if (usuarioExistente != null)
            {
                usuarioExistente.Nombre = usuarioDto.Nombre;
                usuarioExistente.Email = usuarioDto.Email;
                usuarioExistente.Telefono = usuarioDto.Telefono;
                usuarioExistente.Rol_Id = usuarioDto.Rol_Id;
                usuarioExistente.Rol = db.Rol.Find(usuarioDto.Rol_Id);
                Session["UserRol"] = usuarioExistente.Rol.Nombre;

                if (archivoImagen != null && archivoImagen.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(archivoImagen.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/"), fileName);
                    archivoImagen.SaveAs(path);

                    usuarioExistente.Imagen = fileName;
                }

                db.Entry(usuarioExistente).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("VerMiPerfil");
            }

            return HttpNotFound();
        }



        // DELETE: Usuarios/5/Premios/5
        public ActionResult EliminarPerfil(int? idUsuario)
        {
            if (idUsuario == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Usuario usuario = db.Usuario.Find(idUsuario);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            ViewBag.idUsuario = idUsuario;
            return View(usuario);
        }

        [HttpPost, ActionName("EliminarPerfil")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePerfilConfirmed(int? idUsuario)
        {
            Usuario usuario = db.Usuario.Find(idUsuario);
            var tareas = usuario.Tarea.ToList();
            var premios = usuario.Premio.ToList();
            var castigos = usuario.Castigo.ToList();

            foreach (var elem in tareas)
            {
                Tarea tarea = db.Tarea.Find(elem.Id);
                db.Tarea.Remove(tarea);
            }

            foreach (var elem in castigos)
            {
                Castigo castigo = db.Castigo.Find(elem.Id);
                castigo.Usuario.Remove(usuario);
                db.Entry(castigo).State = EntityState.Modified;

            }

            foreach (var elem in premios)
            {
                Premio premio = db.Premio.Find(elem.Id);
                premio.Usuario.Remove(usuario);
                db.Entry(premio).State = EntityState.Modified;

            }

            db.Usuario.Remove(usuario);
            db.SaveChanges();
            return RedirectToAction("ObtenerUsuarios");

        }

        // GET: ver mis premios / ver premios de un usuario
        public ActionResult ObtenerPremios(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario.Premio);
        }

        // GET: ver mi premio / ver premio de un usuario
        public ActionResult ObtenerPremio(int? idUsuario, int? idPremio)
        {
            if (idUsuario == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (idPremio == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(idUsuario);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            Premio premio = usuario.Premio.First(prem => prem.Id == idPremio);
            if (premio == null)
            {
                return HttpNotFound();
            }
            return View(premio);
        }

        // DELETE: Usuarios/5/Premios/5
        public ActionResult EliminarPremio(int? idUsuario, int? idPremio)
        {
            if (idUsuario == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (idPremio == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Usuario usuario = db.Usuario.Find(idUsuario);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            Premio premio = usuario.Premio.First(prem => prem.Id == idPremio);
            if (premio == null)
            {
                return HttpNotFound();
            }
            premio.Usuario.Remove(usuario);
            usuario.Premio.Remove(premio);
            db.SaveChanges();
            return View(usuario);
        }

        // GET: ver mis castigos / ver castigos de un usuario
        public ActionResult ObtenerCastigos(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario.Castigo);
        }

        // GET: ver mi castigo / ver castigo de un usuario
        public ActionResult ObtenerCastigo(int? idUsuario, int? idCastigo)
        {
            if (idUsuario == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (idCastigo == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(idUsuario);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            Castigo castigo = usuario.Castigo.First(cast => cast.Id == idCastigo);
            if (castigo == null)
            {
                return HttpNotFound();
            }
            return View(castigo);
        }

        // DELETE: Usuarios/5/Castigos/5
        public ActionResult EliminarCastigo(int? idUsuario, int? idCastigo)
        {
            if (idUsuario == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (idCastigo == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Usuario usuario = db.Usuario.Find(idUsuario);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            Castigo castigo = usuario.Castigo.First(cast => cast.Id == idCastigo);
            if (castigo == null)
            {
                return HttpNotFound();
            }
            castigo.Usuario.Remove(usuario);
            usuario.Castigo.Remove(castigo);
            db.SaveChanges();
            return View(usuario);
        }

        // GET: Usuarios/5/Tareas
        public ActionResult ObtenerTareas(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario.Tarea);
        }

        // PUT: Usuarios/5/Tareas/5/TareaCompletada
        [HttpPut]
        [ValidateAntiForgeryToken]
        public ActionResult MarcarTareaCompletada(int? idUsuario, int? idTarea, [Bind(Include = "Estado")] Tarea tareaModificada)
        {
            if (idUsuario == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (idTarea == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Usuario usuario = db.Usuario.Find(idUsuario);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            Tarea tarea = db.Tarea.Find(idTarea);
            if (tarea == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                tarea.Estado = tareaModificada.Estado;
                db.Entry(tarea).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Castigo_Id = new SelectList(db.Castigo, "Id", "Nombre", tarea.Castigo_Id);
            ViewBag.Usuario_Id = new SelectList(db.Usuario, "Id", "Nombre", tarea.Usuario_Id);
            return View(tarea);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

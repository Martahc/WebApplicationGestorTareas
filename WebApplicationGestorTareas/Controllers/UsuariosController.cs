using PagedList;
using System;
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
                    var obj = db.Usuario
                                .Include(u => u.Rol) 
                                .FirstOrDefault(a => a.Nombre == objUser.Nombre && a.Contraseña == objUser.Contraseña);


                    if (obj != null)
                    {
                        Session["UserID"] = obj.Id.ToString();
                        Session["UserName"] = obj.Nombre.ToString();
                        Session["UserRol"] = obj.Rol.Nombre;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Usuario y/o contraseña incorrectos"); 
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
        public ActionResult Registro([Bind(Include = "Id,Nombre,Email,Contraseña,Telefono,Imagen,Rol_Id")] UsuarioDto usuarioDto)
        {
            var existingUser = db.Usuario.FirstOrDefault(a => a.Nombre == usuarioDto.Nombre);
            if (existingUser == null)
            {
                Usuario usuario = usuarioDto.CopyFromDto();
                if (ModelState.IsValid)
                {
                    usuario.Rol = db.Rol.Find(usuario.Rol_Id);
                    db.Usuario.Add(usuario);
                    db.SaveChanges();
                    Session["UserID"] = usuario.Id.ToString();
                    Session["UserName"] = usuario.Nombre.ToString();
                    Session["UserRol"] = usuario.Rol.Nombre;
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "El usuario ya existe.");
            }
            ViewBag.Rol_Id = new SelectList(db.Rol, "Id", "Nombre", usuarioDto.Rol_Id);
            return View(usuarioDto);
        }

        public ActionResult Logout()
        {
            Session["UserID"] = null;
            Session["UserName"] = null;
            Session["UserRol"] = null;
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult RecuperarContraseña()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RecuperarContraseña(string UsuarioCorreo, UsuarioContraseñaDto usuarioDto)
        {
            if (ModelState.IsValid)
            {
                string NuevaContraseña = usuarioDto.NuevaContraseña;
                string ConfirmarNuevaContraseña = usuarioDto.ConfirmarNuevaContraseña;

                var usuario = db.Usuario.FirstOrDefault(u => u.Nombre.Equals(UsuarioCorreo) || u.Email.Equals(UsuarioCorreo));

                if (usuario != null)
                {                 
                    usuario.Contraseña = NuevaContraseña;
                    db.Entry(usuario).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Login");                  
                }
                else
                {
                    ModelState.AddModelError("", "No se encontró el usuario.");
                    return View();
                }
            }
            return View();
        }


        #endregion

        #region usuario

        // GET: ver perfiles
        public ActionResult ObtenerUsuarios()
        {
            return View(db.Usuario.ToList());
        }

        // GET: ver detalles de un perfil --> ver puntos
        public ActionResult Detalles(int? id)
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

        public ActionResult VerMiPerfil()
        {
            int id = int.Parse(Session["UserID"].ToString());
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

        #region imagen
        public ActionResult GetImage(string imageName)
        {
            string imagePath = Server.MapPath("~/Content/" + imageName);
            if (System.IO.File.Exists(imagePath))
            {
                using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        byte[] imageBytes = br.ReadBytes((int)fs.Length);
                        return File(imageBytes, "image/jpeg");
                    }
                }
            }
            return null;
        }

        #endregion

        #endregion

        #region premios 

        // GET: ver mis premios 
        public ActionResult ObtenerMisPremios(string sortOrder, string currentFilter, int? page)
        {
            int id = int.Parse(Session["UserID"].ToString());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            ViewBag.idUsuario = id;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentFilter = currentFilter;

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(usuario.Premio.ToPagedList(pageNumber, pageSize));

        }

        // GET: ver premios de un usuario
        public ActionResult ObtenerPremios(int? id, string sortOrder, string currentFilter, int? page)
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

            ViewBag.idUsuario = id;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentFilter = currentFilter;

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(usuario.Premio.ToPagedList(pageNumber, pageSize));

        }

        // GET: ver premio de un usuario
        public ActionResult ObtenerPremio(int? idUsuario, int? idPremio)
        {
            Premio premio = db.Premio.Find(idPremio);
            if (premio == null)
            {
                return HttpNotFound();
            }
            ViewBag.idUsuario = idUsuario;
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

            Premio premio = db.Premio.Find(idPremio);
            if (premio == null)
            {
                return HttpNotFound();
            }

            ViewBag.idUsuario = idUsuario;
            return View(premio);
        }

        [HttpPost, ActionName("EliminarPremio")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePremioConfirmed(int? idUsuario, int? idPremio)
        {
            Usuario usuario = db.Usuario.Find(idUsuario);
            Premio premio = usuario.Premio.First(prem => prem.Id == idPremio);
            premio.Usuario.Remove(usuario);
            usuario.Premio.Remove(premio);
            db.SaveChanges();
            return RedirectToAction("ObtenerPremios", new { id = idUsuario });

        }

        #endregion

        #region castigos

        // GET: ver mis castigos
        public ActionResult ObtenerMisCastigos(string sortOrder, string currentFilter, int? page)
        {
            int id = int.Parse(Session["UserID"].ToString());

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            ViewBag.idUsuario = id;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentFilter = currentFilter;

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(usuario.Castigo.ToPagedList(pageNumber, pageSize));
        }

        // GET: ver castigos de un usuario
        public ActionResult ObtenerCastigos(int? id, string sortOrder, string currentFilter, int? page)
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

            ViewBag.idUsuario = id;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentFilter = currentFilter;

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(usuario.Castigo.ToPagedList(pageNumber, pageSize));

        }

        // GET: ver castigo de un usuario
        public ActionResult ObtenerCastigo(int? idUsuario, int? idCastigo)
        {
            if (idCastigo == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Castigo castigo = db.Castigo.First(cast => cast.Id == idCastigo);
            if (castigo == null)
            {
                return HttpNotFound();
            }
            ViewBag.idUsuario = idUsuario;
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

            ViewBag.idUsuario = idUsuario;

            return View(castigo);
        }

        [HttpPost, ActionName("EliminarCastigo")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCastigoConfirmed(int? idUsuario, int? idCastigo)
        {
            Usuario usuario = db.Usuario.Find(idUsuario);
            Castigo castigo = usuario.Castigo.First(cast => cast.Id == idCastigo);
            castigo.Usuario.Remove(usuario);
            usuario.Castigo.Remove(castigo);
            db.SaveChanges();
            return RedirectToAction("ObtenerCastigos", new { id = idUsuario });
        }

        #endregion

        #region tareas

        // GET: Usuarios/5/Tareas
        public ActionResult ObtenerMisTareas()
        {
            int id = int.Parse(Session["UserID"].ToString());
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

        public ActionResult CambiarEstado(int idTarea)
        {
            Tarea tarea = db.Tarea.Find(idTarea);
            if (tarea == null)
            {
                return HttpNotFound();
            }

            if (tarea.Estado == "Asignada")
            {
                tarea.Estado = "En progreso";
                db.Entry(tarea).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("ObtenerMisTareas");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarcarTareaCompletada(int idUsuario, int idTarea, string estadoTarea)
        {
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

            tarea.Estado = estadoTarea;
            tarea.FechaFin = DateTime.Now;
           
            DateTime fechaLimite = tarea.FechaInicio.Value.AddDays(tarea.Plazo.Value);

            if (tarea.FechaFin > fechaLimite)
            {
                Castigo castigo = db.Castigo.Find(tarea.Castigo_Id);
                usuario.Castigo.Add(castigo);
                castigo.Usuario.Add(usuario);
                db.Entry(castigo).State = EntityState.Modified;
            }
            else
            {
                if (usuario.Puntos.HasValue && tarea.Puntos.HasValue)
                {
                    usuario.Puntos += tarea.Puntos;
                }
                else if (!usuario.Puntos.HasValue && tarea.Puntos.HasValue)
                {
                    usuario.Puntos = tarea.Puntos;
                }

                var premio = db.Premio
                .Where(p => p.Puntos <= usuario.Puntos)
                .OrderByDescending(p => p.Puntos)
                .FirstOrDefault();

                if (premio != null)
                {
                    usuario.Premio.Add(premio);
                    premio.Usuario.Add(usuario);                   
                    db.Entry(premio).State = EntityState.Modified;

                    usuario.Puntos -= premio.Puntos;
                }
            }

            db.Entry(tarea).State = EntityState.Modified;
            db.Entry(usuario).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("ObtenerMisTareas");
        }

        #endregion

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

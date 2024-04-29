using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

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
                                .Include(u => u.Rol) // Asegúrate de incluir la propiedad de rol
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
                        ModelState.AddModelError("", "Usuario y/o contraseña incorrectos"); // Agrega el mensaje de error al ModelState
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
            var existingUser = db.Usuario.FirstOrDefault(a => a.Nombre == usuario.Nombre);
            if (existingUser == null)
            {
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

        // GET: ver mi perfil --> ver mis puntos
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
        public ActionResult ObtenerMisPremios()
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
            return View(usuario.Premio);
        }

        // GET: ver premios de un usuario
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
            ViewBag.idUsuario = id;
            return View(usuario.Premio);
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

            if (usuario.Puntos.HasValue && tarea.Puntos.HasValue)
            {
                usuario.Puntos += tarea.Puntos;
            }
            else if (!usuario.Puntos.HasValue && tarea.Puntos.HasValue)
            {
                usuario.Puntos = tarea.Puntos;
            }

            tarea.Estado = estadoTarea;
            tarea.FechaFin = DateTime.Now;

            int plazo = tarea.Plazo.Value;

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
                var premio = db.Premio
                .Where(p => p.Puntos <= usuario.Puntos)
                .OrderByDescending(p => p.Puntos)
                .FirstOrDefault();

                if (premio != null)
                {
                    usuario.Premio.Add(premio);
                    premio.Usuario.Add(usuario);
                    db.Entry(premio).State = EntityState.Modified;
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

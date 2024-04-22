using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace WebApplicationGestorTareas.Controllers
{
    public class UsuariosController : Controller
    {
        private GestorTareasEntities db = new GestorTareasEntities();

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

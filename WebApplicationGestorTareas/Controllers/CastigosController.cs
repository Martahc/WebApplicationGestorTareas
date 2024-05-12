using System.Linq;
using System.Web.Mvc;
using WebApplicationGestorTareas.Models;

namespace WebApplicationGestorTareas.Controllers
{
    public class CastigosController : Controller
    {
        private GestorTareasEntities db = new GestorTareasEntities();

        // GET: Castigos
        public ActionResult Index()
        {
            return View(db.Castigo.ToList());
        }

        public ActionResult CrearCastigo()
        {
            return View();
        }

        // POST: Castigos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearCastigo([Bind(Include = "Id,Nombre,Descripcion,Duracion")] CastigoDto CastigoDto)
        {
            Castigo castigo = CastigoDto.CopyFromDto();
            if (ModelState.IsValid)
            {
                db.Castigo.Add(castigo);
                db.SaveChanges();
                return RedirectToAction("VerMiPerfil", "Usuarios");
            }
            return View(CastigoDto);
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
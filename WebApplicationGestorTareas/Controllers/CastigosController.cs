using System.Linq;
using System.Net;
using System.Web.Mvc;

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
        public ActionResult CrearCastigo([Bind(Include = "Id,Nombre,Descripcion,Plazo")] Castigo castigo)
        {
            if (ModelState.IsValid)
            {
                db.Castigo.Add(castigo);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(castigo);
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

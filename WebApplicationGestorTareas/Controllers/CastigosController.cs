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

        // POST: Castigos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre,Descripcion,Plazo")] Castigo castigo)
        {
            if (ModelState.IsValid)
            {
                db.Castigo.Add(castigo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(castigo);
        }

        // GET: Castigos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Castigo castigo = db.Castigo.Find(id);
            if (castigo == null)
            {
                return HttpNotFound();
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

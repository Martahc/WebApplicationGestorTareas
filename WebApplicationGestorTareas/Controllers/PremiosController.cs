using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace WebApplicationGestorTareas.Models
{
    public class PremiosController : Controller
    {
        private GestorTareasEntities db = new GestorTareasEntities();

        // GET: Premios
        public ActionResult Index()
        {
            return View(db.Premio.ToList());
        }

        // POST: Premios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre,Descripcion,Puntos")] Premio premio)
        {
            if (ModelState.IsValid)
            {
                db.Premio.Add(premio);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(premio);
        }

        // GET: Premios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Premio premio = db.Premio.Find(id);
            if (premio == null)
            {
                return HttpNotFound();
            }
            return View(premio);
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

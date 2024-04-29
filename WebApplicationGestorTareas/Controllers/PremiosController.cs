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

        public ActionResult CrearPremio()
        {           
            return View();
        }

        // POST: Premios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearPremio([Bind(Include = "Id,Nombre,Descripcion,Puntos")] Premio premio)
        {
            if (ModelState.IsValid)
            {
                db.Premio.Add(premio);
                db.SaveChanges();
                return RedirectToAction("Index","Home");
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

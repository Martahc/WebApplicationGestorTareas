using System.Linq;
using System.Web.Mvc;
using WebApplicationGestorTareas.Models;

namespace WebApplicationGestorTareas
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
        public ActionResult CrearPremio([Bind(Include = "Id,Nombre,Descripcion,Puntos")] PremioDto premioDto)
        {
            Premio premio = premioDto.CopyFromDto();
            if (ModelState.IsValid)
            {
                db.Premio.Add(premio);
                db.SaveChanges();
                return RedirectToAction("VerMiPerfil", "Usuarios");
            }
            return View(premioDto);
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
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace WebApplicationGestorTareas.Controllers
{
    public class TareasController : Controller
    {
        private GestorTareasEntities db = new GestorTareasEntities();

        // GET: Ver todas las Tareas
        public ActionResult ObtenerTareas()
        {
            var tarea = db.Tarea.Include(t => t.Castigo).Include(t => t.Usuario);
            return View(tarea.ToList());
        }

        // GET: Ver todas las Tareas completadas
        public ActionResult ObtenerTareasCompletadas()
        {
            var tarea = db.Tarea.Include(t => t.Castigo).Include(t => t.Usuario).Where(t => t.Estado == "Completada");
            return View(tarea.ToList());
        }

        // GET: Ver todas las Tareas asiganadas
        public ActionResult ObtenerTareasAsignadas()
        {
            var tarea = db.Tarea.Include(t => t.Castigo).Include(t => t.Usuario).Where(t => t.Usuario_Id != null);
            return View(tarea.ToList());
        }

        // GET: Ver todas las Tareas sin asignar
        public ActionResult ObtenerTareasNoAsignadas()
        {
            var tarea = db.Tarea.Include(t => t.Castigo).Include(t => t.Usuario).Where(t => t.Usuario_Id == null);
            return View(tarea.ToList());
        }

        // GET: Tareas/Details/5
        public ActionResult Detalles(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tarea tarea = db.Tarea.Find(id);
            if (tarea == null)
            {
                return HttpNotFound();
            }
            return View(tarea);
        }

        // POST: Tareas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearTarea([Bind(Include = "Id,Nombre,Plazo,Puntos,NivelDificultad,Estado,FechaInicio,FechaFin,Castigo_Id")] Tarea tarea)
        {
            if (ModelState.IsValid)
            {
                db.Tarea.Add(tarea);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Castigo_Id = new SelectList(db.Castigo, "Id", "Nombre", tarea.Castigo_Id);
            ViewBag.Usuario_Id = new SelectList(db.Usuario, "Id", "Nombre", tarea.Usuario_Id);
            return View(tarea);
        }

        // PUT: Tareas/Edit/5
        [HttpPut]
        [ValidateAntiForgeryToken]
        public ActionResult ModificarTarea([Bind(Include = "Id,Nombre,Plazo,Puntos,NivelDificultad,Estado,FechaInicio,FechaFin,Usuario_Id,Castigo_Id")] Tarea tarea)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tarea).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Castigo_Id = new SelectList(db.Castigo, "Id", "Nombre", tarea.Castigo_Id);
            ViewBag.Usuario_Id = new SelectList(db.Usuario, "Id", "Nombre", tarea.Usuario_Id);
            return View(tarea);
        }

        // DELETE: Tareas/Delete/5
        public ActionResult BorrarTarea(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tarea tarea = db.Tarea.Find(id);
            if (tarea == null)
            {
                return HttpNotFound();
            }
            return View(tarea);
        }


        // POST: Tareas/1/Asignar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AsignarTarea(int? idTarea, [Bind(Include = "Usuario_Id")] Tarea tareaAsignar)
        {
            if (idTarea == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tarea tarea = db.Tarea.Find(idTarea);
            if (tarea == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                tarea.Usuario_Id = tareaAsignar.Usuario_Id;
                db.Tarea.Add(tarea);
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

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

        public ActionResult CrearTarea()
        {
            ViewBag.Castigo_Id = new SelectList(db.Castigo, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearTarea([Bind(Include = "Id,Nombre,Plazo,Puntos,NivelDificultad,Castigo_Id")] Tarea tarea)
        {
            if (ModelState.IsValid)
            {
                Castigo castigo = db.Castigo.Find(tarea.Castigo_Id);
                castigo.Tarea.Add(tarea);
                db.Entry(castigo).State = EntityState.Modified;

                tarea.Estado = "No asignada";
                db.Tarea.Add(tarea);

                db.SaveChanges();
                return RedirectToAction("ObtenerTareas");               
            }

            ViewBag.Castigo_Id = new SelectList(db.Castigo, "Id", "Nombre", tarea.Castigo_Id);
            ViewBag.Usuario_Id = new SelectList(db.Usuario, "Id", "Nombre", tarea.Usuario_Id);
            return View(tarea);
        }

        // GET: Pedidoes/Edit/5
        public ActionResult ModificarTarea(int? id)
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
            ViewBag.Usuario_Id = new SelectList(db.Usuario, "Id", "Nombre");
            ViewBag.Castigo_Id = new SelectList(db.Castigo, "Id", "Nombre", tarea.Castigo_Id);
            return View(tarea);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModificarTarea([Bind(Include = "Id,Nombre,Plazo,Puntos,NivelDificultad,Estado,FechaInicio,FechaFin")] Tarea tareaModificada)
        {
            if (ModelState.IsValid)
            {
                var tareaExistente = db.Tarea.Find(tareaModificada.Id);

                if (tareaExistente != null)
                {
                    tareaExistente.Nombre = tareaModificada.Nombre;
                    tareaExistente.Plazo = tareaModificada.Plazo;
                    tareaExistente.Puntos = tareaModificada.Puntos;
                    tareaExistente.NivelDificultad = tareaModificada.NivelDificultad;
                    tareaExistente.Estado = tareaModificada.Estado;
                    tareaExistente.FechaInicio = tareaModificada.FechaInicio;
                    tareaExistente.FechaFin = tareaModificada.FechaFin;
                    db.Entry(tareaExistente).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("ObtenerTareas");
                }
                else
                {
                    return HttpNotFound();
                }
            }
            ViewBag.Castigo_Id = new SelectList(db.Castigo, "Id", "Nombre", tareaModificada.Castigo_Id);
            ViewBag.Usuario_Id = new SelectList(db.Usuario, "Id", "Nombre", tareaModificada.Usuario_Id);
            return View(tareaModificada);
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

        [HttpPost, ActionName("BorrarTarea")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tarea tarea = db.Tarea.Find(id);
            Castigo castigo = db.Castigo.Find(tarea.Castigo_Id);
            Usuario usuario = db.Usuario.Find(tarea.Usuario_Id);
            castigo.Tarea.Remove(tarea);
            usuario.Tarea.Remove(tarea);
            db.Tarea.Remove(tarea);
            db.SaveChanges();
            return RedirectToAction("ObtenerTareas");
        }

        public ActionResult AsignarTarea()
        {
            ViewBag.Usuario_Id = new SelectList(db.Usuario, "Id", "Nombre");
            return View();
        }

        // POST: Tareas/1/Asignar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AsignarTarea([Bind(Include = "Id,Usuario_Id,FechaInicio")] Tarea tareaAsignar)
        {
            if (tareaAsignar.Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tarea tarea = db.Tarea.Find(tareaAsignar.Id);
            if (tarea == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                tarea.Estado = "Asignada";
                tarea.Usuario_Id = tareaAsignar.Usuario_Id;
                tarea.FechaInicio = tareaAsignar.FechaInicio;

                Usuario usuario = db.Usuario.Find(tarea.Usuario_Id);

                usuario.Tarea.Add(tarea);

                db.Entry(tarea).State = EntityState.Modified;
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ObtenerTareasAsignadas");
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

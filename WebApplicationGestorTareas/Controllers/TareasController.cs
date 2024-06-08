﻿using PagedList;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI;
using WebApplicationGestorTareas.Models;

namespace WebApplicationGestorTareas.Controllers
{
    public class TareasController : Controller
    {
        private GestorTareasEntities db = new GestorTareasEntities();

        // GET: Ver todas las Tareas
        public ActionResult ObtenerTareas(string sortOrder, string currentFilter, int? page)
        {
            var tarea = db.Tarea.Include(t => t.Castigo).Include(t => t.Usuario);

            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentFilter = currentFilter;

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(tarea.ToList().ToPagedList(pageNumber, pageSize));
        }

        // GET: Ver todas las Tareas completadas
        public ActionResult ObtenerTareasCompletadas(string sortOrder, string currentFilter, int? page)
        {
            var tarea = db.Tarea.Include(t => t.Castigo).Include(t => t.Usuario).Where(t => t.Estado == "Completada");
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentFilter = currentFilter;

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(tarea.ToList().ToPagedList(pageNumber, pageSize));

        }

        // GET: Ver todas las Tareas asiganadas
        public ActionResult ObtenerTareasAsignadas(string sortOrder, string currentFilter, int? page)
        {
            var tarea = db.Tarea.Include(t => t.Castigo).Include(t => t.Usuario).Where(t => t.Usuario_Id != null);
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentFilter = currentFilter;

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(tarea.ToList().ToPagedList(pageNumber, pageSize));

        }

        // GET: Ver todas las Tareas sin asignar
        public ActionResult ObtenerTareasNoAsignadas(string sortOrder, string currentFilter, int? page)
        {
            var tarea = db.Tarea.Include(t => t.Castigo).Include(t => t.Usuario).Where(t => t.Usuario_Id == null);
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentFilter = currentFilter;

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(tarea.ToList().ToPagedList(pageNumber, pageSize));

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
        public ActionResult CrearTarea([Bind(Include = "Id,Nombre,Plazo,Puntos,NivelDificultad,Castigo_Id")] TareaDto tareaDto)
        {
            Tarea tarea = tareaDto.CopyFromDto();
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
            return View(tareaDto);
        }

        // GET: Pedidoes/Edit/5
        public ActionResult ModificarTarea(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tarea tarea = db.Tarea.Find(id);
            TareaDto tareaDto = new TareaDto()
            {
                Id = tarea.Id,
                Nombre = tarea.Nombre,
                Plazo = tarea.Plazo,
                Puntos = tarea.Puntos,
                NivelDificultad = tarea.NivelDificultad,
                Estado = tarea.Estado,
                FechaInicio = tarea.FechaInicio,
                FechaFin = tarea.FechaFin,
                Castigo_Id = tarea.Castigo_Id,
            };

            ViewBag.Castigos = db.Castigo.ToList();
            ViewBag.Usuario_Id = new SelectList(db.Usuario, "Id", "Nombre");
            ViewBag.Castigo_Id = new SelectList(db.Castigo, "Id", "Nombre");
            return View(tareaDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModificarTarea([Bind(Include = "Id,Nombre,Plazo,Puntos,NivelDificultad,Castigo_Id")] TareaDto tareaModificada)
        {
            if (ModelState.IsValid)
            {
                var tareaExistente = db.Tarea.Find(tareaModificada.Id);

                if (tareaExistente != null)
                {
                    if (tareaExistente.Nombre != tareaModificada.Nombre)
                    {
                        tareaExistente.Nombre = tareaModificada.Nombre;
                    }

                    if (tareaExistente.Plazo != tareaModificada.Plazo)
                    {
                        tareaExistente.Plazo = tareaModificada.Plazo;
                    }

                    if (tareaExistente.Puntos != tareaModificada.Puntos)
                    {
                        tareaExistente.Puntos = tareaModificada.Puntos;
                    }

                    if (tareaExistente.NivelDificultad != tareaModificada.NivelDificultad)
                    {
                        tareaExistente.NivelDificultad = tareaModificada.NivelDificultad;
                    }

                    if (tareaExistente.Castigo_Id != tareaModificada.Castigo_Id)
                    {
                        tareaExistente.Castigo_Id = tareaModificada.Castigo_Id;
                    }

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
            castigo.Tarea.Remove(tarea);
            db.Entry(castigo).State = EntityState.Modified;

            db.Tarea.Remove(tarea);
            db.SaveChanges();
            return RedirectToAction("ObtenerTareas");
        }
        public ActionResult AsignarTarea(int id)
        {
            ViewBag.Usuario_Id = new SelectList(db.Usuario, "Id", "Nombre");
            Tarea tarea = db.Tarea.Find(id);
            return View(tarea);
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

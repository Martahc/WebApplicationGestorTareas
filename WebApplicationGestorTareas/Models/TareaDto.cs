using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebApplicationGestorTareas.Models
{
    public class TareaDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public Nullable<DateTime> Plazo { get; set; }

        public Nullable<int> Puntos { get; set; }

        public string NivelDificultad { get; set; }

        public string Estado { get; set; }

        public Nullable<DateTime> FechaInicio { get; set; }

        public Nullable<DateTime> FechaFin { get; set; }

        public Nullable<int> Usuario_Id { get; set; }

        public int Castigo_Id { get; set; }

        public  CastigoDto Castigo { get; set; }

        public  UsuarioDto Usuario { get; set; }
    }
    public class TareaDBContext : DbContext
    {
        public DbSet<TareaDto> Tareas { get; set; }
    }
}
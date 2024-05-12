using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace WebApplicationGestorTareas.Models
{
    public class TareaDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la tarea es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre de la tarea no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; }

        [Display(Name = "Plazo")]
        [Required(ErrorMessage = "El plazo de la tarea es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El plazo de la tarea debe ser un número positivo.")]
        public Nullable<int> Plazo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Los puntos de la tarea deben ser un número positivo.")]
        public Nullable<int> Puntos { get; set; }

        [Required(ErrorMessage = "El nivel de dificultad de la tarea es obligatorio.")]
        public string NivelDificultad { get; set; }

        public string Estado { get; set; }

        [Display(Name = "Fecha de Inicio")]
        public Nullable<DateTime> FechaInicio { get; set; }

        [Display(Name = "Fecha de Fin")]
        public Nullable<DateTime> FechaFin { get; set; }

        public Nullable<int> Usuario_Id { get; set; }

        [Required(ErrorMessage = "El castigo es obligatorio.")]
        public int Castigo_Id { get; set; }

        public CastigoDto Castigo { get; set; }

        public UsuarioDto Usuario { get; set; }

        public Tarea CopyFromDto()
        {
            var tarea = new Tarea
            {
                Id = this.Id,
                Nombre = this.Nombre,
                Plazo = this.Plazo,
                Puntos = this.Puntos,
                NivelDificultad = this.NivelDificultad,
                Estado = this.Estado,
                FechaInicio = this.FechaInicio,
                FechaFin = this.FechaFin,
                Usuario_Id = this.Usuario_Id,
                Castigo_Id = this.Castigo_Id
            };

            if (this.Castigo != null)
            {
                tarea.Castigo = this.Castigo.CopyFromDto();
            }

            if (this.Usuario != null)
            {
                tarea.Usuario = this.Usuario.CopyFromDto();
            }

            return tarea;
        }
    }

    public class TareaDBContext : DbContext
    {
        public DbSet<TareaDto> Tareas { get; set; }
    }
}

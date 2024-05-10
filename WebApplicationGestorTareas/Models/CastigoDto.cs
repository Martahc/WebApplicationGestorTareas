using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace WebApplicationGestorTareas.Models
{
    public class CastigoDto
    {
        public CastigoDto()
        {
            this.Tareas = new List<TareaDto>();
            this.Usuarios = new List<UsuarioDto>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del castigo es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre del castigo no puede exceder los 50 caracteres.")]
        public string Nombre { get; set; }

        [StringLength(200, ErrorMessage = "La descripción del castigo no puede exceder los 200 caracteres.")]
        public string Descripcion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La duracion debe ser un número positivo.")]
        public Nullable<int> Duracion { get; set; }

        public List<TareaDto> Tareas { get; set; }

        public List<UsuarioDto> Usuarios { get; set; }

        public Castigo CopyFromDto()
        {
            var castigo = new Castigo
            {
                Id = this.Id,
                Nombre = this.Nombre,
                Descripcion = this.Descripcion,
                Duracion = this.Duracion
            };

            if (this.Tareas != null)
            {
                castigo.Tarea = new List<Tarea>();
                foreach (var tareaDto in this.Tareas)
                {
                    var tarea = tareaDto.CopyFromDto();
                    castigo.Tarea.Add(tarea);
                }
            }

            if (this.Usuarios != null)
            {
                castigo.Usuario = new List<Usuario>();
                foreach (var usuarioDto in this.Usuarios)
                {
                    var usuario = usuarioDto.CopyFromDto();
                    castigo.Usuario.Add(usuario);
                }
            }

            return castigo;
        }
    }

    public class CastigoDBContext : DbContext
    {
        public DbSet<CastigoDto> Castigos { get; set; }
    }
}

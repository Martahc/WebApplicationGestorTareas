using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace WebApplicationGestorTareas.Models
{
    public class UsuarioDto
    {
        public UsuarioDto()
        {
            this.Tareas = new List<TareaDto>();
            this.Castigos = new List<CastigoDto>();
            this.Premios = new List<PremioDto>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del usuario es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre del usuario no puede exceder los 50 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string Contraseña { get; set; }

        [RegularExpression(@"^\d{9}$", ErrorMessage = "Ingrese un número de teléfono válido (9 dígitos).")]
        public string Telefono { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Los puntos deben ser un número positivo.")]
        public Nullable<int> Puntos { get; set; }

        [Display(Name = "Imagen")]
        public string Imagen { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio.")]
        public int Rol_Id { get; set; }

        public Rol Rol { get; set; }

        public List<TareaDto> Tareas { get; set; }

        public List<CastigoDto> Castigos { get; set; }

        public List<PremioDto> Premios { get; set; }

        public Usuario CopyFromDto()
        {
            var usuario = new Usuario
            {
                Id = this.Id,
                Nombre = this.Nombre,
                Email = this.Email,
                Contraseña = this.Contraseña,
                Telefono = this.Telefono,
                Puntos = this.Puntos,
                Imagen = this.Imagen,
                Rol_Id = this.Rol_Id
            };

            if (this.Tareas != null)
            {
                usuario.Tarea = new List<Tarea>();
                foreach (var tareaDto in this.Tareas)
                {
                    var tarea = tareaDto.CopyFromDto();
                    usuario.Tarea.Add(tarea);
                }
            }

            if (this.Castigos != null)
            {
                usuario.Castigo = new List<Castigo>();
                foreach (var castigoDto in this.Castigos)
                {
                    var castigo = castigoDto.CopyFromDto();
                    usuario.Castigo.Add(castigo);
                }
            }

            if (this.Premios != null)
            {
                usuario.Premio = new List<Premio>();
                foreach (var premioDto in this.Premios)
                {
                    var premio = premioDto.CopyFromDto();
                    usuario.Premio.Add(premio);
                }
            }

            return usuario;
        }
    }

    public class UsuarioDBContext : DbContext
    {
        public DbSet<UsuarioDto> Usuarios { get; set; }
    }
}

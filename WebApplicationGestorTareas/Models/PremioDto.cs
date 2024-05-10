using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace WebApplicationGestorTareas.Models
{
    public class PremioDto
    {
        public PremioDto()
        {
            this.Usuarios = new List<UsuarioDto>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del premio es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre del premio no puede exceder los 50 caracteres.")]
        public string Nombre { get; set; }

        [StringLength(200, ErrorMessage = "La descripción del premio no puede exceder los 200 caracteres.")]
        public string Descripcion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Los puntos del premio deben ser un número positivo.")]
        public Nullable<int> Puntos { get; set; }

        public List<UsuarioDto> Usuarios { get; set; }

        public Premio CopyFromDto()
        {
            var premio = new Premio
            {
                Id = this.Id,
                Nombre = this.Nombre,
                Descripcion = this.Descripcion,
                Puntos = this.Puntos
            };

            if (this.Usuarios != null)
            {
                premio.Usuario = new List<Usuario>();
                foreach (var usuarioDto in this.Usuarios)
                {
                    var usuario = usuarioDto.CopyFromDto();
                    premio.Usuario.Add(usuario);
                }
            }

            return premio;
        }
    }

    public class PremioDBContext : DbContext
    {
        public DbSet<PremioDto> Premios { get; set; }
    }
}

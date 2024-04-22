using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

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

        public string Nombre { get; set; }

        public string Email { get; set; }

        public string Contraseña { get; set; }

        public string Telefono { get; set; }

        public Nullable<int> Puntos { get; set; }

        public string Imagen { get; set; }

        public int Rol_Id { get; set; }

        public  Rol Rol { get; set; }

        public List<TareaDto> Tareas { get; set; }

        public List<CastigoDto> Castigos { get; set; }

        public List<PremioDto> Premios { get; set; }
    }

    public class UsuarioDBContext : DbContext
    {
        public DbSet<UsuarioDto> Usuarios { get; set; }
    }
}
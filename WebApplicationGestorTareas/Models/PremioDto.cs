using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebApplicationGestorTareas.Models
{
    public class PremioDto
    {
        public PremioDto() 
        { 
            this.Usuarios = new List<UsuarioDto>();
        }
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public Nullable<int> Puntos { get; set; }

        public List<UsuarioDto> Usuarios { get; set; }
    }

    public class PremioDBContext : DbContext
    {
        public DbSet<PremioDto> Premios { get; set; }
    }
}
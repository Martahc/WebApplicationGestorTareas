using System;
using System.Collections.Generic;
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

        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public Nullable<int>  Plazo { get; set; }
        
        public List<TareaDto> Tareas { get; set; }
       
        public List<UsuarioDto> Usuarios { get; set; }
    }

    public class CastigoDBContext : DbContext
    {
        public  DbSet<CastigoDto> Castigos { get; set; }
    }
}
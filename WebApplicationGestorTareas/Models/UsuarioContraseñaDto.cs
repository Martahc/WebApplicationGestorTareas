using System.ComponentModel.DataAnnotations;

namespace WebApplicationGestorTareas.Models
{
    public class UsuarioContraseñaDto
    {
        [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La nueva contraseña debe tener al menos 6 caracteres.")]
        public string NuevaContraseña { get; set; }

        [Compare("NuevaContraseña", ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar Nueva Contraseña")]
        public string ConfirmarNuevaContraseña { get; set; }
    }
}
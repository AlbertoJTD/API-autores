using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTOs
{
    public class CredencialesUsuario
    {
        // Parametros de entrada
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

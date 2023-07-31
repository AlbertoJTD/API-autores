using Microsoft.AspNetCore.Identity;

namespace WebApiAutores.Entidades
{
	public class LlaveAPI
	{
        public int Id { get; set; }
        public string Llave { get; set; }
        public TipoLlave TipoLlave { get; set; }
        public bool Activa { get; set; }
        public string UsuarioId { get; set; }
        public IdentityUser Usuario { get; set; }
    }
}

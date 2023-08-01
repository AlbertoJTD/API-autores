using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTOs
{
	public class ActualizarRestriccionDominioDTO
	{
		[Required]
		public string Dominio { get; set; }
	}
}

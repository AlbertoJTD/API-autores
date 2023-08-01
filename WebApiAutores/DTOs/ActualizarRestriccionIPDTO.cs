using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTOs
{
	public class ActualizarRestriccionIPDTO
	{
		[Required]
		public string IP { get; set; }
	}
}

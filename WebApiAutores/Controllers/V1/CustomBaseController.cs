using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace WebApiAutores.Controllers.V1
{
	public class CustomBaseController: ControllerBase
	{
		protected string ObtenerUsuarioId()
		{
			var usuarioClaium = HttpContext.User.Claims.Where(x => x.Type == "id").FirstOrDefault();
			var usuarioId = usuarioClaium.Value;

			return usuarioId;
		}
	}
}

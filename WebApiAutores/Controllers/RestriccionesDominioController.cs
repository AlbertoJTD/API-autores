using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApiAutores.Controllers.V1;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
	[ApiController]
	[Route("api/v1/restriccionesdominio")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class RestriccionesDominioController: CustomBaseController
	{
		private readonly ApplicationDbContext context;

		public RestriccionesDominioController(ApplicationDbContext context)
        {
			this.context = context;
		}

		[HttpPost]
		public async Task<ActionResult> Post(CrearRestriccionesDominioDTO crearRestriccionesDominioDTO)
		{
			var llaveDB = await context.LlavesAPI.FirstOrDefaultAsync(x => x.Id == crearRestriccionesDominioDTO.LlaveId);
			if (llaveDB == null)
			{
				return NotFound();
			}

			var usuarioId = ObtenerUsuarioId();
			if (llaveDB.UsuarioId != usuarioId)
			{
				return Forbid();
			}

			var restriccionDominio = new RestriccionDominio()
			{
				LlaveId = crearRestriccionesDominioDTO.LlaveId,
				Dominio = crearRestriccionesDominioDTO.Dominio
			};

			context.Add(restriccionDominio);
			await context.SaveChangesAsync();
			return NoContent();
		}

		[HttpPut("{id:int}")]
		public async Task<ActionResult> Put(int id, ActualizarRestriccionDominioDTO actualizarRestriccionDominio)
		{
			var restriccionDB = await context.RestriccionesDominio.Include(x => x.Llave).FirstOrDefaultAsync(x => x.Id == id);
			if (restriccionDB == null)
			{
				return NotFound();
			}

			var usuarioId = ObtenerUsuarioId();
			if (restriccionDB.Llave.UsuarioId != usuarioId)
			{
				return Forbid();
			}

			restriccionDB.Dominio = actualizarRestriccionDominio.Dominio;

			await context.SaveChangesAsync();
			return NoContent();
		}
	}
}

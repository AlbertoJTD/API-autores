using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WebApiAutores.DTOs;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/facturas")]
	public class FacturasController: CustomBaseController
    {
		private readonly ApplicationDbContext context;

		public FacturasController(ApplicationDbContext context)
        {
			this.context = context;
		}

		[HttpPost]
		public async Task<ActionResult> Pagar(PagarFacturaDTO pagarFacturaDTO)
		{
			var facturaDB = await context.Facturas.Include(x => x.Usuario)
												  .FirstOrDefaultAsync(x => x.Id == pagarFacturaDTO.FacturaId);

			if (facturaDB == null)
			{
				return NotFound();
			}

			if (facturaDB.Pagada)
			{
				return BadRequest("La factura ya fue saldada");
			}

			facturaDB.Pagada = true;
			await context.SaveChangesAsync();

			var facturasPendientesVencidas = await context.Facturas.AnyAsync(x => x.UsuarioId == facturaDB.UsuarioId &&
																					!x.Pagada && x.FechaLimiteDePago < DateTime.Today);

			if (!facturasPendientesVencidas)
			{
				facturaDB.Usuario.Deudor = false;
				await context.SaveChangesAsync();
			}

			return NoContent();
		}
	}
}

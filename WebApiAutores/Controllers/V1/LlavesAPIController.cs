using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiAutores.DTOs;
using WebApiAutores.Servicios;

namespace WebApiAutores.Controllers.V1
{
	[ApiController]
	[Route("api/llavesAPI")]
	public class LlavesAPIController: CustomBaseController
	{
		private readonly ApplicationDbContext context;
		private readonly IMapper mapper;
		private readonly ServicioLlaves servicioLlaves;

		public LlavesAPIController(ApplicationDbContext context, IMapper mapper, ServicioLlaves servicioLlaves)
        {
			this.context = context;
			this.mapper = mapper;
			this.servicioLlaves = servicioLlaves;
		}

		[HttpGet]
		public async Task<List<LlaveDTO>> MisLlaves()
		{
			var usuarioId = ObtenerUsuarioId();
			var llaves = await context.LlavesAPI.Where(x => x.UsuarioId == usuarioId).ToListAsync();

			return mapper.Map<List<LlaveDTO>>(llaves);
		}
    }
}

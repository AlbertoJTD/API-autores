using System;
using System.Threading.Tasks;
using WebApiAutores.Entidades;

namespace WebApiAutores.Servicios
{
	public class ServicioLlaves
	{
		private readonly ApplicationDbContext context;

		public ServicioLlaves(ApplicationDbContext context)
        {
			this.context = context;
		}

		public async Task CrearLLave(string usuarioId, TipoLlave tipoLlave)
		{
			var llave = GenerarLlave();

			var llaveAPI = new LlaveAPI
			{
				Activa = true,
				Llave = llave,
				TipoLlave = tipoLlave,
				UsuarioId = usuarioId
			};

			context.Add(llaveAPI);
			await context.SaveChangesAsync();
		}

		public string GenerarLlave()
		{
			return Guid.NewGuid().ToString().Replace("-", "");
		}
	}
}

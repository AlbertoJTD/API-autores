using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WebApiAutores;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Middlewares
{
	public static class LimitarPeticionesMiddlewareExtensions
	{
		public static IApplicationBuilder UseLimitarPeticiones(this IApplicationBuilder app)
		{
			return app.UseMiddleware<LimitarPeticionesMiddleware>();
		}
	}
}

public class LimitarPeticionesMiddleware
{
	private readonly RequestDelegate siguiente;
	private readonly IConfiguration configuration;

	public LimitarPeticionesMiddleware(RequestDelegate siguiente, IConfiguration configuration)
    {
		this.siguiente = siguiente;
		this.configuration = configuration;
	}

	public async Task InvokeAsync(HttpContext httpContext, ApplicationDbContext context)
	{
		var limitarPeticionesConfiguracion = new LimitarPeticionesConfiguracion();
		configuration.GetRequiredSection("limitarPeticiones").Bind(limitarPeticionesConfiguracion);

		var ruta = httpContext.Request.Path.ToString();
		var rutaListaBlanca = limitarPeticionesConfiguracion.ListaBlancaRutas.Any(x => ruta.Contains(x));

		if (rutaListaBlanca)
		{
			await siguiente(httpContext);
			return;
		}

		var llaveStringValues = httpContext.Request.Headers["X-Api-Key"];

		if (llaveStringValues.Count == 0)
		{
			httpContext.Response.StatusCode = 400;
			await httpContext.Response.WriteAsync("Debe proveer la llave en la cabecera X-Api-Key");
			return;
		}

		if (llaveStringValues.Count > 1)
		{
			httpContext.Response.StatusCode = 400;
			await httpContext.Response.WriteAsync("Solo una llave debe estar presente");
			return;
		}

		var llave = llaveStringValues[0];
		var llaveDB = await context.LlavesAPI.FirstOrDefaultAsync(x => x.Llave == llave);

		if (llaveDB == null)
		{
			httpContext.Response.StatusCode = 400;
			await httpContext.Response.WriteAsync("La llave no existe");
			return;
		}

		if (!llaveDB.Activa)
		{
			httpContext.Response.StatusCode = 400;
			await httpContext.Response.WriteAsync("La llave se encuentra inactiva");
			return;
		}

		if (llaveDB.TipoLlave == TipoLlave.Gratuita)
		{
			var hoy = DateTime.Today;
			var manana = hoy.AddDays(1);
			var cantidadPeticionesRealizadasHoy = await context.Peticiones.CountAsync(x =>  x.LlaveId == llaveDB.Id && x.FechaPeticion >= hoy && x.FechaPeticion < manana);

			if (cantidadPeticionesRealizadasHoy >= limitarPeticionesConfiguracion.PeticionesPorDiaGratuito)
			{
				httpContext.Response.StatusCode = 429; // Too many requests
				await httpContext.Response.WriteAsync("Ha excedido el numero de peticiones por dia. Actualice su cuenta a Profesional para realizar mas peticiones");
				return;
			}
		}

		var peticion = new Peticion()
		{
			LlaveId = llaveDB.Id,
			FechaPeticion = DateTime.UtcNow
		};
		context.Add(peticion);
		await context.SaveChangesAsync();

		await siguiente(httpContext);
	}
}



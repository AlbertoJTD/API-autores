using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WebApiAutores;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Migrations;

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
		var llaveDB = await context.LlavesAPI.Include(x => x.RestriccionesDominio)
											 .Include(x => x.RestriccionesIP)
											 .Include(x => x.Usuario)
											 .FirstOrDefaultAsync(x => x.Llave == llave);

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
			var cantidadPeticionesRealizadasHoy = await context.Peticiones.CountAsync(x => x.LlaveId == llaveDB.Id && x.FechaPeticion >= hoy && x.FechaPeticion < manana);

			if (cantidadPeticionesRealizadasHoy >= limitarPeticionesConfiguracion.PeticionesPorDiaGratuito)
			{
				httpContext.Response.StatusCode = 429; // Too many requests
				await httpContext.Response.WriteAsync("Ha excedido el numero de peticiones por dia. Actualice su cuenta a Profesional para realizar mas peticiones");
				return;
			}
		}
		else if(llaveDB.Usuario.Deudor)
		{
			httpContext.Response.StatusCode = 400;
			await httpContext.Response.WriteAsync("El usuario tiene una deuda pendiente");
			return;
		}

		var superaRestricciones = PeticionSuperaAlgunaDeLasPeticiones(llaveDB, httpContext);

		if (!superaRestricciones)
		{
			httpContext.Response.StatusCode = 403;
			return;
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

	private bool PeticionSuperaAlgunaDeLasPeticiones(LlaveAPI llaveAPI, HttpContext httpContext)
	{
		var hayRestricciones = llaveAPI.RestriccionesDominio.Any() || llaveAPI.RestriccionesIP.Any();

		if (!hayRestricciones)
		{
			return true;
		}

		var peticionSuperaRestriccionesDominio = PeticionSuperaRestriccionesDominio(llaveAPI.RestriccionesDominio, httpContext);
		var peticionSuperaLasRestriccionesIP = PeticionSuperaLasRestriciconesIP(llaveAPI.RestriccionesIP, httpContext);

		return peticionSuperaRestriccionesDominio || peticionSuperaLasRestriccionesIP;
	}

	private bool PeticionSuperaRestriccionesDominio(List<RestriccionDominio> restricciones, HttpContext httpContext)
	{
		if (restricciones == null || restricciones.Count == 0)
		{
			return false;
		}

		var referer = httpContext.Request.Headers["Referer"].ToString();

		if (referer == string.Empty)
		{
			return false;
		}

		Uri myUri = new Uri(referer);
		string host = myUri.Host;

		var superaRestriccion = restricciones.Any(x => x.Dominio == host);
		return superaRestriccion;
	}

	private bool PeticionSuperaLasRestriciconesIP(List<RestriccionIP> restricciones, HttpContext httpContext)
	{
		if (restricciones == null || restricciones.Count == 0)
		{
			return false;
		}

		var IP = httpContext.Connection.RemoteIpAddress.ToString();

		if (IP == string.Empty)
		{
			return false;
		}

		var superaRestriccion = restricciones.Any(x => x.IP == IP);
		return superaRestriccion;
	}
}



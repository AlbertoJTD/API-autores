﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiAutores.DTOs;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController: ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "ObtenerRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatoHATEOAS>>> Get()
        {
            var datosHATEOAS = new List<DatoHATEOAS>();
            var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");

            datosHATEOAS.Add(new DatoHATEOAS(enlace: Url.Link("ObtenerRoot", new {}), descripcion: "self", metodo: "GET"));

            // ENDPOINT Autor
            datosHATEOAS.Add(new DatoHATEOAS(enlace: Url.Link("obtenerAutores", new { }), descripcion: "autores", metodo: "GET"));
            if (esAdmin.Succeeded)
            {
                datosHATEOAS.Add(new DatoHATEOAS(enlace: Url.Link("crearAutor", new { }), descripcion: "autor-crear", metodo: "POST"));

                // ENDPOINT Libro
                datosHATEOAS.Add(new DatoHATEOAS(enlace: Url.Link("crearLibro", new { }), descripcion: "libro-crear", metodo: "POST"));
            }

            return datosHATEOAS;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using WebApiAutores.DTOs;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api")]
    public class RootController: ControllerBase
    {
        [HttpGet(Name = "ObtenerRoot")]
        public ActionResult<IEnumerable<DatoHATEOAS>> Get()
        {
            var datosHATEOAS = new List<DatoHATEOAS>();
            datosHATEOAS.Add(new DatoHATEOAS(enlace: Url.Link("ObtenerRoot", new {}), descripcion: "self", metodo: "GET"));

            return datosHATEOAS;
        }
    }
}

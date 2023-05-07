using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebApiAutores.Entidades; // Tener acceso a la clase autor

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/autores")] // Este es el ENDPOINT
    public class AutoresController: ControllerBase
    {
        [HttpGet] //Accion
        public ActionResult<List<Autor>> Get() // Retorna un listado de 2 autores cuando se haga una peticion GET
        {
            return new List<Autor>() { 
                new Autor() { Id = 1, Nombre = "Felipe" },
                new Autor() { Id = 2, Nombre = "Juan" },
            };
        }
    }
}

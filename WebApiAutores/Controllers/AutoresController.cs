using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiAutores.Entidades; // Tener acceso a la clase autor
using WebApiAutores.Filtros;
using WebApiAutores.Servicios;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/autores")] // Este es el ENDPOINT
    // [Authorize] // Protege todos los endpoint
    public class AutoresController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IServicio servicio;
        private readonly ServicioTransient servicioTransient;
        private readonly ServicioScoped servicioScoped;
        private readonly ServicioSingleton servicioSingleton;
        private readonly ILogger<AutoresController> logger;

        public AutoresController(ApplicationDbContext context, IServicio servicio, ServicioTransient servicioTransient, ServicioScoped servicioScoped, ServicioSingleton servicioSingleton, ILogger<AutoresController> logger)
        {
            this.context = context;
            this.servicio = servicio;
            this.servicioTransient = servicioTransient;
            this.servicioScoped = servicioScoped;
            this.servicioSingleton = servicioSingleton;
            this.logger = logger;
        }

        [HttpGet("GUID")]
        //[ResponseCache(Duration = 10)] // Duracion de 10 seg.
        [ServiceFilter(typeof(MiFiltroDeAccion))]
        public ActionResult obtenerGuids()
        {
            return Ok(new {
                AutoresController_Transient = servicioTransient.Guid, // Son distintos
                servicioA_Transient = servicio.ObtenerTransient(),

                AutoresController_Scoped = servicioScoped.Guid, // son iguales pero cambia al ejecutarse nuevamente
                servicioA_Scoped = servicio.ObtenerScoped(),

                AutoresController_Singleton = servicioSingleton.Guid, // son iguales
                servicioA_Singleton = servicio.ObtenerSingleton()
            });
        }

        [HttpGet] //Accion
        [HttpGet("listado")] // 'api/autores/listado' o 'api/autores'
        [HttpGet("/listado")] // 'listado' - sobrescribe la base del endpoint
        //[Authorize] // Protege este endpoint
        [ServiceFilter(typeof(MiFiltroDeAccion))]
        public async Task<ActionResult<List<Autor>>> Get() // Retorna un listado de 2 autores cuando se haga una peticion GET
        {
            throw new System.NotImplementedException();
            logger.LogInformation("Estamos obteniendo los autores");
            return await context.Autores.Include(x => x.Libros).ToListAsync();
        }

        [HttpGet("GetFirstRecord")] // api/autores/GetFirstRecord?nombre=Juan
        public async Task<ActionResult<Autor>> PrimerAutor([FromHeader] int miValor, [FromQuery] string nombre)
        {
            return await context.Autores.FirstOrDefaultAsync();
        }

        [HttpGet("{id:int}/{param2?}")] // api/autores/1 --- '?' el signo de interrogacion indica que el opcional 'param2' --- param2=persona indica un valor por defecto
        public async Task<ActionResult<Autor>> Get(int id)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);
            if (autor == null)
            {
                return NotFound();
            }

            return autor;
        }

        [HttpGet("{nombre}")] // api/autores/juan
        public async Task<ActionResult<Autor>> Get(string nombre)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Nombre.Contains(nombre));
            if (autor == null)
            {
                return NotFound();
            }

            return autor;
        }

        [HttpPost]
        public async Task<ActionResult> Post(Autor autor)
        {
            var existeAutor = await context.Autores.AnyAsync(x => x.Nombre == autor.Nombre);

            if (existeAutor)
            {
                return BadRequest($"Ya existe un autor con el nombre {autor.Nombre}");
            }

            context.Add(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")] // Se combina la ruta del ENDPOINT + lo que se coloca aqui => api/autores/1
        public async Task<ActionResult> Put(Autor autor, int id)
        {
            if (autor.Id != id) { 
                return BadRequest("El ID del autor no coincide con el ID de la URL");
            }

            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Update(autor); // Actualiza el objeto
            await context.SaveChangesAsync(); // Se actualliza el registro en la DB
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(Autor autor, int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Autor() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}

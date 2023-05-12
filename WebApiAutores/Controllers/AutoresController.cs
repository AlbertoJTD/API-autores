using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades; // Tener acceso a la clase autor
using WebApiAutores.Filtros;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/autores")] // Este es el ENDPOINT
    // [Authorize] // Protege todos los endpoint
    public class AutoresController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public AutoresController(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        [HttpGet("configuraciones")]
        public ActionResult<string> ObtenerCConfiguracion()
        {
            // return configuration["connectionStrings:defaultConnection"];
            return configuration["apellido"];
        }

        [HttpGet] //Accion
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Protege este endpoint
        public async Task<List<AutorDTO>> Get() // Retorna un listado de 2 autores cuando se haga una peticion GET
        {
            var autores = await context.Autores.ToListAsync();
            return mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpGet("{id:int}", Name = "obtenerAutor")] // api/autores/1 --- '?' el signo de interrogacion indica que el opcional 'param2' --- param2=persona indica un valor por defecto
        public async Task<ActionResult<AutorDTOConLibros>> Get(int id)
        {
            var autor = await context.Autores.
                Include(autorDB => autorDB.AutoresLibros)
                .ThenInclude(autorLibroDB => autorLibroDB.Libro) // Acceder a los datos de los libros
                .FirstOrDefaultAsync(autorBD => autorBD.Id == id);
            if (autor == null)
            {
                return NotFound();
            }

            return mapper.Map<AutorDTOConLibros>(autor);
        }

        [HttpGet("{nombre}")] // api/autores/juan
        public async Task<ActionResult<List<AutorDTO>>> Get(string nombre)
        {
            var autor = await context.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync();
            if (autor == null)
            {
                return NotFound();
            }

            return mapper.Map<List<AutorDTO>>(autor);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO)
        {
            var existeAutor = await context.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre);

            if (existeAutor)
            {
                return BadRequest($"Ya existe un autor con el nombre {autorCreacionDTO.Nombre}");
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);

            context.Add(autor);
            await context.SaveChangesAsync();

            var autorDTO = mapper.Map<AutorDTO>(autor);
            return CreatedAtRoute("obtenerAutor", new { id = autor.Id}, autorDTO);
        }

        [HttpPut("{id:int}")] // Se combina la ruta del ENDPOINT + lo que se coloca aqui => api/autores/1
        public async Task<ActionResult> Put(AutorCreacionDTO autorCreacionDTO, int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);
            autor.Id = id;

            context.Update(autor); // Actualiza el objeto
            await context.SaveChangesAsync(); // Se actualliza el registro en la DB
            return NoContent();
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

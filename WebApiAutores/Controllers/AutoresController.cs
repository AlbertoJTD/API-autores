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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")] // Protege todos los endpoint
    public class AutoresController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAuthorizationService authorizationService;

        public AutoresController(ApplicationDbContext context, IMapper mapper, IAuthorizationService authorizationService)
        {
            this.context = context;
            this.mapper = mapper;
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "obtenerAutores")] //Accion
        [AllowAnonymous] // Permitir peticiones anonimas
        public async Task<IActionResult> Get([FromQuery] bool incluirHATEOAS = true) // Retorna un listado de 2 autores cuando se haga una peticion GET
        {
            var autores = await context.Autores.ToListAsync();
            var dtos = mapper.Map<List<AutorDTO>>(autores);
            
            if (incluirHATEOAS)
            {
                var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");
                dtos.ForEach(dto => GenerarEnlaces(dto, esAdmin.Succeeded));

                var resultado = new ColeccionDeRecursos<AutorDTO>() { Valores = dtos};
                resultado.Enlaces.Add(new DatoHATEOAS(enlace: Url.Link("obtenerAutores", new { }), descripcion: "self", metodo: "GET"));

                if (esAdmin.Succeeded)
                {
                    resultado.Enlaces.Add(new DatoHATEOAS(enlace: Url.Link("crearAutor", new { }), descripcion: "crear-autor", metodo: "POST"));
                }

                return Ok(resultado);
            }

            
            return Ok(dtos);
        }

        [HttpGet("{id:int}", Name = "obtenerAutor")] // api/autores/1 --- '?' el signo de interrogacion indica que el opcional 'param2' --- param2=persona indica un valor por defecto
        [AllowAnonymous]
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

            var dto =  mapper.Map<AutorDTOConLibros>(autor);
            var esAdmin = await authorizationService.AuthorizeAsync(User, "edAdmin");
            GenerarEnlaces(dto, esAdmin.Succeeded);
            return dto;
        }

        private void GenerarEnlaces(AutorDTO autorDTO, bool esAdmin)
        {
            autorDTO.Enlaces.Add(new DatoHATEOAS(
                enlace: Url.Link("obtenerAutor", new {id = autorDTO.Id}),
                descripcion: "self",
                metodo: "GET"
            ));

            if (esAdmin)
            {
                autorDTO.Enlaces.Add(new DatoHATEOAS(
                    enlace: Url.Link("actualizarAutor", new { id = autorDTO.Id }),
                    descripcion: "autor-actualizar",
                    metodo: "PUT"
                ));

                autorDTO.Enlaces.Add(new DatoHATEOAS(
                    enlace: Url.Link("eliminarAutor", new { id = autorDTO.Id }),
                    descripcion: "self",
                    metodo: "DELETE"
                ));
            }

        }

        [HttpGet("{nombre}", Name = "obtenerAutorPorNombre")] // api/autores/juan
        public async Task<ActionResult<List<AutorDTO>>> Get(string nombre)
        {
            var autor = await context.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync();
            if (autor == null)
            {
                return NotFound();
            }

            return mapper.Map<List<AutorDTO>>(autor);
        }

        [HttpPost(Name = "crearAutor")]
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

        [HttpPut("{id:int}", Name = "actualizarAutor")] // Se combina la ruta del ENDPOINT + lo que se coloca aqui => api/autores/1
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

        [HttpDelete("{id:int}", Name = "eliminarAutor")]
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

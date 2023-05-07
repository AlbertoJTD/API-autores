using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiAutores.Entidades; // Tener acceso a la clase autor

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/autores")] // Este es el ENDPOINT
    public class AutoresController: ControllerBase
    {
        private readonly ApplicationDbContext context;

        public AutoresController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet] //Accion
        public async Task<ActionResult<List<Autor>>> Get() // Retorna un listado de 2 autores cuando se haga una peticion GET
        {
            return await context.Autores.Include(x => x.Libros).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post(Autor autor)
        {
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

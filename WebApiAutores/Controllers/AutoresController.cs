﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public AutoresController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet] //Accion
        //[Authorize] // Protege este endpoint
        public async Task<ActionResult<List<Autor>>> Get() // Retorna un listado de 2 autores cuando se haga una peticion GET
        {
            return await context.Autores.ToListAsync();
        }

        [HttpGet("{id:int}")] // api/autores/1 --- '?' el signo de interrogacion indica que el opcional 'param2' --- param2=persona indica un valor por defecto
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
        public async Task<ActionResult> Post([FromBody] Autor autor)
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

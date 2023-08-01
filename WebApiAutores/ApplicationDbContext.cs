using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebApiAutores.Entidades;

namespace WebApiAutores
{
    public class ApplicationDbContext : IdentityDbContext // Herada de DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AutorLibro>().HasKey(al => new {al.AutorId, al.LibroId}); // Llave primaria compuesta
        }

        public DbSet<Autor> Autores { get; set; } // Autores sera el nombre de la tabla, creara la tabla con las propiedades de la clase Autor
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<AutorLibro> AutoresLibros { get; set; }
        public DbSet<LlaveAPI> LlavesAPI { get; set; }
		public DbSet<Peticion> Peticiones { get; set; }
	}
}

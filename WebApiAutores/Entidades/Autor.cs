using System.Collections.Generic;

namespace WebApiAutores.Entidades
{
    public class Autor
    {
        public int Id { get; set; } // Propiedad Id
        public string Nombre { get; set; } // Propiedad Nombre
        public List<Libro> Libros { get; set; }
    }
}

using System.Collections.Generic;

namespace WebApiAutores.DTOs
{
    public class AutorDTOConLibros: AutorDTO
    {
        public List<LibroDTO> Libros { get; set; }
    }
}

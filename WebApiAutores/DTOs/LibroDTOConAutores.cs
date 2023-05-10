using System.Collections.Generic;

namespace WebApiAutores.DTOs
{
    public class LibroDTOConAutores: LibroDTO
    {
        public List<AutorDTO> Autores { get; set; }
    }
}

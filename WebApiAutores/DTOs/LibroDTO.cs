using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTOs
{
    public class LibroDTO
    {
        // Lista de atributos que seran mostrados al hacer la peticion al endpoint
        public int Id { get; set; }
        public string Titulo { get; set; }
        public List<ComentarioDTO> Comentarios { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
    public class Autor
    {
        public int Id { get; set; } // Propiedad Id

        [Required(ErrorMessage = "El campo nombre es requerido")] // El atributo nombre es requerido
        [StringLength(maximumLength: 120, ErrorMessage = "El campo {0} no debe de tener mas de {1} caracteres")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; } // Propiedad Nombre

        public List<AutorLibro> AutoresLibros { get; set; }
    }
}

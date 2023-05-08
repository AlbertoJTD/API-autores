using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiAutores.Entidades
{
    public class Autor
    {
        public int Id { get; set; } // Propiedad Id

        [Required(ErrorMessage = "El campo nombre es requerido")] // El atributo nombre es requerido
        [StringLength(maximumLength:5, ErrorMessage = "El campo {0} no debe de tener mas de {1} caracteres")]
        public string Nombre { get; set; } // Propiedad Nombre

        [Range(18, 100)]
        [NotMapped] // Propiedades que no van a corresponder con una columna en la BD
        public int Edad { get; set; }

        [CreditCard]
        [NotMapped]
        public string TarjetaDeCredito { get; set; }

        [Url]
        [NotMapped]
        public string URL { get; set; }

        public List<Libro> Libros { get; set; }
    }
}

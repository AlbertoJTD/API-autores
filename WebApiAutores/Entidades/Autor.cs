using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
    public class Autor: IValidatableObject
    {
        public int Id { get; set; } // Propiedad Id

        [Required(ErrorMessage = "El campo nombre es requerido")] // El atributo nombre es requerido
        [StringLength(maximumLength:5, ErrorMessage = "El campo {0} no debe de tener mas de {1} caracteres")]
        //[PrimeraLetraMayuscula]
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

        public int Menor { get; set; }
        public int Mayor { get; set; }

        public List<Libro> Libros { get; set; }


        // Interfaz para validalidaciones
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validar que la primera letra sea mayuscula para el campo Nombre
            if (!string.IsNullOrEmpty(Nombre))
            {
                var primeraLetra = Nombre[0].ToString();

                if (primeraLetra != primeraLetra.ToUpper())
                {
                    // Se retorna el mensaje de error y el campo en que ha ocurrido dicho error
                    yield return new ValidationResult("La primera letra debe ser mayuscula", new string[] {nameof(Nombre)});
                }
            }

            // Validar que la el campo Menor sea mas pequeño que Mayor
            if (Menor > Mayor)
            {
                yield return new ValidationResult("Este valor no puede ser mas grande que el campo mayor", new string[] { nameof(Menor) });
            }
        }
    }
}

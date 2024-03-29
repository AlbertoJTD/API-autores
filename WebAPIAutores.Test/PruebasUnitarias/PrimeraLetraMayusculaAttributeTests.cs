using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebAPIAutores.Test.PruebasUnitarias
{
    [TestClass]
    public class PrimeraLetraMayusculaAttributeTests
    {
        [TestMethod] // Decorador
        public void FirstLowerCaseLetter_ReturnsAnError()
        {
            // Preparacion
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            var valor = "john";
            var valContext = new ValidationContext(new { Nombre = valor });

            // Verificacion
            var result = primeraLetraMayuscula.GetValidationResult(valor, valContext);

            // Ejecucion
            Assert.AreEqual("La primera letra debe ser mayuscula", result.ErrorMessage);
		}

		[TestMethod] // Decorador
		public void NullValue_ReturnsSuccess()
		{
			// Preparacion
			var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
			string valor = null;
			var valContext = new ValidationContext(new { Nombre = valor });

			// Verificacion
			var result = primeraLetraMayuscula.GetValidationResult(valor, valContext);

			// Ejecucion
			Assert.IsNull(result);
		}

		[TestMethod] // Decorador
		public void FirstCapitalLetter_DoesNotReturnError()
		{
			// Preparacion
			var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
			var valor = "John";
			var valContext = new ValidationContext(new { Nombre = valor });

			// Verificacion
			var result = primeraLetraMayuscula.GetValidationResult(valor, valContext);

			// Ejecucion
			Assert.IsNull(result);
		}
	}
}
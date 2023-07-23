using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiAutores.Controllers.V1;
using WebAPIAutores.Test.Mocks;

namespace WebAPIAutores.Test.PruebasUnitarias
{
	[TestClass]
	public class RootControllerTests
	{
		[TestMethod]
		public async Task IfUserIsAdmin_Get4Links()
		{
			// Preparacion
			var authorizationService = new AuthorizationServiceMock();
			authorizationService.Resultado = AuthorizationResult.Success();
			var rootController = new RootController(authorizationService);
			rootController.Url = new URLHelperMock();

			// Ejecucion
			var result = await rootController.Get();

			// Verificacion
			Assert.AreEqual(4, result.Value.Count());
		}

		[TestMethod]
		public async Task IfUserIsNotAdmin_Get2Links()
		{
			// Preparacion
			var authorizationService = new AuthorizationServiceMock();
			authorizationService.Resultado = AuthorizationResult.Failed();
			var rootController = new RootController(authorizationService);
			rootController.Url = new URLHelperMock();

			// Ejecucion
			var result = await rootController.Get();

			// Verificacion
			Assert.AreEqual(2, result.Value.Count());
		}
	}
}

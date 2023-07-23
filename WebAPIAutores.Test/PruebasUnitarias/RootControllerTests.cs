using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

		[TestMethod]
		public async Task IfUserIsNotAdmin_Get2Links_UsingMoq()
		{
			// Preparacion
			var mockAuthorizationService = new Mock<IAuthorizationService>();
			mockAuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(),
											It.IsAny<object>(),
											It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
											.Returns(Task.FromResult(AuthorizationResult.Failed()));

			mockAuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(),
											It.IsAny<object>(),
											It.IsAny<string>()))
											.Returns(Task.FromResult(AuthorizationResult.Failed()));

			var mockURLHelper = new Mock<IUrlHelper>();
			mockURLHelper.Setup(x => x.Link(It.IsAny<string>(),
											It.IsAny<object>())).
											Returns(string.Empty);

			var rootController = new RootController(mockAuthorizationService.Object);
			rootController.Url = mockURLHelper.Object;

			// Ejecucion
			var result = await rootController.Get();

			// Verificacion
			Assert.AreEqual(2, result.Value.Count());
		}
	}
}

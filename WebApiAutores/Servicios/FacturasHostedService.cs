using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiAutores.Servicios
{
	public class FacturasHostedService : IHostedService
	{
		private readonly IServiceProvider serviceProvider;
		private Timer timer;

		public FacturasHostedService(IServiceProvider serviceProvider)
        {
			this.serviceProvider = serviceProvider;
		}

        public Task StartAsync(CancellationToken cancellationToken)
		{
			timer = new Timer(ProcesarFacturas, null, TimeSpan.Zero, TimeSpan.FromDays(1));

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}

		private void ProcesarFacturas(object state)
		{
			using (var scope = serviceProvider.CreateScope())
			{
				var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

				EstablecerDeudor(context);
				EmitirFacturas(context);
			}
		}

		private static void EstablecerDeudor(ApplicationDbContext context)
		{
			context.Database.ExecuteSqlRaw("exec EstablecerDeudor");
		}

		private static void EmitirFacturas(ApplicationDbContext context)
		{
			var hoy = DateTime.Today;
			var fechaComparacion = hoy.AddMonths(-1);
			var facturasDelMesYaFueronEmitidas = context.FacturasEmitidas.Any(x => x.Año == fechaComparacion.Year && x.Mes == fechaComparacion.Month);

			if (!facturasDelMesYaFueronEmitidas)
			{
				var fechaInicio = new DateTime(fechaComparacion.Year, fechaComparacion.Month, 1);
				var fechaFin = fechaInicio.AddMonths(1);
				context.Database.ExecuteSqlInterpolated($"exec CreacionFacturas {fechaInicio.ToString("yyyy-MM-dd")}, {fechaFin.ToString("yyyy-MM-dd")}");
			}
		}
	}
}

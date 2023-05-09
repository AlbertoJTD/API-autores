using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json.Serialization;
using WebApiAutores.Middlewares;
using WebApiAutores.Servicios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApiAutores.Filtros;

namespace WebApiAutores
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers(opciones => {
                opciones.Filters.Add(typeof(FiltroDeExcepcion));
            }).AddJsonOptions(x => 
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
            );
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("defaultConnection"))
            );

            // Si una clase requiere un IServicio pasale el ServicioA
            services.AddTransient<IServicio, ServicioA>();

            services.AddTransient<ServicioTransient>(); // Se usa si en que en la clase se especifica que requiere este tipo
            services.AddScoped<ServicioScoped>(); // Se tendran distintas instancias del mismo servicio
            services.AddSingleton<ServicioSingleton>(); // Se comparte la misma instancia para todas las peticiones
            services.AddTransient<MiFiltroDeAccion>();
            services.AddHostedService<EscribirEnArchivo>();

            services.AddResponseCaching();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            // Guardar el Log de todas las respuestas HTTP
            //app.UseMiddleware<LoguearRespuestaHTTPMiddleware>();
            app.UseLoguearRespuestaHTTP();


            app.Map("/ruta1", app =>
            {
                app.Run(async contexto => { 
                    await contexto.Response.WriteAsync("Estoy interceptando la tuberia");
                });
            });

            if (env.IsDevelopment())
            {
                // Los que tienen al inicio la palabra 'Use' son los middlewares
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseResponseCaching();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}

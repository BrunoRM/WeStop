using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using WeStop.Api.Extensions;
using WeStop.Api.Filters;
using WeStop.Api.Infra.Hubs;
using WeStop.Application.Behaviors;

namespace WeStop.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => 
            {
                options.Filters.Add(new ExceptionFilter());
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

            services.AddCors(options => options.AddPolicy("CorsPolicy",
                builder =>
                {
                    builder.AllowAnyMethod().AllowAnyHeader()
                           .WithOrigins("http://localhost:5001")
                           .AllowCredentials();
                }));

            var assembly = AppDomain.CurrentDomain.Load("WeStop.Application");

            services.AddMediatR(assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

            // Para adicionar as classes de validação ao container de dependências
            AssemblyScanner
                .FindValidatorsInAssembly(assembly)
                .ForEach(result => services.AddScoped(result.InterfaceType, result.ValidatorType));

            services.AddSignalR();
            services.AddServices();
            services.AddAutoMapper(assembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("CorsPolicy");

            app.UseSignalR(routeConfig =>
            {
                routeConfig.MapHub<GameRoomHub>("/gameroom");
            });

            app.UseMvc();
        }
    }
}

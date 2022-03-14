using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Mustaxe.Application.Common;
using Mustaxe.Application.Common.Behaviors;
using Mustaxe.Application.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using Mustaxe.Application.Common.Extensions;
using FluentValidation.AspNetCore;
using Mustaxe.Application.Services.CEP.Validators;
using Mustaxe.Integration.Configurations;
using Mustaxe.Application.Services.CEP.Queries;
using System.Reflection;
using Mustaxe.API.Extensions;
using Mustaxe.Persistence;
using Microsoft.EntityFrameworkCore;
using Mustaxe.Infra.Configurations.MessageBroker;

namespace Mustaxe.API
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
            services.AddControllers();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SqlServer")));

            services.AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<ConsultarCEPValidator>());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorPipelineBehavior<,>));
            services.AddMediatR(typeof(ConsultarCEPQuery).GetTypeInfo().Assembly);
            services.RegisterMapping();

            services.AddRepositoryServices();
            services.AddInfraServices();
            services.AddMessageConsumerServices();

            services.Configure<ApiBehaviorOptions>(options =>
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState);
                    throw new InputValidationException(problemDetails.Errors);
                }
            );

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mustaxe.API", Version = "v1" });
            });

            services.Configure<IntegrationSettings>(Configuration.GetSection("Integration"));
            services.Configure<RabbitMQSettings>(Configuration.GetSection("RabbitMQConfiguration"));

            services.RegisterHttpClients(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mustaxe.API v1"));
            }

            app.ConfigureExceptionHandler();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

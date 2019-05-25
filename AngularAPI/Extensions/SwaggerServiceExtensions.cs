﻿using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace AngularAPI.Extensions
{
    /// <summary>
    /// Extension <see cref="AddSwaggerDocumentation"/> method or middleware for <see cref="SwaggerServiceExtensions"/>.
    /// Swagger configuration in asp.net core for swagger version > 2.0.
    /// Reference From : https://ppolyzos.com/2017/10/30/add-jwt-bearer-authorization-to-swagger-and-asp-net-core/
    /// </summary>
    public static class SwaggerServiceExtensions
    {
        // AddSwaggerDocumentation service collection method
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1.0", new Info { Title = "AngularApp API v1.0", Version = "v1.0" });

                //Locate the XML file being generated by ASP.NET...
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.XML";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (File.Exists(xmlPath))
                {
                    // Tell Swagger to use those XML comments.
                    c.IncludeXmlComments(xmlPath);
                }

                // Swagger 2.+ support.
                // Create security definition dictionary with key pair: Dictionary<string, ICollection<string>>
                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

                // Must be required for swagger version > 2.0
                c.AddSecurityRequirement(security);
            });
            return services;
        }

        // UseSwaggerDocumentation app builder method
        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app, IConfiguration config)
        {
            //var appEndpoint = config["AppBaseApiUrl:AppBaseEndpoint"];
            //var swaggerEndpoint = appEndpoint + "/swagger-ui";

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "Swagger UI v1.0");
                c.DocumentTitle = "Swagger UI v1.0";
                //Reference link : https://stackoverflow.com/questions/22008452/collapse-expand-swagger-response-model-class
                //Reference link : https://swagger.io/docs/open-source-tools/swagger-ui/usage/deep-linking/
                //  c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                // c.DocExpansion(DocExpansion.Full);
                //    //Reference document: https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-2.2&tabs=visual-studio
                //    //To serve the Swagger UI at the app's root (http://localhost:<port>/), set the RoutePrefix property to an empty string:
                c.RoutePrefix = string.Empty;
                //c.RoutePrefix = swaggerEndpoint;
            });
            return app;
        }
    }
}
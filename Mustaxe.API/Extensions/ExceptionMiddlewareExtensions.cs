using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Mustaxe.Application.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mustaxe.Application.Common.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app) 
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = errorFeature.Error;

                    var problemDetails = new ProblemDetails
                    {
                        Title = "Um ou mais erros de validacao ocorreram",
                        Detail = "A requisicao contem parametros invalidos. Seguem alguns desses errors"
                    };

                    switch (exception)
                    {
                        case InputValidationException validationException:
                            problemDetails.Status = StatusCodes.Status400BadRequest;
                            problemDetails.Extensions["errors"] = validationException.Errors;
                            break;
                        case Exception simplException:
                            problemDetails.Title = "Um erro inesperado aconteceu";
                            problemDetails.Detail = "Esse erro nao foi tratado previamente e merece ser investigado";
                            problemDetails.Status = StatusCodes.Status500InternalServerError;
                            problemDetails.Extensions["errors"] = $"{exception.Message}. {exception.InnerException ?? null}";
                            problemDetails.Extensions["tracing"] = Activity.Current?.Id ?? context?.TraceIdentifier;
                            problemDetails.Instance = errorFeature switch
                            {
                                ExceptionHandlerFeature e => e.Path,
                                _ => "desconhecido"
                            };
                            break;
                    }

                    context.Response.ContentType = "application/problem+json";
                    context.Response.StatusCode = problemDetails.Status.Value;
                    context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
                    {
                        NoCache = true,
                    };

                    await JsonSerializer.SerializeAsync(context.Response.Body, problemDetails);
                });
            });
        }
    }
}

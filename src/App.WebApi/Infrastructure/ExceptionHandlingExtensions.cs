using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.WebApi.Infrastructure;

public static class ExceptionHandlingExtensions
{
    public static void UseGlobalExceptionHandling(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(builder =>
        {
            builder.Run(async context =>
            {
                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

                var problem = new ProblemDetails
                {
                    Title = "Произошла непредвиденная ошибка",
                    Detail = exceptionHandlerFeature?.Error.Message,
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "https://httpstatuses.io/500"
                };

                context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(problem).ConfigureAwait(false);
            });
        });
    }
}

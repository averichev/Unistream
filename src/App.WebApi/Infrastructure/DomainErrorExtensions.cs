using App.Domain.Errors;
using Microsoft.AspNetCore.Mvc;

namespace App.WebApi.Infrastructure;

public static class DomainErrorExtensions
{
    public static ActionResult ToActionResult(this ControllerBase controller, DomainError error)
    {
        return error switch
        {
            ValidationFailedError validation => controller.ValidationProblem(new ValidationProblemDetails(
                validation.Issues
                    .GroupBy(issue => issue.Field)
                    .ToDictionary(group => group.Key, group => group.Select(i => i.Message).ToArray()))
            {
                Title = "Ошибка валидации",
                Status = StatusCodes.Status400BadRequest,
                Type = "https://httpstatuses.io/400"
            }),
            ConflictError conflict => controller.Conflict(new ProblemDetails
            {
                Title = "Конфликт",
                Detail = conflict.Message,
                Status = StatusCodes.Status409Conflict,
                Type = "https://httpstatuses.io/409"
            }),
            NotFoundError notFound => controller.NotFound(new ProblemDetails
            {
                Title = "Не найдено",
                Detail = notFound.Message,
                Status = StatusCodes.Status404NotFound,
                Type = "https://httpstatuses.io/404"
            }),
            _ => controller.StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Произошла непредвиденная ошибка",
                Detail = error.Message,
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://httpstatuses.io/500"
            })
        };
    }
}

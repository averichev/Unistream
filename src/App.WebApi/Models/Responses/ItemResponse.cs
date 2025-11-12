namespace App.WebApi.Models.Responses;

public sealed record ItemResponse(Guid Id, string Name, decimal Price, DateTime CreatedAt);

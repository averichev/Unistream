namespace App.WebApi.Models.Responses;

public sealed record ItemListResponse(IReadOnlyList<ItemResponse> Items, int Skip, int Take);

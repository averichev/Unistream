using App.Domain.Entities;
using App.WebApi.Models.Responses;

namespace App.WebApi.Infrastructure;

public static class ItemMapper
{
    public static ItemResponse ToResponse(this Item item)
        => new(item.Id, item.Name, item.Price, item.CreatedAt);
}

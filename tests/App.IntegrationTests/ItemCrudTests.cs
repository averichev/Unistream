using System.Net;
using System.Net.Http.Json;
using App.WebApi.Models.Requests;
using App.WebApi.Models.Responses;

namespace App.IntegrationTests;

public sealed class ItemCrudTests : IClassFixture<AppWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ItemCrudTests(AppWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ItemCrudLifecycle_WorksAsExpected()
    {
        var createRequest = new CreateItemRequest("Test Item", 15.5m);
        var createResponse = await _client.PostAsJsonAsync("/api/v1/items", createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdItem = await createResponse.Content.ReadFromJsonAsync<ItemResponse>();
        Assert.NotNull(createdItem);
        Assert.Equal(createRequest.Name.Trim(), createdItem!.Name);
        Assert.Equal(createRequest.Price, createdItem.Price);

        var getResponse = await _client.GetAsync($"/api/v1/items/{createdItem.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var fetchedItem = await getResponse.Content.ReadFromJsonAsync<ItemResponse>();
        Assert.NotNull(fetchedItem);
        Assert.Equal(createdItem.Id, fetchedItem!.Id);

        var updateRequest = new UpdateItemRequest("Updated Item", 20m);
        var updateResponse = await _client.PutAsJsonAsync($"/api/v1/items/{createdItem.Id}", updateRequest);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updatedItem = await updateResponse.Content.ReadFromJsonAsync<ItemResponse>();
        Assert.NotNull(updatedItem);
        Assert.Equal(updateRequest.Name.Trim(), updatedItem!.Name);
        Assert.Equal(updateRequest.Price, updatedItem.Price);

        var listResponse = await _client.GetAsync("/api/v1/items?skip=0&take=10");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var list = await listResponse.Content.ReadFromJsonAsync<ItemListResponse>();
        Assert.NotNull(list);
        Assert.Contains(list!.Items, item => item.Id == createdItem.Id);

        var deleteResponse = await _client.DeleteAsync($"/api/v1/items/{createdItem.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getAfterDeleteResponse = await _client.GetAsync($"/api/v1/items/{createdItem.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getAfterDeleteResponse.StatusCode);
    }
}

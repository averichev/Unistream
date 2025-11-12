using App.Domain.Abstractions;
using App.Domain.Errors;
using App.Domain.Models;
using App.Domain.Services;
using NSubstitute;

namespace App.Domain.Tests;

public sealed class ItemServiceTests
{
    private readonly IItemRepository _repository = Substitute.For<IItemRepository>();
    private readonly ItemService _service;

    public ItemServiceTests()
    {
        _service = new ItemService(_repository);
    }

    [Fact]
    public async Task CreateAsync_ReturnsSuccess_WhenModelIsValid()
    {
        var model = new CreateItemModel("Valid name", 10m);
        _repository.ExistsByNameAsync(Arg.Any<string>(), null, Arg.Any<CancellationToken>()).Returns(false);

        var result = await _service.CreateAsync(model, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(model.Name, result.Value!.Name);
        Assert.Equal(model.Price, result.Value!.Price);
    }

    [Fact]
    public async Task CreateAsync_ReturnsValidationError_WhenNameIsEmpty()
    {
        var model = new CreateItemModel(string.Empty, 10m);

        var result = await _service.CreateAsync(model, CancellationToken.None);

        Assert.False(result.IsSuccess);
        var error = Assert.IsType<ValidationFailedError>(result.Error);
        Assert.Contains(error.Issues, issue => issue.Field == nameof(model.Name));
    }

    [Fact]
    public async Task CreateAsync_ReturnsValidationError_WhenNameIsTooLong()
    {
        var model = new CreateItemModel(new string('a', 101), 10m);

        var result = await _service.CreateAsync(model, CancellationToken.None);

        Assert.False(result.IsSuccess);
        var error = Assert.IsType<ValidationFailedError>(result.Error);
        Assert.Contains(error.Issues, issue => issue.Field == nameof(model.Name));
    }

    [Fact]
    public async Task CreateAsync_ReturnsValidationError_WhenPriceIsNotPositive()
    {
        var model = new CreateItemModel("Valid", 0m);

        var result = await _service.CreateAsync(model, CancellationToken.None);

        Assert.False(result.IsSuccess);
        var error = Assert.IsType<ValidationFailedError>(result.Error);
        Assert.Contains(error.Issues, issue => issue.Field == nameof(model.Price));
    }

    [Fact]
    public async Task CreateAsync_ReturnsConflict_WhenNameAlreadyExists()
    {
        var model = new CreateItemModel("Duplicate", 10m);
        _repository.ExistsByNameAsync(model.Name, null, Arg.Any<CancellationToken>()).Returns(true);

        var result = await _service.CreateAsync(model, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.IsType<ConflictError>(result.Error);
    }
}

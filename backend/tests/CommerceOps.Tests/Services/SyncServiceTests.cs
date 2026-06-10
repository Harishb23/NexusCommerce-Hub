using AutoMapper;
using CommerceOps.API.Interfaces;
using CommerceOps.API.Mappings;
using CommerceOps.API.Models;
using CommerceOps.API.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace CommerceOps.Tests.Services;

public class SyncServiceTests
{
    private readonly Mock<IAmazonChannelService> _amazonMock = new();
    private readonly Mock<INoonChannelService> _noonMock = new();
    private readonly Mock<IOrderRepository> _orderRepoMock = new();
    private readonly Mock<IProductRepository> _productRepoMock = new();
    private readonly Mock<ISyncLogRepository> _syncLogRepoMock = new();
    private readonly Mock<IIntegrationHealthRepository> _healthRepoMock = new();
    private readonly IMapper _mapper;
    private readonly SyncService _sut;

    public SyncServiceTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _syncLogRepoMock.Setup(r => r.AddAsync(It.IsAny<SyncLog>(), default)).Returns(Task.CompletedTask);
        _syncLogRepoMock.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);
        _healthRepoMock.Setup(r => r.UpsertAsync(It.IsAny<IntegrationHealth>(), default)).Returns(Task.CompletedTask);
        _healthRepoMock.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);
        _orderRepoMock.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);
        _productRepoMock.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

        _sut = new SyncService(
            _amazonMock.Object,
            _noonMock.Object,
            _orderRepoMock.Object,
            _productRepoMock.Object,
            _syncLogRepoMock.Object,
            _healthRepoMock.Object,
            _mapper,
            NullLogger<SyncService>.Instance);
    }

    [Fact]
    public async Task SyncAllChannelsAsync_ReturnsSuccess_WhenBothChannelsSucceed()
    {
        var orders = new List<Order>
        {
            new() { ExternalOrderId = "AMZ-1", Channel = "Amazon", CustomerName = "C1", TotalAmount = 100m, Status = OrderStatus.Pending }
        };
        var products = new List<Product>
        {
            new() { ExternalProductId = "NOON-1", Channel = "Noon", Name = "Product 1", Price = 50m, Stock = 20 }
        };

        _amazonMock.Setup(s => s.FetchOrdersAsync(default)).ReturnsAsync((orders, string.Empty));
        _noonMock.Setup(s => s.FetchProductsAsync(default)).ReturnsAsync((products, string.Empty));
        _orderRepoMock.Setup(r => r.GetByExternalIdAsync("AMZ-1", "Amazon", default)).ReturnsAsync((Order?)null);
        _orderRepoMock.Setup(r => r.AddAsync(It.IsAny<Order>(), default)).Returns(Task.CompletedTask);
        _productRepoMock.Setup(r => r.GetByExternalIdAsync("NOON-1", "Noon", default)).ReturnsAsync((Product?)null);
        _productRepoMock.Setup(r => r.AddAsync(It.IsAny<Product>(), default)).Returns(Task.CompletedTask);

        var result = await _sut.SyncAllChannelsAsync();

        result.Success.Should().BeTrue();
        result.OrdersSynced.Should().Be(1);
        result.ProductsSynced.Should().Be(1);
    }

    [Fact]
    public async Task SyncChannelAsync_ReturnsFailure_WhenAmazonFails()
    {
        _amazonMock.Setup(s => s.FetchOrdersAsync(default)).ReturnsAsync((new List<Order>(), "Connection timeout"));
        _healthRepoMock.Setup(r => r.GetByChannelAsync("Amazon", default)).ReturnsAsync((IntegrationHealth?)null);

        var result = await _sut.SyncChannelAsync("amazon");

        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Connection timeout");
    }

    [Fact]
    public async Task SyncChannelAsync_ReturnsFailure_ForUnknownChannel()
    {
        var result = await _sut.SyncChannelAsync("UnknownChannel");

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Unknown channel");
    }

    [Fact]
    public async Task RetrySyncAsync_ReturnsNotFound_WhenSyncLogMissing()
    {
        _syncLogRepoMock.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((SyncLog?)null);

        var result = await _sut.RetrySyncAsync(999);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task SyncChannelAsync_SkipsDuplicateOrders()
    {
        var orders = new List<Order>
        {
            new() { ExternalOrderId = "AMZ-1", Channel = "Amazon", CustomerName = "C1", TotalAmount = 100m, Status = OrderStatus.Pending }
        };
        var existingOrder = new Order { Id = 1, ExternalOrderId = "AMZ-1", Channel = "Amazon" };

        _amazonMock.Setup(s => s.FetchOrdersAsync(default)).ReturnsAsync((orders, string.Empty));
        _orderRepoMock.Setup(r => r.GetByExternalIdAsync("AMZ-1", "Amazon", default)).ReturnsAsync(existingOrder);
        _healthRepoMock.Setup(r => r.GetByChannelAsync("Amazon", default)).ReturnsAsync((IntegrationHealth?)null);

        var result = await _sut.SyncChannelAsync("amazon");

        result.Success.Should().BeTrue();
        result.OrdersSynced.Should().Be(0);
        _orderRepoMock.Verify(r => r.AddAsync(It.IsAny<Order>(), default), Times.Never);
    }
}

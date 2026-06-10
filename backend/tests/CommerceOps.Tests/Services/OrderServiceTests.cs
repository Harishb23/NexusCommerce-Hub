using AutoMapper;
using CommerceOps.API.DTOs;
using CommerceOps.API.Interfaces;
using CommerceOps.API.Mappings;
using CommerceOps.API.Models;
using CommerceOps.API.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace CommerceOps.Tests.Services;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepoMock = new();
    private readonly IMapper _mapper;
    private readonly OrderService _sut;

    public OrderServiceTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        _sut = new OrderService(_orderRepoMock.Object, _mapper, NullLogger<OrderService>.Instance);
    }

    [Fact]
    public async Task GetOrderByIdAsync_ReturnsDto_WhenOrderExists()
    {
        var order = new Order
        {
            Id = 1,
            ExternalOrderId = "AMZ-1",
            Channel = "Amazon",
            CustomerName = "Customer 42",
            TotalAmount = 99.99m,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        _orderRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(order);

        var result = await _sut.GetOrderByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Channel.Should().Be("Amazon");
        result.Status.Should().Be(OrderStatus.Pending);
    }

    [Fact]
    public async Task GetOrderByIdAsync_ReturnsNull_WhenOrderNotFound()
    {
        _orderRepoMock.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((Order?)null);

        var result = await _sut.GetOrderByIdAsync(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_ThrowsArgumentException_ForInvalidStatus()
    {
        var act = async () => await _sut.UpdateOrderStatusAsync(1, "InvalidStatus");

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Invalid status*");
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_ThrowsKeyNotFoundException_WhenOrderNotFound()
    {
        _orderRepoMock.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((Order?)null);

        var act = async () => await _sut.UpdateOrderStatusAsync(999, OrderStatus.Delivered);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_UpdatesStatus_WhenOrderExists()
    {
        var order = new Order
        {
            Id = 5,
            ExternalOrderId = "AMZ-5",
            Channel = "Amazon",
            CustomerName = "Customer 1",
            TotalAmount = 150m,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        _orderRepoMock.Setup(r => r.GetByIdAsync(5, default)).ReturnsAsync(order);
        _orderRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Order>(), default)).Returns(Task.CompletedTask);
        _orderRepoMock.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

        var result = await _sut.UpdateOrderStatusAsync(5, OrderStatus.Delivered);

        result.Status.Should().Be(OrderStatus.Delivered);
        _orderRepoMock.Verify(r => r.UpdateAsync(It.Is<Order>(o => o.Status == OrderStatus.Delivered), default), Times.Once);
        _orderRepoMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task GetOrdersAsync_ReturnsPaginatedResult()
    {
        var orders = Enumerable.Range(1, 5).Select(i => new Order
        {
            Id = i,
            ExternalOrderId = $"AMZ-{i}",
            Channel = "Amazon",
            CustomerName = $"Customer {i}",
            TotalAmount = i * 10m,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        _orderRepoMock
            .Setup(r => r.GetPagedAsync(It.IsAny<OrderSearchParams>(), default))
            .ReturnsAsync(new PagedResult<Order>(orders, 5, 1, 20));

        var result = await _sut.GetOrdersAsync(new OrderSearchParams(null, null, null));

        result.Items.Should().HaveCount(5);
        result.Total.Should().Be(5);
    }
}

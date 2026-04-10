﻿using Moq;
using Xunit;
using Banking.Core.Entities;
using Banking.Core.Interfaces;
using Banking.Core.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace Banking.Tests;

public class AccountServiceTests
{
    private readonly Mock<IAccountRepository> _mockRepo;
    private readonly AccountService _service;

    public AccountServiceTests()
    {
        _mockRepo = new Mock<IAccountRepository>();
        _service = new AccountService(_mockRepo.Object, NullLogger<AccountService>.Instance);
    }

    [Fact]
    public async Task DepositAsync_ValidRequest_ReturnsSuccess()
    {
        var account = new Account { Id = 17, CustomerId = 5, Balance = 100m };
        
        _mockRepo.Setup(r => r.GetByIdAsync(17, It.IsAny<CancellationToken>())).ReturnsAsync(account);

        var result = await _service.DepositAsync(5, 17, 50m);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
        Assert.Equal(150m, result.Data.Balance);
        
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DepositAsync_WrongCustomer_ReturnsFailure()
    {
        var account = new Account { Id = 17, CustomerId = 99, Balance = 100m };

        _mockRepo.Setup(r => r.GetByIdAsync(17, It.IsAny<CancellationToken>())).ReturnsAsync(account);

        var result = await _service.DepositAsync(5, 17, 50m);

        Assert.False(result.Succeeded);
        Assert.Contains("access denied", result.ErrorMessage?.ToLower());
    }

    [Fact]
    public async Task DepositAsync_NonExistentAccount_ReturnsFailure()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(17, It.IsAny<CancellationToken>())).ReturnsAsync((Account?)null);

        var result = await _service.DepositAsync(5, 17, 50m);

        Assert.False(result.Succeeded);
        Assert.Contains("not found", result.ErrorMessage?.ToLower());
    }

    [Fact]
    public async Task DepositAsync_NegativeAmount_ReturnsFailure()
    {
        var account = new Account { Id = 1, CustomerId = 4, Balance = 100m };

        _mockRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(account);

        var result = await _service.DepositAsync(4, 1, -50m);

        Assert.False(result.Succeeded);
        Assert.Contains("must be positive", result.ErrorMessage?.ToLower());
    }
}
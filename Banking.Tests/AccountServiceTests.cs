using Moq;
using Xunit;
using Banking.Core.Entities;
using Banking.Core.Interfaces;
using Banking.Core.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace Banking.Tests;

public class AccountServiceTests
{
    private readonly Mock<IAccountRepository> _mockRepo;
    private readonly Mock<ICustomerRepository> _mockCustomerRepo;
    private readonly AccountService _service;

    public AccountServiceTests()
    {
        _mockRepo = new Mock<IAccountRepository>();
        _mockCustomerRepo = new Mock<ICustomerRepository>();
        _service = new AccountService(
                _mockRepo.Object,
                _mockCustomerRepo.Object,
                new NullLogger<AccountService>());
    }

    #region Deposit Tests

    [Fact]
    public async Task DepositAsync_ValidRequest_ReturnsSuccess()
    {
        var account = new Account(17, 5, 100m, AccountType.Savings);

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
        var account = new Account(17, 99, 100m, AccountType.Savings);

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
        var account = new Account(1, 4, 100m, AccountType.Savings);

        _mockRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(account);

        var result = await _service.DepositAsync(4, 1, -50m);

        Assert.False(result.Succeeded);
        Assert.Contains("must be positive", result.ErrorMessage?.ToLower());
    }
    #endregion

    #region Withdrawal Tests
    [Fact]
    public async Task WithdrawAsync_ValidRequest_ReturnsSuccess()
    {
        var account = new Account(17, 5, 100m, AccountType.Savings);

        _mockRepo.Setup(r => r.GetByIdAsync(17, It.IsAny<CancellationToken>())).ReturnsAsync(account);

        var result = await _service.WithdrawalAsync(5, 17, 50m);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
        Assert.Equal(50m, result.Data.Balance);

        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WithdrawAsync_WrongCustomer_ReturnsFailure()
    {
        var account = new Account(17, 99, 100m, AccountType.Savings);

        _mockRepo.Setup(r => r.GetByIdAsync(17, It.IsAny<CancellationToken>())).ReturnsAsync(account);

        var result = await _service.WithdrawalAsync(5, 17, 50m);

        Assert.False(result.Succeeded);
        Assert.Contains("access denied", result.ErrorMessage?.ToLower());
    }

    [Fact]
    public async Task WithdrawAsync_NonExistentAccount_ReturnsFailure()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(17, It.IsAny<CancellationToken>())).ReturnsAsync((Account?)null);

        var result = await _service.WithdrawalAsync(5, 17, 50m);

        Assert.False(result.Succeeded);
        Assert.Contains("not found", result.ErrorMessage?.ToLower());
    }

    [Fact]
    public async Task WithdrawAsync_NegativeAmount_ReturnsFailure()
    {
        var account = new Account(1, 4, 100m, AccountType.Savings);

        _mockRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(account);

        var result = await _service.WithdrawalAsync(4, 1, -50m);

        Assert.False(result.Succeeded);
        Assert.Contains("must be positive", result.ErrorMessage?.ToLower());
    }

    [Fact]
    public async Task WithdrawAsync_InsufficientFunds_ReturnsFailure()
    {
        var account = new Account(1, 4, 30m, AccountType.Savings);

        _mockRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(account);

        var result = await _service.WithdrawalAsync(4, 1, 50m);

        Assert.False(result.Succeeded);
        Assert.Contains("insufficient", result.ErrorMessage?.ToLower());
    }
    #endregion

    #region Close Account Tests

    [Fact]
    public async Task CloseAccountAsync_ValidRequest_ReturnsSuccess()
    {
        var account = new Account(17, 5, 0m, AccountType.Savings);

        _mockRepo.Setup(r => r.GetByIdAsync(17, It.IsAny<CancellationToken>())).ReturnsAsync(account);

        var result = await _service.CloseAsync(5, 17);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.IsClosed);

        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CloseAccountAsync_WrongCustomer_ReturnsFailure()
    {
        var account = new Account(17, 99, 0m, AccountType.Savings);

        _mockRepo.Setup(r => r.GetByIdAsync(17, It.IsAny<CancellationToken>())).ReturnsAsync(account);

        var result = await _service.CloseAsync(5, 17);

        Assert.False(result.Succeeded);
        Assert.Contains("access denied", result.ErrorMessage?.ToLower());
    }

    [Fact]
    public async Task CloseAccountAsync_NonExistentAccount_ReturnsFailure()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(17, It.IsAny<CancellationToken>())).ReturnsAsync((Account?)null);

        var result = await _service.CloseAsync(5, 17);

        Assert.False(result.Succeeded);
        Assert.Contains("not found", result.ErrorMessage?.ToLower());
    }

    [Fact]
    public async Task CloseAccountAsync_NonZeroBalance_ReturnsFailure()
    {
        var account = new Account(17, 5, 50m, AccountType.Savings);

        _mockRepo.Setup(r => r.GetByIdAsync(17, It.IsAny<CancellationToken>())).ReturnsAsync(account);

        var result = await _service.CloseAsync(5, 17);

        Assert.False(result.Succeeded);
        Assert.Contains("balance must be zero", result.ErrorMessage?.ToLower());
    }

    #endregion

    #region Open Account Tests

    [Fact]
    public async Task OpenAccountAsync_FirstAccountSavings_ReturnsSuccess()
    {
        _mockCustomerRepo.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockRepo.Setup(r => r.GetCustomerAccountsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Account>());
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>())).ReturnsAsync((Account a, CancellationToken ct) => a);
        var result = await _service.OpenAsync(1, 100m, (int)AccountType.Savings);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
        Assert.Equal(100m, result.Data.Balance);
        Assert.Equal(AccountType.Savings, result.Data.AccountType);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task OpenAccountAsync_CustomerNotFound_ReturnsFailure()
    {
        _mockCustomerRepo.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var result = await _service.OpenAsync(1, 100m, (int)AccountType.Savings);

        Assert.False(result.Succeeded);
        Assert.Contains("customer not found", result.ErrorMessage?.ToLower());
    }

    [Fact]
    public async Task OpenAccountAsync_InsufficientInitialDeposit_ReturnsFailure()
    {
        _mockCustomerRepo.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var result = await _service.OpenAsync(1, 5m, (int)AccountType.Savings);

        Assert.False(result.Succeeded);
        Assert.Contains("initial deposit", result.ErrorMessage?.ToLower());
    }

    [Fact]
    public async Task OpenAccountAsync_InvalidAccountType_ReturnsFailure()
    {
        _mockCustomerRepo.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var result = await _service.OpenAsync(1, 100m, 999);

        Assert.False(result.Succeeded);
        Assert.Contains("invalid account type", result.ErrorMessage?.ToLower());
    }

    [Fact]
    public async Task OpenAccountAsync_FirstAccountNotSavings_ReturnsFailure()
    {
        _mockCustomerRepo.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockRepo.Setup(r => r.GetCustomerAccountsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Account>());
        var result = await _service.OpenAsync(1, 100m, (int)AccountType.Checking);

        Assert.False(result.Succeeded);
        Assert.Contains("first account must be a savings account", result.ErrorMessage?.ToLower());
    }

    [Fact]
    public async Task OpenAccountAsync_CheckingAccountWhenHasAccounts_ReturnsSuccess()
    {
        var existingAccount = new Account(1, 1, 100m, AccountType.Savings);
        _mockRepo.Setup(r => r.GetCustomerAccountsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Account> { existingAccount });
        _mockCustomerRepo.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>())).ReturnsAsync((Account a, CancellationToken ct) => a);
        var result = await _service.OpenAsync(1, 100m, (int)AccountType.Checking);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
        Assert.Equal(100m, result.Data.Balance);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

}
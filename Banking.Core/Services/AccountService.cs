using Banking.Core.Common;
using Banking.Core.Entities;
using Banking.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Banking.Core.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _repository;
    private readonly ILogger<AccountService> _logger;

    public AccountService(IAccountRepository repository, ILogger<AccountService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<OperationResult<Account>> DepositAsync(int customerId, int accountId, decimal amount, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting deposit of {Amount} into Account {AccountId}", amount, accountId);

        var account = await _repository.GetByIdAsync(accountId, cancellationToken);

        if (account == null)
        {
            _logger.LogWarning("Deposit failed: Account {AccountId} not found", accountId);
            return OperationResult<Account>.Failure("Account not found.");
        }

        if (account.CustomerId != customerId)
        {
            _logger.LogWarning("Security Alert: Customer {CustomerId} attempted to access Account {AccountId} owned by {ActualOwner}", 
                customerId, accountId, account.CustomerId);
            return OperationResult<Account>.Failure("Access denied.");
        }

        try
        {
            account.Deposit(amount);

            await _repository.UpdateAsync(account, cancellationToken);

            _logger.LogInformation("Deposit successful for Account {AccountId}. New Balance: {Balance}", accountId, account.Balance);
            return OperationResult<Account>.Success(account);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Domain validation failed for Account {AccountId}: {Message}", accountId, ex.Message);
            return OperationResult<Account>.Failure(ex.Message);
        }
    }
}
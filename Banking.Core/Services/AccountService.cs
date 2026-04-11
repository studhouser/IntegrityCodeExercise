using Banking.Core.Common;
using Banking.Core.Entities;
using Banking.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Banking.Core.Services;

/// <summary>
/// Service class responsible for handling business logic related to account operations such as 
/// deposits and withdrawals. It interacts with the repository layer to perform data access and 
/// manipulation, and uses logging to track important events and potential issues throughout the 
/// process. The service ensures that all operations are validated according to business
/// rules and returns structured results indicating success or failure of the operations.
/// </summary>
public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<AccountService> _logger;

    public AccountService(IAccountRepository accountRepository, ICustomerRepository customerRepository, ILogger<AccountService> logger)
    {
        _accountRepository = accountRepository;
        _customerRepository = customerRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles deposit operations for a customer's account. Validates the request, 
    /// checks for account existence and ownership, performs the deposit, and returns the result 
    /// of the operation. Logs all relevant information and warnings throughout the process.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="accountId"></param>
    /// <param name="amount"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<OperationResult<Account>> DepositAsync(int customerId, int accountId, decimal amount, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting deposit of {Amount} into Account {AccountId}", amount, accountId);

        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);

        if (account == null || account.IsClosed)
        {
            _logger.LogWarning("Deposit failed: Account {AccountId} not found or closed", accountId);
            return OperationResult<Account>.Failure("Account not found or closed.");
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

            await _accountRepository.UpdateAsync(account, cancellationToken);

            _logger.LogInformation("Deposit successful for Account {AccountId}. New Balance: {Balance}", accountId, account.Balance);
            return OperationResult<Account>.Success(account);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Domain validation failed for Account {AccountId}: {Message}", accountId, ex.Message);
            return OperationResult<Account>.Failure(ex.Message);
        }
    }

    /// <summary>
    /// Handles withdrawal operations for a customer's account. Validates the request, checks for 
    /// account existence and ownership, performs the withdrawal, and returns the result of the operation. 
    /// Logs all relevant information and warnings throughout the process.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="accountId"></param>
    /// <param name="amount"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<OperationResult<Account>> WithdrawalAsync(int customerId, int accountId, decimal amount, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting withdrawal of {Amount} from Account {AccountId}", amount, accountId);

        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);

        if (account == null || account.IsClosed)
        {
            _logger.LogWarning("Withdrawal failed: Account {AccountId} not found or closed", accountId);
            return OperationResult<Account>.Failure("Account not found or closed.");
        }

        if (account.CustomerId != customerId)
        {
            _logger.LogWarning("Security Alert: Customer {CustomerId} attempted to access Account {AccountId} owned by {ActualOwner}",
                customerId, accountId, account.CustomerId);
            return OperationResult<Account>.Failure("Access denied.");
        }

        try
        {
            account.Withdrawal(amount);

            await _accountRepository.UpdateAsync(account, cancellationToken);

            _logger.LogInformation("Withdrawal successful for Account {AccountId}. New Balance: {Balance}", accountId, account.Balance);
            return OperationResult<Account>.Success(account);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Domain validation failed for Account {AccountId}: {Message}", accountId, ex.Message);
            return OperationResult<Account>.Failure(ex.Message);
        }
    }

    /// <summary>
    /// Handles closing a customer's account. Validates the request, checks for account existence 
    /// and ownership, and closes the account if all validations pass.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="accountId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<OperationResult<Account>> CloseAsync(int customerId, int accountId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to close Account {AccountId}", accountId);

        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);

        if (account == null)
        {
            _logger.LogWarning("Close failed: Account {AccountId} not found", accountId);
            return OperationResult<Account>.Failure("Account not found.");
        }

        if (account.IsClosed)
        {
            _logger.LogWarning("Close failed: Account {AccountId} is already closed", accountId);
            return OperationResult<Account>.Failure("Account is already closed.");
        }

        if (account.CustomerId != customerId)
        {
            _logger.LogWarning("Security Alert: Customer {CustomerId} attempted to close Account {AccountId} owned by {ActualOwner}",
                customerId, accountId, account.CustomerId);
            return OperationResult<Account>.Failure("Access denied.");
        }

        try
        {
            account.Close();

            await _accountRepository.UpdateAsync(account, cancellationToken);

            _logger.LogInformation("Account {AccountId} successfully closed", accountId);
            return OperationResult<Account>.Success(account);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Domain validation failed for Account {AccountId}: {Message}", accountId, ex.Message);
            return OperationResult<Account>.Failure(ex.Message);
        }
    }

    /// <summary>
    /// Handles opening a new account for a customer. Validates the request, 
    /// checks for customer existence, and creates a new account if all validations pass. 
    /// The first account for a customer is required to be a savings account.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="initialDeposit"></param>
    /// <param name="accountTypeId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<OperationResult<Account>> OpenAsync(int customerId, decimal initialDeposit, int accountTypeId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to open account for Customer {CustomerId} with type {AccountTypeId} and initial deposit {InitialDeposit}", customerId, accountTypeId, initialDeposit);

        if (!Enum.IsDefined(typeof(AccountType), accountTypeId))
        {
            _logger.LogWarning("Account opening failed: Invalid account type {AccountTypeId} for Customer {CustomerId}", accountTypeId, customerId);
            return OperationResult<Account>.Failure("Invalid Account Type.");
        }

        var customerExists = await _customerRepository.ExistsAsync(customerId, cancellationToken);

        if (!customerExists)
        {
            _logger.LogWarning("Account opening failed: Customer {CustomerId} not found", customerId);
            return OperationResult<Account>.Failure("Customer not found.");
        }

        var customerAccounts = await _accountRepository.GetCustomerAccountsAsync(customerId, cancellationToken);

        try
        {
            var account = Account.CreateNew(customerId, initialDeposit, (AccountType)accountTypeId, !customerAccounts.Any());

            var created = await _accountRepository.AddAsync(account, cancellationToken);
            return OperationResult<Account>.Success(created);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Domain validation failed for Customer {CustomerId} when opening account: {Message}", customerId, ex.Message);
            return OperationResult<Account>.Failure(ex.Message);
        }
    }
}
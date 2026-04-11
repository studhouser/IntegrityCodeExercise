using Banking.Core.Common;
using Banking.Core.Entities;

namespace Banking.Core.Interfaces;

public interface IAccountService
{
    Task<OperationResult<Account>> DepositAsync(int customerId, int accountId, decimal amount, CancellationToken cancellationToken = default);
    Task<OperationResult<Account>> WithdrawalAsync(int customerId, int accountId, decimal amount, CancellationToken cancellationToken = default);
    Task<OperationResult<Account>> CloseAsync(int customerId, int accountId, CancellationToken cancellationToken = default);
    Task<OperationResult<Account>> OpenAsync(int customerId, decimal initialDeposit, int accountTypeId, CancellationToken cancellationToken = default);
}

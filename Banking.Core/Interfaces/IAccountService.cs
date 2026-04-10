using Banking.Core.Common;
using Banking.Core.Entities;

namespace Banking.Core.Interfaces;

public interface IAccountService
{
    Task<OperationResult<Account>> DepositAsync(int customerId, int accountId, decimal amount, CancellationToken cancellationToken = default);
}

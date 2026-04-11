using Banking.Core.Entities;

namespace Banking.Core.Interfaces;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(int accountId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Account account, CancellationToken cancellationToken = default);
    Task<IEnumerable<Account>> GetCustomerAccountsAsync(int customerId, CancellationToken cancellationToken = default);
    Task<Account> AddAsync(Account account, CancellationToken cancellationToken = default);
}
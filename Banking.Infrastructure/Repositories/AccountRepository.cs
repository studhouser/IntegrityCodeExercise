using Banking.Core.Entities;
using Banking.Core.Interfaces;
using Banking.Infrastructure.Data;

namespace Banking.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    public async Task<Account?> GetByIdAsync(int accountId, CancellationToken cancellationToken = default)
    {
        // async database call simulation
        await Task.Delay(50, cancellationToken);
        var account = MockDataStore.Accounts.FirstOrDefault(a => a.Id == accountId);

        return account;
    }

    public Task UpdateAsync(Account account, CancellationToken cancellationToken = default)
    {
        // In a real implementation, this would involve an async database call to update the account record.
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Account>> GetCustomerAccountsAsync(int customerId, CancellationToken cancellationToken = default)
    {
        // async database call simulation
        await Task.Delay(50, cancellationToken);
        return MockDataStore.Accounts.Where(a => a.CustomerId == customerId).ToList();
    }

    public async Task<Account> AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        // async database call simulation
        await Task.Delay(50, cancellationToken);

        var id = MockDataStore.GetNextAccountId();

        var created = new Account(id, account.CustomerId, account.Balance, account.AccountType);

        MockDataStore.Accounts.Add(created);

        return created;
    }
}
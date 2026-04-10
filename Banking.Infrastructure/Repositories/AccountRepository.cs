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
        MockDataStore.Accounts.TryGetValue(accountId, out var account);

        return account;
    }

    public Task UpdateAsync(Account account, CancellationToken cancellationToken = default)
    {
        // In a real implementation, this would involve an async database call to update the account record.
        return Task.CompletedTask;
    }
}
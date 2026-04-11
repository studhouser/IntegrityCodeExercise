using Banking.Core.Interfaces;
using Banking.Infrastructure.Data;

namespace Banking.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    public async Task<bool> ExistsAsync(int customerId, CancellationToken cancellationToken = default)
    {
        // async database call simulation
        await Task.Delay(50, cancellationToken);
        return MockDataStore.Customers.Any(c => c.Id == customerId);
    }
}
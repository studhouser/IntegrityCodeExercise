using Banking.Core.Entities;

namespace Banking.Core.Interfaces;

public interface ICustomerRepository
{
    Task<bool> ExistsAsync(int customerId, CancellationToken cancellationToken = default);
}
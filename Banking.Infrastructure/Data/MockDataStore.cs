using System.Collections.Concurrent;
using Banking.Core.Entities;

namespace Banking.Infrastructure.Data;

public static class MockDataStore
{
    public static ConcurrentDictionary<int, Account> Accounts = new()
    {
        [17] = new Account { Id = 17, CustomerId = 5, Balance = 2175.13m },
        [18] = new Account { Id = 18, CustomerId = 6, Balance = 500.00m },
        [19] = new Account { Id = 19, CustomerId = 5, Balance = 10.00m },
        [20] = new Account { Id = 20, CustomerId = 7, Balance = 1000.00m },
        [21] = new Account { Id = 21, CustomerId = 8, Balance = 750.50m }
    };
}
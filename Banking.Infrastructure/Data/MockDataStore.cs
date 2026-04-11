using System.Collections.Concurrent;
using Banking.Core.Entities;

namespace Banking.Infrastructure.Data;

public static class MockDataStore
{
    // Use a static constructor to seed the data once
    static MockDataStore()
    {
        var john = Customer.Create(1, "John Smith");
        var johnAccount1 = new Account(1, 1, 2287.13m, AccountType.Savings);
        var johnAccount2 = new Account(2, 1, 500.00m, AccountType.Savings);
        Customers.Add(john);
        Accounts.Add(johnAccount1);
        Accounts.Add(johnAccount2);

        var jane = Customer.Create(2, "Jane Doe");
        var janeAccount1 = new Account(3, 2, 10.00m, AccountType.Checking);
        Customers.Add(jane);
        Accounts.Add(janeAccount1);

        var bob = Customer.Create(3, "Bob Johnson");
        Customers.Add(bob);
    }

    public static List<Customer> Customers { get; } = new();
    public static List<Account> Accounts { get; } = new();

    public static int GetNextAccountId() 
    {
        return Accounts.Any() ? Accounts.Max(a => a.Id) + 1 : 1;
    }
}
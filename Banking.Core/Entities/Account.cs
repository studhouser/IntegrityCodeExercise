namespace Banking.Core.Entities;

public class Account
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public decimal Balance { get; set; }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Deposit amount must be positive.");

        Balance += amount;
    }
}
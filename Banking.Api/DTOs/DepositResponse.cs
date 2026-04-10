namespace Banking.Api.DTOs;

public class DepositResponse
{
    public int CustomerId { get; set; }
    public int AccountId { get; set; }
    public decimal Balance { get; set; }
    public bool Succeeded { get; set; }
}
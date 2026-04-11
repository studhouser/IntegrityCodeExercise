using System.Text.Json.Serialization;

namespace Banking.Api.DTOs.Deposit;

public record DepositRequest(
    [property: JsonPropertyName("customerId")] int CustomerId,
    [property: JsonPropertyName("accountId")] int AccountId,
    [property: JsonPropertyName("amount")] decimal Amount
);
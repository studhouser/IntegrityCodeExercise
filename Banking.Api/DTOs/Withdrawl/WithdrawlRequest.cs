using System.Text.Json.Serialization;

namespace Banking.Api.DTOs.Withdrawal;

public record WithdrawalRequest(
    [property: JsonPropertyName("customerId")] int CustomerId,
    [property: JsonPropertyName("accountId")] int AccountId,
    [property: JsonPropertyName("amount")] decimal Amount
);
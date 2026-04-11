using System.Text.Json.Serialization;

namespace Banking.Api.DTOs.Withdrawal;

public record WithdrawalResponse(
    [property: JsonPropertyName("customerId")] int CustomerId,
    [property: JsonPropertyName("accountId")] int AccountId,
    [property: JsonPropertyName("balance")] decimal Balance,
    [property: JsonPropertyName("succeeded")] bool Succeeded
);
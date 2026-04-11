using System.Text.Json.Serialization;

namespace Banking.Api.DTOs.Open;

public record OpenResponse(
    [property: JsonPropertyName("customerId")] int CustomerId,
    [property: JsonPropertyName("accountId")] int AccountId,
    [property: JsonPropertyName("accountTypeId")] int AccountTypeId,
    [property: JsonPropertyName("balance")] decimal Balance,
    [property: JsonPropertyName("succeeded")] bool Succeeded
);
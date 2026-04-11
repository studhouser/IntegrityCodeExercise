using System.Text.Json.Serialization;

namespace Banking.Api.DTOs.Close;

public record CloseResponse(
    [property: JsonPropertyName("customerId")] int CustomerId,
    [property: JsonPropertyName("accountId")] int AccountId,
    [property: JsonPropertyName("succeeded")] bool Succeeded
);
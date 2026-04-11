using System.Text.Json.Serialization;

namespace Banking.Api.DTOs.Close;

public record CloseRequest(
    [property: JsonPropertyName("customerId")] int CustomerId,
    [property: JsonPropertyName("accountId")] int AccountId
);
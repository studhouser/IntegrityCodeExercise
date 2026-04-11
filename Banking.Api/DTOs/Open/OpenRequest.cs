using System.Text.Json.Serialization;

namespace Banking.Api.DTOs.Open;

public record OpenRequest(
    [property: JsonPropertyName("customerId")] int CustomerId,
    [property: JsonPropertyName("initialDeposit")] decimal InitialDeposit,
    [property: JsonPropertyName("accountTypeId")] int AccountTypeId
);
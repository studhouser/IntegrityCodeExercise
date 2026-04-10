using Banking.Api.DTOs;
using Banking.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Banking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly ILogger<AccountsController> _logger;
    private readonly IAccountService _accountService;

    public AccountsController(ILogger<AccountsController> logger, IAccountService accountService)
    {
        _logger = logger;
        _accountService = accountService;
    }

    [HttpPost(nameof(Deposit))] 
    public async Task<IActionResult> Deposit([FromBody] DepositRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received deposit request for Account {AccountId} by Customer {CustomerId}", 
            request.AccountId, request.CustomerId);

        var result = await _accountService.DepositAsync(request.CustomerId, request.AccountId, request.Amount, cancellationToken);

        if (!result.Succeeded || result.Data == null)
        {
            _logger.LogWarning("Deposit failed for Account {AccountId}: {Reason}", 
                request.AccountId, result.ErrorMessage);
                
            if (result.ErrorMessage != null && result.ErrorMessage.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(new { Succeeded = false, Message = result.ErrorMessage });
            }
            if (result.ErrorMessage != null && result.ErrorMessage.Contains("Access denied", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Succeeded = false, Message = result.ErrorMessage });
            }

            return BadRequest(new { Succeeded = false, Message = result.ErrorMessage });
        }


        _logger.LogInformation("Successfully processed deposit of {Amount} for Account {AccountId}", 
            request.Amount, request.AccountId);

        // Map the result to the response DTO (in a real project, I might use AutoMapper or similar)
        var response = new DepositResponse
        {
            CustomerId = result.Data.CustomerId,
            AccountId = result.Data.Id,
            Balance = result.Data.Balance,
            Succeeded = true
        };

        return Ok(response);
    }
}
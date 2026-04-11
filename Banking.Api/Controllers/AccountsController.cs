using Banking.Api.DTOs.Close;
using Banking.Api.DTOs.Deposit;
using Banking.Api.DTOs.Open;
using Banking.Api.DTOs.Withdrawal;
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

    /// <summary>
    /// Endpoint to handle deposits into a customer's account. Validates the request, interacts with the service layer, and returns appropriate HTTP responses based on the outcome of the operation.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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
        var response = new DepositResponse(result.Data.CustomerId, result.Data.Id, result.Data.Balance, true);

        return Ok(response);
    }

    /// <summary>
    /// Endpoint to handle withdrawals from a customer's account. Validates the request, interacts with the service layer, and returns appropriate HTTP responses based on the outcome of the operation.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost(nameof(Withdrawal))]
    public async Task<IActionResult> Withdrawal([FromBody] WithdrawalRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received withdraw request for Account {AccountId} by Customer {CustomerId}", 
            request.AccountId, request.CustomerId);

        var result = await _accountService.WithdrawalAsync(request.CustomerId, request.AccountId, request.Amount, cancellationToken);

        if (!result.Succeeded || result.Data == null)
        {
            _logger.LogWarning("Withdraw failed for Account {AccountId}: {Reason}", 
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

        _logger.LogInformation("Successfully processed withdraw of {Amount} for Account {AccountId}", 
            request.Amount, request.AccountId);

        var response = new WithdrawalResponse(result.Data.CustomerId, result.Data.Id, result.Data.Balance, true);

        return Ok(response);
    }

    /// <summary>
    /// Endpoint to handle closing a customer's account. Validates the request, interacts with the 
    /// service layer, and returns appropriate HTTP responses based on the outcome of the operation.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost(nameof(Close))]
    public async Task<IActionResult> Close([FromBody] CloseRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received close request for Account {AccountId} by Customer {CustomerId}", 
            request.AccountId, request.CustomerId);

        var result = await _accountService.CloseAsync(request.CustomerId, request.AccountId, cancellationToken);

        if (!result.Succeeded || result.Data == null)
        {
            _logger.LogWarning("Close failed for Account {AccountId}: {Reason}", 
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

        _logger.LogInformation("Successfully closed Account {AccountId}", request.AccountId);

        var response = new CloseResponse(result.Data.CustomerId, result.Data.Id, true);

        return Ok(response);
    }

    /// <summary>
    /// Endpoint to handle opening a new account for a customer. Validates the request, 
    /// interacts with the service layer, and returns appropriate HTTP responses based on 
    /// the outcome of the operation.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost(nameof(Open))]
    public async Task<IActionResult> Open([FromBody] OpenRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received open request for Customer {CustomerId}", request.CustomerId);

        var result = await _accountService.OpenAsync(request.CustomerId, request.InitialDeposit, request.AccountTypeId, cancellationToken);

        if (!result.Succeeded || result.Data == null)
        {
            _logger.LogWarning("Open failed for Customer {CustomerId}: {Reason}", 
                request.CustomerId, result.ErrorMessage);
                
            if (result.ErrorMessage != null && result.ErrorMessage.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(new { Succeeded = false, Message = result.ErrorMessage });
            }

            return BadRequest(new { Succeeded = false, Message = result.ErrorMessage });
        }

        _logger.LogInformation("Successfully opened Account {AccountId} for Customer {CustomerId}", 
            result.Data.Id, request.CustomerId);

        var response = new OpenResponse(result.Data.CustomerId, result.Data.Id, (int)result.Data.AccountType, result.Data.Balance, true);

        return Ok(response);
    }
}
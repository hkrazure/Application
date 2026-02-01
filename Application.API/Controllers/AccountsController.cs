
using API.Dtos;
using Application.Accounts.Commands;
using Application.Accounts.Queries;
using Application.Accounts.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/v{v:apiVersion}/accounts")]
[ApiVersion("1.0")]
[ApiController]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}/balance")]
    public async Task<ActionResult<GetBalanceResponse>> GetAccountBalance(Guid id, CancellationToken ct)
    {
        var query = new GetBalanceQuery(id);
        var response = await _mediator.Send(query, ct);

        if (response == null)
            return NotFound();  

        return Ok(response);   
    }

    [HttpPost]
    public async Task<ActionResult<CreateAccountResponse>> CreateAccount(CreateAccountCommand dto, CancellationToken ct)
    {
        var response = await _mediator.Send(dto, ct);

        return Ok(response);
    }

    [HttpPost("{id:guid}/deposits")]
    public async Task<IActionResult> Deposit(Guid id, DepositAmoundDto dto, CancellationToken ct)
    {
        var command = new DepositAmountCommand(id, dto.Amount.ToDomain());

        await _mediator.Send(command, ct);

        return NoContent();
    }

    [HttpPost("{id:guid}/transfers")]
    public async Task<IActionResult> Transfer(Guid id, TransferAmountDto dto, CancellationToken ct)
    {
        var command = new TransferAmountCommand(id, dto.ToAccountId, dto.Amount.ToDomain());

        await _mediator.Send(command, ct);

        return NoContent();
    }
}

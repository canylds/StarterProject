using Application.Features.Users.Commands.Create;
using Application.Features.Users.Commands.Delete;
using Application.Features.Users.Commands.Update;
using Application.Features.Users.Commands.UpdateFromAuth;
using Application.Features.Users.Queries.GetById;
using Application.Features.Users.Queries.GetList;
using Core.Application.Requests;
using Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateUserCommand createUserCommand)
    {
        CreatedUserResponse result = await Mediator.Send(createUserCommand);

        return Created(uri: string.Empty, result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateUserCommand updateUserCommand)
    {
        UpdatedUserResponse result = await Mediator.Send(updateUserCommand);

        return Ok(result);
    }

    [HttpPut("FromAuth")]
    public async Task<IActionResult> UpdateFromAuth([FromBody] UpdateUserFromAuthCommand updateUserFromAuthCommand)
    {
        updateUserFromAuthCommand.Id = GetUserIdFromRequest();

        UpdatedUserFromAuthResponse result = await Mediator.Send(updateUserFromAuthCommand);

        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteUserCommand deleteUserCommand)
    {
        DeletedUserResponse result = await Mediator.Send(deleteUserCommand);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        GetByIdUserQuery getByIdUserQuery = new() 
        { 
            Id = id 
        };

        GetByIdUserResponse result = await Mediator.Send(getByIdUserQuery);

        return Ok(result);
    }

    [HttpGet("GetFromAuth")]
    public async Task<IActionResult> GetFromAuth()
    {
        GetByIdUserQuery getByIdUserQuery = new() 
        { 
            Id = GetUserIdFromRequest()
        };

        GetByIdUserResponse result = await Mediator.Send(getByIdUserQuery);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListUserQuery getListUserQuery = new() 
        {
            PageRequest = pageRequest
        };

        GetListResponse<GetListUserListItemDto> result = await Mediator.Send(getListUserQuery);

        return Ok(result);
    }
}

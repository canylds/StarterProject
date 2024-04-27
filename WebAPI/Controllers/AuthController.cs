using Application.Features.Auth.Commands.EnableEmailAuthenticator;
using Application.Features.Auth.Commands.EnableOtpAuthenticator;
using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.RefreshToken;
using Application.Features.Auth.Commands.Register;
using Application.Features.Auth.Commands.RevokeToken;
using Application.Features.Auth.Commands.VerifyEmailAuthenticator;
using Application.Features.Auth.Commands.VerifyOtpAuthenticator;
using Core.Application.Dtos;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]

public class AuthController : BaseController
{
    private readonly WebApiConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        const string configurationSection = "WebAPIConfiguration";
        _configuration =
            configuration.GetSection(configurationSection).Get<WebApiConfiguration>()
            ?? throw new NullReferenceException($"\"{configurationSection}\" section cannot found in configuration.");
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserForLoginDto userForLoginDto)
    {
        LoginCommand loginCommand = new()
        {
            UserForLoginDto = userForLoginDto,
            IpAddress = GetIpAddress()
        };

        LoggedResponse result = await Mediator.Send(loginCommand);

        if (result.RefreshToken is not null)
            SetRefreshTokenToCookie(result.RefreshToken);

        return Ok(result.ToHttpResponse());
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] UserForRegisterDto userForRegisterDto)
    {
        RegisterCommand registerCommand = new()
        {
            UserForRegisterDto = userForRegisterDto,
            IpAddress = GetIpAddress()
        };

        RegisteredResponse result = await Mediator.Send(registerCommand);

        SetRefreshTokenToCookie(result.RefreshToken);

        return Created(uri: string.Empty, result.AccessToken);
    }

    [HttpGet("RefreshToken")]
    public async Task<IActionResult> RefreshToken()
    {
        RefreshTokenCommand refreshTokenCommand = new()
        {
            RefreshToken = GetRefreshTokenFromCookies(),
            IpAddress = GetIpAddress()
        };

        RefreshedTokensResponse result = await Mediator.Send(refreshTokenCommand);

        SetRefreshTokenToCookie(result.RefreshToken);

        return Created(uri: "", result.AccessToken);
    }

    [HttpPut("RevokeToken")]
    public async Task<IActionResult> RevokeToken([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] string? refreshToken)
    {
        RevokeTokenCommand revokeTokenCommand = new()
        {
            Token = refreshToken ?? GetRefreshTokenFromCookies(),
            IpAddress = GetIpAddress()
        };

        RevokedTokenResponse result = await Mediator.Send(revokeTokenCommand);

        return Ok(result);
    }

    [HttpGet("EnableEmailAuthenticator")]
    public async Task<IActionResult> EnableEmailAuthenticator()
    {
        EnableEmailAuthenticatorCommand enableEmailAuthenticatorCommand = new()
        {
            UserId = GetUserIdFromRequest(),
            VerifyEmailUrlPrefix = $"{_configuration.ApiDomain}/Auth/VerifyEmailAuthenticator"
        };

        await Mediator.Send(enableEmailAuthenticatorCommand);

        return Ok();
    }

    [HttpGet("EnableOtpAuthenticator")]
    public async Task<IActionResult> EnableOtpAuthenticator()
    {
        EnableOtpAuthenticatorCommand enableOtpAuthenticatorCommand = new()
        {
            UserId = GetUserIdFromRequest()
        };

        EnabledOtpAuthenticatorResponse result = await Mediator.Send(enableOtpAuthenticatorCommand);

        return Ok(result);
    }

    [HttpGet("VerifyEmailAuthenticator")]
    public async Task<IActionResult> VerifyEmailAuthenticator([FromQuery] VerifyEmailAuthenticatorCommand verifyEmailAuthenticatorCommand)
    {
        await Mediator.Send(verifyEmailAuthenticatorCommand);
        return Ok();
    }

    [HttpPost("VerifyOtpAuthenticator")]
    public async Task<IActionResult> VerifyOtpAuthenticator([FromBody] string authenticatorCode)
    {
        VerifyOtpAuthenticatorCommand verifyEmailAuthenticatorCommand = new()
        {
            UserId = GetUserIdFromRequest(),
            ActivationCode = authenticatorCode
        };

        await Mediator.Send(verifyEmailAuthenticatorCommand);

        return Ok();
    }

    private string GetRefreshTokenFromCookies()
    {
        return Request.Cookies["refreshToken"] ?? throw new ArgumentException("Refresh token is not found in request cookies.");
    }

    private void SetRefreshTokenToCookie(RefreshToken refreshToken)
    {
        CookieOptions cookieOptions = new()
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        Response.Cookies.Append(key: "refreshToken", refreshToken.Token, cookieOptions);
    }
}

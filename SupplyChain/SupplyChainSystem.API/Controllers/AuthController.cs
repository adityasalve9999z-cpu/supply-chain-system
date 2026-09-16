using Microsoft.AspNetCore.Mvc;
using SupplyChainSystem.Application.DTOs;
using SupplyChainSystem.Application.Interfaces;

namespace SupplyChainSystem.API.Controllers;

[ApiController, Route("api/auth")]
public sealed class AuthController(IAuthService auth) : ControllerBase
{
    [HttpPost("register"), ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    public Task<AuthResponse> Register(RegisterRequest request, CancellationToken cancellationToken) => auth.RegisterAsync(request, cancellationToken);

    [HttpPost("login"), ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    public Task<AuthResponse> Login(LoginRequest request, CancellationToken cancellationToken) => auth.LoginAsync(request, cancellationToken);
}

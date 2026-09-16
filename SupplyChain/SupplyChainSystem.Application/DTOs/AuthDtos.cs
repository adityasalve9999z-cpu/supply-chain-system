using System.ComponentModel.DataAnnotations;

namespace SupplyChainSystem.Application.DTOs;

public sealed record RegisterRequest(
    [Required, MinLength(3), MaxLength(100)] string Username,
    [Required, EmailAddress, MaxLength(256)] string Email,
    [Required, MinLength(12), MaxLength(128)] string Password,
    [Required] string Role);

public sealed record LoginRequest(
    [Required] string Email,
    [Required] string Password);

public sealed record AuthResponse(int UserId, string Username, string Email, string Role, string Token);

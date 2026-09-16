using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SupplyChainSystem.Application.DTOs;
using SupplyChainSystem.Application.Interfaces;
using SupplyChainSystem.Core.Entities;
using SupplyChainSystem.Infrastructure.Data;

namespace SupplyChainSystem.Infrastructure.Services;

public sealed class AuthService(SupplyChainDbContext db, IConfiguration configuration) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        // Public registration must never grant elevated privileges.
        const string roleName = "Staff";

        if (await db.Users.AnyAsync(x => x.Email == request.Email, cancellationToken))
            throw new InvalidOperationException("An account with this email already exists.");

        var role = await db.Roles.SingleOrDefaultAsync(x => x.Name == roleName, cancellationToken)
            ?? throw new InvalidOperationException("The requested role is not configured.");
        var user = new User
        {
            Username = request.Username.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = role.Id,
            Role = role
        };
        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);
        return CreateResponse(user, roleName);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await db.Users.Include(x => x.Role).SingleOrDefaultAsync(x => x.Email == request.Email.Trim().ToLowerInvariant(), cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");
        return CreateResponse(user, user.Role.Name);
    }

    private AuthResponse CreateResponse(User user, string role)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, role)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is not configured.")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(configuration["Jwt:Issuer"], configuration["Jwt:Audience"], claims,
            expires: DateTime.UtcNow.AddMinutes(int.TryParse(configuration["Jwt:ExpiryMinutes"], out var expiryMinutes) ? expiryMinutes : 60),
            signingCredentials: credentials);
        return new AuthResponse(user.Id, user.Username, user.Email, role, new JwtSecurityTokenHandler().WriteToken(token));
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using AllStay.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AllStay.Infrastructure.Services;

public class AuthService(AllStayDbContext db, IOptions<JwtOptions> jwtOptions) : IAuthService
{
    private readonly JwtOptions _jwt = jwtOptions.Value;

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        try
        {
            var staff = await db.HotelStaffUsers
               .Include(s => s.Hotel)
               .FirstOrDefaultAsync(s => s.Email == request.Email && s.IsActive, ct);

            if (staff is null || staff.Hotel is null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(request.Password, staff.PasswordHash))
                return null;

            var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_jwt.ExpiryMinutes);

            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, staff.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, staff.Email),
            new("hotel_id", staff.HotelId.ToString()),
            new(ClaimTypes.Role, staff.Role.ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: expiresAt.UtcDateTime,
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var profile = new StaffProfileDto(staff.Id, staff.Email, staff.FullName, staff.Role, staff.HotelId, staff.Hotel.Name);
            return new LoginResponse(tokenString, expiresAt, profile);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}

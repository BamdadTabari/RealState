using DataLayer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class JwtTokenService
{
	private readonly IConfiguration _configuration;

	public JwtTokenService(IConfiguration configuration)
	{
		_configuration = configuration;
	}
	public string GenerateToken(User user, List<string> roles)
	{
		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
			new Claim(ClaimTypes.Name, user.user_name),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // JWT ID برای امنیت بیشتر
        };

		// افزودن نقش‌ها
		claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(
			issuer: _configuration["Jwt:Issuer"],
			audience: _configuration["Jwt:Audience"],
			claims: claims,
			expires: DateTime.UtcNow.AddMinutes(15), // زمان انقضا کوتاه برای امنیت بیشتر
			signingCredentials: credentials
		);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	public string GenerateRefreshToken()
	{
		var randomBytes = new byte[64];
		using var rng = RandomNumberGenerator.Create();
		rng.GetBytes(randomBytes);
		return Convert.ToBase64String(randomBytes);
	}

	public string? GetUserIdFromClaims(ClaimsPrincipal user)
	{
		return user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
	}


	public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
	{
		var tokenValidationParameters = new TokenValidationParameters
		{
			ValidateAudience = false,
			ValidateIssuer = false,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!)),
			ValidateLifetime = false // مهمه: چون توکن منقضی شده
		};

		var tokenHandler = new JwtSecurityTokenHandler();
		var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

		if (securityToken is not JwtSecurityToken jwtSecurityToken ||
			!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
			throw new SecurityTokenException("Invalid token");

		return principal;
	}

	public int GetTokenExpiryMinutes(string token)
	{
		var handler = new JwtSecurityTokenHandler();
		var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

		if (jwtToken == null)
			throw new ArgumentException("Invalid token");

		var expUnix = jwtToken.Payload.Exp;
		if (!expUnix.HasValue)
			throw new ArgumentException("Token does not contain expiration");

		var expDateTime = DateTimeOffset.FromUnixTimeSeconds(expUnix.Value).UtcDateTime;
		var remainingMinutes = (int)(expDateTime - DateTime.UtcNow).TotalMinutes;

		return Math.Max(remainingMinutes, 0);
	}

}

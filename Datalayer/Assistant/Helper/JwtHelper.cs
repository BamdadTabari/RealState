﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Claim = System.Security.Claims.Claim;

namespace DataLayer;

public static class JwtHelper
{
	public static readonly SecurityTokenConfig Config = new();

	public static string CreateJwtAccessToken(this User user) =>
		user.CreateJwt(Config.AccessTokenSecretKey, Config.AccessTokenLifetime);

	public static string CreateJwtRefreshToken(this User user) =>
		user.CreateJwt(Config.RefreshTokenSecretKey, Config.RefreshTokenLifetime);

	private static string CreateJwt(this User user, string key, TimeSpan lifetime)
	{
		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
		var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

		var claims = new List<Claim>
	{
		new Claim(ClaimTypes.NameIdentifier, user.id.ToString()), // ✅ مهم: برای استفاده در فیلترها
        new Claim(ClaimTypes.Name, user.Username.ToLower()),
		new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // اختیاری: برای امنیت بیشتر
    };

		var accessToken = new JwtSecurityToken(
			issuer: Config.Issuer,
			audience: Config.Audience,
			claims: claims,
			expires: DateTime.UtcNow.Add(lifetime),
			signingCredentials: credentials
		);

		return new JwtSecurityTokenHandler().WriteToken(accessToken);
	}


	public static bool Validate(string token)
	{
		if (string.IsNullOrEmpty(token))
			return false;

		var isValid = true;

		var payload = new JwtSecurityTokenHandler().ReadJwtToken(token).Payload;

		//Validation rules
		if (payload == null)
			return false;

		if (payload.Iss != Config.Issuer)
			isValid = false;

		if (!payload.Aud.Contains(Config.Audience))
			isValid = false;

		if (payload.ValidTo.Add(Config.RefreshTokenLifetime) < DateTime.UtcNow)
			isValid = false;

		return isValid;
	}

	public static JwtPayload? GetPayload(string token)
	{
		if (string.IsNullOrEmpty(token))
			return null;
		return !Validate(token) ? null : new JwtSecurityTokenHandler().ReadJwtToken(token).Payload;
	}

	public static string GetUsername(string token)
	{
		var payload = GetPayload(token);
		return payload?.Claims.SingleOrDefault(x => x.Type == "unique_name")?.Value ?? string.Empty;
	}
}
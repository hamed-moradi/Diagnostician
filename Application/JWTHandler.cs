using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Model.Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Application {
  public class JWTHandler(
      AppSetting appSetting,
      JwtSecurityTokenHandler jwtSecurityTokenHandler) {

    private readonly byte[] buffer = Encoding.UTF8.GetBytes(appSetting.Identity.Key);
    //private readonly byte[] buffer = Convert.FromBase64String(appSetting.Identity.Key);
    private readonly string issuer = appSetting.Identity.Issuer;
    private readonly string audience = appSetting.Identity.Audience;

    public string Sign(ClaimsIdentity claims, DateTime? expires = null) {
      var tokenDescriptor = new SecurityTokenDescriptor {
        Issuer = issuer,
        Audience = audience,
        Expires = expires ?? DateTime.UtcNow.AddDays(7),
        Subject = claims,
        SigningCredentials = new SigningCredentials(
          new SymmetricSecurityKey(buffer),
          SecurityAlgorithms.HmacSha256Signature,
          SecurityAlgorithms.Sha512Digest)
      };

      var secretString = jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
      var parts = jwtSecurityTokenHandler.WriteToken(secretString).Split('.');
      var token = new List<string>();
      parts.ToList().ForEach(e => token.Add(WebUtility.UrlEncode(e)));
      //return $"Bearer {string.Join(".", token)}";
      return string.Join(".", token);
    }

    public ClaimsPrincipal Verify(string token) {
      var param = new TokenValidationParameters {
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(buffer),
      };
      IdentityModelEventSource.ShowPII = true;
      var parts = token.Split('.');
      var securityToken = new List<string>();
      parts.ToList().ForEach(e => securityToken.Add(WebUtility.UrlDecode(e)));
      try {
        var claimPrincipal = jwtSecurityTokenHandler.ValidateToken(string.Join(".", token), param, out var validatedToken);
        return claimPrincipal;
      }
      catch(Exception ex) {
        Log.Error(ex, ex.Source);
        return null;
      }
    }
  }
}


using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace TaskService.Core.AuditWriter;

public class JwtParser : IJwtParser
{
    public const string ClientIp = "ClientIp";

    private readonly string _userName = string.Empty;
    private readonly string _ip = string.Empty;

    public JwtParser(IHttpContextAccessor httpContextAccessor)
    {
        StringValues? jwt = httpContextAccessor.HttpContext?.Request.Headers[HeaderNames.Authorization];

        if (!string.IsNullOrWhiteSpace(jwt))
        {
            JwtSecurityTokenHandler securityTokenHandler = new();

            if (securityTokenHandler.CanReadToken(jwt))
            {
                JwtSecurityToken jwtSecurityToken = securityTokenHandler.ReadJwtToken(jwt);

                _userName = jwtSecurityToken.Claims.Single(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value;
                _ip = httpContextAccessor.HttpContext?.Request.Headers[ClientIp] ?? string.Empty;
            }
        }
    }

    public string GetIpAddress()
    {
        return _ip;
    }

    public string GetUserName()
    {
        return _userName;
    }
}

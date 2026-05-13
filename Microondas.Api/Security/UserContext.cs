using System.Security.Claims;
using Microondas.Domain.Interfaces;

namespace Microondas.Api.Security;

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public string ObterUsuarioAtual()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null)
            return "console";

        var user = httpContext.User;

        var name = user.Identity?.Name
                   ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? user.FindFirst("sub")?.Value;

        if (!string.IsNullOrEmpty(name))
            return name;

        var token = httpContext.Request.Query["access_token"].ToString();
        if (string.IsNullOrEmpty(token))
            return "anonimo";


        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        return jwtToken.Subject ?? jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value ?? "anonimo";
    }
}
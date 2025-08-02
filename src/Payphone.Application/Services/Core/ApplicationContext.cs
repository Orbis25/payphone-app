using Microsoft.AspNetCore.Http;

namespace Payphone.Application.Services.Core;

public interface IApplicationContext
{
    public string? Token { get; set; }
    public string? UserId { get; set; }
}

public class ApplicationContext : IApplicationContext
{
    public string? Token { get; set; }
    public string? UserId { get; set; }

    public ApplicationContext(IHttpContextAccessor httpContextAccessor)
    {
        var headers = httpContextAccessor?.HttpContext?.Request?.Headers;

        if (headers != null)
        {
            string? token = headers["Authorization"];

            if (!string.IsNullOrEmpty(token))
            {
                token = token.Replace("Bearer", string.Empty).Replace("bearer", string.Empty).Trim();

                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                if (jwt == null) return;
                try
                {
                    //set token
                    Token = string.IsNullOrEmpty(token) ? string.Empty : token;
                    var userId = jwt.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
                    UserId = userId;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Invalid parse token :{ex.Message ?? ex.InnerException?.Message}");
                }
            }
        }
    }
}
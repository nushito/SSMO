using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace SSMO.Infrastructure
{
    public class HttpContextUserIdExtension
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HttpContextUserIdExtension(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor; 
        }

        public string ContextAccessUserId()
        => _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}

using System.Security.Claims;

namespace Deal.DeskOne.API.Security
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var value = user.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? user.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(value) || !Guid.TryParse(value, out var id))
                throw new InvalidOperationException("Claim de usuário inválida ou ausente.");

            return id;
        }
    }
}

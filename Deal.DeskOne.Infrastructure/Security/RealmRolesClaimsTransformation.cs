using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;

namespace Deal.DeskOne.Infrastructure.Security
{
    public sealed class RealmRolesClaimsTransformation : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Identity is not ClaimsIdentity { IsAuthenticated: true } identity)
                return Task.FromResult(principal);

            var realmAccess = identity.FindFirst("realm_access")?.Value;

            if (string.IsNullOrWhiteSpace(realmAccess))
                return Task.FromResult(principal);

            using var doc = JsonDocument.Parse(realmAccess);

            if (!doc.RootElement.TryGetProperty("roles", out var roles) || roles.ValueKind != JsonValueKind.Array)
                return Task.FromResult(principal);


            foreach (var roleName in roles.EnumerateArray().Select(role => role.GetString()).Where(roleName => !string.IsNullOrWhiteSpace(roleName)))
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, roleName));
            }

            return Task.FromResult(principal);
        }
    }
}

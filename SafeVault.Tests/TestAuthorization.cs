using NUnit.Framework;
using System.Security.Claims;

namespace SafeVault.Tests
{
    [TestFixture]
    public class TestAuthorization
    {
        [Test]
        public void AdminRoleGrantsAccess()
        {
            var claims    = new[] { new Claim(ClaimTypes.Role, "Admin") };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
            
            Assert.That(principal.IsInRole("Admin"), Is.True, "Admin role should grant access");
        }

        [Test]
        public void NonAdminDeniedAccess()
        {
            var claims    = new[] { new Claim(ClaimTypes.Role, "User") };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
            
            Assert.That(principal.IsInRole("Admin"), Is.False, "User role should deny access");
        }
    }
}

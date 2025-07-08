using NUnit.Framework;
using SafeVaultProject.Data;
using SafeVaultProject.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace SafeVault.Tests
{
    [TestFixture]
    public class TestAuthentication
    {
        private SafeVaultContext ctx = null!;

        [SetUp]
        public void Setup()
        {
            // Use a GUID for database name → brand new each time
            var opts = new DbContextOptionsBuilder<SafeVaultContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            ctx = new SafeVaultContext(opts);
            var hash = BCrypt.Net.BCrypt.HashPassword("Secret123!");
            ctx.Users.Add(new User
            {
                Username     = "alice",
                PasswordHash = hash,
                Role         = "User"
            });
            ctx.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            ctx.Dispose();
        }

        [Test]
        public void ValidLoginSucceeds()
        {
            var user   = ctx.Users.Single(u => u.Username == "alice");
            var result = BCrypt.Net.BCrypt.Verify("Secret123!", user.PasswordHash);

            Assert.That(result, Is.True, "Valid password must verify");
        }

        [Test]
        public void InvalidLoginFails()
        {
            var user   = ctx.Users.Single(u => u.Username == "alice");
            var result = BCrypt.Net.BCrypt.Verify("WrongPass", user.PasswordHash);

            Assert.That(result, Is.False, "Invalid password must not verify");
        }
    }
}
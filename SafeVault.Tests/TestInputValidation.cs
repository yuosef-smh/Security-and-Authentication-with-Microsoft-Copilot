using NUnit.Framework;
using SafeVaultProject.Data;
using SafeVaultProject.Models;
using SafeVaultProject.Services;

namespace SafeVault.Tests
{
    [TestFixture]
    public class TestInputValidation
    {
        private IInputSanitizer sanitizer = null!;

        [SetUp]
        public void Setup() =>
            sanitizer = new InputSanitizer();

        [Test]
        public void SanitizesSqlInjectionAttempt()
        {
            var raw   = "Robert'); DROP TABLE Users;--";
            var clean = sanitizer.Sanitize(raw);

            Assert.That(clean.Contains("'"), Is.False, "Single-quote was not stripped");
            Assert.That(clean.ToLower().Contains("drop"), Is.False, "'drop' keyword was not stripped");
        }

        [Test]
        public void SanitizesXssAttempt()
        {
            var raw   = "<script>alert('XSS')</script>";
            var clean = sanitizer.Sanitize(raw);

            Assert.That(clean.Contains("<") || clean.Contains(">"), Is.False,
                "Angle-brackets were not stripped");
            Assert.That(clean.ToLower().Contains("script"), Is.False,
                "'script' was not stripped");
        }
    }
}

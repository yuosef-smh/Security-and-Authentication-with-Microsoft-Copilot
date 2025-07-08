using System.Net;
using System.Text.RegularExpressions;

namespace SafeVaultProject.Services
{
    public class InputSanitizer : IInputSanitizer
    {
        private static readonly string[] BannedWords = { "drop", "script", "alert", "table" };

        public string Sanitize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // 1) HTML‐encode
            var encoded = WebUtility.HtmlEncode(input);

            // 2) Remove non‐alphanumeric plus allowed chars
            encoded = Regex.Replace(encoded, @"[^a-zA-Z0-9@.\-_ ]", string.Empty);

            // 3) Strip banned keywords (case‐insensitive)
            foreach (var word in BannedWords)
            {
                encoded = Regex.Replace(
                    encoded,
                    Regex.Escape(word),
                    string.Empty,
                    RegexOptions.IgnoreCase);
            }

            return encoded;
        }
    }
}

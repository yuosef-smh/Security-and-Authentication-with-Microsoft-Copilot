# Input Sanitization
SafeVault uses an InputSanitizer service to clean every form field:
1. HTML Encoding Raw input is passed through WebUtility.HtmlEncode to neutralize HTML tags.
2. Character Whitelist A regex removes all characters except letters, digits, @ . - _ and spaces.
3. Keyword Banning Reserved words (drop, script, alert, table) are stripped out case-insensitively.

All controller actions call IInputSanitizer.Sanitize(...) on user inputs before any processing or storage.

# Authentication & RBAC
## Registration
1. Sanitized username and email
2. Password hashed via BCrypt.Net.BCrypt.HashPassword
3. Stored in an EF Core In-Memory database

## Login
1. Sanitized username lookup
2. Password validated with BCrypt.Net.BCrypt.Verify
3. On success, issues a cookie with ClaimTypes.Name and ClaimTypes.Role

## Role-Based Authorization
1. Two roles: User (default) and Admin (seedable)
2. An “AdminOnly” policy enforces [Authorize(Policy="AdminOnly")] on the Admin controller
3. Unauthorized users are prevented from accessing the admin dashboard

You can seed an admin in Program.cs by creating a user with Role = "Admin".

# Automated Testing
The SafeVault.Tests project covers three areas:

## Input Validation Tests
SQL-Injection Attempt Verifies that quotes, semicolons, and the word “drop” are stripped.

XSS Attempt Confirms that angle brackets and the word “script” are removed.

## Authentication Tests
Each test uses a unique in-memory database (GUID-named) to prevent leftover data.

Valid Login: correct password yields true on BCrypt verify.

Invalid Login: wrong password yields false.

## Authorization Tests
Builds a ClaimsPrincipal with “Admin” or “User” roles.

Uses Assert.That(..., Is.True/Is.False) to check IsInRole("Admin").

# Debugging & Copilot Enhancements
During development, Microsoft Copilot helped:

Generate boilerplate for controllers, models, and services.

Iterate on test failures:

Switched Assert calls to Assert.That(...).

Added [TearDown] disposal for DbContext.

Enhanced the sanitizer to ban specific keywords after initial tests still contained “drop” and “script.”

Resolved dependency errors by correcting project references and NuGet package versions.

This cycle of test → debug → copilot revision → retest ensured robust security and clean code.

## Test Results
![Screenshot](https://github.com/yuosef-smh/Security-and-Authentication-with-Microsoft-Copilot/blob/main/Test%20Result.png)

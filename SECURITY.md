# Security Policy

## Supported versions

PlaylistShare is pre-1.0; only the latest version on the `main` branch receives security fixes.

| Version | Supported |
| ------- | --------- |
| latest  | yes       |
| older   | no        |

## Reporting a vulnerability

**Please do not report security vulnerabilities through public GitHub issues.**

Instead, use GitHub's private reporting:

1. Go to the repository's **Security** tab.
2. Click **Report a vulnerability** (Privately report a vulnerability).
3. Describe the issue, affected version(s), and steps to reproduce.

<!-- Maintainers: enable "Private vulnerability reporting" in Settings > Security to make the button
     above available, and optionally add a contact email here. -->

We aim to acknowledge a report within a few days and will keep you updated on the fix and disclosure
timeline. Please give us reasonable time to address the issue before any public disclosure.

## Secrets & configuration

PlaylistShare talks to the Yandex Music API on the user's behalf and handles JWT signing keys, a Data
Protection key ring, per-user Yandex access tokens and a database connection string. **Never commit
real secrets.** Provide them through:

- ASP.NET Core **User Secrets** for local development (`dotnet user-secrets`), or
- **environment variables** / a secrets store in production.

The committed `appsettings.json` contains placeholders only. If you believe a real credential has
been committed at any point, rotate it immediately.

Thank you for helping keep PlaylistShare and its users safe.

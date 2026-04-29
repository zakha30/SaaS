using System;

namespace SaaS.Shared;

/// <summary>
/// JWT authentication configuration settings.
/// Shared between Infrastructure and Auth modules.
/// </summary>
public sealed class JwtSettings
{
    public const string Section = "JwtSettings";

    // Cookie names - used by both AuthController and ServiceCollectionExtensions
    public const string AccessTokenCookie = "transhub_access";
    public const string RefreshTokenCookie = "transhub_refresh";

    public string Secret { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public int ExpiryMinutes { get; init; } = 60;
    public int RefreshExpiryDays { get; init; } = 30;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Secret) || Secret.Length < 32)
            throw new InvalidOperationException(
                "JwtSettings.Secret must be at least 32 characters. " +
                "Use Azure Key Vault in production.");

        if (string.IsNullOrWhiteSpace(Issuer))
            throw new InvalidOperationException("JwtSettings.Issuer is required.");

        if (string.IsNullOrWhiteSpace(Audience))
            throw new InvalidOperationException("JwtSettings.Audience is required.");
    }
}

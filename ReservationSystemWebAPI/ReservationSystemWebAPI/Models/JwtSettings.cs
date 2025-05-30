namespace ReservationSystemWebAPI.Models
{
    /// <summary>
    /// Represents the configuration settings required for generating and validating JWT tokens.
    /// These values are typically loaded from appsettings.json or environment variables.
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Gets or sets the secret key used for signing JWT tokens.
        /// This should be a long, random string kept securely.
        /// </summary>
        public string? SecretKey { get; set; }

        /// <summary>
        /// Gets or sets the issuer (iss claim) of the JWT token.
        /// Typically represents the authentication server or application.
        /// </summary>
        public string? Issuer { get; set; }

        /// <summary>
        /// Gets or sets the audience (aud claim) of the JWT token.
        /// Represents the intended recipient of the token (usually your API).
        /// </summary>
        public string? Audience { get; set; }

        /// <summary>
        /// Gets or sets the token expiry duration in minutes.
        /// After this time, the token will no longer be valid.
        /// </summary>
        public int ExpiryMinutes { get; set; }
    }
}

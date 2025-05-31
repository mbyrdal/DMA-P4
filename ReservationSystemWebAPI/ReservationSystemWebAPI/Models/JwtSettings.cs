namespace ReservationSystemWebAPI.Models
{
    /// <summary>
    /// Represents the configuration settings used for generating and validating JWT tokens.
    /// These settings are typically bound from configuration sources such as <c>appsettings.json</c> or environment variables.
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Gets or sets the secret key used to sign JWT tokens.
        /// This should be a strong, securely stored string.
        /// </summary>
        public string? SecretKey { get; set; }

        /// <summary>
        /// Gets or sets the issuer (<c>iss</c> claim) of the JWT token.
        /// Usually identifies the authority that generates the token, such as the authentication server.
        /// </summary>
        public string? Issuer { get; set; }

        /// <summary>
        /// Gets or sets the audience (<c>aud</c> claim) of the JWT token.
        /// Specifies the intended recipient of the token, typically your API or application.
        /// </summary>
        public string? Audience { get; set; }

        /// <summary>
        /// Gets or sets the token's expiration duration, in minutes.
        /// After this time elapses, the token is considered invalid.
        /// </summary>
        public int ExpiryMinutes { get; set; }
    }
}
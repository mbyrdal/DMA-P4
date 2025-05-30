using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.Repositories;

namespace ReservationSystemWebAPI.Services
{
    /// <summary>
    /// Provides services for user authentication and login-related business logic.
    /// </summary>
    public class LoginService : ILoginService
    {
        private readonly ILoginRepository _loginRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginService"/> class.
        /// </summary>
        /// <param name="loginRepository">Repository for accessing user data.</param>
        public LoginService(ILoginRepository loginRepository)
        {
            _loginRepository = loginRepository;
        }

        /// <summary>
        /// Authenticates a user by verifying the provided email and password.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's plaintext password.</param>
        /// <returns>The authenticated <see cref="User"/> if credentials are valid.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the user with the specified email does not exist.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the password does not match.</exception>
        public async Task<User> AuthenticateUserAsync(string email, string password)
        {
            User user = await _loginRepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                throw new KeyNotFoundException($"Bruger ikke fundet med e-mail: {email}");
            }

            bool passwordMatch = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (!passwordMatch)
            {
                throw new UnauthorizedAccessException("Forkert adgangskode.");
            }

            return user;
        }
    }
}
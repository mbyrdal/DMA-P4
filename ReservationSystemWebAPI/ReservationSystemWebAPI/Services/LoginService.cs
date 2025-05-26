using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.Repositories;

namespace ReservationSystemWebAPI.Services
{
    /// <summary>
    /// Handles business logic related to user login and authentication.
    /// </summary>
    public class LoginService : ILoginService
    {
        private readonly ILoginRepository _loginRepository;

        public LoginService(ILoginRepository loginRepository)
        {
            _loginRepository = loginRepository;
        }

        /// <summary>
        /// Authenticates a user based on email and password.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<User> AuthenticateUserAsync(string email, string password)
        {
            User user = await _loginRepository.GetUserByEmailAsync(email);

            if(user == null)
            {
                throw new KeyNotFoundException($"Bruger ikke fundet med e-mail: {email}");
            }

            bool passwordMatch = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if(!passwordMatch)
            {
                throw new UnauthorizedAccessException("Forkert adgangskode.");
            }

            return user;
        }
    }
}

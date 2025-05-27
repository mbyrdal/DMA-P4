using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.Repositories;

namespace ReservationSystemWebAPI.Services
{
    /// <summary>
    /// Service class responsible for handling business logic related to users.
    /// Interacts with the user repository to perform data operations and adds validation and error handling.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Retrieves all users from the system asynchronously.
        /// </summary>
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();

            if(users == null)
            {
                throw new ArgumentNullException(nameof(users), "Fejl ved hentning af brugere fra databasen. Tjek din forbindelse.");
            }

            if(!users.Any())
            {
                throw new InvalidOperationException("Der er ingen brugere registreret i systemet.");
            }

            return users;

        }

        /// <summary>
        /// Retrieves a specific user by ID asynchronously.
        /// </summary>
        public async Task<User> GetByIdAsync(int id)
        {
            var foundUser = await _userRepository.GetByIdAsync(id);

            if(foundUser == null)
            {
                throw new KeyNotFoundException($"Ingen bruger fundet med ID {id}.");
            }

            return foundUser;
        }

        /// <summary>
        /// Adds a new user to the system asynchronously.
        /// </summary>
        public async Task<int> AddAsync(User newUser)
        {
            if(string.IsNullOrWhiteSpace(newUser.Email))
            {
                throw new ArgumentException("Ugyldig email: ", nameof(newUser.Email));
            }

            try
            {
                return await _userRepository.AddAsync(newUser);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Fejl ved tilføjelse af bruger til databasen.", ex);
            }
        }

        /// <summary>
        /// Updates an existing user in the system asynchronously.
        /// </summary>
        public async Task<int> UpdateAsync(User user)
        {
            bool userExists = await _userRepository.ExistsAsync(user.Id);

            if (!userExists)
            {
                throw new KeyNotFoundException($"Kan ikke opdatere. Bruger med ID {user.Id} findes ikke.");
            }

            try
            {
                return await _userRepository.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Fejl under opdatering af bruger i databasen.", ex);
            }
        }

        /// <summary>
        /// Deletes a user from the system by ID asynchronously.
        /// </summary>
        public async Task<int> DeleteAsync(int id)
        {
            bool userExists = await _userRepository.ExistsAsync(id);

            if(!userExists)
            {
                throw new KeyNotFoundException($"Kan ikke slette. Bruger med ID {id} findes ikke.");
            }

            try
            {
                return await _userRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Fejl under sletning af bruger fra databasen.", ex);
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.Repositories;

namespace ReservationSystemWebAPI.Services
{
    /// <summary>
    /// Service class responsible for business logic and error handling related to user operations.
    /// Interacts with the <see cref="IUserRepository"/> to perform CRUD operations asynchronously.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class with the specified repository.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Retrieves all users asynchronously.
        /// Throws <see cref="ArgumentNullException"/> if retrieval fails.
        /// Throws <see cref="InvalidOperationException"/> if no users are found.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.
        /// The task result contains a collection of all users.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the retrieved user collection is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no users exist in the system.</exception>
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();

            if (users == null)
                throw new ArgumentNullException(nameof(users), "Fejl ved hentning af brugere fra databasen. Tjek din forbindelse.");

            if (!users.Any())
                throw new InvalidOperationException("Der er ingen brugere registreret i systemet.");

            return users;
        }

        /// <summary>
        /// Retrieves a user by their ID asynchronously.
        /// Throws <see cref="KeyNotFoundException"/> if the user is not found.
        /// </summary>
        /// <param name="id">The user's unique identifier.</param>
        /// <returns>A task representing the asynchronous operation.
        /// The task result contains the user.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when no user with the specified ID exists.</exception>
        public async Task<User> GetByIdAsync(int id)
        {
            var foundUser = await _userRepository.GetByIdAsync(id);

            if (foundUser == null)
                throw new KeyNotFoundException($"Ingen bruger fundet med ID {id}.");

            return foundUser;
        }

        /// <summary>
        /// Adds a new user asynchronously.
        /// Throws <see cref="ArgumentException"/> if the user's email is null or whitespace.
        /// Throws <see cref="InvalidOperationException"/> if an error occurs during addition.
        /// </summary>
        /// <param name="newUser">The new user to add.</param>
        /// <returns>A task representing the asynchronous operation.
        /// The task result contains the ID of the newly added user.</returns>
        /// <exception cref="ArgumentException">Thrown when <see cref="User.Email"/> is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown if adding the user fails.</exception>
        public async Task<int> AddAsync(User newUser)
        {
            if (string.IsNullOrWhiteSpace(newUser.Email))
                throw new ArgumentException("Ugyldig email: ", nameof(newUser.Email));

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
        /// Updates an existing user asynchronously.
        /// Throws <see cref="KeyNotFoundException"/> if the user does not exist.
        /// Throws <see cref="InvalidOperationException"/> if an error occurs during update.
        /// </summary>
        /// <param name="user">The user with updated information.</param>
        /// <returns>A task representing the asynchronous operation.
        /// The task result contains the number of affected records.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the user is not found.</exception>
        /// <exception cref="InvalidOperationException">Thrown if updating the user fails.</exception>
        public async Task<int> UpdateAsync(User user)
        {
            bool userExists = await _userRepository.ExistsAsync(user.Id);

            if (!userExists)
                throw new KeyNotFoundException($"Kan ikke opdatere. Bruger med ID {user.Id} findes ikke.");

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
        /// Deletes a user by ID asynchronously.
        /// Throws <see cref="KeyNotFoundException"/> if the user does not exist.
        /// Throws <see cref="ArgumentException"/> if the provided <paramref name="rowVersion"/> is invalid.
        /// Throws <see cref="InvalidOperationException"/> if a concurrency conflict or other deletion error occurs.
        /// </summary>
        /// <param name="id">The unique identifier of the user to delete.</param>
        /// <param name="rowVersion">Concurrency token as a Base64 string.</param>
        /// <returns>A task representing the asynchronous operation.
        /// The task result contains the number of affected records.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the user is not found.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="rowVersion"/> is not valid Base64 string.</exception>
        /// <exception cref="InvalidOperationException">Thrown if a concurrency conflict or deletion error occurs.</exception>
        public async Task<int> DeleteAsync(int id, string rowVersion)
        {
            bool userExists = await _userRepository.ExistsAsync(id);
            if (!userExists)
                throw new KeyNotFoundException($"Kan ikke slette. Bruger med ID {id} findes ikke.");

            byte[] rowVersionBytes;
            try
            {
                rowVersionBytes = Convert.FromBase64String(rowVersion);
            }
            catch (FormatException)
            {
                throw new ArgumentException("Ugyldig RowVersion format.");
            }

            try
            {
                return await _userRepository.DeleteAsync(id, rowVersionBytes);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Brugeren er blevet ændret eller slettet af en anden proces.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Fejl under sletning af bruger fra databasen.", ex);
            }
        }
    }
}
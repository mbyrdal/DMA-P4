using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.Repositories;
using ReservationSystemWebAPI.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ReservationSystemWebAPI.Tests.Integration
{
    public class LoginServiceIntegrationTests
    {
        private readonly ILoginService _loginService;

        public LoginServiceIntegrationTests()
        {
            var services = new ServiceCollection();

            services.AddDbContext<ReservationDbContext>(options =>
                options.UseSqlServer("Server=hildur.ucn.dk;Database=DMA-CSD-S235_10632110;User Id=DMA-CSD-S235_10632110;Password=Password1!;TrustServerCertificate=true"));

            services.AddScoped<ILoginRepository, LoginRepository>();
            services.AddScoped<ILoginService, LoginService>();

            var provider = services.BuildServiceProvider();
            _loginService = provider.GetRequiredService<ILoginService>();
        }

        [Fact]
        public async Task AuthenticateUserAsync_ValidCredentials_ReturnsUser()
        {
            // Husk: Brugeren skal findes i databasen
            var email = "tommy@wexo.dk";
            var password = "admin1234";

            var user = await _loginService.AuthenticateUserAsync(email, password);

            Assert.NotNull(user);
            Assert.Equal(email, user.Email);
        }

        [Fact]
        public async Task AuthenticateUserAsync_InvalidPassword_ThrowsUnauthorizedAccessException()
        {
            var email = "tommy@wexo.dk";
            var wrongPassword = "forkertKode";

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _loginService.AuthenticateUserAsync(email, wrongPassword));
        }

        [Fact]
        public async Task AuthenticateUserAsync_UnknownEmail_ThrowsKeyNotFoundException()
        {
            var unknownEmail = $"unknown_{Guid.NewGuid()}@example.com";
            var password = "hvadSomHelst";

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _loginService.AuthenticateUserAsync(unknownEmail, password));
        }
    }
}

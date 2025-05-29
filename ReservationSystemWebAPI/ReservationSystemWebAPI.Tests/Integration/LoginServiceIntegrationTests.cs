using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.Repositories;
using ReservationSystemWebAPI.Services;
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

        // TODO: Tilføj integrationstests her
    }
}

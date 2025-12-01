using CORE.Interfaces;
using CORE.Mapping;
using REPOSITORY.Repsitories;
using SERVICES;

namespace AUTH.API.Extentions
{
    public static class AppServices
    {
        public static IServiceCollection AddServices(this IServiceCollection Services)
        {

            #region Services
            Services.AddSingleton<ITokenService, TokenService>();
            Services.AddScoped<IAuthService, AuthService>();
            #endregion
            #region Mapper
            Services.AddAutoMapper(typeof(MappingProfile));
            #endregion
            #region Repositories
            Services.AddScoped<IUserRepository, UserRepository>();
            Services.AddScoped<IUnitOfWork, UnitOfWork>();
            #endregion
            return Services;
        }
    }
}

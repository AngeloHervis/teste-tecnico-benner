using Microondas.Domain.Interfaces;
using Microondas.Domain.Services;
using Microondas.Infrastructure.Data;
using Microondas.Infrastructure.Repositories;
using Microondas.Infrastructure.Security;
using Microondas.Infrastructure.State;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microondas.IoC;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var encryptedConnString = configuration.GetConnectionString("DefaultConnection") ?? "";
        var decryptedConnString = CryptoHelper.DecryptConnectionString(encryptedConnString);

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(decryptedConnString));

        services.AddScoped<IProgramaRepository, ProgramaRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        
        services.AddSingleton<IMicroondasStateProvider, MicroondasStateProvider>();
        services.AddSingleton<ICryptoProvider, CryptoProvider>();
        services.AddSingleton<ITokenService, TokenService>();
    }

    public static void AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IProgramaService, ProgramaService>();
        services.AddScoped<IAquecimentoService, AquecimentoService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
    }

    public static void MigrateDatabase(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
    }
}



using Microondas.Console;
using Microondas.Domain.Interfaces;
using Microondas.Domain.Services;
using Microondas.Infrastructure.Data;
using Microondas.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build();

var services = new ServiceCollection();

// Configurações
services.AddSingleton<IConfiguration>(configuration);

// Banco de Dados
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// Negócio e Persistência
services.AddScoped<IProgramaRepository, ProgramaRepository>();
services.AddScoped<IProgramaService, ProgramaService>();

// UI
services.AddTransient<MicroondasInterface>();

var serviceProvider = services.BuildServiceProvider();

using (var scope = serviceProvider.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync(); 
}

var interfaceApp = serviceProvider.GetRequiredService<MicroondasInterface>();
await interfaceApp.IniciarLoopAsync();

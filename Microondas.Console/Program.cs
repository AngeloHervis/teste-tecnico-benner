using Microondas.Console;
using Microondas.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build();

var services = new ServiceCollection();

services.AddSingleton<IConfiguration>(configuration);

services.AddInfrastructure(configuration);
services.AddDomainServices();

services.AddTransient<MicroondasInterface>();

var serviceProvider = services.BuildServiceProvider();

serviceProvider.MigrateDatabase();

var interfaceApp = serviceProvider.GetRequiredService<MicroondasInterface>();
await interfaceApp.IniciarLoopAsync();


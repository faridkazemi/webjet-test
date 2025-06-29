using CinemaWorldDataSync;
using CinemaWorldDataSync.OptionModels;
using CinemaWorldDataSync.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder();

builder.ConfigureServices((hostconfig, services) =>
{
    var cinemaWorldConfigOption = hostconfig.Configuration
   .GetSection(nameof(CinemaWorldConfigurationOption))
   .Get<CinemaWorldConfigurationOption>();

    services.Configure<CinemaWorldConfigurationOption>(hostconfig.Configuration.GetSection(nameof(CinemaWorldConfigurationOption)));

    var redisConfigOption = hostconfig.Configuration
    .GetSection(nameof(RedisConfigOption))
    .Get<RedisConfigOption>();

    services.Configure<RedisConfigOption>(hostconfig.Configuration.GetSection(nameof(RedisConfigOption)));

    services.AddLogging();

    // I have added an extesion method to config and add the services to the serviceCollection.
    // It is just cleaner
    services.AddServices(cinemaWorldConfigOption, redisConfigOption);
});

builder.ConfigureAppConfiguration((hostConfig, config) =>
{
    // I have created another extention method for adding secrets.
    // This centralizes secret loading and keeps Program.cs cleaner.
    // We just need to make sure the files in the secret folder follow the nameing convention.
    // Nameing convention: OptionClassName__PropertyName
    // In the real world examples I would use Azure Key Vault. 
    // This is only good for this Test

    var isRunninningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER");

    var secretPath = isRunninningInContainer.ToLower() == "true" ? "/run/secrets" : "./secrets";

    config.AddSecrets(secretPath);
});

using var host = builder.Build();

var syncService = host.Services.GetRequiredService<ISyncDataService>();

using var cts = new CancellationTokenSource();

Console.CancelKeyPress += (sender, args) =>
{
    Console.WriteLine("Cancel requested...");
    args.Cancel = true;
    cts.Cancel();
};

AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
{
    Console.WriteLine("Process exiting...");
    cts.Cancel();
};

await syncService.RunAsync(cts.Token);



using Microsoft.Extensions.DependencyInjection;
using Polly.Extensions.Http;
using Polly;
using FilmWorldDataSync.OptionModels;
using RedisLibrary;
using StackExchange.Redis;
using FilmWorldDataSync.Services.Interfaces;
using FilmWorldDataSync.Services;
using Microsoft.Extensions.Configuration;
using filmWorldDataSync.Services.Interfaces;

namespace FilmWorldDataSync
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services, FilmWorldConfigurationOption configOption, RedisConfigOption redisConfigOption)
        {
            services.AddHttpClient(configOption.FilmMovieHttpClientName,
                client =>
                {
                    client.BaseAddress = new Uri(configOption.BaseUrl);
                    client.DefaultRequestHeaders.Add("x-access-token", configOption.AccessToken);
                }).AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetTimeoutPolicy());

            services.AddSingleton<IConnectionMultiplexer>(x =>
            {
                // For this test project I haven't added any password for connecting to the redis.
                // But in the real world project connection needs to be secure and move password to secrets/Azure Key vaut
                return ConnectionMultiplexer.Connect(redisConfigOption.ConnectionString);
            });

            services.AddTransient<IRedisService, RedisService>();
            services.AddTransient<IFilmWorldMovieProvider, FilmWorldMovieProvider>();
            services.AddTransient<ISyncDataService, SyncDataService>();

            return services;
        }

        public static IConfigurationBuilder AddSecrets(this IConfigurationBuilder  config, string secretDir)
        {
            if (Directory.Exists(secretDir))
            {
                var files = Directory.GetFiles(secretDir);

                var secrets = new Dictionary<string, string>();

                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);

                    var secretKey = fileName.Replace("__", ":");

                    var secretValue = File.ReadAllText(file);

                    secrets[secretKey] = secretValue;

                }
                config.AddInMemoryCollection(secrets);

            }
            return config;
        }
        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(retryCount: 3,
                sleepDurationProvider: attemp => TimeSpan.FromSeconds(Math.Pow(2, attemp)));
        }

        static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(10);
        }
    }
}

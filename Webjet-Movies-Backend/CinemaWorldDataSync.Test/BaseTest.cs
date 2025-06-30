using CinemaWorldDataSync.Services.Interfaces;
using CinemaWorldDataSync.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedisLibrary;
using CinemaWorldDataSync.OptionModels;
using Moq.Protected;
using System.Net;
using Moq;

namespace CinemaWorldDataSync.Test
{
    public class BaseTest
    {
        public IServiceCollection ServiceCollection;
        public void Setup() 
        {
            var builder = Host.CreateDefaultBuilder();
            
            builder.ConfigureServices((hostconfig, services) =>
            {
                services.Configure<CinemaWorldConfigurationOption>(hostconfig.Configuration.GetSection(nameof(CinemaWorldConfigurationOption)));
                services.Configure<RedisConfigOption>(hostconfig.Configuration.GetSection(nameof(RedisConfigOption)));

                services.AddTransient<IRedisService, RedisService>();
                services.AddTransient<ISinemaWorldMovieProvider, SinemaWorldMovieProvider>();
                services.AddTransient<ISyncDataService, SyncDataService>();
                services.AddLogging();
                services.AddScoped<IRedisService, RedisService>();
                ServiceCollection = services;
            }).Build();
        }

        public HttpClient GetMockHttpClient(HttpStatusCode httpStatusCode, params string[] apiResponses) 
        {
            var mockHandler = new Mock<HttpMessageHandler>();

            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Returns<HttpRequestMessage, CancellationToken>((request, token) => 
                {
                    if (request.RequestUri.AbsolutePath.Contains("movies"))
                    {
                        return Task.FromResult(new HttpResponseMessage 
                        {
                            StatusCode = httpStatusCode,
                            Content = new StringContent(apiResponses[0], Encoding.UTF8, "application/json")
                        });
                    }
                    else if (request.RequestUri.AbsolutePath.Contains("/movie/cw0076759"))
                    {
                        return Task.FromResult(new HttpResponseMessage
                        {
                            StatusCode = httpStatusCode,
                            Content = new StringContent(apiResponses[1], Encoding.UTF8, "application/json")
                        });
                    }
                    else if (request.RequestUri.AbsolutePath.Contains("/movie/cw0080684"))
                    {
                        return Task.FromResult(new HttpResponseMessage
                        {
                            StatusCode = httpStatusCode,
                            Content = new StringContent(apiResponses[2], Encoding.UTF8, "application/json")
                        });
                    }
                    else
                    {
                        return Task.FromResult(new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.NotFound
                        });
                    }
                } );

            var httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri("https://fakeapi.com")
            };

            return httpClient;
        }
    }
}

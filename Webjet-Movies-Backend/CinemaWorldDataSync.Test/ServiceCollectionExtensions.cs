using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace CinemaWorldDataSync.Test
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection ReplaceWithMock<T>(this IServiceCollection services, Mock<T> mock) where T : class
        {
            services.Replace(ServiceDescriptor.Scoped(_ =>
            {
                return mock.Object;
            }));
            
            return services;
        }

        internal static IServiceCollection ReplaceWith<T>(this IServiceCollection services, T fakeObj) where T : class
        {
            services.Replace(ServiceDescriptor.Scoped(_ =>
            {
                return fakeObj;
            }));

            return services;
        }
    }
}

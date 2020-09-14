using ErosionFinder;
using ErosionFinder.Domain.Interfaces;
using ErosionFinder.Logger;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class ErosionFinderServiceCollectionExtensions
    {
        public static IServiceCollection AddAnalysisToolAdapter(this IServiceCollection services)
        {
            services.AddScoped(serviceProvider =>
            {
                var loggerFactory = serviceProvider.GetService<ILoggerFactory>();

                if (loggerFactory == null)
                    throw new ArgumentNullException(nameof(loggerFactory));

                return new BuildLogger(loggerFactory);
            });

            services.AddScoped<IErosionFinderService, ErosionFinderService>();

            return services;
        }
    }
}
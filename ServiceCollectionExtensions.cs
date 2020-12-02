using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using Tidy.AdventOfCode;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>An extension to register the default implementations to a given <see cref="IServiceCollection"/>.</summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>An extension to register the default implementations to a given <see cref="IServiceCollection"/>.<br/>
        /// The following steps are taken:<br/>
        /// - console logging is added,<br/>
        /// - the <see cref="ParameterValidator"/> is registered an an <see cref="IParameterValidator"/>,<br/>
        /// - the <see cref="DayResolver"/> is registered as an <see cref="IDayResolver"/>, using the provided <paramref name="additionalSolutionAssemblies"/>,<br/>
        /// - the <see cref="HtmlAgilityPackContentExtractor"/> is registered as an <see cref="IHtmlContentExtractor"/>,<br/>
        /// - the given <paramref name="cacheDirectoryPath"/> is used to register a <see cref="DirectoryCacheManagerPathProvider"/> as an <see cref="IDirectoryCacheManagerPathProvider"/>,<br/>
        /// - the <see cref="CachingApiHandler"/> is registered as an <see cref="ICachingApiHandler"/>,<br/>
        /// - the <see cref="DirectoryCacheManager"/> is registered for resolving for both <see cref="IApiCacheManager"/> and <see cref="IApiCookieAccessor"/> (technically it is <see cref="ServiceLifetime.Transient"/> for requesting as an <see cref="IApiCookieAccessor"/>),<br/>
        /// - a <see cref="Runner"/> instance (as itself).<br/>
        /// All instances are registered for <see cref="ServiceLifetime.Singleton"/> lifetimes.<br/>
        /// Any and all implementations can be switched out by registering the relevant service type after calling this method.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to populate with <see cref="Tidy.AdventOfCode"/> services.</param>
        /// <param name="cacheDirectoryPath">The directory path used for caching the cookie, inputs, answers and responses.</param>
        /// <param name="additionalSolutionAssemblies">The assemblies to scan when looking for <see cref="IDay"/> implementations beside the entry assembly.</param>
        /// <returns>The <paramref name="services"/> collection for chaining.</returns>
        public static IServiceCollection AddTidyAdventOfCode(this IServiceCollection services, string cacheDirectoryPath, params Assembly[] additionalSolutionAssemblies) => services
            .AddLogging(l => l.AddConsole())
            .AddSingleton<IParameterValidator, ParameterValidator>()
            .AddSingleton<IDayResolver>(s => new DayResolver(s.GetRequiredService<IParameterValidator>(), s, additionalSolutionAssemblies))
            .AddSingleton<IHtmlContentExtractor, HtmlAgilityPackContentExtractor>()
            .AddSingleton<IDirectoryCacheManagerPathProvider>(new DirectoryCacheManagerPathProvider(cacheDirectoryPath))
            .AddSingleton<ICachingApiHandler, CachingApiHandler>()
            .AddSingleton<IApiCacheManager, DirectoryCacheManager>()
            .AddTransient<IApiCookieAccessor>(s => s.GetRequiredService<IApiCacheManager>() as DirectoryCacheManager ?? throw new InvalidOperationException($"Unable to resolve {nameof(IApiCacheManager)} as {nameof(DirectoryCacheManager)} to produce {nameof(IApiCookieAccessor)}. Either add or remove custom registration for both interfaces."))
            .AddSingleton<Runner>();
    }
}

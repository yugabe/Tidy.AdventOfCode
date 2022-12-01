using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        /// - configures the <see cref="RunnerOptions"/> using the provided <paramref name="configureOptions"/> (if available),<br/>
        /// - the <see cref="ParameterValidator"/> is registered an an <see cref="IParameterValidator"/>,<br/>
        /// - the <see cref="ParameterParser"/> is registered an an <see cref="IParameterParser"/>,<br/>
        /// - the <see cref="DayResolver"/> is registered as an <see cref="IDayResolver"/>, using the provided <paramref name="additionalSolutionAssemblies"/>,<br/>
        /// - the <see cref="HtmlAgilityPackContentExtractor"/> is registered as an <see cref="IHtmlContentExtractor"/>,<br/>
        /// - the given <paramref name="cacheDirectoryPath"/> is used to register a <see cref="DirectoryCacheManagerPathProvider"/> as an <see cref="IDirectoryCacheManagerPathProvider"/>,<br/>
        /// - the <see cref="CachingApiHandler"/> is registered as an <see cref="ICachingApiHandler"/>,<br/>
        /// - the <see cref="DirectoryCacheManager"/> is registered for resolving for <see cref="IApiCacheManager"/>, <see cref="IApiCookieAccessor"/> and <see cref="ICachedContentManager"/>,<br/>
        /// - a <see cref="Runner"/> instance (as itself).<br/>
        /// All instances are registered for <see cref="ServiceLifetime.Singleton"/> lifetimes.<br/>
        /// Any and all implementations can be switched out by registering the relevant service type after calling this method.<br/>
        /// Technically <see cref="IApiCookieAccessor"/> and <see cref="ICachedContentManager"/> are <see cref="ServiceLifetime.Transient"/>, as these, by default, resolve to the <see cref="IApiCacheManager"/> instance, which is <see cref="DirectoryCacheManager"/>. If both <see cref="RunnerOptions.DisableAutomaticAnswerUpload"/> and <see cref="RunnerOptions.DisableAutomaticInputDownload"/> are true, an empty cookie accessor instance is provided instead for <see cref="IApiCookieAccessor"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to populate with <see cref="Tidy.AdventOfCode"/> services.</param>
        /// <param name="cacheDirectoryPath">The directory path used for caching the cookie, inputs, answers and responses.</param>
        /// <param name="configureOptions">An action used to configure different aspects of the <see cref="Runner"/>.</param>
        /// <param name="additionalSolutionAssemblies">The assemblies to scan when looking for <see cref="IDay"/> implementations beside the entry assembly.</param>
        /// <returns>The <paramref name="services"/> collection for chaining.</returns>
        public static IServiceCollection AddTidyAdventOfCode(this IServiceCollection services, string cacheDirectoryPath, Action<RunnerOptions>? configureOptions = null, params Assembly[] additionalSolutionAssemblies) => services
            .AddLogging(l => l.AddConsole())
            .Configure(configureOptions ?? (o => { }))
            .AddSingleton<IParameterValidator, ParameterValidator>()
            .AddSingleton<IParameterParser, ParameterParser>()
            .AddSingleton<IDayResolver>(s => new DayResolver(s.GetRequiredService<IParameterValidator>(), s, additionalSolutionAssemblies))
            .AddSingleton<IHtmlContentExtractor, HtmlAgilityPackContentExtractor>()
            .AddSingleton<IDirectoryCacheManagerPathProvider>(new DirectoryCacheManagerPathProvider(cacheDirectoryPath))
            .AddSingleton<ICachingApiHandler, CachingApiHandler>()
            .AddSingleton<IApiCacheManager, DirectoryCacheManager>()
            .AddTransient<ICachedContentManager>(s => s.GetRequiredService<IApiCacheManager>() as DirectoryCacheManager ?? throw new InvalidOperationException($"Unable to resolve {nameof(IApiCacheManager)} as {nameof(DirectoryCacheManager)} to produce {nameof(ICachedContentManager)}. Either add or remove custom registration for both interfaces."))
            .AddTransient<IApiCookieAccessor>(s => (s.GetRequiredService<IOptions<RunnerOptions>>() is var options && options.Value.DisableAutomaticAnswerUpload && options.Value.DisableAutomaticInputDownload)
                ? EmptyApiCookieAccessor.Instance
                : s.GetRequiredService<IApiCacheManager>() as DirectoryCacheManager ?? throw new InvalidOperationException($"Unable to resolve {nameof(IApiCacheManager)} as {nameof(DirectoryCacheManager)} to produce {nameof(IApiCookieAccessor)}. Either add or remove custom registration for both interfaces."))
            .AddSingleton<Runner>();
    }
}

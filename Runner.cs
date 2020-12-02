using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Tidy.AdventOfCode
{
    /// <summary>The default runner for executing potential solutions for Advent of Code riddles. By default, uses caching of input, answer and response values and directly calls the server at the https://adventofcode.com/ website to get the inputs and post the answers to the riddles.</summary>
    public class Runner
    {
        /// <summary>Creates a runner to execute <see cref="IDay"/> instances' <see cref="IDay.ExecuteAsync(int, CancellationToken)"/> methods, provide and parse the inputs, post the answers and log and store the result.</summary>
        /// <param name="dayResolver">The resolver used to create <see cref="IDay"/> instances.</param>
        /// <param name="cachingApiHandler">The handler used to communicate with the server.</param>
        /// <param name="logger">The logger used for logging.</param>
        public Runner(IDayResolver dayResolver, ICachingApiHandler cachingApiHandler, ILogger<Runner> logger)
        {
            DayResolver = dayResolver;
            CachingApiHandler = cachingApiHandler;
            Logger = logger;
        }

        /// <summary>
        /// Creates a default <see cref="Runner"/> by creating a <see cref="ServiceProvider"/> instance by configuring the <see cref="ServiceCollectionExtensions.AddTidyAdventOfCode(IServiceCollection, string, Assembly[])"/> extension with the supplied parameters and retrieving the <see cref="Runner"/> instance from the provider.
        /// </summary>
        /// <param name="cacheDirectoryPath">This parameter is passed to the <see cref="ServiceCollectionExtensions.AddTidyAdventOfCode(IServiceCollection, string, Assembly[])"/> method.</param>
        /// <param name="configureServices">An optional call to augment the created <see cref="IServiceCollection"/> instance with custom services or overrides.</param>
        /// <param name="additionalSolutionAssemblies">This parameter is passed to the <see cref="ServiceCollectionExtensions.AddTidyAdventOfCode(IServiceCollection, string, Assembly[])"/> method.</param>
        /// <returns>The <see cref="Runner"/> instance from the <see cref="ServiceProvider"/>.</returns>
        public static Runner CreateDefault(string cacheDirectoryPath, Action<IServiceCollection>? configureServices = null, params Assembly[] additionalSolutionAssemblies)
        {
            var services = new ServiceCollection().AddTidyAdventOfCode(cacheDirectoryPath, additionalSolutionAssemblies);
            configureServices?.Invoke(services);
            return services.BuildServiceProvider().GetRequiredService<Runner>();
        }

        /// <summary>The resolver used to create <see cref="IDay"/> instances.</summary>
        public IDayResolver DayResolver { get; }
        /// <summary>The handler used to communicate with the server.</summary>
        public ICachingApiHandler CachingApiHandler { get; }
        /// <summary>The logger used for logging.</summary>
        public ILogger<Runner> Logger { get; }

        /// <summary>
        /// Executes the solution found in <see cref="IDay"/> for <paramref name="year"/>, <paramref name="dayNumber"/> and <paramref name="part"/>. By default, a run consist of the following:<br/>
        /// - creating an <see cref="IDay"/> using <see cref="DayResolver.CreateDay(int, int)"/>,<br/>
        /// - getting the input value for the riddle (either from the server or API),<br/>
        /// - parse the input using the <see cref="IDay.ParseInput(string)"/> method and store the result in the <see cref="IDay.Input"/> property,<br/>
        /// - execute the <see cref="IDay.ExecuteAsync(int, CancellationToken)"/> method to get the answer to the riddle,<br/>
        /// - post the answer to the server or retrieve it from cache (if it was already posted).<br/>
        /// At all points, the method logs useful information (running times, the answer, the result) to the <see cref="Logger"/>.
        /// </summary>
        /// <param name="year">The riddle's corresponding year (between 2015 and the current year).</param>
        /// <param name="dayNumber">The riddle's corresponding day number (between 1 and 25).</param>
        /// <param name="part">The riddle's corresponding part (1 or 2).</param>
        /// <param name="cancellationToken">The cancellation token to cancel any pending operations in case a cancellation (e.g. application exit) is requested.</param>
        /// <returns>The result of posting the answer to the server.</returns>
        public virtual async Task<string> ExecuteAsync(int year, int dayNumber, int part, CancellationToken cancellationToken = default)
        {
            var stopwatch = new Stopwatch();

            return await MeasureAndLogAsync(async () =>
            {
                using var day = MeasureAndLog(() => DayResolver.CreateDay(year, dayNumber),
                    (r, t) => Logger.LogDebug("{Year}-{Day}: Day of type {DayType} was created in {Elapsed}.", year, dayNumber, r.GetType().FullName, t));

                var input = await MeasureAndLogAsync(async () => await CachingApiHandler.GetInputAsync(year, dayNumber, cancellationToken),
                    (r, t) => Logger.LogDebug("{Year}-{Day}: Input of length {InputLength} was acquired in {Elapsed}.", year, dayNumber, r.Length, t));

                day.Input = MeasureAndLog(() => day.ParseInput(input),
                    (r, t) => Logger.LogDebug("{Year}-{Day}: Input {ParsedType} was parsed in {Elapsed}.", year, dayNumber, r?.GetType().Name ?? "(unknown)", stopwatch.Elapsed));

                var answer = await MeasureAndLogAsync(async () => (await day.ExecuteAsync(part, cancellationToken)).ToString(),
                    (r, t) =>
                    {
                        Logger.LogDebug("{Year}-{Day}: Part {Part} completed in {Elapsed}.", year, dayNumber, r?.GetType().Name ?? "(unknown)", t);
                        Logger.LogInformation("{Year}-{Day}: Answer:\n{Answer}", year, dayNumber, r);
                    });

                if (string.IsNullOrWhiteSpace(answer))
                    throw new InvalidOperationException($"The answer for year {year}, day {dayNumber} part {part} was null or white space.");

                var result = await MeasureAndLogAsync(async () => await CachingApiHandler.PostAnswerAsync(year, dayNumber, part, answer, cancellationToken),
                    (r, t) =>
                    {
                        Logger.LogDebug("{Year}-{Day}: Path {Part} response recieved in {Elapsed}.", year, dayNumber, part, t);
                        var doc = new HtmlAgilityPack.HtmlDocument();
                        doc.LoadHtml(r);
                        Logger.LogInformation("{Year}-{Day}: Result:\n{Result}", year, dayNumber, doc.DocumentNode.InnerText);
                    });

                return $"{answer}\n\n{result}";
            },
            (r, t) => Logger.LogInformation("{Year}-{Day}: Part {Part} run completed in {Elapsed}.", year, dayNumber, part, t), new Stopwatch());

            T MeasureAndLog<T>(Func<T> func, Action<T, TimeSpan> logFunction, Stopwatch? innerStopwatch = null)
            {
                (innerStopwatch ?? stopwatch).Restart();
                var result = func();
                logFunction(result, stopwatch.Elapsed);
                return result;
            }

            async Task<T> MeasureAndLogAsync<T>(Func<Task<T>> func, Action<T, TimeSpan> logFunction, Stopwatch? innerStopwatch = null)
            {
                (innerStopwatch ?? stopwatch).Restart();
                var result = await func();
                logFunction(result, stopwatch.Elapsed);
                return result;
            }
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Tidy.AdventOfCode
{
    /// <summary>The default runner for executing potential solutions for Advent of Code riddles. By default, uses caching of input, answer and response values and directly calls the server at the https://adventofcode.com/ website to get the inputs and post the answers to the riddles.</summary>
    public partial class Runner
    {
        private static IReadOnlyDictionary<Regex, int> RegexMatchColorCodes { get; } = new Dictionary<Regex, int>() { [CorrectAnswer()] = 92, [IncorrectAnswer()] = 31, [GoldStar()] = 93 };

        /// <summary>Creates a runner to execute <see cref="IDay"/> instances' <see cref="IDay.ExecuteAsync(int, CancellationToken)"/> methods, provide and parse the inputs, post the answers and log and store the result.</summary>
        /// <param name="dayResolver">The resolver used to create <see cref="IDay"/> instances.</param>
        /// <param name="cachingApiHandler">The handler used to communicate with the server.</param>
        /// <param name="logger">The logger used for logging.</param>
        /// <param name="options">The options object used for configuring different aspects of the runner.</param>
        /// <param name="parameterValidator">The validator used to validate yearday and part values.</param>
        /// <param name="cachedContentManager">Used for caching the execution parameters (year, dayNumber and part values).</param>
        /// <param name="parameterParser">The parser used to get the valid format of the parsable year-dayNumber-part strings.</param>
        public Runner(IDayResolver dayResolver, ICachingApiHandler cachingApiHandler, ILogger<Runner> logger, IOptions<RunnerOptions> options, IParameterValidator parameterValidator, ICachedContentManager cachedContentManager, IParameterParser parameterParser)
        {
            DayResolver = dayResolver;
            CachingApiHandler = cachingApiHandler;
            Logger = logger;
            Options = options;
            ParameterValidator = parameterValidator;
            CachedContentManager = cachedContentManager;
            ParameterParser = parameterParser;
        }

        /// <summary>The resolver used to create <see cref="IDay"/> instances.</summary>
        public IDayResolver DayResolver { get; }
        /// <summary>The handler used to communicate with the server.</summary>
        public ICachingApiHandler CachingApiHandler { get; }
        /// <summary>The logger used for logging.</summary>
        public ILogger<Runner> Logger { get; }
        /// <summary>Options for configuring different aspects of the <see cref="Runner"/>.</summary>
        public IOptions<RunnerOptions> Options { get; }
        /// <summary>The validator used to validate yearday and part values.</summary>
        public IParameterValidator ParameterValidator { get; }
        /// <summary>Used for caching the execution parameters (year, dayNumber and part values).</summary>
        public ICachedContentManager CachedContentManager { get; }
        /// <summary>The parser used to get the valid format of the parsable year-dayNumber-part strings.</summary>
        public IParameterParser ParameterParser { get; }

        /// <summary>
        /// Creates a default <see cref="Runner"/> by creating a <see cref="ServiceProvider"/> instance by configuring the <see cref="ServiceCollectionExtensions.AddTidyAdventOfCode"/> extension with the supplied parameters and retrieving the <see cref="Runner"/> instance from the provider.
        /// </summary>
        /// <param name="cacheDirectoryPath">This parameter is passed to the <see cref="ServiceCollectionExtensions.AddTidyAdventOfCode"/> method. If null, on Windows, a directory named Tidy.AdventOfCode is created in the user's AppData folder (as provided by the APPDATA environment variable). If null, but not on Windows, an <see cref="ArgumentNullException"/> is thrown.</param>
        /// <param name="configureOptions">An action used to configure different aspects of the <see cref="Runner"/>.</param>
        /// <param name="configureServices">An optional call to augment the created <see cref="IServiceCollection"/> instance with custom services or overrides.</param>
        /// <param name="additionalSolutionAssemblies">This parameter is passed to the <see cref="ServiceCollectionExtensions.AddTidyAdventOfCode"/> method.</param>
        /// <returns>The <see cref="Runner"/> instance from the <see cref="ServiceProvider"/>.</returns>
        public static Runner CreateDefault(string? cacheDirectoryPath = null, Action<RunnerOptions>? configureOptions = null, Action<IServiceCollection>? configureServices = null, params Assembly[] additionalSolutionAssemblies)
        {
            var services = new ServiceCollection().AddTidyAdventOfCode(cacheDirectoryPath ??
                (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? Directory.CreateDirectory(Path.Combine(Environment.GetEnvironmentVariable("APPDATA")!, "Tidy.AdventOfCode")).FullName
                    : throw new ArgumentNullException(nameof(cacheDirectoryPath), "The path has to be supplied if not running on Windows.")), configureOptions, additionalSolutionAssemblies);
            configureServices?.Invoke(services);
            return services.BuildServiceProvider().GetRequiredService<Runner>();
        }

        /// <summary>Gets the parameters (year, day number and part) from the Console standard input. The default is the cached value. This method loops until a correctly formatted parameter tuple is returned.</summary>
        /// <returns>The values provided by the user.</returns>
        public virtual (int year, int dayNumber, int part) GetParametersFromConsole()
        {
            Logger.LogInformation("Getting parameters from console...");
            if (!CachedContentManager.TryGetLastParameters(out var parameters))
                parameters = (DateTime.Today.Year, DateTime.Today.Month == 12 && DateTime.Today.Day <= 25 ? DateTime.Today.Day : 1, 1);

            while (true)
            {
                static void WriteInverted(string text)
                {
                    Console.ResetColor();
                    (Console.BackgroundColor, Console.ForegroundColor) = (Console.ForegroundColor, Console.BackgroundColor);
                    Console.Write(text);
                    Console.ResetColor();
                }

                Console.ResetColor();
                Console.Write($"Please input the year, day and part values as '");
                WriteInverted(ParameterParser.LongFormatString);
                Console.Write("' to execute the corresponding riddle's solution.\nLeave empty to use '");
                WriteInverted(ParameterParser.Convert(parameters.Value.year, parameters.Value.dayNumber, parameters.Value.part));
                Console.WriteLine("':");
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    return parameters.Value;
                if (ParameterParser.TryParseFull(line, out var parsed))
                    return parsed.Value;
                Console.WriteLine("Invalid value provided.");
            }
        }

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
        /// <returns>The result of posting the answer to the server, or the answer if posting is disabled.</returns>
        public virtual async Task<string> ExecuteAsync(int year, int dayNumber, int part, CancellationToken cancellationToken = default)
        {
            await CachedContentManager.WriteLastParametersAsync(year, dayNumber, part);

            return await MeasureAndLogAsync(async () =>
            {
                using var day = MeasureAndLog(() => DayResolver.CreateDay(year, dayNumber),
                    (r, t) => Logger.LogDebug("{Year}-{Day}-{Part}: Day of type {DayType} was created in {Elapsed}.", year, dayNumber, part, r.GetType().FullName, t));

                var input = await MeasureAndLogAsync(async () => await CachingApiHandler.GetInputAsync(year, dayNumber, Options.Value.DisableAutomaticInputDownload, cancellationToken),
                    (r, t) => Logger.LogDebug("{Year}-{Day}-{Part}: Input of length {InputLength} was acquired in {Elapsed}.", year, dayNumber, part, r.Length, t));

                day.Input = MeasureAndLog(() => day.ParseInput(input),
                    (r, t) => Logger.LogDebug("{Year}-{Day}-{Part}: Input {ParsedType} was parsed in {Elapsed}.", year, dayNumber, part, r?.GetType().Name ?? "(unknown)", t));

                var answer = await MeasureAndLogAsync(async () => (await day.ExecuteAsync(part, cancellationToken)).ToString(),
                    (r, t) => Logger.LogInformation("{Year}-{Day}-{Part}: Completed in {Elapsed}. Answer:\n{Answer}", year, dayNumber, part, t, r));

                if (string.IsNullOrWhiteSpace(answer))
                    throw new InvalidOperationException($"The answer for year {year}, day {dayNumber} part {part} was null or white space.");

                if (Options.Value.CopyAnswerToClipboard && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = $"/c \"echo {answer.Replace("\"", "\\\"")}| clip\"",
                            RedirectStandardOutput = true,
                        }
                    };
                    process.Start();
                    process.StandardOutput.ReadToEnd();
                }

                if (Options.Value.DisableAutomaticAnswerUpload)
                    return answer;

                var result = await MeasureAndLogAsync(async () => await CachingApiHandler.PostAnswerAsync(year, dayNumber, part, answer, cancellationToken),
                    (r, t) =>
                    {
                        Logger.LogDebug("{Year}-{Day}-{Part}: Response recieved in {Elapsed}.", year, dayNumber, part, t);
                        var doc = new HtmlAgilityPack.HtmlDocument();
                        doc.LoadHtml(r);
                        var responseText = MultipleWhitespace().Replace(doc.DocumentNode.InnerText, " ").Trim();
                        if (Options.Value.ColorizeResponses)
                            responseText = RegexMatchColorCodes.Aggregate(responseText, (acc, token) => token.Key.Replace(acc, $"\x1B[{token.Value}m$1\x1B[0m"));
                        Logger.LogInformation("{Year}-{Day}-{Part}: Result:\n{Result}", year, dayNumber, part, responseText);
                    });

                return $"{answer}\n\n{result}";
            },
            (r, t) => Logger.LogDebug("{Year}-{Day}-{Part}: Run completed in {Elapsed}.", year, dayNumber, part, t));

            T MeasureAndLog<T>(Func<T> func, Action<T, TimeSpan> logFunction)
            {
                var stopwatch = Stopwatch.StartNew();
                var result = func();
                logFunction(result, stopwatch.Elapsed);
                return result;
            }

            async Task<T> MeasureAndLogAsync<T>(Func<Task<T>> func, Action<T, TimeSpan> logFunction)
            {
                var stopwatch = Stopwatch.StartNew();
                var result = await func();
                logFunction(result, stopwatch.Elapsed);
                return result;
            }
        }

        /// <summary>Convenience method to execute the solution found in <see cref="IDay"/> for the year, day number and part provided by the user via <see cref="Console"/>. Uses <see cref="GetParametersFromConsole"/> and <see cref="ExecuteAsync(int, int, int, CancellationToken)"/>. See those methods for further information.</summary>
        /// <param name="cancellationToken">The cancellation token to cancel any pending operations in case a cancellation (e.g. application exit) is requested.</param>
        /// <returns>The result of posting the answer to the server, or the answer if posting is disabled.</returns>
        public virtual async Task<string> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var (year, dayNumber, part) = GetParametersFromConsole();
            return await ExecuteAsync(year, dayNumber, part, cancellationToken);
        }

        [GeneratedRegex("\\s{2,}")]
        private static partial Regex MultipleWhitespace();

        [GeneratedRegex("(That's the right answer!)")]
        private static partial Regex CorrectAnswer();

        [GeneratedRegex("(That's not the right answer(.*)\\.)")]
        private static partial Regex IncorrectAnswer();

        [GeneratedRegex("(gold star)")]
        private static partial Regex GoldStar();

    }
}

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tidy.AdventOfCode
{
    /// <summary>A cache manager object that stores the cookie, the riddle inputs and responses as file entries in the provided directory.</summary>
    public class DirectoryCacheManager : IApiCacheManager, IApiCookieAccessor
    {
        /// <summary>Create a directory-based cache manager.</summary>
        /// <param name="directoryCacheManagerPathProvider">The provider used to acquire the base cache directory path.</param>
        /// <param name="parameterValidator">The validator used to validate year, day and part values.</param>
        public DirectoryCacheManager(IDirectoryCacheManagerPathProvider directoryCacheManagerPathProvider, IParameterValidator parameterValidator)
        {
            Directory = new DirectoryInfo(directoryCacheManagerPathProvider.Path);
            Directory.Create();
            InputsDirectory = Directory.CreateSubdirectory("Inputs");
            OutputsDirectory = Directory.CreateSubdirectory("Outputs");
            DirectoryCacheManagerPathProvider = directoryCacheManagerPathProvider;
            ParameterValidator = parameterValidator;
        }

        private string? _cookieValue;
        /// <summary>Gets or sets the stored cookie value. The cookie should contain the cookie value for the key 'session', and should be placed in the directory provided by the <see cref="DirectoryCacheManagerPathProvider"/> instance named 'cookie.txt'. Consequent calls return a cached value, but setting the value updates the cache and stores the value in the text file.</summary>
        public string CookieValue
        {
            get => _cookieValue ??= File.ReadAllText((Directory.GetFiles("cookie.txt").SingleOrDefault() ?? throw new InvalidOperationException($"You should create a file at \"{Path.Combine(DirectoryCacheManagerPathProvider.Path, "cookie.txt")}\" that contains the \"session\" cookie value from the https://adventofcode.com/ website after logging in in a browser.")).FullName);
            set => File.WriteAllText(Path.Combine(Directory.FullName, "cookie.txt"), _cookieValue = value);
        }

        /// <summary>Get the base directory used for caching.</summary>
        public DirectoryInfo Directory { get; }
        /// <summary>Get the base directory used for caching input values.</summary>
        public DirectoryInfo InputsDirectory { get; }
        /// <summary>Get the base directory used for caching output values.</summary>
        public DirectoryInfo OutputsDirectory { get; }
        /// <summary>The provider used to acquire the base cache directory path.</summary>
        public IDirectoryCacheManagerPathProvider DirectoryCacheManagerPathProvider { get; }
        /// <summary>The validator used to validate year, day and part values.</summary>
        public IParameterValidator ParameterValidator { get; }

        /// <inheritdoc/>
        private static bool TryReadValue(FileInfo? fileInfo, [NotNullWhen(true)] out string? contents) =>
            (contents = fileInfo?.Exists == true ? File.ReadAllText(fileInfo.FullName) : null) != null;

        /// <inheritdoc/>
        public async Task WriteInputAsync(int year, int day, string input, CancellationToken cancellationToken)
        {
            ParameterValidator.Validate(year, day);
            await File.WriteAllTextAsync(Path.Combine(InputsDirectory.FullName, $"{year}-{day}.txt"), input, cancellationToken);
        }

        /// <inheritdoc/>
        public bool TryGetInputValue(int year, int day, [NotNullWhen(true)] out string? input)
        {
            ParameterValidator.Validate(year, day);
            return TryReadValue(InputsDirectory.GetFiles($"{year}-{day}.txt").SingleOrDefault(), out input);
        }

        /// <inheritdoc/>
        public string GetInputValue(int year, int day)
        {
            if (!TryGetInputValue(year, day, out var input))
                throw new InvalidOperationException($"The input for year {year} and day {day} was not found in the cache. The location should be:\n'{Path.Combine(InputsDirectory.FullName, $"{year}-{day}.txt")}'");
            return input;
        }

        /// <inheritdoc/>
        public async Task WriteAnswerAsync(int year, int day, int part, string answer, string htmlResponse, CancellationToken cancellationToken)
        {
            ParameterValidator.Validate(year, day, part);
            await File.WriteAllTextAsync(Path.Combine(OutputsDirectory.CreateSubdirectory(@$"Year {year}\Day {day}").FullName, $"{year}-{day}-{part}-{GetStableHash(answer)}.answer.txt"), answer, cancellationToken);
            await File.WriteAllTextAsync(Path.Combine(OutputsDirectory.CreateSubdirectory($"Year {year}").CreateSubdirectory($"Day {day}").FullName, $"{year}-{day}-{part}-{GetStableHash(answer)}.response.html"), htmlResponse, cancellationToken);
        }

        /// <inheritdoc/>
        public bool TryGetResponseForAnswer(int year, int day, int part, string answer, [NotNullWhen(true)] out string? htmlResponse)
        {
            ParameterValidator.Validate(year, day, part);
            return TryReadValue(OutputsDirectory.GetDirectories($"Year {year}").SingleOrDefault()?.GetDirectories($"Day {day}").SingleOrDefault()?.GetFiles($"{year}-{day}-{part}-{GetStableHash(answer)}.response.html").SingleOrDefault(), out htmlResponse);
        }

        /// <summary>Calculates a simple, repeatable hash for the given <paramref name="text"/> (an answer) that is used as part of naming the cached file entries. If the <paramref name="text"/> can be parsed as an <see cref="int"/>, the number value is used. If the <paramref name="text"/> is not longer than 10 characters long, the string value itself is used. Otherwise, a default <see cref="MD5"/> hash is calculated using <see cref="Encoding.UTF8"/>, of which the first 10 characters are used.</summary>
        /// <param name="text">The text to calculate the simple hash for.</param>
        /// <returns>The repeatable hash for the input <paramref name="text"/>.</returns>
        public static string GetStableHash(string text) =>
            int.TryParse(text, out var result) ? result.ToString() : text.Length <= 10 ? text : Encoding.UTF8.GetString(MD5.ComputeHash(Encoding.UTF8.GetBytes(text)))[..10];

        private static MD5 MD5 { get; } = MD5.Create();
    }
}

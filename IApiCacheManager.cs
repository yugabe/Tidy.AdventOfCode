using System.Diagnostics.CodeAnalysis;

namespace Tidy.AdventOfCode
{
    /// <summary>Represents an object responsible for storing and retrieving riddle inputs and corresponding responses for caching purposes.</summary>
    public interface IApiCacheManager
    {
        /// <summary>Gets the <paramref name="input"/> value for the corresponding <paramref name="year"/> and <paramref name="day"/> if available.</summary>
        /// <param name="year">The corresponding year.</param>
        /// <param name="day">The corresponding day.</param>
        /// <param name="input">The input, if available. Not null if found.</param>
        /// <returns>True if the <paramref name="input"/> was found.</returns>
        bool TryGetInputValue(int year, int day, [NotNullWhen(true)] out string? input);

        /// <summary>Gets the input value for the corresponding <paramref name="year"/> and <paramref name="day"/>, or throws an exception if not found.</summary>
        /// <param name="year">The corresponding year.</param>
        /// <param name="day">The corresponding day.</param>
        /// <returns>The plain text input value as found in the cache.</returns>
        string GetInputValue(int year, int day);

        /// <summary>Gets the <paramref name="htmlResponse"/> value for the corresponding <paramref name="year"/>, <paramref name="day"/>, <paramref name="part"/> and <paramref name="answer"/>, if available.</summary>
        /// <param name="year">The corresponding year.</param>
        /// <param name="day">The corresponding day.</param>
        /// <param name="part">The corresponding part.</param>
        /// <param name="answer">The provided answer.</param>
        /// <param name="htmlResponse">The HTML response, if available. Not null if found.</param>
        /// <returns>True if the <paramref name="htmlResponse"/> was found.</returns>
        bool TryGetResponseForAnswer(int year, int day, int part, string answer, [NotNullWhen(true)] out string? htmlResponse);

        /// <summary>Store the <paramref name="answer"/> and corresponding <paramref name="htmlResponse"/> in the cache.</summary>
        /// <param name="year">The corresponding year.</param>
        /// <param name="day">The corresponding day.</param>
        /// <param name="part">The corresponding part.</param>
        /// <param name="answer">The provided answer.</param>
        /// <param name="htmlResponse">The response, as recieved from the server.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> that completes when the write operations finished successfully.</returns>
        Task WriteAnswerAsync(int year, int day, int part, string answer, string htmlResponse, CancellationToken cancellationToken = default);

        /// <summary>Store the provided <paramref name="input"/> in the cache.</summary>
        /// <param name="year">The corresponding year.</param>
        /// <param name="day">The corresponding day.</param>
        /// <param name="input">The input corresponding to the year and day (for the given user).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> that completes when the write operation finished successfully.</returns>
        Task WriteInputAsync(int year, int day, string input, CancellationToken cancellationToken = default);
    }
}

namespace Tidy.AdventOfCode
{
    /// <summary>
    /// A handler responsible for getting the inputs from and posting the answers to https://adventofcode.com/ using the user's session cookie.
    /// Note that this handler uses local caching of the inputs and answers/results.
    /// </summary>
    public interface ICachingApiHandler
    {
        /// <summary>
        /// Get the input string value for the given <paramref name="year"/> and <paramref name="day"/> from the server (unless ignored by <paramref name="useCacheOnly"/>). Note that input does not vary between the two parts of a riddle. If the input was already obtained (or <paramref name="useCacheOnly"/> is true), it will be returned from local file cache.
        /// </summary>
        /// <param name="year">The relevant year.</param>
        /// <param name="day">The relevant day.</param>
        /// <param name="useCacheOnly">Whether to use the local cache to look for the input only, and not the server.</param>
        /// <param name="cancellationToken">The token used for cancelling asynchronous requests.</param>
        /// <returns>The input given for the <paramref name="year"/> and <paramref name="day"/>.</returns>
        Task<string> GetInputAsync(int year, int day, bool useCacheOnly = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the result of a given answer to a riddle from the server. If the same answer was already posted, the result will be returned from local file cache. Note that this might interfere with correctly solving a riddle when multiple answers were posted in a short amount of time as AoC allows only one try per minute. In this case the warning result will be cached, and the cache might need to be cleared manually.
        /// </summary>
        /// <param name="year">The relevant year.</param>
        /// <param name="day">The relevant day.</param>
        /// <param name="part">The relevant part.</param>
        /// <param name="answer">The answer, which is normally provided on the UI in an input box.</param>
        /// <param name="cancellationToken">The token used for cancelling asynchronous requests.</param>
        /// <returns>The response HTML's content of the &lt;main&gt; tag.</returns>
        Task<string> PostAnswerAsync(int year, int day, int part, string answer, CancellationToken cancellationToken = default);
    }
}
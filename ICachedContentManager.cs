using System.Diagnostics.CodeAnalysis;

namespace Tidy.AdventOfCode
{
    /// <summary>Manages non-critical cached content, like the last used year-day-part combination.</summary>
    public interface ICachedContentManager
    {
        /// <summary>Tries to get the parameters used when last run, if available.</summary>
        /// <param name="parameters">The year, dayNumber and part values, as stored (if available).</param>
        /// <returns>True if the parameters were found in the cache.</returns>
        bool TryGetLastParameters([NotNullWhen(true)] out (int year, int dayNumber, int part)? parameters);

        /// <summary>Saves the provided parameters to the cache. Validation is used before storing the values.</summary>
        /// <param name="year">The year value to store.</param>
        /// <param name="dayNumber">The day number value to store.</param>
        /// <param name="part">The part value to store.</param>
        /// <returns>A task that completes when the cache write operation finishes.</returns>
        Task WriteLastParametersAsync(int year, int dayNumber, int part);
    }
}

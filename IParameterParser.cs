using System.Diagnostics.CodeAnalysis;

namespace Tidy.AdventOfCode
{
    /// <summary>A simple string parser, which can parse a string value to a year-dayNumber(-part) tuple.</summary>
    public interface IParameterParser
    {
        /// <summary>Tries parsing the provided text to a year-dayNumber-part tuple.</summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="parameters">The parsed values, if parsing was successful.</param>
        /// <returns>True if parsing was successful.</returns>
        bool TryParseFull(string text, [NotNullWhen(true)] out (int year, int dayNumber, int part)? parameters);

        /// <summary>Tries parsing the provided text to a year-dayNumber tuple.</summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="parameters">The parsed values, if parsing was successful.</param>
        /// <returns>True if parsing was successful.</returns>
        bool TryParse(string text, [NotNullWhen(true)] out (int year, int dayNumber)? parameters);

        /// <summary>Parses the provided text to a year-dayNumber-part tuple, or throws an <see cref="ArgumentException"/>.</summary>
        /// <param name="text">The text to parse.</param>
        /// <returns>The parsed values</returns>
        /// <exception cref="ArgumentException">The provided <paramref name="text"/> was not in the correct format.</exception>
        (int year, int dayNumber, int part) ParseFull(string text);

        /// <summary>Parses the provided text to a year-dayNumber tuple, or throws an <see cref="ArgumentException"/>.</summary>
        /// <param name="text">The text to parse.</param>
        /// <returns>The parsed values</returns>
        /// <exception cref="ArgumentException">The provided <paramref name="text"/> was not in the correct format.</exception>
        (int year, int dayNumber) Parse(string text);

        /// <summary>Converts the provided parameters to the format readable by this parser. The values are validated using the <see cref="TryParse"/> or <see cref="TryParseFull"/> method.</summary>
        /// <param name="year">The year value.</param>
        /// <param name="dayNumber">The day number value.</param>
        /// <param name="part">The (optional) part value.</param>
        /// <returns>The converted string.</returns>
        string Convert(int year, int dayNumber, int? part = null);

        /// <summary>Gets the short format string.</summary>
        string ShortFormatString { get; }

        /// <summary>Gets the long format string.</summary>
        string LongFormatString { get; }
    }
}

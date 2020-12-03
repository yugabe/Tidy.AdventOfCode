using System;
using System.Diagnostics.CodeAnalysis;

namespace Tidy.AdventOfCode
{
    /// <summary>Parses strings in the YYYY-D, YYYY-DD, YYYY-D-P and YYYY-DD-P formats to year-dayNumber(-part) values. The values are not validated.</summary>
    public class ParameterParser : IParameterParser
    {
        /// <summary>Create a parameter parser for simple parsing of parameters.</summary>
        /// <param name="parameterValidator">The validator used to only parse/resolve correct values.</param>
        public ParameterParser(IParameterValidator parameterValidator)
        {
            ParameterValidator = parameterValidator;
        }
        /// <inheritdoc/>
        public string ShortFormatString => "YYYY-D[D]";

        /// <inheritdoc/>
        public string LongFormatString => "YYYY-D[D]-P";

        /// <summary>The validator used to only parse/resolve correct values.</summary>
        public IParameterValidator ParameterValidator { get; }

        /// <inheritdoc/>
        public string Convert(int year, int dayNumber, int? part = null)
        {
            string value;
            if (part == null)
            {
                value = $"{year}-{dayNumber}";
                if (!TryParse(value, out var values) || values.Value.year != year || values.Value.dayNumber != dayNumber)
                    throw new ArgumentException($"The provided {nameof(year)} ({year}) and/or {nameof(dayNumber)} ({dayNumber}) values are not convertible.");
            }
            else
            {
                value = $"{year}-{dayNumber}-{part}";
                if (!TryParseFull(value, out var valuesWithPart) || valuesWithPart.Value.year != year || valuesWithPart.Value.dayNumber != dayNumber || valuesWithPart.Value.part != part)
                    throw new ArgumentException($"The provided {nameof(year)} ({year}) and/or {nameof(dayNumber)} ({dayNumber}) and/or {nameof(part)} ({part}) values are not convertible.");
            }

            return value;
        }

        /// <inheritdoc/>
        public (int year, int dayNumber) Parse(string text) =>
            TryParse(text, out var parameters) ? parameters.Value : throw new ArgumentException($"The provided text ({text}) was not in the correct YYYY-D or YYYY-DD format.", nameof(text));

        /// <inheritdoc/>
        public (int year, int dayNumber, int part) ParseFull(string text) =>
            TryParseFull(text, out var parameters) ? parameters.Value : throw new ArgumentException($"The provided text ({text}) was not in the correct YYYY-D-P or YYYY-DD-P format.", nameof(text));

        /// <inheritdoc/>
        public bool TryParse(string text, [NotNullWhen(true)] out (int year, int dayNumber)? parameters)
        {
            parameters = null;
            var parts = text.Split('-');
            if (parts.Length != 2 || parts[0].Length != 4 || parts[1].Length is not (1 or 2) || !int.TryParse(parts[0], out var year) || !int.TryParse(parts[1], out var dayNumber))
                return false;
            if (!ParameterValidator.IsValid(year, dayNumber))
                return false; parameters = (year, dayNumber);
            return true;
        }

        /// <inheritdoc/>
        public bool TryParseFull(string text, [NotNullWhen(true)] out (int year, int dayNumber, int part)? parameters)
        {
            parameters = null;
            var parts = text.Split('-');
            if (parts.Length != 3 || parts[0].Length != 4 || parts[1].Length is not (1 or 2) || !int.TryParse(parts[0], out var year) || !int.TryParse(parts[1], out var dayNumber) || parts[2].Length != 1 || !int.TryParse(parts[2], out var part))
                return false;
            if (!ParameterValidator.IsValid(year, dayNumber, part))
                return false;
            parameters = (year, dayNumber, part);
            return true;
        }
    }
}

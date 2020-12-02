using System;

namespace Tidy.AdventOfCode
{
    /// <inheritdoc/>
    public class ParameterValidator : IParameterValidator
    {
        /// <inheritdoc/>
        public void Validate(int year, int day)
        {
            if (year < 2015 || year > DateTime.Today.Year)
                throw new ArgumentOutOfRangeException(nameof(year), "The 'year' value has to be between 2015 and the current year.");

            if (day < 1 || day > (year == DateTime.Today.Year ? DateTime.Today.Day : 25))
                throw new ArgumentOutOfRangeException(nameof(day), "The 'day' value has to be between 1 and the current day of the month, or 25 in previous years.");
        }

        /// <inheritdoc/>
        public void Validate(int year, int day, int part)
        {
            Validate(year, day);

            if (part is < 1 or > 2)
                throw new ArgumentOutOfRangeException(nameof(part), "The 'part' value has to be either 1 or 2.");
        }
    }
}

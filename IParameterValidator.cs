namespace Tidy.AdventOfCode
{
    /// <summary>Validates year, day and part values.</summary>
    public interface IParameterValidator
    {
        /// <summary>Validates a <paramref name="year"/>-<paramref name="day"/> pair. Throws an <see cref="ArgumentOutOfRangeException"/> if not valid.</summary>
        /// <param name="year">The year value. Should be at least 2015, and shouldn't be larger than the current year.</param>
        /// <param name="day">The day value. Should be between 1 and 25, but the <paramref name="year"/>-day pair should point to a day that is not in the future.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the parameters are out of the defined range.</exception>
        void Validate(int year, int day);
        /// <summary>Validates a <paramref name="year"/>-<paramref name="day"/> pair and a <paramref name="part"/> value. Throws an <see cref="ArgumentOutOfRangeException"/> if not valid.</summary>
        /// <param name="year">The year value. Should be at least 2015, and shouldn't be larger than the current year.</param>
        /// <param name="day">The day value. Should be between 1 and 25, but the <paramref name="year"/>-day pair should point to a day that is not in the future.</param>
        /// <param name="part">Should be either 1 or 2.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the parameters are out of the defined range.</exception>
        void Validate(int year, int day, int part);

        /// <summary>Validates a <paramref name="year"/>-<paramref name="day"/> pair.</summary>
        /// <param name="year">The year value. Should be at least 2015, and shouldn't be larger than the current year.</param>
        /// <param name="day">The day value. Should be between 1 and 25, but the <paramref name="year"/>-day pair should point to a day that is not in the future.</param>
        bool IsValid(int year, int day);
        /// <summary>Validates a <paramref name="year"/>-<paramref name="day"/> pair and a <paramref name="part"/> value.</summary>
        /// <param name="year">The year value. Should be at least 2015, and shouldn't be larger than the current year.</param>
        /// <param name="day">The day value. Should be between 1 and 25, but the <paramref name="year"/>-day pair should point to a day that is not in the future.</param>
        /// <param name="part">Should be either 1 or 2.</param>
        bool IsValid(int year, int day, int part);
    }
}

namespace Tidy.AdventOfCode
{
    /// <summary>Responsible for creating <see cref="IDay"/> instances.</summary>
    public interface IDayResolver
    {
        /// <summary>Create a day object for a given <paramref name="year"/>-<paramref name="day"/> pair.</summary>
        /// <param name="year">The year the <see cref="IDay"/> instance corresponds to.</param>
        /// <param name="day">The day the <see cref="IDay"/> instance corresponds to.</param>
        /// <returns>The constructed <see cref="IDay"/> instance.</returns>
        IDay CreateDay(int year, int day);
    }
}
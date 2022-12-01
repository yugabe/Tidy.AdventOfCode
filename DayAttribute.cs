namespace Tidy.AdventOfCode
{
    /// <summary>Used to annotate a non-conventionally named <see cref="Day{T}"/> object with the corresponding year and day values, or ignore a <see cref="Day{T}"/> subclass from being resolved by an <see cref="IDayResolver"/>. By convention, <see cref="Day{T}"/> objects should be named Day# or Day##, where # and ## represents the day number, and be placed in a namespace which has a last segment of Year####, where #### corresponds to the relevant year.</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class DayAttribute : Attribute
    {
        /// <summary>Annotate a <see cref="Day{T}"/> type with the given <paramref name="year"/> and <paramref name="dayNumber"/> values when not conventionally named.</summary>
        /// <param name="year">The relevant year for the <see cref="Day{T}"/> type.</param>
        /// <param name="dayNumber">The relevant day for the <see cref="Day{T}"/> type.</param>
        public DayAttribute(int year, int dayNumber)
        {
            Year = year;
            DayNumber = dayNumber;
        }

        /// <summary>The year number that the <see cref="Day{T}"/> belongs to.</summary>
        public int Year { get; }
        /// <summary>The day number that the <see cref="Day{T}"/> belongs to.</summary>
        public int DayNumber { get; }
        /// <summary>Indicates whether to ignore the <see cref="Day{T}"/> type from being included in automatic resolution from an <see cref="IDayResolver"/>.</summary>
        public bool Ignore { get; set; }
    }
}

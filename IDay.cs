namespace Tidy.AdventOfCode
{
    /// <summary>Represents a riddle solution in the Advent of Code calendar.</summary>
    public interface IDay : IDisposable
    {
        /// <summary>
        /// Executes the riddle solution and returns whatever result is materialized.
        /// </summary>
        /// <param name="part">The part to execute (1 or 2).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The materialized result that should be posted on the website's input field.</returns>
        Task<object> ExecuteAsync(int part, CancellationToken cancellationToken = default);

        /// <summary>
        /// Parses the provided input value to something that can be understood by the riddle's solution.
        /// </summary>
        /// <param name="rawInput">The raw input to be provided to the riddle.</param>
        /// <returns>The parsed input value.</returns>
        object ParseInput(string rawInput);

        /// <summary>The parsed input value.</summary>
        object Input { get; set; }
    }
}
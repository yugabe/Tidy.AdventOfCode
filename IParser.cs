namespace Tidy.AdventOfCode
{
    /// <summary>Represents a parser that can be reused as providing an instance of the type <typeparamref name="T"/> from the raw input.</summary>
    /// <typeparam name="T">The instance that should be constructed from the provided input.</typeparam>
    public interface IParser<out T>
    {
        /// <summary>Parses/transforms the provided <paramref name="rawInput"/> input to a <typeparamref name="T"/> instance.</summary>
        /// <param name="rawInput">The input to be used for parsing. Generally the input can be obtained from the website after logging in, or the input can be mocked to provide the same input as if the provided examples in the riddle were given to the user as input.</param>
        /// <returns>The constructed <typeparamref name="T"/> value.</returns>
        T Parse(string rawInput);
    }
}
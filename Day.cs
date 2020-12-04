using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tidy.AdventOfCode
{
    /// <summary>Uses no parsing of the input value, simply provides the input string unmodified. See also <seealso cref="Day{T}"/>. Can also be used to access the nested class <see cref="NewLineSplitParsed{T}"/> to use the default <see cref="TypeDescriptor.GetConverter(object)"/> for a given type for automatic conversion from strings. You can and should create additional abstractions based on your needs from this class.</summary>
    public abstract class Day : Day<string>
    {
        /// <summary>Splits the input string along newlines ('\n') and uses <see cref="TypeDescriptor.GetConverter(object)"/> for <typeparamref name="T"/> to convert using the <see cref="TypeConverter.ConvertFromString(string)"/> method. See also <seealso cref="Day{T}"/>.</summary>
        /// <typeparam name="T">The type of the data items (as an array) used when processing the input, looking for the correct solution. If using the default <see cref="Runner"/> and service configuration, the input is provided by the <see cref="ParseInput(string)"/> method to the <see cref="Day{T}.Input"/> property to be used in one of the executor methods.</typeparam>
        public abstract class NewLineSplitParsed<T> : Day<T[]>
        {
            private static TypeConverter Converter { get; } = TypeDescriptor.GetConverter(typeof(T));

            /// <summary>
            /// Parses the provided <paramref name="rawInput"/> by splitting along the newline ('\n') character, and projecting each line to a <typeparamref name="T"/> type by using the default <see cref="TypeConverter"/>.
            /// </summary>
            /// <param name="rawInput">The raw input value.</param>
            /// <returns>The parsed <typeparamref name="T"/> values as a generic array.</returns>
            public override T[] ParseInput(string rawInput) => rawInput.Split('\n').Select(i => (T)Converter.ConvertFromString(i)).ToArray();
        }

        /// <summary>Returns the input string.</summary>
        /// <param name="rawInput">The input to return.</param>
        /// <returns>The <paramref name="rawInput"/> unmodified.</returns>
        public override string ParseInput(string rawInput) => rawInput;
    }

    /// <summary>The base convenience class to inherit from when creating solutions for Advent of Code riddles.<br/>
    /// Note that this class contains an additional convenience nested class for reusable parsing of the input value: <see cref="Day{T}.WithParser{TParser}"/>, with which you can use custom (reusable) <see cref="IParser{T}"/> implementations.
    /// You can and should create additional abstractions based on your needs from this class.</summary>
    /// <typeparam name="T">The type of the data used when processing the input, looking for the correct solution. If using the default <see cref="Runner"/> and service configuration, then the input is provided by the <see cref="ParseInput(string)"/> method to the <see cref="Input"/> property to be used in one of the executor methods.</typeparam>
    public abstract class Day<T> : IDay
    {
        /// <summary>Creates an instance of <typeparamref name="T"/> for processing, by constructing an <typeparamref name="TParser"/> instance. See also <seealso cref="Day{T}"/>.</summary>
        /// <typeparam name="TParser">The <see cref="IParser{T}"/> type to be used for constructing the <typeparamref name="T"/> instance.</typeparam>
        public abstract class WithParser<TParser> : Day<T> where TParser : IParser<T>, new()
        {
            /// <summary>Creates an instance of <typeparamref name="T"/> for processing, by constructing an <typeparamref name="TParser"/> instance and calling <see cref="IParser{T}.Parse(string)"/>.</summary>
            /// <param name="rawInput">The raw input value.</param>
            /// <returns>The parsed <typeparamref name="T"/> instance.</returns>
            public override T ParseInput(string rawInput) => new TParser().Parse(rawInput);
        }

        /// <summary>The input value. Generally provided by a <see cref="Runner"/> by calling this instance's <see cref="ParseInput(string)"/> method.</summary>
        public virtual T Input { get; set; } = default!;
        object IDay.Input { get => Input!; set => Input = (T)value; }

        /// <summary>
        /// Executes the given part. Calls either <see cref="ExecutePart1Async(CancellationToken)"/> or <see cref="ExecutePart2Async(CancellationToken)"/> and returns the result (or faults).
        /// </summary>
        /// <param name="part">The part to be used. Should be either 1 or 2.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The result from the part-specific execution method (<see cref="ExecutePart1Async(CancellationToken)"/> or <see cref="ExecutePart2Async(CancellationToken)"/>);</returns>
        public virtual async Task<object> ExecuteAsync(int part, CancellationToken cancellationToken = default) =>
            part switch
            {
                1 => await ExecutePart1Async(cancellationToken),
                2 => await ExecutePart2Async(cancellationToken),
                _ => throw new ArgumentOutOfRangeException(nameof(part), "The part to execute should be either 1 or 2.")
            };

        /// <inheritdoc cref="IDay.ParseInput(string)"/>
        public abstract T ParseInput(string rawInput);

        /// <summary>Executes part 1 of the solution. By default, <see cref="ExecutePart1Async(CancellationToken)"/> calls this method, and this throws a <see cref="NotImplementedException"/> and either this or <see cref="ExecutePart1Async(CancellationToken)"/> should be overridden.</summary>
        /// <returns>The answer to part 1 of the riddle, as computed.</returns>
        public virtual object ExecutePart1() => throw new NotImplementedException();

        /// <summary>Executes part 2 of the solution. By default, <see cref="ExecutePart2Async(CancellationToken)"/> calls this method, and this throws a <see cref="NotImplementedException"/> and either this or <see cref="ExecutePart2Async(CancellationToken)"/> should be overridden.</summary>
        /// <returns>The answer to part 2 of the riddle, as computed.</returns>
        public virtual object ExecutePart2() => throw new NotImplementedException();

        /// <summary>Asynchronously executes part 1 of the solution. By default, this method calls <see cref="ExecutePart1"/>, which throws a <see cref="NotImplementedException"/> and either this or <see cref="ExecutePart1"/> should be overridden.</summary>
        /// <returns>The answer to part 1 of the riddle, as computed asynchronously.</returns>
        public virtual Task<object> ExecutePart1Async(CancellationToken cancellationToken = default) => Task.FromResult(ExecutePart1());

        /// <summary>Asynchronously executes part 2 of the solution. By default, this method calls <see cref="ExecutePart2"/>, which throws a <see cref="NotImplementedException"/> and either this or <see cref="ExecutePart1"/> should be overridden.</summary>
        /// <returns>The answer to part 1 of the riddle, as computed asynchronously.</returns>
        public virtual Task<object> ExecutePart2Async(CancellationToken cancellationToken = default) => Task.FromResult(ExecutePart2());

        /// <inheritdoc/>
        public virtual void Dispose() => GC.SuppressFinalize(this);

        object IDay.ParseInput(string rawInput) => ParseInput(rawInput)!;
    }
}

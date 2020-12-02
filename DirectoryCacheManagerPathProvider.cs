namespace Tidy.AdventOfCode
{
    /// <inheritdoc/>
    public class DirectoryCacheManagerPathProvider : IDirectoryCacheManagerPathProvider
    {
        /// <summary>Create a simple provider that returns the provided path in the <see cref="Path"/> property.</summary>
        /// <param name="path">The path value to provide.</param>
        public DirectoryCacheManagerPathProvider(string path) => Path = path;

        /// <summary>The path value. Doesn't mutate once the provider is instantiated.</summary>
        public string Path { get; }
    }
}

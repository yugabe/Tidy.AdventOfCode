namespace Tidy.AdventOfCode
{
    /// <summary>The provider used to acquire the base cache directory path.</summary>
    public interface IDirectoryCacheManagerPathProvider
    {
        /// <summary>The consumed property.</summary>
        string Path { get; }
    }
}

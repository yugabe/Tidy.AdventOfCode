namespace Tidy.AdventOfCode
{
    /// <summary>Used to retrieve or store the value for the 'session' cookie used on the https://adventofcode.com/ website.</summary>
    public interface IApiCookieAccessor
    {
        /// <summary>The value for the cookie named 'session'.</summary>
        string CookieValue { get; set; }
    }
}

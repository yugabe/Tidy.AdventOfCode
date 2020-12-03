namespace Tidy.AdventOfCode
{
    internal sealed class EmptyApiCookieAccessor : IApiCookieAccessor
    {
        internal static EmptyApiCookieAccessor Instance { get; } = new EmptyApiCookieAccessor();
        public string CookieValue { get; set; } = "";
    }
}

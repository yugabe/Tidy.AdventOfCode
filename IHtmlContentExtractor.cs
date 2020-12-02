namespace Tidy.AdventOfCode
{
    /// <summary>An extractor that is able to extract the &lt;main&gt; tag from the provided HTML content.</summary>
    public interface IHtmlContentExtractor
    {
        /// <summary>Extracts the &lt;main&gt; tag from the provided HTML content.</summary>
        string Extract(string htmlContent);
    }
}

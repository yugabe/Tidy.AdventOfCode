using HtmlAgilityPack;

namespace Tidy.AdventOfCode
{
    /// <summary>An <see cref="IHtmlContentExtractor"/> that uses <see cref="HtmlAgilityPack"/> for extracting the &lt;main&gt; tag's value from provided HTML content.</summary>
    public class HtmlAgilityPackContentExtractor : IHtmlContentExtractor
    {
        /// <inheritdoc/>
        public string Extract(string htmlContent)
        {
            var document = new HtmlDocument();
            document.LoadHtml(htmlContent);
            return document.DocumentNode.SelectSingleNode("//main").InnerHtml;
        }
    }
}

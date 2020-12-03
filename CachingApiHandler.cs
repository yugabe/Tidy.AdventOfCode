using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tidy.AdventOfCode
{
    /// <inheritdoc/>
    public class CachingApiHandler : ICachingApiHandler
    {
        /// <summary>The client instance used for communicating with the https://adventofcode.com/ website.</summary>
        public HttpClient Client { get; }
        /// <summary>The API cache manager. Used to look up and store the inputs and results.</summary>
        public IApiCacheManager ApiCacheManager { get; }
        /// <summary>The parameter validator. Used to validate year, day and part values.</summary>
        public IParameterValidator ParameterValidator { get; }
        /// <summary>The HTML content extractor. Used to extract the &lt;main&gt; tag's content from the full HTML from HTTP body.</summary>
        public IHtmlContentExtractor HtmlContentExtractor { get; }

        /// <summary>Creates a caching API handler object.</summary>
        /// <param name="apiCookieAccessor">The cookie accessor object. Used to put the relevant request header to the <see cref="HttpClient"/> when constructing.</param>
        /// <param name="apiCacheManager">The API cache manager. Used to look up and store the inputs and results.</param>
        /// <param name="parameterValidator">The parameter validator. Used to validate year, day and part values.</param>
        /// <param name="htmlContentExtractor">The HTML content extractor. Used to extract the &lt;main&gt; tag's content from the full HTML from HTTP body.</param>
        public CachingApiHandler(IApiCookieAccessor apiCookieAccessor, IApiCacheManager apiCacheManager, IParameterValidator parameterValidator, IHtmlContentExtractor htmlContentExtractor)
        {
            Client = new HttpClient() { BaseAddress = new Uri("https://adventofcode.com/") };
            Client.DefaultRequestHeaders.Add("Cookie", $"session={apiCookieAccessor.CookieValue}");
            ApiCacheManager = apiCacheManager;
            ParameterValidator = parameterValidator;
            HtmlContentExtractor = htmlContentExtractor;
        }

        /// <inheritdoc/>
        public async Task<string> GetInputAsync(int year, int day, bool useCacheOnly, CancellationToken cancellationToken)
        {
            ParameterValidator.Validate(year, day);

            if (useCacheOnly)
                ApiCacheManager.GetInputValue(year, day);

            if (ApiCacheManager.TryGetInputValue(year, day, out var input))
                return input;

            input = (await (await Client.GetAsync($"/{year}/day/{day}/input", cancellationToken))
                .EnsureSuccessStatusCode().Content.ReadAsStringAsync(cancellationToken)).Trim();
            await ApiCacheManager.WriteInputAsync(year, day, input, cancellationToken);
            return input;
        }

        /// <inheritdoc/>
        public async Task<string> PostAnswerAsync(int year, int day, int part, string answer, CancellationToken cancellationToken)
        {
            if (ApiCacheManager.TryGetResponseForAnswer(year, day, part, answer, out var htmlResponse))
                return htmlResponse;

            var response = HtmlContentExtractor.Extract((await (await Client.PostAsync($"/{year}/day/{day}/answer", new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["level"] = $"{part}",
                ["answer"] = answer
            }!), cancellationToken)).EnsureSuccessStatusCode().Content.ReadAsStringAsync(cancellationToken)));

            await ApiCacheManager.WriteAnswerAsync(year, day, part, answer, response, cancellationToken);
            return response;
        }
    }
}

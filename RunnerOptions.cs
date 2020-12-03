namespace Tidy.AdventOfCode
{
    /// <summary>The options object used to configure different aspects of the default <see cref="Runner"/>.</summary>
    public sealed class RunnerOptions
    {
        /// <summary>If set to true, the download (and caching) of the user's input will be disabled. Default value is false.</summary>
        public bool DisableAutomaticInputDownload { get; set; } = false;

        /// <summary>If set to true, the upload (and caching) of the user's answers and the caching of recieved responses will be disabled. Default value is false.</summary>
        public bool DisableAutomaticAnswerUpload { get; set; } = false;

        /// <summary>If set to true, the answer value will be copied to the clipboard after a successful run (only on Windows). Default is false.</summary>
        public bool CopyAnswerToClipboard { get; set; } = false;
    }
}
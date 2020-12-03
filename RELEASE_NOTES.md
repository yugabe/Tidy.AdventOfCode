![Advent of Code logo](icon.png)

# Tidy.AdventOfCode - Release Notes

## 1.1.0

Made the `Runner` configurable in a few aspects:
- Configuration is done optionally using the provided action in `IServiceCollection.AddTidyAdventOfCode` or `Runner.CreateDefault`.
- You can disable automatically getting your inputs from the server. In this case, you have to put your input files according to the IApiCacheManager instance's internal policy (which is, by default placed in the provided directory root's Inputs folder named YYYY-D.txt or YYYY-DD.txt). If no file would be found, you'll get an exception.
- You can disable automatically posting your results to the server. You'll still get the results in the console output.
- You can choose to automatically copy the results of the last successful run to the clipboard using `cmd`, `echo` and `clip` (on Windows only, on other platforms this flag is ignored).

----

*Keep on hackin'! Click the ⭐!*
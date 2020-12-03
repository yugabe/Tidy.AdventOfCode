![Advent of Code logo](icon.png)

# Tidy.AdventOfCode - Release Notes

## 1.2.0

So much work, so little code! Now you can run the runner with all the default settings by just putting this into your Program.cs file:

``` C#
await Tidy.AdventOfCode.Runner.CreateDefault().ExecuteAsync();
```

I mean, yeah, that's ALL the code that's needed, not even any more `using`s or anything.

The little features that made these possible are:
- A new `IParameterParser` service, that can parse strings to year-dayNumber(-part) tuples.
- A new caching mechanism, that stores the last parameters that are supplied to the runner's `ExecuteAsync` method.
- The default cache directory on Windows is the APPDATA (usually C:\Users\you\AppData\Roaming) folder's 'Tidy.AdventOfCode' subdirectory. You can find all the cached files here.

## 1.1.0

Made the `Runner` configurable in a few aspects:
- Configuration is done optionally using the provided action in `IServiceCollection.AddTidyAdventOfCode` or `Runner.CreateDefault`.
- You can disable automatically getting your inputs from the server. In this case, you have to put your input files according to the IApiCacheManager instance's internal policy (which is, by default placed in the provided directory root's Inputs folder named YYYY-D.txt or YYYY-DD.txt). If no file would be found, you'll get an exception.
- You can disable automatically posting your results to the server. You'll still get the results in the console output.
- You can choose to automatically copy the results of the last successful run to the clipboard using `cmd`, `echo` and `clip` (on Windows only, on other platforms this flag is ignored).

----

*Keep on hackin'! Click the ⭐!*
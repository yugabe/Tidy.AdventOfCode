![Advent of Code logo](icon.png)

# Tidy.AdventOfCode - Release Notes

## 2.0.2

Another minor fix: when returning with a long value from your solution, there was a chance for the `long`-`string`-`int` safe conversion to fail when trying to create the file in the file system. Now the conversion uses `long` values.

## 2.0.1

Well, the first bug has shown its head. Introduced with the feature in 1.2.1, the correct string to look for in the response didn't have the correct punctuation (there is no pediod character after the text "You gave an answer too recently"). I **always** say: *Don't use magic strings in your code!*... but it seems even I can't follow my own advice. Oh well. 

I have to say, it's a little bit of a Christmas miracle it took this long for a bug to show up at all 🐱‍👤

### Migration from 1.2.1~2.0.0 to 2.0.1 or above
If you jumped the gun on 1.2.1 or 2.0.0 early, you have to clear the wrong responses from your cache directory manually.

## 2.0.0

The first breaking change! It's nothing major though. Created a simple `Day` class to inherit from when using no parsing of the input value (instead of using `Day<TAnything>.Raw`, which actually was still a `Day<string>` and `TAnything` was discarded). So now you can inherit from these classes you can write your solutions in:
- `Day`: Inherit from this class when you don't want to parse the input to any other format. This way you have access to the raw input (as a string value accessible via the `Input` property). Technically this is a subclass of `Day<string>`.
- `Day.NewLineSplitParsed<T>`: Inherit from this class when you don't want to parse the input, but want to convert it to a simple `T` type like `int` or `float` for each line of the input. The conversion is automatic by using the default `TypeConverter` for the given `T` type, but it might fail (in which case you have to choose any of the other ones). Technically this is a subclass of `Day<T[]>`.
- `Day<T>`: Inherit from this class when you want to work on an input of type `T`. You have to provide the parsing by overriding the `ParseInput` method.
- `Day<T>.WithParser<TParser>`: Inherit from this class when you want to work on an input of type `T`, and you want to use an `IParser<T>` object that can parse simple text to a `T` instance.

Also, `ISimpleParser` and `IMultipleParser` were consolidated into `IParser`, and you are given the option to... well... simply parse into whatever data structure you like. The corresponding APIs broke as a result, so you'll have to rename your `ForMany` and `ForOne` calls to `WithParser`, and rename the implementor's `ParseOne` and `ParseMany` methods to... yeah, you guessed it: `Parse`.

### Migration from 1._._ to 2.0.0

Your solutions which inherited from `Day<_>.Raw` now should inherit from `Day`. Your solutions which inherited from `Day<_>.NewLineSplitParsed<T>` should now inherit from `Day.NewLineSplitParsed<T>`. 

No other changes are necessary.

## 1.2.1

Now the server responses won't be cached if the response's parsed content contains the text "You gave an answer too recently.".

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
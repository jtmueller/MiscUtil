# Introduction 

These are some miscellaneous utilities that I find useful. You might too!
Feel free to use the library as-is, or pull the code into your own project.
Supports .NET Standard 2.0 and 2.1 (.NET Framework 4.6.1+ and .NET Core)

# What's Included?

**Parsing extension methods for `ReadOnlySpan<char>`.** These methods return nullable
value types to indicate parse failure.  Parse values from a string without first 
allocating a substring! Save memory, enjoy great performance!

 * `ToInt32`
 * `ToDouble`
 * `ToDecimal`
 * `ToGuid`
 * `ToBoolean`
 * `ToDateTime` (limited format support in .NET Framework/Standard 2.0)
 * `ToDateTimeOffset` (limited format support in .NET Framework/Standard 2.0)
 * Support for other types added as my needs, or your requests (or pull requests!) dictate.
   See [Utf8Parser](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.text.utf8parser?view=netstandard-2.1) 
   for available types.

**Comparison:**

 * TrimEquals for both `string` and `ReadOnlySpan<char>`. Find out if two strings would
   be equal if they were both trimmed, without actually allocating any new strings.

**Split Spans:**

  * `Split` and `SplitAll` extension methods for `ReadOnlySpan<T>`. The main use is
    for splitting strings without allocating a bunch of substrings. With the parsing
    extension methods, you can parse numbers/dates/guids out of a CSV file with zero 
    heap allocations! Works with any type of ReadOnlySpan, however: split a sequence
    of bytes or integers just as easily.

**StringBuilder Extensions:** Do you ever find yourself manipulating the string returned
by `StringBuilder.ToString()`? Maybe trimming it, or converting the case? Save yourself
a string allocation by using these extension methods to apply your changes to the 
StringBuilder itself, before allocating the return string.

  * `TrimEnd`, `TrimStart`, `Trim`
  * `ToUpperInvariant`/`ToUpper`
  * `ToLowerInvariant`/`ToLower`
  * .NET Standard 2.0 only: adds `Append` and `CopyTo` overloads that accept `ReadOnlySpan<char>`
    and `Span<char>` respectively (these overloads are built-in to .NET Standard 2.1).

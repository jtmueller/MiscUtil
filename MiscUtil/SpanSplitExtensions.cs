using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

// based on: https://gist.github.com/LordJZ/92b7decebe52178a445a0b82f63e585a

namespace MiscUtil
{
    public static class SpanSplitExtensions
    {
        public ref struct Enumerable1<T> where T : IEquatable<T>
        {
            public Enumerable1(ReadOnlySpan<T> span, T separator, SpanSplitOptions options)
            {
                Span = span;
                Separator = separator;
                Options = options;
            }

            ReadOnlySpan<T> Span { get; }
            T Separator { get; }
            SpanSplitOptions Options { get; }

            public Enumerator1<T> GetEnumerator() => new Enumerator1<T>(Span, Separator, Options);
        }

        public ref struct Enumerable2<T> where T : IEquatable<T>
        {
            public Enumerable2(ReadOnlySpan<T> span, T separator1, T separator2, SpanSplitOptions options)
            {
                Span = span;
                Separator1 = separator1;
                Separator2 = separator2;
                Options = options;
            }

            ReadOnlySpan<T> Span { get; }
            T Separator1 { get; }
            T Separator2 { get; }
            SpanSplitOptions Options { get; }

            public Enumerator2<T> GetEnumerator() => new Enumerator2<T>(Span, Separator1, Separator2, Options);
        }

        public ref struct Enumerable3<T> where T : IEquatable<T>
        {
            public Enumerable3(ReadOnlySpan<T> span, T separator1, T separator2, T separator3, SpanSplitOptions options)
            {
                Span = span;
                Separator1 = separator1;
                Separator2 = separator2;
                Separator3 = separator3;
                Options = options;
            }

            ReadOnlySpan<T> Span { get; }
            T Separator1 { get; }
            T Separator2 { get; }
            T Separator3 { get; }
            SpanSplitOptions Options { get; }

            public Enumerator3<T> GetEnumerator() =>
                new Enumerator3<T>(Span, Separator1, Separator2, Separator3, Options);
        }

        public ref struct EnumerableN<T> where T : IEquatable<T>
        {
            public EnumerableN(ReadOnlySpan<T> span, ReadOnlySpan<T> separators, SpanSplitOptions options)
            {
                Span = span;
                Separators = separators;
                Options = options;
            }

            ReadOnlySpan<T> Span { get; }
            ReadOnlySpan<T> Separators { get; }
            SpanSplitOptions Options { get; }

            public EnumeratorN<T> GetEnumerator() => new EnumeratorN<T>(Span, Separators, Options);
        }

        public ref struct EnumerableAll<T> where T : IEquatable<T>
        {
            public EnumerableAll(ReadOnlySpan<T> span, ReadOnlySpan<T> separator, SpanSplitOptions options)
            {
                Span = span;
                Separator = separator;
                Options = options;
            }

            ReadOnlySpan<T> Span { get; }
            ReadOnlySpan<T> Separator { get; }
            SpanSplitOptions Options { get; }

            public EnumeratorWord<T> GetEnumerator() => new EnumeratorWord<T>(Span, Separator, Options);
        }

        public ref struct Enumerator1<T> where T : IEquatable<T>
        {
            public Enumerator1(ReadOnlySpan<T> span, T separator, SpanSplitOptions options)
            {
                Span = span;
                Separator = separator;
                Current = default;
                Options = options;

                if (Span.IsEmpty && Options == SpanSplitOptions.None)
                    TrailingEmptyItem = true;
            }

            ReadOnlySpan<T> Span { get; set; }
            T Separator { get; }
            SpanSplitOptions Options { get; }
            int SeparatorLength => 1;

            ReadOnlySpan<T> TrailingEmptyItemSentinel => Unsafe.As<T[]>(nameof(TrailingEmptyItemSentinel)).AsSpan();

            bool TrailingEmptyItem
            {
                get => Span == TrailingEmptyItemSentinel;
                set => Span = value ? TrailingEmptyItemSentinel : default;
            }

            public bool MoveNext()
            {
                if (TrailingEmptyItem)
                {
                    TrailingEmptyItem = false;
                    Current = default;
                    return true;
                }

                retry:
                if (Span.IsEmpty)
                {
                    Span = Current = default;
                    return false;
                }

                int idx = Span.IndexOf(Separator);
                if (idx < 0)
                {
                    Current = Span;
                    Span = default;
                }
                else
                {
                    Current = Span.Slice(0, idx);
                    Span = Span.Slice(idx + SeparatorLength);
                    if (Current.IsEmpty && Options == SpanSplitOptions.RemoveEmptyEntries)
                        goto retry;
                    if (Span.IsEmpty && Options == SpanSplitOptions.None)
                        TrailingEmptyItem = true;
                }

                return true;
            }

            public ReadOnlySpan<T> Current { get; private set; }
        }

        public ref struct Enumerator2<T> where T : IEquatable<T>
        {
            public Enumerator2(ReadOnlySpan<T> span, T separator1, T separator2, SpanSplitOptions options)
            {
                Span = span;
                Separator1 = separator1;
                Separator2 = separator2;
                Current = default;
                Options = options;

                if (Span.IsEmpty && Options == SpanSplitOptions.None)
                    TrailingEmptyItem = true;
            }

            ReadOnlySpan<T> Span { get; set; }
            T Separator1 { get; }
            T Separator2 { get; }
            SpanSplitOptions Options { get; }
            int SeparatorLength => 1;

            ReadOnlySpan<T> TrailingEmptyItemSentinel => Unsafe.As<T[]>(nameof(TrailingEmptyItemSentinel)).AsSpan();

            bool TrailingEmptyItem
            {
                get => Span == TrailingEmptyItemSentinel;
                set => Span = value ? TrailingEmptyItemSentinel : default;
            }

            public bool MoveNext()
            {
                if (TrailingEmptyItem)
                {
                    TrailingEmptyItem = false;
                    Current = default;
                    return true;
                }

                retry:
                if (Span.IsEmpty)
                {
                    Span = Current = default;
                    return false;
                }

                int idx = Span.IndexOfAny(Separator1, Separator2);
                if (idx < 0)
                {
                    Current = Span;
                    Span = default;
                }
                else
                {
                    Current = Span.Slice(0, idx);
                    Span = Span.Slice(idx + SeparatorLength);
                    if (Current.IsEmpty && Options == SpanSplitOptions.RemoveEmptyEntries)
                        goto retry;
                    if (Span.IsEmpty && Options == SpanSplitOptions.None)
                        TrailingEmptyItem = true;
                }

                return true;
            }

            public ReadOnlySpan<T> Current { get; private set; }
        }

        public ref struct Enumerator3<T> where T : IEquatable<T>
        {
            public Enumerator3(ReadOnlySpan<T> span, T separator1, T separator2, T separator3, SpanSplitOptions options)
            {
                Span = span;
                Separator1 = separator1;
                Separator2 = separator2;
                Separator3 = separator3;
                Current = default;
                Options = options;

                if (Span.IsEmpty && Options == SpanSplitOptions.None)
                    TrailingEmptyItem = true;
            }

            ReadOnlySpan<T> Span { get; set; }
            T Separator1 { get; }
            T Separator2 { get; }
            T Separator3 { get; }
            SpanSplitOptions Options { get; }
            int SeparatorLength => 1;

            ReadOnlySpan<T> TrailingEmptyItemSentinel => Unsafe.As<T[]>(nameof(TrailingEmptyItemSentinel)).AsSpan();

            bool TrailingEmptyItem
            {
                get => Span == TrailingEmptyItemSentinel;
                set => Span = value ? TrailingEmptyItemSentinel : default;
            }

            public bool MoveNext()
            {
                if (TrailingEmptyItem)
                {
                    TrailingEmptyItem = false;
                    Current = default;
                    return true;
                }

                retry:
                if (Span.IsEmpty)
                {
                    Span = Current = default;
                    return false;
                }

                int idx = Span.IndexOfAny(Separator1, Separator2, Separator3);
                if (idx < 0)
                {
                    Current = Span;
                    Span = default;
                }
                else
                {
                    Current = Span.Slice(0, idx);
                    Span = Span.Slice(idx + SeparatorLength);
                    if (Current.IsEmpty && Options == SpanSplitOptions.RemoveEmptyEntries)
                        goto retry;
                    if (Span.IsEmpty && Options == SpanSplitOptions.None)
                        TrailingEmptyItem = true;
                }

                return true;
            }

            public ReadOnlySpan<T> Current { get; private set; }
        }

        public ref struct EnumeratorN<T> where T : IEquatable<T>
        {
            public EnumeratorN(ReadOnlySpan<T> span, ReadOnlySpan<T> separators, SpanSplitOptions options)
            {
                Span = span;
                Separators = separators;
                Current = default;
                Options = options;

                if (Span.IsEmpty && Options == SpanSplitOptions.None)
                    TrailingEmptyItem = true;
            }

            ReadOnlySpan<T> Span { get; set; }
            ReadOnlySpan<T> Separators { get; }
            SpanSplitOptions Options { get; }
            int SeparatorLength => 1;

            ReadOnlySpan<T> TrailingEmptyItemSentinel => Unsafe.As<T[]>(nameof(TrailingEmptyItemSentinel)).AsSpan();

            bool TrailingEmptyItem
            {
                get => Span == TrailingEmptyItemSentinel;
                set => Span = value ? TrailingEmptyItemSentinel : default;
            }

            public bool MoveNext()
            {
                if (TrailingEmptyItem)
                {
                    TrailingEmptyItem = false;
                    Current = default;
                    return true;
                }

                retry:
                if (Span.IsEmpty)
                {
                    Span = Current = default;
                    return false;
                }

                int idx = Span.IndexOfAny(Separators);
                if (idx < 0)
                {
                    Current = Span;
                    Span = default;
                }
                else
                {
                    Current = Span.Slice(0, idx);
                    Span = Span.Slice(idx + SeparatorLength);
                    if (Current.IsEmpty && Options == SpanSplitOptions.RemoveEmptyEntries)
                        goto retry;
                    if (Span.IsEmpty && Options == SpanSplitOptions.None)
                        TrailingEmptyItem = true;
                }

                return true;
            }

            public ReadOnlySpan<T> Current { get; private set; }
        }

        public ref struct EnumeratorWord<T> where T : IEquatable<T>
        {
            public EnumeratorWord(ReadOnlySpan<T> span, ReadOnlySpan<T> separator, SpanSplitOptions options)
            {
                Span = span;
                Separator = separator;
                Current = default;
                Options = options;

                if (Span.IsEmpty && Options == SpanSplitOptions.None)
                    TrailingEmptyItem = true;
            }

            ReadOnlySpan<T> Span { get; set; }
            ReadOnlySpan<T> Separator { get; }
            SpanSplitOptions Options { get; }
            int SeparatorLength => Separator.Length;

            ReadOnlySpan<T> TrailingEmptyItemSentinel => Unsafe.As<T[]>(nameof(TrailingEmptyItemSentinel)).AsSpan();

            bool TrailingEmptyItem
            {
                get => Span == TrailingEmptyItemSentinel;
                set => Span = value ? TrailingEmptyItemSentinel : default;
            }

            public bool MoveNext()
            {
                if (TrailingEmptyItem)
                {
                    TrailingEmptyItem = false;
                    Current = default;
                    return true;
                }

            retry:
                if (Span.IsEmpty)
                {
                    Span = Current = default;
                    return false;
                }

                int idx = Span.IndexOf(Separator);
                if (idx < 0)
                {
                    Current = Span;
                    Span = default;
                }
                else
                {
                    Current = Span.Slice(0, idx);
                    Span = Span.Slice(idx + SeparatorLength);
                    if (Current.IsEmpty && Options == SpanSplitOptions.RemoveEmptyEntries)
                        goto retry;
                    if (Span.IsEmpty && Options == SpanSplitOptions.None)
                        TrailingEmptyItem = true;
                }

                return true;
            }

            public ReadOnlySpan<T> Current { get; private set; }
        }

        [Pure]
        public static Enumerable1<T> Split<T>(this ReadOnlySpan<T> span, T separator, SpanSplitOptions options = SpanSplitOptions.None)
            where T : IEquatable<T> => new Enumerable1<T>(span, separator, options);

        [Pure]
        public static Enumerable2<T> Split<T>(this ReadOnlySpan<T> span, T separator1, T separator2, SpanSplitOptions options = SpanSplitOptions.None)
            where T : IEquatable<T> => new Enumerable2<T>(span, separator1, separator2, options);

        [Pure]
        public static Enumerable3<T> Split<T>(this ReadOnlySpan<T> span, T separator1, T separator2, T separator3, SpanSplitOptions options = SpanSplitOptions.None)
            where T : IEquatable<T> => new Enumerable3<T>(span, separator1, separator2, separator3, options);

        /// <summary>
        /// Splits on any of the <paramref name="splitValues"/> values given.
        /// </summary>
        /// <param name="span">The span to split.</param>
        /// <param name="splitValues">A span containing values, any one of which will trigger a split.</param>
        /// <param name="options">The <see cref="SpanSplitOptions"/> for this call.</param>
        [Pure]
        public static EnumerableN<T> Split<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> splitValues, SpanSplitOptions options = SpanSplitOptions.None)
            where T : IEquatable<T> => new EnumerableN<T>(span, splitValues, options);

        /// <summary>
        /// Splits on any occurrence of all of the values in <paramref name="splitAll"/>, in sequence.
        /// </summary>
        /// <param name="span">The span to split.</param>
        /// <param name="splitValues">A span containing values, any one of which will trigger a split.</param>
        /// <param name="options">The <see cref="SpanSplitOptions"/> for this call.</param>
        [Pure]
        public static EnumerableAll<T> SplitAll<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> splitAll, SpanSplitOptions options = SpanSplitOptions.None)
            where T : IEquatable<T> => new EnumerableAll<T>(span, splitAll, options);
    }

    /// <summary>
    /// Specifies whether <see cref="ReadOnlySpan{T}"/> split methods can return empty spans.
    /// </summary>
    [Flags]
    public enum SpanSplitOptions : byte
    {
        /// <summary>
        /// The return value may include empty spans.
        /// </summary>
        None = 0,
        /// <summary>
        /// The return value does not include empty spans.
        /// </summary>
        RemoveEmptyEntries = 1
    }
}
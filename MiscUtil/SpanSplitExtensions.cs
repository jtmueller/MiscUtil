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
            public Enumerable1(ReadOnlySpan<T> span, T separator, StringSplitOptions options)
            {
                Span = span;
                Separator = separator;
                Options = options;
            }

            ReadOnlySpan<T> Span { get; }
            T Separator { get; }
            StringSplitOptions Options { get; }

            public Enumerator1<T> GetEnumerator() => new Enumerator1<T>(Span, Separator, Options);
        }

        public ref struct Enumerable2<T> where T : IEquatable<T>
        {
            public Enumerable2(ReadOnlySpan<T> span, T separator1, T separator2, StringSplitOptions options)
            {
                Span = span;
                Separator1 = separator1;
                Separator2 = separator2;
                Options = options;
            }

            ReadOnlySpan<T> Span { get; }
            T Separator1 { get; }
            T Separator2 { get; }
            StringSplitOptions Options { get; }

            public Enumerator2<T> GetEnumerator() => new Enumerator2<T>(Span, Separator1, Separator2, Options);
        }

        public ref struct Enumerable3<T> where T : IEquatable<T>
        {
            public Enumerable3(ReadOnlySpan<T> span, T separator1, T separator2, T separator3, StringSplitOptions options)
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
            StringSplitOptions Options { get; }

            public Enumerator3<T> GetEnumerator() =>
                new Enumerator3<T>(Span, Separator1, Separator2, Separator3, Options);
        }

        public ref struct EnumerableN<T> where T : IEquatable<T>
        {
            public EnumerableN(ReadOnlySpan<T> span, ReadOnlySpan<T> separators, StringSplitOptions options)
            {
                Span = span;
                Separators = separators;
                Options = options;
            }

            ReadOnlySpan<T> Span { get; }
            ReadOnlySpan<T> Separators { get; }
            StringSplitOptions Options { get; }

            public EnumeratorN<T> GetEnumerator() => new EnumeratorN<T>(Span, Separators, Options);
        }

        public ref struct Enumerator1<T> where T : IEquatable<T>
        {
            public Enumerator1(ReadOnlySpan<T> span, T separator, StringSplitOptions options)
            {
                Span = span;
                Separator = separator;
                Current = default;
                Options = options;

                if (Span.IsEmpty && Options == StringSplitOptions.None)
                    TrailingEmptyItem = true;
            }

            ReadOnlySpan<T> Span { get; set; }
            T Separator { get; }
            StringSplitOptions Options { get; }
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
                    if (Current.IsEmpty && Options == StringSplitOptions.RemoveEmptyEntries)
                        goto retry;
                    if (Span.IsEmpty && Options == StringSplitOptions.None)
                        TrailingEmptyItem = true;
                }

                return true;
            }

            public ReadOnlySpan<T> Current { get; private set; }
        }

        public ref struct Enumerator2<T> where T : IEquatable<T>
        {
            public Enumerator2(ReadOnlySpan<T> span, T separator1, T separator2, StringSplitOptions options)
            {
                Span = span;
                Separator1 = separator1;
                Separator2 = separator2;
                Current = default;
                Options = options;

                if (Span.IsEmpty && Options == StringSplitOptions.None)
                    TrailingEmptyItem = true;
            }

            ReadOnlySpan<T> Span { get; set; }
            T Separator1 { get; }
            T Separator2 { get; }
            StringSplitOptions Options { get; }
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
                    if (Current.IsEmpty && Options == StringSplitOptions.RemoveEmptyEntries)
                        goto retry;
                    if (Span.IsEmpty && Options == StringSplitOptions.None)
                        TrailingEmptyItem = true;
                }

                return true;
            }

            public ReadOnlySpan<T> Current { get; private set; }
        }

        public ref struct Enumerator3<T> where T : IEquatable<T>
        {
            public Enumerator3(ReadOnlySpan<T> span, T separator1, T separator2, T separator3, StringSplitOptions options)
            {
                Span = span;
                Separator1 = separator1;
                Separator2 = separator2;
                Separator3 = separator3;
                Current = default;
                Options = options;

                if (Span.IsEmpty && Options == StringSplitOptions.None)
                    TrailingEmptyItem = true;
            }

            ReadOnlySpan<T> Span { get; set; }
            T Separator1 { get; }
            T Separator2 { get; }
            T Separator3 { get; }
            StringSplitOptions Options { get; }
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
                    if (Current.IsEmpty && Options == StringSplitOptions.RemoveEmptyEntries)
                        goto retry;
                    if (Span.IsEmpty && Options == StringSplitOptions.None)
                        TrailingEmptyItem = true;
                }

                return true;
            }

            public ReadOnlySpan<T> Current { get; private set; }
        }

        public ref struct EnumeratorN<T> where T : IEquatable<T>
        {
            public EnumeratorN(ReadOnlySpan<T> span, ReadOnlySpan<T> separators, StringSplitOptions options)
            {
                Span = span;
                Separators = separators;
                Current = default;
                Options = options;

                if (Span.IsEmpty && Options == StringSplitOptions.None)
                    TrailingEmptyItem = true;
            }

            ReadOnlySpan<T> Span { get; set; }
            ReadOnlySpan<T> Separators { get; }
            StringSplitOptions Options { get; }
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
                    if (Current.IsEmpty && Options == StringSplitOptions.RemoveEmptyEntries)
                        goto retry;
                    if (Span.IsEmpty && Options == StringSplitOptions.None)
                        TrailingEmptyItem = true;
                }

                return true;
            }

            public ReadOnlySpan<T> Current { get; private set; }
        }

        [Pure]
        public static Enumerable1<T> Split<T>(this ReadOnlySpan<T> span, T separator, StringSplitOptions options = StringSplitOptions.None)
            where T : IEquatable<T> => new Enumerable1<T>(span, separator, options);

        [Pure]
        public static Enumerable2<T> Split<T>(this ReadOnlySpan<T> span, T separator1, T separator2, StringSplitOptions options = StringSplitOptions.None)
            where T : IEquatable<T> => new Enumerable2<T>(span, separator1, separator2, options);

        [Pure]
        public static Enumerable3<T> Split<T>(this ReadOnlySpan<T> span, T separator1, T separator2, T separator3, StringSplitOptions options = StringSplitOptions.None)
            where T : IEquatable<T> => new Enumerable3<T>(span, separator1, separator2, separator3, options);

        [Pure]
        public static EnumerableN<T> Split<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> splitChars, StringSplitOptions options = StringSplitOptions.None)
            where T : IEquatable<T> => new EnumerableN<T>(span, splitChars, options);
    }
}
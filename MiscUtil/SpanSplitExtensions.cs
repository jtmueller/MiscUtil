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
            public Enumerable1(ReadOnlySpan<T> span, T separator)
            {
                Span = span;
                Separator = separator;
            }

            ReadOnlySpan<T> Span { get; }
            T Separator { get; }

            public Enumerator1<T> GetEnumerator() => new Enumerator1<T>(Span, Separator);
        }

        public ref struct Enumerable2<T> where T : IEquatable<T>
        {
            public Enumerable2(ReadOnlySpan<T> span, T separator1, T separator2)
            {
                Span = span;
                Separator1 = separator1;
                Separator2 = separator2;
            }

            ReadOnlySpan<T> Span { get; }
            T Separator1 { get; }
            T Separator2 { get; }

            public Enumerator2<T> GetEnumerator() => new Enumerator2<T>(Span, Separator1, Separator2);
        }

        public ref struct Enumerable3<T> where T : IEquatable<T>
        {
            public Enumerable3(ReadOnlySpan<T> span, T separator1, T separator2, T separator3)
            {
                Span = span;
                Separator1 = separator1;
                Separator2 = separator2;
                Separator3 = separator3;
            }

            ReadOnlySpan<T> Span { get; }
            T Separator1 { get; }
            T Separator2 { get; }
            T Separator3 { get; }

            public Enumerator3<T> GetEnumerator() =>
                new Enumerator3<T>(Span, Separator1, Separator2, Separator3);
        }

        public ref struct EnumerableN<T> where T : IEquatable<T>
        {
            public EnumerableN(ReadOnlySpan<T> span, ReadOnlySpan<T> separators)
            {
                Span = span;
                Separators = separators;
            }

            ReadOnlySpan<T> Span { get; }
            ReadOnlySpan<T> Separators { get; }

            public EnumeratorN<T> GetEnumerator() => new EnumeratorN<T>(Span, Separators);
        }

        public ref struct Enumerator1<T> where T : IEquatable<T>
        {
            public Enumerator1(ReadOnlySpan<T> span, T separator)
            {
                Span = span;
                Separator = separator;
                Current = default;

                if (Span.IsEmpty)
                    TrailingEmptyItem = true;
            }

            ReadOnlySpan<T> Span { get; set; }
            T Separator { get; }
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
                    if (Span.IsEmpty)
                        TrailingEmptyItem = true;
                }

                return true;
            }

            public ReadOnlySpan<T> Current { get; private set; }
        }

        public ref struct Enumerator2<T> where T : IEquatable<T>
        {
            public Enumerator2(ReadOnlySpan<T> span, T separator1, T separator2)
            {
                Span = span;
                Separator1 = separator1;
                Separator2 = separator2;
                Current = default;

                if (Span.IsEmpty)
                    TrailingEmptyItem = true;
            }

            ReadOnlySpan<T> Span { get; set; }
            T Separator1 { get; }
            T Separator2 { get; }
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
                    if (Span.IsEmpty)
                        TrailingEmptyItem = true;
                }

                return true;
            }

            public ReadOnlySpan<T> Current { get; private set; }
        }

        public ref struct Enumerator3<T> where T : IEquatable<T>
        {
            public Enumerator3(ReadOnlySpan<T> span, T separator1, T separator2, T separator3)
            {
                Span = span;
                Separator1 = separator1;
                Separator2 = separator2;
                Separator3 = separator3;
                Current = default;

                if (Span.IsEmpty)
                    TrailingEmptyItem = true;
            }

            ReadOnlySpan<T> Span { get; set; }
            T Separator1 { get; }
            T Separator2 { get; }
            T Separator3 { get; }
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
                    if (Span.IsEmpty)
                        TrailingEmptyItem = true;
                }

                return true;
            }

            public ReadOnlySpan<T> Current { get; private set; }
        }

        public ref struct EnumeratorN<T> where T : IEquatable<T>
        {
            public EnumeratorN(ReadOnlySpan<T> span, ReadOnlySpan<T> separators)
            {
                Span = span;
                Separators = separators;
                Current = default;

                if (Span.IsEmpty)
                    TrailingEmptyItem = true;
            }

            ReadOnlySpan<T> Span { get; set; }
            ReadOnlySpan<T> Separators { get; }
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
                    if (Span.IsEmpty)
                        TrailingEmptyItem = true;
                }

                return true;
            }

            public ReadOnlySpan<T> Current { get; private set; }
        }

        [Pure]
        public static Enumerable1<T> Split<T>(this ReadOnlySpan<T> span, T separator)
            where T : IEquatable<T> => new Enumerable1<T>(span, separator);

        [Pure]
        public static Enumerable2<T> Split<T>(this ReadOnlySpan<T> span, T separator1, T separator2)
            where T : IEquatable<T> => new Enumerable2<T>(span, separator1, separator2);

        [Pure]
        public static Enumerable3<T> Split<T>(this ReadOnlySpan<T> span, T separator1, T separator2, T separator3)
            where T : IEquatable<T> => new Enumerable3<T>(span, separator1, separator2, separator3);

        [Pure]
        public static EnumerableN<T> Split<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> values)
            where T : IEquatable<T> => new EnumerableN<T>(span, values);
    }
}
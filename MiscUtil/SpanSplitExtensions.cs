﻿using System;

// Borrowed from here until .net 5
// https://github.com/dotnet/runtime/blob/master/src/libraries/System.Private.CoreLib/src/System/SpanSplitEnumerator.T.cs

namespace MiscUtil
{
    public static class SpanSplitExtensions
    {
        /// <summary>
        /// Returns a type that allows for enumeration of each element within a split span
        /// using a single space as a separator character.
        /// </summary>
        /// <param name="span">The source span to be enumerated.</param>
        /// <returns>Returns a <see cref="System.SpanSplitEnumerator{T}"/>.</returns>
        public static SpanSplitEnumerator<char> Split(this ReadOnlySpan<char> span)
            => new SpanSplitEnumerator<char>(span, ' ');

        /// <summary>
        /// Returns a type that allows for enumeration of each element within a split span
        /// using the provided separator character.
        /// </summary>
        /// <param name="span">The source span to be enumerated.</param>
        /// <param name="separator">The separator character to be used to split the provided span.</param>
        /// <returns>Returns a <see cref="System.SpanSplitEnumerator{T}"/>.</returns>
        public static SpanSplitEnumerator<char> Split(this ReadOnlySpan<char> span, char separator)
            => new SpanSplitEnumerator<char>(span, separator);

        /// <summary>
        /// Returns a type that allows for enumeration of each element within a split span
        /// using the provided separator string.
        /// </summary>
        /// <param name="span">The source span to be enumerated.</param>
        /// <param name="separator">The separator string to be used to split the provided span.</param>
        /// <returns>Returns a <see cref="System.SpanSplitEnumerator{T}"/>.</returns>
        public static SpanSplitEnumerator<char> Split(this ReadOnlySpan<char> span, string separator)
            => new SpanSplitEnumerator<char>(span, (separator ?? string.Empty).AsSpan());
    }

    /// <summary>
    /// <see cref="System.SpanSplitEnumerator{T}"/> allows for enumeration of each element within a <see cref="System.ReadOnlySpan{T}"/>
    /// that has been split using a provided separator.
    /// </summary>
    public ref struct SpanSplitEnumerator<T> where T : IEquatable<T>
    {
        private readonly ReadOnlySpan<T> _buffer;

        private readonly ReadOnlySpan<T> _separators;
        private readonly T _separator;

        private readonly int _separatorLength;
        private readonly bool _splitOnSingleToken;

        private readonly bool _isInitialized;

        private int _startCurrent;
        private int _endCurrent;
        private int _startNext;

        /// <summary>
        /// Returns an enumerator that allows for iteration over the split span.
        /// </summary>
        /// <returns>Returns a <see cref="System.SpanSplitEnumerator{T}"/> that can be used to iterate over the split span.</returns>
        public SpanSplitEnumerator<T> GetEnumerator() => this;

        /// <summary>
        /// Returns the current element of the enumeration.
        /// </summary>
        /// <returns>Returns a <see cref="System.Range"/> instance that indicates the bounds of the current element withing the source span.</returns>
        public Range Current => new Range(_startCurrent, _endCurrent);

        internal SpanSplitEnumerator(ReadOnlySpan<T> span, ReadOnlySpan<T> separators)
        {
            _isInitialized = true;
            _buffer = span;
            _separators = separators;
            _separator = default!;
            _splitOnSingleToken = false;
            _separatorLength = _separators.Length != 0 ? _separators.Length : 1;
            _startCurrent = 0;
            _endCurrent = 0;
            _startNext = 0;
        }

        internal SpanSplitEnumerator(ReadOnlySpan<T> span, T separator)
        {
            _isInitialized = true;
            _buffer = span;
            _separator = separator;
            _separators = default;
            _splitOnSingleToken = true;
            _separatorLength = 1;
            _startCurrent = 0;
            _endCurrent = 0;
            _startNext = 0;
        }

        /// <summary>
        /// Advances the enumerator to the next element of the enumeration.
        /// </summary>
        /// <returns><see langword="true"/> if the enumerator was successfully advanced to the next element; <see langword="false"/> if the enumerator has passed the end of the enumeration.</returns>
        public bool MoveNext()
        {
            if (!_isInitialized || _startNext > _buffer.Length)
            {
                return false;
            }

            ReadOnlySpan<T> slice = _buffer.Slice(_startNext);
            _startCurrent = _startNext;

            int separatorIndex = _splitOnSingleToken ? slice.IndexOf(_separator) : slice.IndexOf(_separators);
            int elementLength = separatorIndex != -1 ? separatorIndex : slice.Length;

            _endCurrent = _startCurrent + elementLength;
            _startNext = _endCurrent + _separatorLength;
            return true;
        }
    }
}
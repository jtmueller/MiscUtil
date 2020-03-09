﻿using System;

namespace MiscUtil
{
    public static class Utilities
    {
        /// <summary>
        /// Returns a disposable object that will call the given action upon being disposed.
        /// </summary>
        /// <param name="onDispose">The action to call on disposing.</param>
        /// <returns></returns>
        public static DisposableWrapper<T> Dispose<T>(T obj, Action<T> onDispose) => new DisposableWrapper<T>(obj, onDispose);

        public ref struct DisposableWrapper<T>
        {
            private Action<T>? _onDispose;
            private T _obj;

            internal DisposableWrapper(T obj, Action<T> onDispose)
            {
                _obj = obj;
                _onDispose = onDispose;
            }

            public void Dispose()
            {
                if (_onDispose is object)
                {
                    _onDispose.Invoke(_obj);
                    _onDispose = null;
                    _obj = default!;
                }
            }
        }
    }
}

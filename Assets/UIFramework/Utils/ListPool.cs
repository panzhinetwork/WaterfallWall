﻿using System.Collections.Generic;

namespace UIFramework.Utilities {
    /// <summary>
    /// 从UGUI源码中挪过来的
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal static class ListPool<T> {
        // Object pool to avoid allocations.
        private static readonly ObjectPool<List<T>> s_ListPool = new ObjectPool<List<T>>(null, l => l.Clear());

        public static List<T> Get() {
            return s_ListPool.Get();
        }

        public static void Release(List<T> toRelease) {
            s_ListPool.Release(toRelease);
        }
    }
}
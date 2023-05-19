using System.Collections.Generic;

namespace UIFramework.Utilities {

    internal static class DictionaryPool<T1, T2> {
        // Object pool to avoid allocations.
        private static readonly ObjectPool<Dictionary<T1, T2>> s_DictionaryPool = new ObjectPool<Dictionary<T1, T2>>(null, d => d.Clear());

        public static Dictionary<T1, T2> Get() {
            return s_DictionaryPool.Get();
        }

        public static void Release(Dictionary<T1, T2> toRelease) {
            s_DictionaryPool.Release(toRelease);
        }
    }

}
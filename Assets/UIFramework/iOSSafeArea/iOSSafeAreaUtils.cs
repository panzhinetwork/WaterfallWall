namespace UIFramework {

    using System.Runtime.InteropServices;
    using UnityEngine;

    public struct EdgeInsets {
        public float left;
        public float right;
        public float top;
        public float bottom;

        public EdgeInsets(float left, float right, float top, float bottom) {
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }
    }

    public class iOSSafeAreaUtils {
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void _GetSafeAreaInsets(out float left, out float right, out float top, out float bottom);

        [DllImport("__Internal")]
        private static extern float _GetScreenScale();
#endif

        /// <summary>
        /// 获取安全边缘，单位是points。
        /// </summary>
        /// <returns></returns>
        public static EdgeInsets GetSafeAreaInsets() {
            float left, right, top, bottom;
#if UNITY_EDITOR
            left = 0;
            right = 0;
            top = 0;
            bottom = 0;
#elif UNITY_IOS
            _GetSafeAreaInsets(out left, out right, out top, out bottom);      
#else
            left = 0;
            right = 0;
            top = 0;
            bottom = 0;
#endif
            return new EdgeInsets(left, right, top, bottom);
        }

        /// <summary>
        /// 获取屏幕的缩放系数。
        /// </summary>
        /// <returns></returns>
        public static float GetScreenScale() {
#if UNITY_IOS && !UNITY_EDITOR
            return _GetScreenScale();
#else
            return 2f;
#endif
        }
    }

}
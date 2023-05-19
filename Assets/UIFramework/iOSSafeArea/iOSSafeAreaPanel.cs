
namespace UIFramework {

    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif


    public class iOSSafeAreaPanel : MonoBehaviour {

        public float topMarginPoints = -8f;
        public float bottomMarginPoints = 0f;
        public Vector2 canvasSize = new Vector2(720f, 1280f);

#if UNITY_EDITOR
        public const int IPHONE_6 = 0; // 分辨率 750 x 1334
        public const int IPHONE_11 = 1; // 分辨率 828 x 1792
        public const int IPHONE_11_PRO = 2; // 分辨率 1125 x 2436
        private const string SIMULATE_IPHONE_KEY = "SimulateiPhoneDevice";
        private static int s_simulateiPhoneDevice = -1;

        public static int SimulateiPhoneDevice {
            get {
                if (s_simulateiPhoneDevice == -1)
                    s_simulateiPhoneDevice = EditorPrefs.GetInt(SIMULATE_IPHONE_KEY, 0);
                return s_simulateiPhoneDevice;
            }
            set {
                if (value != s_simulateiPhoneDevice) {
                    s_simulateiPhoneDevice = value;
                    EditorPrefs.SetInt(SIMULATE_IPHONE_KEY, value);
                }
            }

        }
#endif

        void Awake() {
#if UNITY_IOS
            ApplySafeArea();
#else
            Destroy(this);
#endif
        }

        void ApplySafeArea() {
            var safeAreaInsets = iOSSafeAreaUtils.GetSafeAreaInsets();

#if UNITY_EDITOR
            var device = SimulateiPhoneDevice;
            if (device != IPHONE_6) {
                safeAreaInsets.top = 44f;
                safeAreaInsets.bottom = 34f;
            }
#endif

            if (safeAreaInsets.bottom > 0f) {
                // Is iPhone 11 or simular
                var rect = GetComponent<RectTransform>();

                var scale = iOSSafeAreaUtils.GetScreenScale();

#if UNITY_EDITOR
                scale = (device == IPHONE_11 ? 2f : 3f);
#endif

                var topPixels = (safeAreaInsets.top + topMarginPoints) * scale;
                var bottomPixels = (safeAreaInsets.bottom + bottomMarginPoints) * scale;
                var topOffset = Mathf.Floor(topPixels / Screen.height * canvasSize.y);
                var bottomOffset = Mathf.Floor(bottomPixels / Screen.height * canvasSize.y);
                rect.offsetMin = new Vector2(0f, bottomOffset);
                rect.offsetMax = new Vector2(0f, -topOffset);
            }
        }
    }


}
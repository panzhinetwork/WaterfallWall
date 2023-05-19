
namespace UIFramework {

    using UnityEditor;

    public static class iOSSafeAreaEditor {
        private const string MENU_NAME_SIMULATE_IPHONE_6 = "Tools/Simulate iPhone/iPhone 6 (750x1334)";
        private const string MENU_NAME_SIMULATE_IPHONE_11 = "Tools/Simulate iPhone/iPhone 11 (828x1792)";
        private const string MENU_NAME_SIMULATE_IPHONE_11_PRO = "Tools/Simulate iPhone/iPhone 11 Pro (1125x2436)";



        [MenuItem(MENU_NAME_SIMULATE_IPHONE_6)]
        static void SimulateiPhone6() {
            iOSSafeAreaPanel.SimulateiPhoneDevice = iOSSafeAreaPanel.IPHONE_6;
        }

        [MenuItem(MENU_NAME_SIMULATE_IPHONE_6, true)]
        static bool ValidateSimulateiPhone6() {
            Menu.SetChecked(MENU_NAME_SIMULATE_IPHONE_6,
                iOSSafeAreaPanel.SimulateiPhoneDevice == iOSSafeAreaPanel.IPHONE_6);
            return true;
        }

        [MenuItem(MENU_NAME_SIMULATE_IPHONE_11)]
        static void SimulateiPhone11() {
            iOSSafeAreaPanel.SimulateiPhoneDevice = iOSSafeAreaPanel.IPHONE_11;
        }

        [MenuItem(MENU_NAME_SIMULATE_IPHONE_11, true)]
        static bool ValidateSimulateiPhone11() {
            Menu.SetChecked(MENU_NAME_SIMULATE_IPHONE_11,
                iOSSafeAreaPanel.SimulateiPhoneDevice == iOSSafeAreaPanel.IPHONE_11);
            return true;
        }

        [MenuItem(MENU_NAME_SIMULATE_IPHONE_11_PRO)]
        static void SimulateiPhone11Pro() {
            iOSSafeAreaPanel.SimulateiPhoneDevice = iOSSafeAreaPanel.IPHONE_11_PRO;
        }

        [MenuItem(MENU_NAME_SIMULATE_IPHONE_11_PRO, true)]
        static bool ValidateSimulateiPhone11Pro() {
            Menu.SetChecked(MENU_NAME_SIMULATE_IPHONE_11_PRO,
                iOSSafeAreaPanel.SimulateiPhoneDevice == iOSSafeAreaPanel.IPHONE_11_PRO);
            return true;
        }


    }

}
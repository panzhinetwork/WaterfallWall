//获取iPhoneX safeArea
//文件名 SafeArea.m
#include <UIKit/UIKit.h>

//外部调用接口
void _GetSafeAreaInsets(float* left, float* right, float* top, float* bottom)
{
    UIEdgeInsets safeAreaInsets;
    if (@available(iOS 11.0, *)) {
        UIWindow *mainWindow = [[[UIApplication sharedApplication] delegate] window];
        safeAreaInsets = mainWindow.safeAreaInsets;
    } else {
        safeAreaInsets = UIEdgeInsetsMake(0, 0, 0, 0);
    }
    
    *left = safeAreaInsets.left;
    *right = safeAreaInsets.right;
    *top = safeAreaInsets.top;
    *bottom = safeAreaInsets.bottom;
}

float _GetScreenScale() {
    return UIScreen.mainScreen.scale;
}

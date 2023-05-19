using System;
using UnityEngine;

namespace UIFramework {

    public enum UIAnimationType {
        In,
        Out,
    }

    /// <summary>
    /// UI 切换动画的抽象类。
    /// </summary>
    public abstract class UIAnimation : MonoBehaviour {

        /// <summary>
        /// 动画类型。
        /// </summary>
        public UIAnimationType type;

        /// <summary>
        /// 开始 UI 切换动画。
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="onCompleted">On animation completed.</param>
        public abstract void StartAnimation(Transform target, Action onCompleted);

        /// <summary>
        /// 结束并立刻完成动画。
        /// </summary>
        public abstract void Kill();
    }

}
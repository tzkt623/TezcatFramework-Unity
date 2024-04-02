using System;
using tezcat.Framework.Game;
using UnityEngine;

namespace tezcat.Unity.Utility
{
    /// <summary>
    /// 
    /// 按键配置管理器类
    /// 
    /// 具体使用方法看TestKeyConfig
    /// 
    /// </summary>
    public class UnityKeyConfigSystem : TezKeyConfigManager
    {
        static int sMask = 0;
        static bool[] sLockMouseButton = new bool[3]
        {
            false,
            false,
            false
        };

        static Func<int, bool>[] sMouseButtonDown = new Func<int, bool>[3]
        {
            Input.GetMouseButtonDown,
            Input.GetMouseButtonDown,
            Input.GetMouseButtonDown
        };

        static Func<int, bool>[] sMouseButtonUp = new Func<int, bool>[3]
        {
            Input.GetMouseButtonUp,
            Input.GetMouseButtonUp,
            Input.GetMouseButtonUp
        };

        static Func<int, bool>[] sMouseButtonPressed = new Func<int, bool>[3]
        {
            Input.GetMouseButton,
            Input.GetMouseButton,
            Input.GetMouseButton
        };

        /// <summary>
        /// 鼠标点击是否锁住
        /// </summary>
        public static bool isAllLocked
        {
            ///6 = 1 + 2 + 3 (0 + 1 + 2)
            get { return sMask == 6; }
        }

        public static bool getMouseButtonDown(int button)
        {
            return sMouseButtonDown[button](button);
        }

        public static bool getMouseButtonUp(int button)
        {
            return sMouseButtonUp[button](button);
        }

        public static bool getMouseButtonPressed(int button)
        {
            return sMouseButtonPressed[button](button);
        }

        public static bool getMouseLockState(int button)
        {
            return sLockMouseButton[button];
        }

        /// <summary>
        /// 锁住某个鼠标按键
        /// </summary>
        public static void lockMouseButton(int button)
        {
            sMask += (button + 1);
            sLockMouseButton[button] = true;
            sMouseButtonDown[button] = returnFalse;
            sMouseButtonUp[button] = returnFalse;
            sMouseButtonPressed[button] = returnFalse;
        }

        /// <summary>
        /// 解锁某个鼠标按键
        /// </summary>
        public static void unlockMouseButton(int button)
        {
            sMask -= (button + 1);
            sLockMouseButton[button] = false;
            sMouseButtonDown[button] = Input.GetMouseButtonDown;
            sMouseButtonUp[button] = Input.GetMouseButtonUp;
            sMouseButtonPressed[button] = Input.GetMouseButton;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void lockAllMouseButton()
        {
            sMouseButtonDown[0] = returnFalse;
            sMouseButtonUp[0] = returnFalse;
            sMouseButtonPressed[0] = returnFalse;

            sMouseButtonDown[1] = returnFalse;
            sMouseButtonUp[1] = returnFalse;
            sMouseButtonPressed[1] = returnFalse;

            sMouseButtonDown[2] = returnFalse;
            sMouseButtonUp[2] = returnFalse;
            sMouseButtonPressed[2] = returnFalse;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void unlockAllMouseButton()
        {
            sMouseButtonDown[0] = Input.GetMouseButtonDown;
            sMouseButtonUp[0] = Input.GetMouseButtonUp;
            sMouseButtonPressed[0] = Input.GetMouseButton;

            sMouseButtonDown[1] = Input.GetMouseButtonDown;
            sMouseButtonUp[1] = Input.GetMouseButtonUp;
            sMouseButtonPressed[1] = Input.GetMouseButton;

            sMouseButtonDown[2] = Input.GetMouseButtonDown;
            sMouseButtonUp[2] = Input.GetMouseButtonUp;
            sMouseButtonPressed[2] = Input.GetMouseButton;
        }

        private static bool returnFalse(int button)
        {
            return false;
        }
    }
}
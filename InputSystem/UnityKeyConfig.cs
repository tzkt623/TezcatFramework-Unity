using System;
using System.Collections;
using UnityEngine;

namespace tezcat.Framework.InputSystem
{
    public abstract class UnityKeyWrapper : TezKeyWrapper
    {
        static readonly Array s_Keys = Enum.GetValues(typeof(KeyCode));

        public override string name => keyCode.ToString();

        public KeyCode keyCode;

        public override void close()
        {

        }

        protected override bool onChangeKeyDown()
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode item in s_Keys)
                {
                    if (Input.GetKeyDown(item))
                    {
                        keyCode = item;
                        return true;
                    }
                }
            }

            return false;
        }
    }

    public class UnityKeyDownWrapper : UnityKeyWrapper
    {
        public override bool active()
        {
            return Input.GetKeyDown(keyCode);
        }
    }

    public class UnityKeyUpWrapper : UnityKeyWrapper
    {
        public override bool active()
        {
            return Input.GetKeyUp(keyCode);
        }
    }

    public class UnityKeyPressWrapper : UnityKeyWrapper
    {
        public override bool active()
        {
            return Input.GetKey(keyCode);
        }
    }

    /// <summary>
    /// 通用型按键配置
    /// 可以单个
    /// 可以组合
    /// 组合按键必须是press+press+...+down的配置模式
    /// 并且存在最后一个按下的按键必须为down按键的触发方式
    /// </summary>
    public abstract class UnityKeyConfig : TezKeyConfig
    {
        public UnityKeyConfig(string name, int keyCount = 1) : base(name, keyCount)
        {

        }
    }

    /// <summary>
    /// 改进型组合按键配置
    /// 解决了press+press+...+down型最后一个按键顺序的问题
    /// 可以随意按键
    /// 但是会延迟一帧触发下一次按键效果
    /// </summary>
    public abstract class UnityCombineKeyConfig : TezKeyConfig
    {
        bool m_LockKey = false;
        protected UnityCombineKeyConfig(string name, int keyCount) : base(name, keyCount)
        {

        }

        public sealed override bool active()
        {
            if (m_LockKey)
            {
                ///如果锁住了,检测是否放开了
                m_LockKey = this.onActive();
            }
            else
            {
                ///如果没有锁住,检测是否激活
                m_LockKey = this.onActive();
                return m_LockKey;
            }

            return false;
        }

        protected abstract bool onActive();
    }
}
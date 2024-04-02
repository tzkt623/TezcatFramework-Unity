using System;
using System.Collections;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.TypeTraits;

namespace tezcat.Framework.ECS
{
    public class TezEntity : ITezCloseable
    {
        #region Pool
        static int s_IDGiver = 0;
        static Queue<TezEntity> s_FreeEntity = new Queue<TezEntity>();

        public static TezEntity create()
        {
            if (s_FreeEntity.Count > 0)
            {
                return s_FreeEntity.Dequeue();
            }

            return new TezEntity()
            {
                ID = s_IDGiver++
            };
        }
        #endregion

        /// <summary>
        /// 签名
        /// </summary>
        //         public BitArray signature => m_Signature;
        //         BitArray m_Signature = new BitArray(TezComponentManager.count);

        ITezComponent[] m_Components = new ITezComponent[TezComponentManager.count];
        public int ID { get; private set; } = -1;

        private TezEntity()
        {

        }

        /// <summary>
        /// 根据BasicComponent类型获得抽象组件实例
        /// </summary>
        /// <typeparam name="BaseComponent">组件检索类型</typeparam>
        /// <returns>抽象组件</returns>
        public BaseComponent getComponent<BaseComponent>()
            where BaseComponent : ITezComponent
        {
            if (TezComponentID<BaseComponent>.ID == TezTypeInfo.ErrorID)
            {
                throw new ArgumentException(string.Format("This type [{0}] is not a Component", typeof(BaseComponent).Name));
            }

            if (TezComponentID<BaseComponent>.ID >= m_Components.Length)
            {
                throw new ArgumentOutOfRangeException(string.Format("This type [{0}]`s ID[{1}] is out of range", typeof(BaseComponent).Name, TezComponentID<BaseComponent>.ID));
            }

            return (BaseComponent)m_Components[TezComponentID<BaseComponent>.ID];
        }

        /// <summary>
        /// 根据BasicComponent类型获得抽象组件实例之后转换为Component实际组件
        /// </summary>
        /// <typeparam name="BaseComponent">组件检索类型</typeparam>
        /// <typeparam name="Component">组件实际类型</typeparam>
        /// <returns>实际组件</returns>
        public Component getComponent<BaseComponent, Component>()
            where BaseComponent : ITezComponent
            where Component : BaseComponent
        {
            if (TezComponentID<BaseComponent>.ID == TezTypeInfo.ErrorID)
            {
                throw new ArgumentException(string.Format("This type [{0}] is not a Component ({1})", typeof(Component).Name, typeof(BaseComponent).Name));
            }

            if (TezComponentID<BaseComponent>.ID >= m_Components.Length)
            {
                throw new ArgumentOutOfRangeException(string.Format("This type [{0}]`s ID[{1}] is out of range", typeof(BaseComponent).Name, TezComponentID<BaseComponent>.ID));
            }

            return (Component)m_Components[TezComponentID<BaseComponent>.ID];
        }

        /// <summary>
        /// 根据BasicComponent类型获得组件ID后添加实际组件
        /// </summary>
        /// <typeparam name="BasicComponent">组件检索类型</typeparam>
        /// <param name="component">实际组件对象</param>
        public void addComponent(ITezComponent component)
        {
            var id = component.comUID;
            foreach (var item in m_Components)
            {
                item?.onOtherComponentAdded(component, id);
            }

            m_Components[id] = component;
            component.onAdd(this);
        }

        public Com addComponent<Com>() where Com : ITezComponent, new()
        {
            Com component = new Com();
            this.addComponent(component);
            return component;
        }

        /// <summary>
        /// 替换Component
        /// </summary>
        /// <param name="component"></param>
        public void replaceComponent(ITezComponent component, bool close_old = true)
        {
            var id = component.comUID;

            var old = m_Components[id];
            if (old != null)
            {
                m_Components[id] = null;
                old.onRemove(this);
                foreach (var item in m_Components)
                {
                    item?.onOtherComponentRemoved(old, id);
                }

                if (close_old)
                {
                    old.close();
                }
            }

            foreach (var item in m_Components)
            {
                item?.onOtherComponentAdded(component, id);
            }
            m_Components[id] = component;
            component.onAdd(this);
        }

        /// <summary>
        /// 根据BasicComponent类型检索并移除组件
        /// </summary>
        /// <typeparam name="BaseComponent"></typeparam>
        public void removeComponent<BaseComponent>(bool close_old = true)
            where BaseComponent : ITezComponent
        {
            var id = TezComponentID<BaseComponent>.ID;
            if (id == TezTypeInfo.ErrorID)
            {
                throw new ArgumentException(string.Format("This type [{0}] is not a Component IDGetter, Please Use its BasicClass"
                    , typeof(BaseComponent).Name));
            }

            var temp = m_Components[id];
            m_Components[id] = null;
            temp.onRemove(this);

            foreach (var item in m_Components)
            {
                item?.onOtherComponentRemoved(temp, id);
            }

            if (close_old)
            {
                temp.close();
            }
        }

        public void removeComponent(ITezComponent component)
        {
            var id = component.comUID;
            var temp = m_Components[id];
            m_Components[id] = null;
            temp.onRemove(this);

            foreach (var item in m_Components)
            {
                item?.onOtherComponentRemoved(temp, id);
            }
        }

        void ITezCloseable.deleteThis()
        {
            for (int i = 0; i < m_Components.Length; i++)
            {
                ///TODO:删除组件
                var com = m_Components[i];
                if (com != null)
                {
                    m_Components[i] = null;
                    com.onRemove(this);
                    com.close();
                }
            }

            s_FreeEntity.Enqueue(this);
        }

        #region 重载操作
        //         public static bool operator true(TezEntity entity)
        //         {
        //             return !object.ReferenceEquals(entity, null);
        //         }
        // 
        //         public static bool operator false(TezEntity entity)
        //         {
        //             return object.ReferenceEquals(entity, null);
        //         }
        // 
        //         public static bool operator !(TezEntity entity)
        //         {
        //             return object.ReferenceEquals(entity, null);
        //         }
        #endregion
    }
}
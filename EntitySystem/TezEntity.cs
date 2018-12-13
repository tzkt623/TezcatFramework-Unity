﻿using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.TypeTraits;
using UnityEngine;

namespace tezcat.Framework.ECS
{
    public interface ITezEntityManager : ITezService
    {
        TezEntity create();
        void recycle(TezEntity entity);
    }

    public class TezEntityManager : ITezEntityManager
    {
        List<TezEntity> m_FreeEntity = new List<TezEntity>();

        public TezEntity create()
        {
            if (m_FreeEntity.Count > 0)
            {
                var index = m_FreeEntity.Count - 1;
                var temp = m_FreeEntity[index];
                m_FreeEntity.RemoveAt(index);
                return temp;
            }

            return null;
        }

        public void recycle(TezEntity entity)
        {
            Debug.Log(string.Format("Entity Recycle {0}", entity.ID));
            m_FreeEntity.Add(entity);
        }

        public void close()
        {
            m_FreeEntity.Clear();
            m_FreeEntity = null;
        }
    }


    public class TezEntity : ITezCloseable
    {
        ITezComponent[] m_Components = new ITezComponent[TezService.get<TezComponentManager>().componentCount];

        public int ID { get; private set; } = -1;
        static int m_IDGiver = 0;

        public static TezEntity create()
        {
            TezEntity entity = TezService.get<ITezEntityManager>().create();
            if (entity == null)
            {
                entity = new TezEntity()
                {
                    ID = m_IDGiver++
                };
            }

            return entity;
        }

        private TezEntity()
        {
        }

        /// <summary>
        /// 根据BasicComponent类型获得抽象组件实例
        /// </summary>
        /// <typeparam name="BasicComponent">组件检索类型</typeparam>
        /// <returns>抽象组件</returns>
        public BasicComponent getComponent<BasicComponent>()
            where BasicComponent : ITezComponent
        {
            if (TezComponentID<BasicComponent>.ID == TezTypeInfo.ErrorID)
            {
                throw new ArgumentException(string.Format("This type [{0}] is not a Component", typeof(BasicComponent).Name));
            }

            return (BasicComponent)m_Components[TezComponentID<BasicComponent>.ID];
        }

        /// <summary>
        /// 根据BasicComponent类型获得抽象组件实例之后转换为Component实际组件
        /// </summary>
        /// <typeparam name="BasicComponent">组件检索类型</typeparam>
        /// <typeparam name="Component">组件实际类型</typeparam>
        /// <returns>实际组件</returns>
        public Component getComponent<BasicComponent, Component>()
            where BasicComponent : ITezComponent
            where Component : BasicComponent
        {
            if (TezComponentID<BasicComponent>.ID == TezTypeInfo.ErrorID)
            {
                throw new ArgumentException(string.Format("This type [{0}] is not a Component", typeof(Component).Name));
            }

            if(TezComponentID<BasicComponent>.ID >= m_Components.Length)
            {
                throw new ArgumentOutOfRangeException(string.Format("This type [{0}]`s ID[{1}] is out of range"
                    , typeof(BasicComponent).Name
                    , TezComponentID<BasicComponent>.ID));
            }

            return (Component)m_Components[TezComponentID<BasicComponent>.ID];
        }

        /// <summary>
        /// 根据BasicComponent类型获得组件ID后添加实际组件
        /// </summary>
        /// <typeparam name="BasicComponent">组件检索类型</typeparam>
        /// <param name="component">实际组件对象</param>
        public void addComponent<BasicComponent>(ITezComponent component)
            where BasicComponent : ITezComponent
        {
            var id = TezComponentID<BasicComponent>.ID;
            if (id == TezTypeInfo.ErrorID)
            {
                throw new ArgumentException(string.Format("This type [{0}] is not a Component IDGetter, Please Use its BasicClass"
                    , component.GetType().Name));
            }
            m_Components[id] = component;
            component.onAdd(this);
        }

        /// <summary>
        /// 根据BasicComponent类型检索并移除组件
        /// </summary>
        /// <typeparam name="BasicComponent"></typeparam>
        public void removeComponent<BasicComponent>()
            where BasicComponent : ITezComponent
        {
            var id = TezComponentID<BasicComponent>.ID;
            if (id == TezTypeInfo.ErrorID)
            {
                throw new ArgumentException(string.Format("This type [{0}] is not a Component IDGetter, Please Use its BasicClass"
                    , typeof(BasicComponent).Name));
            }
            var temp = m_Components[id];
            m_Components[id] = null;
            temp.onRemove(this);
        }

        public void close()
        {
            for (int i = 0; i < m_Components.Length; i++)
            {
                ///TODO:删除组件
                m_Components[i]?.onRemove(this);
                m_Components[i] = null;
            }

            TezService.get<ITezEntityManager>().recycle(this);
        }
    }
}
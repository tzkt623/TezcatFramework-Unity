using System;
using System.Collections.Generic;

namespace tezcat.Framework.Core
{
    public class TezECS
    {
        internal interface IComponentManager
        {
            bool destoryComponent(int entityID);
        }

        public abstract class Component
        {
            public int entityID => mEntityID;

            internal int mEntityID;
            internal int mComponentIndex;
            internal IComponentManager mManager;

            public virtual void destory()
            {
                mManager = null;
            }

            public abstract void onOtherComponentAdded(Component other);
            public abstract void onOtherComponentRemoved(Component other);
        }

        public sealed class Entity
        {
            public int id => mID;

            internal int mID;
            internal bool mIsRecycled = false;
            internal Entity(int id)
            {
                mID = id;
            }

            HashSet<Component> mComponents = new HashSet<Component>();

            internal void registerComponent(Component component)
            {
                if (mComponents.Add(component))
                {
                    foreach (var item in mComponents)
                    {
                        item.onOtherComponentAdded(component);
                    }
                }
            }

            internal void unregisterComponent(Component component)
            {
                if (mComponents.Remove(component))
                {
                    foreach (var item in mComponents)
                    {
                        item.onOtherComponentRemoved(component);
                    }
                }
            }

            internal void clear()
            {
                foreach (var item in mComponents)
                {
                    item.mManager.destoryComponent(this.mID);
                }

                mComponents.Clear();
                mComponents.TrimExcess();
                if (mComponents.Count != 0)
                {
                    throw new Exception("ECS Entity Clear Error");
                }
            }

            /// <summary>
            /// 摧毁方式不需要清理component
            /// 因为已经在外部调用ComponentManager.destoryAll()摧毁所有组件
            /// </summary>
            internal void destory()
            {
                mComponents.Clear();
                mComponents = null;
            }
        }

        public abstract class System
        {
            internal class Executor
            {
                public System system;
                public Action executor;
                public int priority = 0;
                public int innerID = 0;

                internal void destory()
                {
                    this.system = null;
                    this.executor = null;
                }
            }

            public void addUpdater(Action executor, int priority = 0, int innerID = 0)
            {
                TezECS.registerSystemExecutor(this, executor, priority, innerID);
            }
        }

        public abstract class ComponentManager
        {
            static List<ComponentManager> mAllManagers = new List<ComponentManager>();

            protected int mIndex = -1;

            protected ComponentManager()
            {
                mIndex = mAllManagers.Count;
                mAllManagers.Add(this);
            }

            public static void destoryAll()
            {
                foreach (var manager in mAllManagers)
                {
                    manager.destoryAllComponents();
                    //manager.destoryInstance();
                }
                //mAllManagers.Clear();
                //mAllManagers.TrimExcess();
            }

            public void destory()
            {
                mAllManagers[mIndex] = null;
                this.destoryAllComponents();
            }

            protected abstract void destoryInstance();

            protected abstract void destoryAllComponents();
        }

        public class ComponentManager<Com>
            : ComponentManager
            , IComponentManager
            where Com : Component, new()
        {
            static ComponentManager<Com> mInstance = new ComponentManager<Com>();
            public static ComponentManager<Com> instance => mInstance;

            //存实体ID的稀疏数组
            List<int> mSparseEntityIDs = new List<int>();
            //List<int> mDense = new List<int>();
            //存组件的密集数组
            List<Com> mDenseComponents = new List<Com>();

            public IReadOnlyCollection<Com> components => mDenseComponents;

            public Com add(Entity entity)
            {
                while (entity.mID >= mSparseEntityIDs.Count)
                {
                    mSparseEntityIDs.Add(-1);
                }

                Com com = new Com()
                {
                    mComponentIndex = mDenseComponents.Count,
                    mEntityID = entity.mID,
                    mManager = this
                };

                mSparseEntityIDs[entity.mID] = com.mComponentIndex;
                mDenseComponents.Add(com);

                entity.registerComponent(com);

                return com;
            }

            public Com get(Entity entity)
            {
                return mDenseComponents[mSparseEntityIDs[entity.mID]];
            }

            public bool tryGet(Entity entity, out Com component)
            {
                if (this.has(entity))
                {
                    component = mDenseComponents[mSparseEntityIDs[entity.mID]];
                    return true;
                }

                component = null;
                return false;
            }

            public bool has(Entity entity)
            {
                return (entity.mID < mSparseEntityIDs.Count) && (mSparseEntityIDs[entity.mID] > -1);
            }

            private void removeComponent(int id, bool isDestory)
            {
                var removing_index = mSparseEntityIDs[id];
                //重设稀疏数组中被删除组件实体的组件索引
                mSparseEntityIDs[id] = -1;
                //需要删除的组件
                var removing_com = mDenseComponents[removing_index];
                //最后一个组件的索引
                var last_com_index = mDenseComponents.Count - 1;

                if (removing_index != last_com_index)
                {
                    var last_com = mDenseComponents[last_com_index];
                    //交换最后一个组件到被删除组件的位置
                    mDenseComponents[removing_index] = last_com;
                    //重设最后一个组件的位置ID
                    last_com.mComponentIndex = removing_index;
                    //重设稀疏数组中索引的位置
                    mSparseEntityIDs[last_com.mEntityID] = removing_index;
                }
                //删除最后一个
                mDenseComponents.RemoveAt(last_com_index);

                //如果是摧毁实体，则不需要通知本实体的其他组件
                //因为本实体的组件都要被摧毁
                if (!isDestory)
                {
                    mEntityList[id].unregisterComponent(removing_com);
                }
                removing_com.destory();
            }

            public bool remove(Entity entity)
            {
                if (entity.mID >= mSparseEntityIDs.Count || mSparseEntityIDs[entity.mID] == -1)
                {
                    return false;
                }

                this.removeComponent(entity.mID, false);
                return true;
            }

            bool IComponentManager.destoryComponent(int entityID)
            {
                this.removeComponent(entityID, true);
                return true;
            }

            protected override void destoryAllComponents()
            {
                mSparseEntityIDs.Clear();
                foreach (var com in mDenseComponents)
                {
                    com.destory();
                }
                mDenseComponents.Clear();
                mDenseComponents.TrimExcess();
            }

            protected override void destoryInstance()
            {
                mInstance = null;
            }
        }

        static List<Entity> mEntityList = new List<Entity>();
        static Queue<int> mFreeIndices = new Queue<int>();
        static List<System.Executor> mSystemList = new List<System.Executor>();
        static bool mSystemExecutorDirty = false;

        public static int entityCount => mEntityList.Count;
        public static int freeCount => mFreeIndices.Count;

        public static Entity createEntity()
        {
            if (mFreeIndices.Count == 0)
            {
                Entity entity = new Entity(mEntityList.Count);
                mEntityList.Add(entity);
                return entity;
            }
            else
            {
                Console.WriteLine($"!!!Reusing entity id: {mFreeIndices.Peek()}!!!");
                var entity = mEntityList[mFreeIndices.Dequeue()];
                if (!entity.mIsRecycled)
                {
                    throw new Exception($"Entity {entity.mID} is not recycled");
                }
                entity.mIsRecycled = false;
                return entity;
            }
        }

        public static Entity getEntity(int id)
        {
            return mEntityList[id];
        }

        public static void recycleEntity(Entity entity)
        {
            recycleEntity(entity.mID);
        }

        public static void recycleEntity(int id)
        {
            if (mEntityList[id].mIsRecycled)
            {
                throw new Exception($"Entity {id} has been recycled");
            }

            mEntityList[id].mIsRecycled = true;
            mFreeIndices.Enqueue(id);
            mEntityList[id].clear();
        }

        public static Com addComponent<Com>(Entity entity) where Com : Component, new()
        {
            return ComponentManager<Com>.instance.add(entity);
        }

        public static Com getComponent<Com>(Entity entity) where Com : Component, new()
        {
            return ComponentManager<Com>.instance.get(entity);
        }

        public static bool tryGetComponent<Com>(Entity entity, out Com component) where Com : Component, new()
        {
            return ComponentManager<Com>.instance.tryGet(entity, out component);
        }

        public static bool hasComponent<Com>(Entity entity) where Com : Component, new()
        {
            return ComponentManager<Com>.instance.has(entity);
        }

        public static bool removeComponent<Com>(Entity entity) where Com : Component, new()
        {
            return ComponentManager<Com>.instance.remove(entity);
        }

        public static ComponentManager<Com> getComponentManager<Com>() where Com : Component, new()
        {
            return ComponentManager<Com>.instance;
        }

        public static void registerSystemExecutor(System system, Action executor, int priority = 0, int innerID = 0)
        {
            mSystemList.Add(new System.Executor()
            {
                system = system,
                executor = executor,
                priority = priority,
                innerID = innerID
            });
            mSystemExecutorDirty = true;
        }

        public static void destoryAllData()
        {
            ComponentManager.destoryAll();

            foreach (var entity in mEntityList)
            {
                entity.destory();
            }

            foreach (var item in mSystemList)
            {
                item.destory();
            }

            mEntityList.Clear();
            mFreeIndices.Clear();
            mSystemList.Clear();

            mEntityList.TrimExcess();
            mFreeIndices.TrimExcess();
            mSystemList.TrimExcess();
        }

        public static void updateSystem()
        {
            if (mSystemExecutorDirty)
            {
                mSystemList.Sort((a, b) => a.priority.CompareTo(b.priority));
                mSystemExecutorDirty = false;
            }

            foreach (var item in mSystemList)
            {
                item.executor();
            }
        }
    }
}

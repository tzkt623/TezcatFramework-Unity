using System;
using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.ArchetypeECS
{
    /// <summary>
    /// Init Sequence: register ArchetypeMaskID -> buildingWorld
    /// </summary>
    public class TezWorld
    {
        private const int COMPONENT_MARK_64 = 64;
        private const int COMPONENT_MARK_127 = 127;

        public abstract class BaseComponentID
        {
            protected static int UID = 0;
        }

        public sealed class ComponentID<T>
            : BaseComponentID
            where T : IComponent
        {
            public static readonly int ID;

            static ComponentID()
            {
                ID = UID++;
            }
        }

        public interface IComponent : ITezCloseable
        {
            int componentID { get; }
            int entityID { get; set; }
        }

        #region ArchetpyeMaskID
        class ArchetypeMaskCreator
        {
            protected static List<ArchetypeMaskID> mMaskIDs = new List<ArchetypeMaskID>();
            public static IReadOnlyList<ArchetypeMaskID> allMask => mMaskIDs;
        }

        class ArchetypeMaskCreator<T>
            : ArchetypeMaskCreator
            where T : ArchetypeMaskID, new()
        {
            public static readonly T value = new T();

            static ArchetypeMaskCreator()
            {
                mMaskIDs.Add(value);
            }
        }

        public abstract class ArchetypeMaskID
        {
            protected static int baseID = 0;
            public static int count => baseID;
            public abstract int getID();
            public int[] componentIDs { get; }

            protected ArchetypeMaskID(params int[] componentIDs)
            {
                this.componentIDs = componentIDs;
            }
        }

        public class ArchetypeMaskID<T1>
            : ArchetypeMaskID
            where T1 : IComponent
        {
            public static readonly int ID;

            static ArchetypeMaskID()
            {
                ID = baseID++;
            }

            public ArchetypeMaskID() : base(ComponentID<T1>.ID)
            {

            }

            public override int getID()
            {
                return ID;
            }
        }

        public class ArchetypeMaskID<T1, T2>
            : ArchetypeMaskID
            where T1 : IComponent
            where T2 : IComponent
        {
            public static readonly int ID;
            static ArchetypeMaskID()
            {
                ID = baseID++;
            }

            public ArchetypeMaskID() : base(ComponentID<T1>.ID, ComponentID<T2>.ID)
            {

            }

            public override int getID()
            {
                return ID;
            }
        }

        public class ArchetypeMaskID<T1, T2, T3>
            : ArchetypeMaskID
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
        {
            public static readonly int ID;
            static ArchetypeMaskID()
            {
                ID = baseID++;
            }

            public ArchetypeMaskID() : base(ComponentID<T1>.ID, ComponentID<T2>.ID, ComponentID<T3>.ID)
            {

            }

            public override int getID()
            {
                return ID;
            }
        }

        public class ArchetypeMaskID<T1, T2, T3, T4>
            : ArchetypeMaskID
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
        {
            public static readonly int ID;
            static ArchetypeMaskID()
            {
                ID = baseID++;
            }

            public ArchetypeMaskID() : base
                (ComponentID<T1>.ID
                , ComponentID<T2>.ID
                , ComponentID<T3>.ID
                , ComponentID<T4>.ID)
            {

            }

            public override int getID()
            {
                return ID;
            }
        }

        public class ArchetypeMaskID<T1, T2, T3, T4, T5>
            : ArchetypeMaskID
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
        {
            public static readonly int ID;
            static ArchetypeMaskID()
            {
                ID = baseID++;
            }

            public ArchetypeMaskID() : base
                (ComponentID<T1>.ID
                , ComponentID<T2>.ID
                , ComponentID<T3>.ID
                , ComponentID<T4>.ID
                , ComponentID<T5>.ID)
            {

            }

            public override int getID()
            {
                return ID;
            }
        }

        public class ArchetypeMaskID<T1, T2, T3, T4, T5, T6>
            : ArchetypeMaskID
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
        {
            public static readonly int ID;
            static ArchetypeMaskID()
            {
                ID = baseID++;
            }

            public ArchetypeMaskID() : base
                (ComponentID<T1>.ID
                , ComponentID<T2>.ID
                , ComponentID<T3>.ID
                , ComponentID<T4>.ID
                , ComponentID<T5>.ID
                , ComponentID<T6>.ID)
            {

            }

            public override int getID()
            {
                return ID;
            }
        }

        public class ArchetypeMaskID<T1, T2, T3, T4, T5, T6, T7>
            : ArchetypeMaskID
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
        {
            public static readonly int ID;
            static ArchetypeMaskID()
            {
                ID = baseID++;
            }

            public ArchetypeMaskID() : base
                (ComponentID<T1>.ID
                , ComponentID<T2>.ID
                , ComponentID<T3>.ID
                , ComponentID<T4>.ID
                , ComponentID<T5>.ID
                , ComponentID<T6>.ID
                , ComponentID<T7>.ID)
            {

            }

            public override int getID()
            {
                return ID;
            }
        }

        public class ArchetypeMaskID<T1, T2, T3, T4, T5, T6, T7, T8>
            : ArchetypeMaskID
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
        {
            public static readonly int ID;
            static ArchetypeMaskID()
            {
                ID = baseID++;
            }

            public ArchetypeMaskID() : base
                (ComponentID<T1>.ID
                , ComponentID<T2>.ID
                , ComponentID<T3>.ID
                , ComponentID<T4>.ID
                , ComponentID<T5>.ID
                , ComponentID<T6>.ID
                , ComponentID<T7>.ID
                , ComponentID<T8>.ID)
            {

            }

            public override int getID()
            {
                return ID;
            }
        }

        public class ArchetypeMaskID<T1, T2, T3, T4, T5, T6, T7, T8, T9>
            : ArchetypeMaskID
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
            where T9 : IComponent
        {
            public static readonly int ID;
            static ArchetypeMaskID()
            {
                ID = baseID++;
            }

            public ArchetypeMaskID() : base
                (ComponentID<T1>.ID
                , ComponentID<T2>.ID
                , ComponentID<T3>.ID
                , ComponentID<T4>.ID
                , ComponentID<T5>.ID
                , ComponentID<T6>.ID
                , ComponentID<T7>.ID
                , ComponentID<T8>.ID
                , ComponentID<T9>.ID)
            {

            }

            public override int getID()
            {
                return ID;
            }
        }

        public class ArchetypeMaskID<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
            : ArchetypeMaskID
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
            where T9 : IComponent
            where T10 : IComponent
        {
            public static readonly int ID;
            static ArchetypeMaskID()
            {
                ID = baseID++;
            }

            public ArchetypeMaskID() : base
                (ComponentID<T1>.ID
                , ComponentID<T2>.ID
                , ComponentID<T3>.ID
                , ComponentID<T4>.ID
                , ComponentID<T5>.ID
                , ComponentID<T6>.ID
                , ComponentID<T7>.ID
                , ComponentID<T8>.ID
                , ComponentID<T9>.ID
                , ComponentID<T10>.ID)
            {

            }

            public override int getID()
            {
                return ID;
            }
        }

        #endregion
        public struct ArchetypeKey
        {
            private ulong mask1;
            private ulong mask2;

            public ArchetypeKey(ulong m1, ulong m2)
            {
                mask1 = m1;
                mask2 = m2;
            }

            public ArchetypeKey(params int[] types)
            {
                mask1 = 0;
                mask2 = 0;
                foreach (var type in types)
                {
                    this.register(type);
                }
            }

            internal void register(int index)
            {
                if (index > COMPONENT_MARK_127)
                {
                    throw new ArgumentException("Out Of Component MaxCount");
                }

                if (index < COMPONENT_MARK_64)
                {
                    mask1 |= (1u << index);
                }
                else
                {
                    mask2 |= (1u << (index - COMPONENT_MARK_64));
                }
            }

            public bool equalOf(ArchetypeKey other)
            {
                return mask1 == other.mask1 && mask2 == other.mask2;
            }

            public bool anyOf(ArchetypeKey other)
            {
                return ((mask1 & other.mask1) == other.mask1) && ((mask2 & other.mask2) == other.mask2);
            }

            public bool anyOf(int maskIndex)
            {
                if (maskIndex > COMPONENT_MARK_127)
                {
                    throw new ArgumentException("Out Of Component MaxCount");
                }

                if (maskIndex < COMPONENT_MARK_64)
                {
                    return (mask1 & (1u << maskIndex)) != 0;
                }

                return (mask2 & (1u << (maskIndex - COMPONENT_MARK_64))) != 0;
            }

            public bool noneOf(ArchetypeKey other)
            {
                return ((mask1 & other.mask1) == 0) && ((mask2 & other.mask2) == 0);
            }

            public override int GetHashCode()
            {
                //return (mask1, mask2).GetHashCode();

                unchecked // 允许溢出，这是哈希计算中的常规操作
                {
                    int hash = 17;
                    hash = hash * 31 + (int)(mask1 ^ (mask1 >> 32));
                    hash = hash * 31 + (int)(mask2 ^ (mask2 >> 32));
                    return hash;
                }
            }

            public override bool Equals(object obj)
            {
                if (obj is ArchetypeKey other)
                {
                    return this.equalOf(other);
                }

                return false;
            }
        }

        public struct QueryList
        {
            List<IReadOnlyList<IComponent>> components;

            public QueryList(List<IReadOnlyList<IComponent>> components)
            {
                this.components = components;
            }

            public void forEach(Action<IReadOnlyList<IComponent>> action)
            {
                foreach (var item in components)
                {
                    action(item);
                }
            }
        }

        public interface IChunk
        {
            int count { get; }
            IReadOnlyList<IComponent> getComponents(int typeID);
        }

        public class Chunk : IChunk
        {
            List<IComponent>[] mComponents = new List<IComponent>[COMPONENT_MARK_64];
            int mCount = 0;
            public int count => mCount;


            public IReadOnlyList<IComponent> getComponents(int typeID)
            {
                return mComponents[typeID];
            }

            public Chunk(int[] typeIDs)
            {
                foreach (var item in typeIDs)
                {
                    mComponents[item] = new List<IComponent>();
                }
            }

            internal int onAddEntity(int[] typeIDs)
            {
                foreach (var item in typeIDs)
                {
                    mComponents[item].Add(null);
                }

                return mCount++;
            }

            internal void addComponent(IComponent com)
            {
                mComponents[com.componentID][mCount - 1] = com;
            }

            //             internal void romoveComponent(int chunkSlot, int componentIndex)
            //             {
            //                 var list = mComponents[componentIndex];
            //                 list[chunkSlot].close();
            //                 list[chunkSlot] = null;
            //             }

            internal void removeEntity(Chunk chunk, int chunkSlot, int[] masks)
            {
                //需要把最后一个chunk的entity与当前的对调
                var last_index = mCount - 1;
                if (chunkSlot == last_index)
                {
                    foreach (var item in masks)
                    {
                        var list = mComponents[item];
                        list[chunkSlot].close();
                        list.RemoveAt(chunkSlot);
                    }
                }
                else
                {
                    var last_com_entity = mComponents[masks[0]][last_index].entityID;
                    var last_data = mEntityManager.getData(last_com_entity);

                    List<IComponent> list = null;
                    foreach (var item in masks)
                    {
                        list = mComponents[item];
                        list[chunkSlot].close();
                        list[chunkSlot] = list[last_index];
                        list.RemoveAt(last_index);
                    }

                    last_data.chunkSlot = chunkSlot;
                }

                mCount--;
            }

            internal IComponent getComponent(int chunkSlot, int typeID)
            {
                if (mComponents[typeID] == null)
                {
                    throw new InvalidOperationException("Use Parent to get this Component");
                }

                return mComponents[typeID][chunkSlot];
            }
        }

        public class Archetype
        {
            int mUID = 0;
            ArchetypeKey mKey;
            public ArchetypeKey key => mKey;
            int[] mTypeIDs = null;
            List<Chunk> mChunks = new List<Chunk>();
            public IReadOnlyList<Chunk> chunks => mChunks;
            int mCurrentChunk = -1;

            internal int[] typeMask => mTypeIDs;
            bool mInit = false;

            internal void init(int UID, params int[] typeIDs)
            {
                if (mInit)
                {
                    return;
                }

                mInit = true;
                mTypeIDs = typeIDs;
                mUID = UID;
                mKey = new ArchetypeKey(mTypeIDs);
            }

            internal void onAddEntity(EntityData data)
            {
                data.archetype = this;
                if (mCurrentChunk < 0)
                {
                    var chunk = new Chunk(mTypeIDs);
                    data.chunk = chunk;
                    data.chunkSlot = chunk.onAddEntity(mTypeIDs);
                    mCurrentChunk = mChunks.Count;
                    mChunks.Add(chunk);
                    return;
                }

                if (mChunks[mCurrentChunk].count >= 1000)
                {
                    var new_chunk = new Chunk(mTypeIDs);
                    data.chunk = new_chunk;
                    data.chunkSlot = new_chunk.onAddEntity(mTypeIDs);
                    mCurrentChunk = mChunks.Count;
                    mChunks.Add(new_chunk);
                }
                else
                {
                    data.chunk = mChunks[mCurrentChunk];
                    data.chunkSlot = mChunks[mCurrentChunk].onAddEntity(mTypeIDs);
                }
            }

            internal void onRemoveEntity(Chunk chunk, int chunkSlot)
            {
                chunk.removeEntity(chunk, chunkSlot, mTypeIDs);
            }

            internal bool hasComponent(int componentID)
            {
                return mKey.anyOf(componentID);
            }
        }

        public struct Entity
        {
            public int ID { get; }

            public Entity(int id)
            {
                this.ID = id;
            }

            public bool Equals(Entity other)
            {
                return ID == other.ID;
            }

            public override bool Equals(object obj)
            {
                return obj is Entity other && this.Equals(other);
            }

            public override int GetHashCode()
            {
                return ID.GetHashCode();
            }

            public static bool operator ==(Entity a, Entity b)
            {
                return a.ID == b.ID;
            }

            public static bool operator !=(Entity a, Entity b)
            {
                return a.ID != b.ID;
            }

            public bool IsNull => ID < 0;

            public override string ToString()
            {
                return $"Entity {ID}";
            }
        }

        internal class EntityData
        {
            internal ArchetypeKey archetypeMask;
            internal Archetype archetype;
            internal Chunk chunk;
            internal int chunkSlot;

            public void remove()
            {
                archetype.onRemoveEntity(chunk, chunkSlot);
                archetype = null;
                chunk = null;
                chunkSlot = -1;
            }
        }

        class EntityManager
        {
            Queue<int> mFree = new Queue<int>();
            List<EntityData> mData = new List<EntityData>();

            public (Entity, EntityData) createEntity()
            {
                if (mFree.Count > 0)
                {
                    var id = mFree.Dequeue();
                    return (new Entity(id), mData[id]);
                }

                var e = new Entity(mData.Count);
                var data = new EntityData();
                mData.Add(data);
                return (e, data);
            }

            public void removeEntity(Entity entity)
            {
                mFree.Enqueue(entity.ID);
            }

            public EntityData getData(int id)
            {
                return mData[id];
            }
        }

        static EntityManager mEntityManager = new EntityManager();
        static Archetype[] mArchetypeArray = null;
        static bool mIsBuildCompleted = false;

        public static Entity createEntity()
        {
            (Entity entity, EntityData data) = mEntityManager.createEntity();
            return entity;
        }

        public static Entity createEntity<T>() where T : ArchetypeMaskID, new()
        {
            var id = ArchetypeMaskCreator<T>.value.getID();
            (Entity entity, EntityData data) = mEntityManager.createEntity();
            mArchetypeArray[id].onAddEntity(data);
            return entity;
        }

        public static void registerEntity<T>(Entity entity) where T : ArchetypeMaskID, new ()
        {
            var id = ArchetypeMaskCreator<T>.value.getID();
            var data = mEntityManager.getData(entity.ID);
            mArchetypeArray[id].onAddEntity(data);
        }

        public static void removeEntity(Entity entity)
        {
            var data = mEntityManager.getData(entity.ID);
            data.remove();
//             data.archetype.onRemoveEntity(data.chunk, data.chunkSlot);
//             data.archetype = null;
//             data.chunk = null;
//             data.chunkSlot = -1;
            mEntityManager.removeEntity(entity);
        }

        public static T initComponent<T>(Entity entity) where T : IComponent, new()
        {
            T com = new T()
            {
                entityID = entity.ID
            };
            var data = mEntityManager.getData(entity.ID);
            data.chunk.addComponent(com);
            return com;
        }

        public static U initComponent<T, U>(Entity entity)
            where T : IComponent
            where U : T, new()
        {
            U com = new U()
            {
                entityID = entity.ID
            };
            var data = mEntityManager.getData(entity.ID);
            data.chunk.addComponent(com);
            return com;
        }

        public static U initComponent<T, U>(Entity entity, U com)
            where T : IComponent
            where U : T, new()
        {
            com.entityID = entity.ID;
            var data = mEntityManager.getData(entity.ID);
            data.chunk.addComponent(com);
            return com;
        }

        public static T initComponent<T>(Entity entity, T com)
            where T : IComponent, new()
        {
            com.entityID = entity.ID;
            var data = mEntityManager.getData(entity.ID);
            data.chunk.addComponent(com);
            return com;
        }

        public static T getComponent<T>(Entity entity) where T : IComponent
        {
            var data = mEntityManager.getData(entity.ID);
            return (T)data.chunk.getComponent(data.chunkSlot, ComponentID<T>.ID);
        }

        public static T getComponent<T>(int entityID) where T : IComponent
        {
            var data = mEntityManager.getData(entityID);
            return (T)data.chunk.getComponent(data.chunkSlot, ComponentID<T>.ID);
        }

        public static bool tryGetComponent<T>(int entityID, out T component) where T : class, IComponent
        {
            var data = mEntityManager.getData(entityID);
            if (data.archetype.hasComponent(ComponentID<T>.ID))
            {
                component = (T)data.chunk.getComponent(data.chunkSlot, ComponentID<T>.ID);
                return true;
            }

            component = null;
            return false;
        }

        public static ImpCom getComponent<IDCom, ImpCom>(int entityID)
            where IDCom : IComponent
            where ImpCom : IDCom
        {
            var data = mEntityManager.getData(entityID);
            return (ImpCom)data.chunk.getComponent(data.chunkSlot, ComponentID<IDCom>.ID);
        }

        public static ImpCom getComponent<IDCom, ImpCom>(Entity entity)
            where IDCom : IComponent
            where ImpCom : IDCom
        {
            var data = mEntityManager.getData(entity.ID);
            return (ImpCom)data.chunk.getComponent(data.chunkSlot, ComponentID<IDCom>.ID);
        }

        public static int registerArchetype<T>() where T : ArchetypeMaskID, new()
        {
            var creator = ArchetypeMaskCreator<T>.value;
            return creator.getID();
        }

//         public static List<Archetype> query(ArchetypeKey key)
//         {
//             List<Archetype> list = new List<Archetype>();
//             foreach (var item in mArchetypeArray)
//             {
//                 if (item.key.anyOf(key))
//                 {
//                     list.Add(item);
//                 }
//             }
//             return list;
//         }

        public static List<IChunk> query(ArchetypeKey key)
        {
            //ArchetypeKey key = new ArchetypeKey(typeIDs);
            List<IChunk> list = new List<IChunk>();

            foreach (var item in mArchetypeArray)
            {
                if (item.key.anyOf(key))
                {
                    foreach (var chunk in item.chunks)
                    {
                        list.Add(chunk);
                    }
                }
            }
            return list;
        }

        public static void buildingWorld()
        {
            if (mIsBuildCompleted)
            {
                return;
            }

            mIsBuildCompleted = true;
            mArchetypeArray = new Archetype[ArchetypeMaskID.count];
            for (int i = 0; i < ArchetypeMaskCreator.allMask.Count; i++)
            {
                var archetype = new Archetype();
                var creator = ArchetypeMaskCreator.allMask[i];
                archetype.init(creator.getID(), creator.componentIDs);
                mArchetypeArray[creator.getID()] = archetype;
            }
        }
    }
}
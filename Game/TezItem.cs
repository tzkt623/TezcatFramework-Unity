using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace tezcat
{

    public abstract class TezItem : ITezSerializable
    {
        /// <summary>
        /// 图标
        /// </summary>
        public List<int> icon = new List<int>();

        /// <summary>
        /// 图片
        /// </summary>
        public List<int> tex2d = new List<int>();

        public bool invalid
        {
            get { return resUID.invalid; }
        }

        #region ID
        /// <summary>
        /// 资源ID
        /// </summary>
        public readonly TezResUID resUID = new TezResUID();

        /// <summary>
        /// 
        /// </summary>
        public string name
        {
            get; protected set;
        }
        #endregion

        protected abstract int groupID { get; }
        protected abstract int typeID { get; }

        #region 序列化
        public virtual TezJsonWriter serialization()
        {
            TezJsonWriter writer = new TezJsonWriter();
            writer.pushValue("name", name);
            return writer;
        }

        public virtual void deserialization(TezJsonReader reader)
        {
            this.name = reader.getString("name");
            this.resUID.setGroupID(this.groupID);
            this.resUID.setTypeID(this.typeID);

            if (reader.tryPush("asset"))
            {
                reader.push("icon");
                for (int i = 0; i < reader.count(); i++)
                {
                    var name = reader.getString(i);
                    icon.Add(TezTextureManager.instance.getSpriteID(name));
                }
                reader.pop();

                reader.pop();
            }
        }
        #endregion

        public bool equal(TezItem other)
        {
            return resUID == other.resUID;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace tezcat
{

    public abstract class TezItem : ITezSerializable
    {
        public TezIconPack icon { get; private set; }
        public TezResUID resUID { get; private set; }
        public TezStaticString name { get; private set; }
        public bool invalid { get { return resUID.invalid; } }

        protected abstract int groupID { get; }
        protected abstract int typeID { get; }

        public TezItem()
        {
            icon = new TezIconPack();
            resUID = new TezResUID();
            name = TezStaticString.empty;
        }

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
                    icon.setIcon(new TezSprite(name), (TezIconType)i);
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
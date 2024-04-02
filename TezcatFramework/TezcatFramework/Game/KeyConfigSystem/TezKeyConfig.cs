using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// 具体配置类
    /// 通过和TezKeyWrapper配合
    /// 来配置具体按键规则以触发按键效果
    /// </summary>
    public abstract class TezKeyConfig : ITezCloseable
    {
        public string name { get; }

        TezKeyWrapper[] mWrappers = null;

        public TezKeyConfig(string name, int keyCount)
        {
            this.name = name;
            mWrappers = new TezKeyWrapper[keyCount];
        }

        public abstract bool active();

        public void setWrapper(int index, TezKeyWrapper wrapper)
        {
            mWrappers[index] = wrapper;
        }

        public void resetToDefault(TezReader reader)
        {
            this.readFromSave(reader);
        }

        public TezKeyWrapper getWrapper(int index)
        {
            return mWrappers[index];
        }

        public void writeToSave(TezWriter writer)
        {
            for (int i = 0; i < mWrappers.Length; i++)
            {
                writer.beginObject(i);
                mWrappers[i].writeToSave(writer);
                writer.endObject(i);
            }
        }
        public void readFromSave(TezReader reader)
        {
            for (int i = 0; i < reader.count; i++)
            {
                reader.beginObject(i);
                mWrappers[i].readFromSave(reader);
                reader.endObject(i);
            }
        }

        void ITezCloseable.deleteThis()
        {
            this.onClose();
        }

        protected virtual void onClose()
        {
            foreach (var item in mWrappers)
            {
                item.close();
            }
            mWrappers = null;
        }
    }
}
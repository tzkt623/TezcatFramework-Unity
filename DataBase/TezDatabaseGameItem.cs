using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.ECS;

namespace tezcat.Framework.Database
{
    /// <summary>
    /// 游戏对象Item
    /// </summary>
    public abstract class TezDatabaseGameItem : TezDatabaseItem
    {
        /// <summary>
        /// Class ID
        /// </summary>
        public string CID { get; private set; }


        public TezCategory category { get; private set; }


        public List<string> TAGS { get; private set; } = new List<string>();

        /// <summary>
        /// 使用Category系统
        /// 建立Item的分类路径
        /// </summary>
        protected abstract void onBuildCategory(TezReader reader, List<ITezCategoryBaseToken> list);

        public override void close()
        {
            base.close();

            this.category.close();
            this.TAGS.Clear();

            this.category = null;
            this.CID = null;
            this.TAGS = null;
        }

        public TezEntity createObject()
        {
            var obj = this.onCreateObject();
            obj.initWithData(this);

            var entity = TezEntity.create();
            entity.addComponent(obj);
            return entity;
        }

        protected override void onSerialize(TezWriter writer)
        {
            base.onSerialize(writer);
            writer.write(TezReadOnlyString.CID, this.CID);
            writer.write(TezReadOnlyString.NID, this.NID);
        }

        protected override void onDeserialize(TezReader reader)
        {
            base.onDeserialize(reader);
            this.buildCategory(reader);
            this.CID = reader.readString(TezReadOnlyString.CID);
            this.NID = reader.readString(TezReadOnlyString.NID);
        }

        private void buildCategory(TezReader reader)
        {
            ///6层初始容量应该够用了
            var list = new List<ITezCategoryBaseToken>(6);
            this.onBuildCategory(reader, list);
            TezCategorySystem.generate((ITezCategoryRootToken)list[0], (ITezCategoryToken)list[list.Count - 1], out TezCategory result, () =>
            {
                var category = new TezCategory();
                category.setToken(list);
                return category;
            });
            this.category = result;
        }

        protected virtual TezComData onCreateObject()
        {
            throw new Exception(string.Format("Please override this method for {0}", this.GetType().Name));
        }
    }
}
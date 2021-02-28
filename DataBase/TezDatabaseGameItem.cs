using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.ECS;
using tezcat.Framework.Utility;

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

        /// <summary>
        /// 分类系统
        /// </summary>
        public TezCategory category { get; protected set; }

        /// <summary>
        /// Tags用于对Item进行分类
        /// 如果为空
        /// 则表示不需要分类
        /// </summary>
        public string[] TAGS = null;

        /// <summary>
        /// 可堆叠数量
        /// </summary>
        public int stackCount { get; private set; } = 0;


        public override void close()
        {
            base.close();
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
            if (this.category != null)
            {
                writer.write(TezReadOnlyString.CTG_FT, this.category.finalToken.toName);
            }
        }

        protected override void onDeserialize(TezReader reader)
        {
            base.onDeserialize(reader);
            this.CID = reader.readString(TezReadOnlyString.CID);
            this.NID = reader.readString(TezReadOnlyString.NID);
            if (reader.tryRead(TezReadOnlyString.CTG_FT, out string final_token_name))
            {
                this.category = TezCategorySystem.getCategory(final_token_name);
            }
        }

//         private void buildCategory(TezReader reader)
//         {
//             ///6层初始容量应该够用了
//             var list = new List<ITezCategoryBaseToken>(6);
//             this.onBuildCategory(reader, list);
//             TezCategorySystem.generate((ITezCategoryRootToken)list[0], (ITezCategoryToken)list[list.Count - 1], out TezCategory result, () =>
//             {
//                 var category = new TezCategory();
//                 category.setToken(list);
//                 return category;
//             });
//         }

        protected virtual TezComData onCreateObject()
        {
            throw new Exception(string.Format("Please override this method for {0}", this.GetType().Name));
        }
    }
}
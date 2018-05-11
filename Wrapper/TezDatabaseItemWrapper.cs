using tezcat.DataBase;
using tezcat.Utility;
using UnityEngine;
namespace tezcat.Wrapper
{
    /// <summary>
    /// 数据库专用Item包装器
    /// </summary>
    public class TezDatabaseItemWrapper : TezItemWrapper<TezItem>
    {
        public TezDatabaseItemWrapper(int GUID) : base(GUID)
        {

        }

        public Sprite getIcon()
        {
            return TezTextureManager.getSprite(this.myItem.asset.icon_0);
        }

        public override void showTip()
        {
            var tips = TezTipManager.instance
                .setName(this.myName)
                .setDescription(this.myDescription)
                .pushAttribute(TezReadOnlyString.Database.GUID, this.myItem.GUID)
                .pushAttribute(TezReadOnlyString.Database.object_id, this.myItem.objectID);

            tips.show();
        }

        public override void hideTip()
        {
            TezTipManager.instance.hide();
        }
    }
}
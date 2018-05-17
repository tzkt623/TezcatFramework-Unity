using tezcat.DataBase;
using tezcat.Utility;
using UnityEngine;
namespace tezcat.Wrapper
{
    /// <summary>
    /// 数据库专用Item包装器
    /// </summary>
    public class TezDatabaseItemWrapper : TezItemSlotWrapper<TezDatabase.ContainerSlot>
    {
        public TezDatabaseItemWrapper(TezDatabase.ContainerSlot slot) : base(slot)
        {
        }

        public Sprite getIcon()
        {
            return TezTextureManager.getSprite(this.mySlot.item.asset.icon_0);
        }

        public override void showTip()
        {
            var tips = TezTipManager.instance
                .setName(this.myName)
                .setDescription(this.myDescription)
                .pushAttribute(TezReadOnlyString.Database.GUID, this.mySlot.item.GUID)
                .pushAttribute(TezReadOnlyString.Database.OID, this.mySlot.item.OID);

            foreach (var item in mySlot.item.properties)
            {
                tips.pushAttribute(item);
            }

            tips.show();
        }

        public override void hideTip()
        {
            TezTipManager.instance.hide();
        }
    }
}
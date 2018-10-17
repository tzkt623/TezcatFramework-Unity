using tezcat.DataBase;
using UnityEngine;
namespace tezcat.Wrapper
{
    /// <summary>
    /// 数据库专用Item包装器
    /// </summary>
    public class TezDatabaseItemWrapper
    {

        public Sprite getIcon()
        {
            return null;
        }

        public void showTip()
        {
//             var tips = TezService.tip
//                 .setName(this.myName)
//                 .setDescription(this.myDescription)
//                 .pushAttribute(TezReadOnlyString.Database.GUID, this.mySlot.myItem.GUID)
//                 .pushAttribute(TezReadOnlyString.Database.OID, this.mySlot.myItem.OID);
// 
//             foreach (var item in mySlot.myItem.properties)
//             {
//                 tips.pushAttribute(item);
//             }
// 
//             tips.show();
        }

        public void hideTip()
        {
//            TezService.tip.hide();
        }
    }
}
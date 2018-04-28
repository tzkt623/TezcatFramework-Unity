using tezcat.DataBase;
using UnityEngine;

using tezcat.Utility;
namespace tezcat.Wrapper
{
    public class TezDatabaseItemWrapper : ITezItemWrapper
    {
        public TezItem item
        {
            get { return TezDatabase.instance.getItem(m_GUID); }
        }

        public int count
        {
            get { return -1; }
        }

        public string name
        {
            get { return TezLocalization.getName(this.item.nameID); }
        }

        public string description
        {
            get { return TezLocalization.getDescription(this.item.nameID); }
        }

        public Sprite getIcon()
        {
            return new TezSprite().convertToSprite();
        }

        int m_GUID = -1;
        public TezDatabaseItemWrapper(int id)
        {
            m_GUID = id;
        }

        public void showTip()
        {
            var tips = TezTipManager.instance
                .setName(this.name)
                .setDescription(this.description)
                .pushAttributeSeparator()
                .pushAttribute("GUID", m_GUID)
                .pushAttribute("ObjectID", item.objectID);

            tips.show();
        }

        public void hideTip()
        {
            TezTipManager.instance.hide();
        }

        public void onDrop()
        {

        }

        public void clear()
        {
            m_GUID = -1;
        }
    }
}
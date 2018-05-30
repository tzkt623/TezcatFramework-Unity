using tezcat.DataBase;
using tezcat.Utility;
using UnityEngine;

namespace tezcat.UI
{
    public class TezItemEditor : TezBasicItemEditor
    {
        [Header("Prefab")]
        [SerializeField]
        TezPE_IFS m_PrefabPE_IFS = null;
        [SerializeField]
        TezPE_Type m_PrefabPE_Type = null;
        [SerializeField]
        TezPropertyView m_PrefabPE_View = null;
        [SerializeField]
        TezPE_StaticString m_PrefabPE_StaticString = null;

        [SerializeField]
        RectTransform m_Content = null;

        public override TezDatabase.CategoryType[] supportCategory
        {
            get { return new TezDatabase.CategoryType[0]; }
        }

        TezItem m_Item = null;

        protected override void preInit()
        {
            base.preInit();
        }

        protected override TezItem getItem()
        {
            return m_Item;
        }

        public override void bind(TezDatabase.CategoryType category_type)
        {
            m_Item = category_type.create();
            this.dirty = true;
        }

        public override void bind(TezItem item)
        {
            m_Item = item;
            this.dirty = true;
        }

        private TezPropertyView createPV(string name, string value)
        {
            var pv = Instantiate(m_PrefabPE_View, m_Content, false);
            pv.set(name, value);
            return pv;
        }

        private void createPE_IFS(TezPropertyValue property)
        {
            var pe = Instantiate(m_PrefabPE_IFS, m_Content, false);
            pe.bind(property);
            pe.open();
        }

        private void createPE_Type(TezPropertyValue property)
        {
            var pro = Instantiate(m_PrefabPE_Type, m_Content, false);
            pro.bind(property);
            pro.open();
        }

        protected override void onRefresh()
        {
            foreach (RectTransform item in m_Content)
            {
                item.GetComponent<TezWidget>().close();
            }

            if (m_Item != null)
            {
                var view = Instantiate(m_PrefabPE_View, m_Content, false);
                view.set(() => TezTranslater.translateName(TezReadOnlyString.Database.OID, TezReadOnlyString.Database.OID),
                    () => m_Item.OID.ToString());
                view.open();

                view = Instantiate(m_PrefabPE_View, m_Content, false);
                view.set(() => TezTranslater.translateName(TezReadOnlyString.Database.GUID, TezReadOnlyString.Database.GUID),
                    () => m_Item.GUID.ToString());
                view.open();

                var editor = Instantiate(m_PrefabPE_StaticString, m_Content, false);
                editor.bind(() => TezTranslater.translateName(TezReadOnlyString.Database.NID, TezReadOnlyString.Database.NID), m_Item.NID);
                editor.open();

                var properties = m_Item.properties;
                for (int i = 0; i < properties.Count; i++)
                {
                    switch (properties[i].getParameterType())
                    {
                        case TezPropertyType.Type:
                            this.createPE_Type(properties[i]);
                            break;
                        case TezPropertyType.Int:
                        case TezPropertyType.Float:
                        case TezPropertyType.String:
                        case TezPropertyType.StaticString:
                            this.createPE_IFS(properties[i]);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}


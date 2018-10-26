using tezcat.Framework.Core;
using tezcat.Framework.DataBase;
using tezcat.Framework.Utility;
using UnityEngine;

namespace tezcat.Framework.UI
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

        public override int[] supportCategory      {
            get { return new int[0]; }
        }

        TezDataBaseGameItem m_Item = null;

        protected override void preInit()
        {
            base.preInit();
        }

        protected override TezDataBaseItem getItem()
        {
            return m_Item;
        }

        public override void bind(int category)
        {
            this.refresh = RefreshPhase.System1;
        }

//         public override void bind(TezItem item)
//         {
//             m_Item = item;
//             this.dirty = true;
//         }

        private TezPropertyView createPV(string name, string value)
        {
            var pv = Instantiate(m_PrefabPE_View, m_Content, false);
            pv.set(name, value);
            return pv;
        }

        private void createPE_IFS(TezValueWrapper property)
        {
            var pe = Instantiate(m_PrefabPE_IFS, m_Content, false);
            pe.bind(property);
            pe.open();
        }

        private void createPE_Type(TezValueWrapper property)
        {
            var pro = Instantiate(m_PrefabPE_Type, m_Content, false);
            pro.bind(property);
            pro.open();
        }

        protected override void refreshAfterInit()
        {
            foreach (RectTransform item in m_Content)
            {
                item.GetComponent<TezUIWidget>().close();
            }

            if (m_Item != null)
            {
                var view = Instantiate(m_PrefabPE_View, m_Content, false);
//                view.set(() => TezTranslator.translateName(TezReadOnlyString.Database.OID, TezReadOnlyString.Database.OID), () => m_Item.OID.ToString());
                view.open();

                view = Instantiate(m_PrefabPE_View, m_Content, false);
                view.set(() => TezTranslator.translateName(TezReadOnlyString.Database.NID, TezReadOnlyString.Database.NID),
                    () => m_Item.NID.ToString());
                view.open();

                var editor = Instantiate(m_PrefabPE_StaticString, m_Content, false);
                editor.bind(() => TezTranslator.translateName(TezReadOnlyString.Database.NID, TezReadOnlyString.Database.NID), m_Item.NID);
                editor.open();

                var properties = m_Item.properties;
                for (int i = 0; i < properties.count; i++)
                {
                    var property = properties.get(i);
                    switch (property.valueType)
                    {
                        case TezValueType.Type:
                            this.createPE_Type(property);
                            break;
                        case TezValueType.Int:
                        case TezValueType.Float:
                        case TezValueType.String:
                        case TezValueType.StaticString:
                            this.createPE_IFS(property);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        protected override void saveFailed()
        {

        }
    }
}


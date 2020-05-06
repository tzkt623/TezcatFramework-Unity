using tezcat.Framework.Core;
using tezcat.Framework.Database;
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

        public override int[] supportCategory
        {
            get { return new int[0]; }
        }

        TezDatabaseGameItem m_Item = null;

        protected override void preInit()
        {
            base.preInit();
        }

        protected override TezDatabaseItem getItem()
        {
            return m_Item;
        }

        public override void bind(int category)
        {
            this.refreshPhase = TezRefreshPhase.P_Custom1;
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

        protected override void onRefresh(TezRefreshPhase phase)
        {
            base.onRefresh(phase);
            switch (phase)
            {
                case TezRefreshPhase.P_OnInit:
                    this.refreshInit();
                    break;
                case TezRefreshPhase.P_OnEnable:
                    break;
                case TezRefreshPhase.P_Custom1:
                    break;
                case TezRefreshPhase.P_Custom2:
                    break;
                case TezRefreshPhase.P_Custom3:
                    break;
                case TezRefreshPhase.P_Custom4:
                    break;
                case TezRefreshPhase.P_Custom5:
                    break;
                default:
                    break;
            }
        }

        private void refreshInit()
        {
            foreach (RectTransform item in m_Content)
            {
                item.GetComponent<TezUIWidget>().close();
            }

            if (m_Item != null)
            {
                var view = Instantiate(m_PrefabPE_View, m_Content, false);
                //                view.set(() => TezService.get<TezTranslator>().translateName(TezReadOnlyString.Database.OID, TezReadOnlyString.Database.OID), () => m_Item.OID.ToString());
                view.open();

                view = Instantiate(m_PrefabPE_View, m_Content, false);
                view.set(() => TezService.get<TezTranslator>().translateName(TezReadOnlyString.NID, TezReadOnlyString.NID),
                    () => m_Item.NID.ToString());
                view.open();

                var editor = Instantiate(m_PrefabPE_StaticString, m_Content, false);
                editor.bind(() => TezService.get<TezTranslator>().translateName(TezReadOnlyString.NID, TezReadOnlyString.NID), m_Item.NID);
                editor.open();

                var properties = m_Item.properties;
                for (int i = 0; i < properties.count; i++)
                {
                    var property = properties[i];
                    switch (property.valueType)
                    {
                        case TezValueType.Type:
                            this.createPE_Type((TezValueWrapper)property);
                            break;
                        case TezValueType.Int:
                        case TezValueType.Float:
                        case TezValueType.String:
                        case TezValueType.StaticString:
                            this.createPE_IFS((TezValueWrapper)property);
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


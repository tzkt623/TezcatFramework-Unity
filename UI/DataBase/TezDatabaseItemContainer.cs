using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Database;
using tezcat.Framework.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.Framework.UI
{
    public class TezDatabaseItemContainer
        : TezSubwindow
    {
        [Header("Widget")]
        [SerializeField]
        RectTransform m_Content = null;
        [SerializeField]
        RectTransform m_Vernier = null;

        [Header("Page")]
        [SerializeField]
        int m_CountPerPage = 100;
        [SerializeField]
        TezButton m_PageUp = null;
        [SerializeField]
        TezButton m_PageDown = null;
        [SerializeField]
        InputField m_Page = null;
        [SerializeField]
        Text m_MaxPage = null;

        TezPageController m_PageController = new TezPageController();
        List<TezDatabaseSlot> m_SlotList = new List<TezDatabaseSlot>();

        TezDatabaseGroup m_Group = null;
        TezDatabaseSlot m_CurrentSlot = null;
        public TezDatabaseItem currentItem
        {
            get; private set;
        }

        protected override void preInit()
        {
            base.preInit();

            ///page
            m_PageController.pageCapacity = m_CountPerPage;
//            m_PageController.setListener(this.onPageChanged);
            m_PageUp.onClick += onPageUpClick;
            m_PageDown.onClick += onPageDownClick;
            m_Page.contentType = InputField.ContentType.IntegerNumber;
            m_Page.onEndEdit.AddListener(this.onPageSet);
        }

        protected override void onClose()
        {
            foreach (var slot in m_SlotList)
            {
                slot.close();
            }
            m_SlotList.Clear();
            m_SlotList = null;

            m_PageController.close();
            m_PageController = null;
            m_PageUp.onClick -= onPageUpClick;
            m_PageDown.onClick -= onPageDownClick;
            m_Page.onEndEdit.RemoveListener(this.onPageSet);

            m_Group = null;
            m_CurrentSlot = null;
            this.currentItem = null;

            base.onClose();
        }

        protected override void linkEvent()
        {
            base.linkEvent();
//            TezDatabase.onRegsiterItem.add(this.onAdd);
        }

        protected override void unLinkEvent()
        {
            base.unLinkEvent();
//            TezDatabase.onRegsiterItem.remove(this.onAdd);
        }

        public void removeItem()
        {
            if(m_CurrentSlot)
            {
                m_CurrentSlot.removeItem();
            }
        }

#if false
        private void onAdd(TezDatabase.CategoryContainerSlot slot)
        {
            var id = m_PageController.isInPage(slot.ID);
            if(id >= 0)
            {
                while(m_SlotList.Count <= id)
                {
                    var ui = Instantiate(TezPrefabDatabase.get<TezDatabaseSlot>(), m_Content, false);
                    ui.container = this;
                    ui.open();
                    m_SlotList.Add(ui);
                }

                m_SlotList[id].bind(new TezDatabaseItemWrapper(slot));

                var items = TezService.DB.getItems(m_Group.groupType.ID, m_Group.categoryType.ID);
                m_PageController.calculateMaxPage(items.Count);
                m_MaxPage.text = "/" + m_PageController.maxPage.ToString();
                m_Page.text = m_PageController.currentPage.ToString();
            }
        }

        public void onSelectSlot(TezDatabaseSlot slot, TezItem item)
        {
            m_CurrentSlot = slot;
            this.currentItem = item;

            m_Vernier.SetParent(slot.transform, false);
            m_Vernier.localScale = Vector3.one;
            m_Vernier.localPosition = Vector3.zero;
            TezLayout.setLayout(m_Vernier, -4, -4, 4, 4);
            m_Vernier.gameObject.SetActive(true);
        }

        private void onPageChanged(int begin, int end)
        {
            if (m_Group.groupType == null || m_Group.categoryType == null)
            {
                return;
            }

            this.reset();

            var items = TezService.DB.getItems(m_Group.groupType.ID, m_Group.categoryType.ID);
            if (items.Count > 0)
            {
                m_PageController.calculateMaxPage(items.Count);
                m_MaxPage.text = "/" + m_PageController.maxPage.ToString();
                m_Page.text = m_PageController.currentPage.ToString();

                end = Mathf.Min(end, items.Count);
                for (int i = begin; i < end; i++)
                {
                    var ui = Instantiate(TezPrefabDatabase.get<TezDatabaseSlot>(), m_Content, false);
                    ui.container = this;
                    ui.open();
                    m_SlotList.Add(ui);
                }

                for (int i = begin; i < end; i++)
                {
                    if (items[i].myItem)
                    {
                        m_SlotList[i].bind(new TezDatabaseItemWrapper(items[i]));
                    }
                }
            }
        }
#endif

        protected override void onRefresh(TezRefreshPhase phase)
        {
            base.onRefresh(phase);
            switch (phase)
            {
                case TezRefreshPhase.P_OnInit:
                    m_PageController.setPage(m_PageController.currentPage);
                    break;
                case TezRefreshPhase.P_OnEnable:
                    break;
                default:
                    break;
            }
        }


        private void onPageSet(string page)
        {
            if (m_Vernier.gameObject.activeSelf)
            {
                m_Vernier.SetParent(this.transform, false);
                m_Vernier.gameObject.SetActive(false);
            }

            var current = int.Parse(page);
            m_PageController.setPage(current);
        }

        private void onPageDownClick(TezButton button, PointerEventData eventData)
        {
            m_PageController.pageDown();
        }

        private void onPageUpClick(TezButton button, PointerEventData eventData)
        {
            m_PageController.pageUp();
        }

        public override void reset()
        {
            m_CurrentSlot = null;
            this.currentItem = null;
            if (m_Vernier.gameObject.activeSelf)
            {
                m_Vernier.parent = null;
                m_Vernier.gameObject.SetActive(false);
            }

            foreach (var slot in m_SlotList)
            {
                slot.close();
            }

            m_SlotList.Clear();
        }

        public void setGroup(TezDatabaseGroup group)
        {
            m_Group = group;
        }
    }
}

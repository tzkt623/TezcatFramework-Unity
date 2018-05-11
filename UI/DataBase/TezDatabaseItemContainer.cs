using System.Collections.Generic;
using tezcat.DataBase;
using tezcat.TypeTraits;
using tezcat.Wrapper;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezDatabaseItemContainer
        : TezArea
    {
        [Header("Prefab")]
        [SerializeField]
        TezDatabaseSlot m_Prefab = null;

        [Header("Widget")]
        [SerializeField]
        RectTransform m_Content = null;
        [SerializeField]
        RectTransform m_Vernier = null;

        [Header("Page")]
        [SerializeField]
        int m_CountPerPage = 100;
        [SerializeField]
        TezImageButton m_PageUp = null;
        [SerializeField]
        TezImageButton m_PageDown = null;
        [SerializeField]
        InputField m_Page = null;
        [SerializeField]
        Text m_MaxPage = null;

        TezPageController m_PageController = new TezPageController();
        List<TezDatabaseSlot> m_SlotList = new List<TezDatabaseSlot>();

        TezDatabaseGroup m_Group = null;

        protected override void Awake()
        {
            base.Awake();

            ///page
            m_PageController.countPerPage = m_CountPerPage;
            m_PageController.setListener(this.onPageChanged);
            m_PageUp.onClick += onPageUpClick;
            m_PageDown.onClick += onPageDownClick;
            m_Page.contentType = InputField.ContentType.IntegerNumber;
            m_Page.onEndEdit.AddListener(this.onPageSet);
        }

        protected override void Start()
        {
            base.Start();
            this.dirty = true;
        }

        protected override void onRefresh()
        {
            m_PageController.setPage(m_PageController.currentPage);
        }

        private void onPageChanged(int begin, int end)
        {
            if (m_Group.groupType == null || m_Group.categoryType == null)
            {
                return;
            }

            this.reset();

            TezDatabase.tryForeachItems(m_Group.groupType.ID, m_Group.categoryType.ID,
            (TezDatabase.ContainerSlot slot) =>
            {
                m_PageController.calculateMaxPage(items.Count);
                m_MaxPage.text = "/" + m_PageController.maxPage.ToString();
                m_Page.text = m_PageController.currentPage.ToString();

                end = Mathf.Min(end, items.Count);
                for (int i = begin; i < end; i++)
                {
                    var slot = Instantiate(m_Prefab, m_Content, false);
                    slot.open();
                    m_SlotList.Add(slot);
                }

                for (int i = 0; i < items.Count; i++)
                {
                    m_SlotList[i].bind(new TezDatabaseItemWrapper(items[i].item.GUID));
                }
            });
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

        private void onPageDownClick(PointerEventData.InputButton button)
        {
            m_PageController.pageDown();
        }

        private void onPageUpClick(PointerEventData.InputButton button)
        {
            m_PageController.pageUp();
        }

        private void reset()
        {
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

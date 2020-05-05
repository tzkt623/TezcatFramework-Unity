using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.Framework.UI
{
    public class TezLocalizationDescriptionList : TezSubwindow
    {
        [Header("Widget")]
        [SerializeField]
        RectTransform m_Content = null;
        [SerializeField]
        RectTransform m_Vernier = null;

        [Header("Load Property")]
        [SerializeField]
        TezButton m_LoadProperty = null;
        [SerializeField]
        TezButton m_LoadPropertySingal = null;

        [Header("Add And Remove")]
        [SerializeField]
        TezButton m_Add = null;
        [SerializeField]
        TezButton m_Remove = null;

        [Header("Search")]
        [SerializeField]
        TezButton m_Search = null;
        [SerializeField]
        TezButton m_ClearSearch = null;
        [SerializeField]
        InputField m_SearchKey = null;

        [Header("Page Controller")]
        [SerializeField]
        GameObject m_PageGO = null;
        [SerializeField]
        int m_CountPerPage = 20;
        [SerializeField]
        TezButton m_PageUp = null;
        [SerializeField]
        TezButton m_PageDown = null;
        [SerializeField]
        InputField m_Page = null;
        [SerializeField]
        Text m_MaxPage = null;

        TezPageController m_PageController = new TezPageController();
        TezLocalizationDescriptionItem m_SearchResult = null;
        TezLocalizationDescriptionItem m_SelectItem = null;

        List<TezLocalizationDescriptionItem> m_ItemList = new List<TezLocalizationDescriptionItem>();

        protected override void preInit()
        {
            base.preInit();

            ///function
            m_Add.onClick += onAddClick;
            m_Remove.onClick += onRemoveClick;
            m_LoadProperty.onClick += onLoadPropertyClick;

            ///search
            m_SearchKey.onEndEdit.AddListener(this.onSearch);
            m_Search.onClick += onSearchClick;
            m_ClearSearch.onClick += onClearSearchClick;

            ///page
            m_PageController.pageCapacity = m_CountPerPage;
            m_PageController.setListener(this.onPageChanged, onEmptyPage);
            m_PageUp.onClick += onPageUpClick;
            m_PageDown.onClick += onPageDownClick;
            m_Page.contentType = InputField.ContentType.IntegerNumber;
            m_Page.onEndEdit.AddListener(this.onPageSet);
        }

        private void onEmptyPage()
        {
            throw new NotImplementedException();
        }

        protected override void onHide()
        {

        }

        protected override void onRefresh(TezRefreshPhase phase)
        {
            base.onRefresh(phase);

            switch (phase)
            {
                case TezRefreshPhase.P_OnInit:
                    this.refreshData();
                    break;
                case TezRefreshPhase.P_OnEnable:
                    break;
                default:
                    break;
            }
        }

        private void refreshData()
        {
            m_PageController.count = TezService.get<TezTranslator>().descriptionCount;
            m_PageController.setPage(m_PageController.currentPage);
        }

        private void onPageChanged(int begin, int end)
        {
            m_MaxPage.text = "/" + m_PageController.maxPage.ToString();
            m_Page.text = m_PageController.currentPage.ToString();

            foreach (var item in m_ItemList)
            {
                item.close();
            }
            m_ItemList.Clear();

            TezService.get<TezTranslator>().foreachDescription(
                this.createItem,
                begin,
                end);
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

        private void onAddClick(TezButton button, PointerEventData eventData)
        {
            var editor = TezService.get<TezcatFramework>().createWidget<TezLocalizationDescriptionEditor>("DescriptionEditor", this.window.overlay);
            editor.listArea = this;
            editor.newItem();
            editor.open();
        }

        private void onRemoveClick(TezButton button, PointerEventData eventData)
        {
            TezService.get<TezTranslator>().removeDescription(m_SelectItem.key);
            m_Vernier.SetParent(this.transform, false);
            m_Vernier.gameObject.SetActive(false);
            this.refreshPhase = TezRefreshPhase.P_Custom3;
        }

        private void onClearSearchClick(TezButton button, PointerEventData eventData)
        {
            if (m_SearchResult != null)
            {
                m_SearchResult.close();
                m_SearchResult = null;
                this.showAllItem();
            }

            m_SearchKey.text = string.Empty;
            m_PageGO.SetActive(true);
        }

        private void onSearchClick(TezButton button, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                this.onSearch(m_SearchKey.text);
            }
        }

        private void onSearch(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                string value = null;
                if (TezService.get<TezTranslator>().translateDescription(key, out value))
                {
                    if (m_SearchResult != null)
                    {
                        m_SearchResult.set(key);
                    }
                    else
                    {
                        m_SearchResult = TezService.get<TezcatFramework>().createWidget<TezLocalizationDescriptionItem>("DescriptionItem", m_Content);
                        m_SearchResult.listArea = this;
                        m_SearchResult.set(key);
                        m_SearchResult.open();
                    }
                }

                this.hideAllItem();
                m_PageGO.SetActive(false);
            }
        }

        private void onLoadPropertyClick(TezButton button, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
//                 TezValueDescriptor.foreachName((ITezValueDescriptor name) =>
//                 {
//                     TezService.get<TezTranslator>().tryAddDescription(name.name, name.name);
//                 });

                this.refreshPhase = TezRefreshPhase.P_Custom3;
            }
        }

        private void hideAllItem()
        {
            foreach (var item in m_ItemList)
            {
                item.hide();
            }
        }

        private void showAllItem()
        {
            foreach (var item in m_ItemList)
            {
                item.open();
            }
        }

        private void createItem(string key, string value)
        {
            var item = TezService.get<TezcatFramework>().createWidget<TezLocalizationDescriptionItem>("DescriptionItem", m_Content);
            item.set(key);
            item.listArea = this;
            item.open();
            m_ItemList.Add(item);
        }

        public void editItem(string key)
        {
            var editor = TezService.get<TezcatFramework>().createWidget<TezLocalizationDescriptionEditor>("DescriptionEditor", this.window.overlay);
            editor.listArea = this;
            editor.set(key);
            editor.open();
        }

        public void onFocus(TezLocalizationDescriptionItem item)
        {
            m_SelectItem = item;
            m_Vernier.gameObject.SetActive(true);
            m_Vernier.SetParent(m_SelectItem.transform, false);
            TezLayout.setLayout(m_Vernier, -4, -4, 4, 4);
        }
    }
}


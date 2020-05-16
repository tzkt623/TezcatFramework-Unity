using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Game;
using tezcat.Framework.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.Framework.UI
{
    public class TezLocalizationNameList : TezSubwindow
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
        TezLocalizationNameItem m_SearchResult = null;
        TezLocalizationNameItem m_SelectItem = null;

        List<TezLocalizationNameItem> m_ItemList = new List<TezLocalizationNameItem>();

        protected override void preInit()
        {
            base.preInit();

            ///
            m_Add.onClick += onAddClick;
            m_Remove.onClick += onRemoveClick;
            m_LoadProperty.onClick += onLoadPropertyClick;

            ///
            m_SearchKey.onEndEdit.AddListener(this.onSearch);
            m_Search.onClick += onSearchClick;
            m_ClearSearch.onClick += onClearSearchClick;

            ///
            m_PageController.pageCapacity = m_CountPerPage;
            m_PageController.setListener(this.onPageChanged, onEmptyPage);
            m_PageUp.onClick += onPageUpClick;
            m_PageDown.onClick += onPageDownClick;
            m_Page.onValueChanged.AddListener(this.onPageSet);
            m_Page.contentType = InputField.ContentType.IntegerNumber;
        }

        private void onEmptyPage()
        {
            throw new NotImplementedException();
        }

        protected override void onClose(bool self_close = true)
        {
            m_Add.onClick -= onAddClick;
            m_Remove.onClick -= onRemoveClick;
            m_LoadProperty.onClick -= onLoadPropertyClick;

            ///
            m_SearchKey.onEndEdit.RemoveListener(this.onSearch);
            m_Search.onClick -= onSearchClick;
            m_ClearSearch.onClick -= onClearSearchClick;

            ///
            m_PageUp.onClick -= onPageUpClick;
            m_PageDown.onClick -= onPageDownClick;
            m_Page.onValueChanged.RemoveListener(this.onPageSet);

            base.onClose(self_close);
        }

        private void refreshData()
        {
            m_PageController.count = TezService.get<TezTranslator>().nameCount;
            m_PageController.setPage(m_PageController.currentPage);
        }

        protected override void onHide()
        {

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

            TezService.get<TezTranslator>().foreachName(this.createItem, begin, end);
        }

        private void onPageDownClick(TezButton button, PointerEventData eventData)
        {
            m_PageController.pageDown();
        }

        private void onPageUpClick(TezButton button, PointerEventData eventData)
        {
            m_PageController.pageUp();
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

        private void onAddClick(TezButton button, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                //                 var editor = TezService.get<TezcatFramework>().createWidget<TezLocalizationNameEditor>("NameEditor", this.window.overlay);
                //                 editor.listArea = this;
                //                 editor.transform.SetAsLastSibling();
                //                 editor.newItem();
                //                 editor.open();
            }
        }

        private void onRemoveClick(TezButton button, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (TezService.get<TezTranslator>().removeName(m_SelectItem.key))
                {
                    m_Vernier.SetParent(this.transform, false);
                    m_Vernier.gameObject.SetActive(false);
                    this.refreshPhase = TezRefreshPhase.Refresh;
                }
            }
        }

        private void onClearSearchClick(TezButton button, PointerEventData eventData)
        {
            if (m_SearchResult != null)
            {
                m_SearchResult.close();
                m_SearchResult = null;
            }

            m_SearchKey.text = string.Empty;
            m_PageGO.SetActive(true);
            this.showAllItem();
        }

        private void onSearch(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                string value = null;
                if (TezService.get<TezTranslator>().translateName(key, out value))
                {
                    if (m_SearchResult != null)
                    {
                        m_SearchResult.set(key, value);
                    }
                    else
                    {
                        m_SearchResult = TezService.get<TezcatFramework>().createWidget<TezLocalizationNameItem>(m_Content);
                        m_SearchResult.listArea = this;
                        m_SearchResult.set(key, value);
                        m_SearchResult.open();
                    }
                }

                this.hideAllItem();
                m_PageGO.SetActive(false);
            }
        }

        private void onSearchClick(TezButton button, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                this.onSearch(m_SearchKey.text);
            }
        }

        private void onLoadPropertyClick(TezButton button, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                //                 TezValueDescriptor.foreachName((ITezValueDescriptor name) =>
                //                 {
                //                     TezService.get<TezTranslator>().tryAddName(name.name, name.name);
                //                 });

                this.refreshPhase = TezRefreshPhase.Refresh;
            }
        }

        public void onFocus(TezLocalizationNameItem item)
        {
            m_SelectItem = item;
            m_Vernier.gameObject.SetActive(true);
            m_Vernier.SetParent(m_SelectItem.transform, false);
            TezLayout.setLayout(m_Vernier, -4, -4, 4, 4);
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

        public void edit(string key)
        {
            //             var editor = TezService.get<TezcatFramework>().createWidget<TezLocalizationNameEditor>("NameEditor", this.window.overlay);
            //             editor.listArea = this;
            //             editor.transform.SetAsLastSibling();
            //             editor.set(key);
            //             editor.open();
        }

        private void createItem(string key, string value)
        {
            var item = TezService.get<TezcatFramework>().createWidget<TezLocalizationNameItem>(m_Content);
            item.listArea = this;
            item.set(key, value);
            item.open();

            m_ItemList.Add(item);
        }
    }
}


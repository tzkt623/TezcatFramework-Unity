﻿using System.Collections.Generic;
using tezcat.Core;
using tezcat.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezLocalizationDescriptionList : TezArea
    {
        [Header("Widget")]
        [SerializeField]
        RectTransform m_Content = null;
        [SerializeField]
        RectTransform m_Vernier = null;

        [Header("Load Property")]
        [SerializeField]
        TezImageLabelButton m_LoadProperty = null;
        [SerializeField]
        TezImageLabelButton m_LoadPropertySingal = null;

        [Header("Add And Remove")]
        [SerializeField]
        TezImageLabelButton m_Add = null;
        [SerializeField]
        TezImageLabelButton m_Remove = null;

        [Header("Search")]
        [SerializeField]
        TezImageLabelButton m_Search = null;
        [SerializeField]
        TezImageLabelButton m_ClearSearch = null;
        [SerializeField]
        InputField m_SearchKey = null;

        [Header("Page Controller")]
        [SerializeField]
        GameObject m_PageGO = null;
        [SerializeField]
        int m_CountPerPage = 20;
        [SerializeField]
        TezImageButton m_PageUp = null;
        [SerializeField]
        TezImageButton m_PageDown = null;
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
            m_PageController.setListener(this.onPageChanged);
            m_PageUp.onClick += onPageUpClick;
            m_PageDown.onClick += onPageDownClick;
            m_Page.contentType = InputField.ContentType.IntegerNumber;
            m_Page.onEndEdit.AddListener(this.onPageSet);
        }

        protected override void onShow()
        {

        }

        protected override void onHide()
        {

        }

        protected override void onRefresh()
        {
            m_PageController.calculateMaxPage(TezTranslater.descriptionCount);
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

            TezTranslater.foreachDescription(
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

        private void onPageDownClick(PointerEventData.InputButton button)
        {
            m_PageController.pageDown();
        }

        private void onPageUpClick(PointerEventData.InputButton button)
        {
            m_PageController.pageUp();
        }

        private void onAddClick(PointerEventData.InputButton button)
        {
            var editor = TezcatFramework.instance.createWidget<TezLocalizationDescriptionEditor>("DescriptionEditor", this.window.overlay);
            editor.listArea = this;
            editor.newItem();
            editor.open();
        }

        private void onRemoveClick(PointerEventData.InputButton button)
        {
            TezTranslater.removeDescription(m_SelectItem.key);
            m_Vernier.SetParent(this.transform, false);
            m_Vernier.gameObject.SetActive(false);
            this.dirty = true;
        }

        private void onClearSearchClick(PointerEventData.InputButton button)
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

        private void onSearchClick(PointerEventData.InputButton button)
        {
            if (button == PointerEventData.InputButton.Left)
            {
                this.onSearch(m_SearchKey.text);
            }
        }

        private void onSearch(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                string value = null;
                if (TezTranslater.translateDescription(key, out value))
                {
                    if (m_SearchResult != null)
                    {
                        m_SearchResult.set(key);
                    }
                    else
                    {
                        m_SearchResult = TezcatFramework.instance.createWidget<TezLocalizationDescriptionItem>("DescriptionItem", m_Content);
                        m_SearchResult.listArea = this;
                        m_SearchResult.set(key);
                        m_SearchResult.open();
                    }
                }

                this.hideAllItem();
                m_PageGO.SetActive(false);
            }
        }

        private void onLoadPropertyClick(PointerEventData.InputButton button)
        {
            if (button == PointerEventData.InputButton.Left)
            {
                TezPropertyManager.foreachProperty((TezPropertyName name) =>
                {
                    TezTranslater.tryAddDescription(name.name, name.name);
                });

                this.dirty = true;
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
            var item = TezcatFramework.instance.createWidget<TezLocalizationDescriptionItem>("DescriptionItem", m_Content);
            item.set(key);
            item.listArea = this;
            item.open();
            m_ItemList.Add(item);
        }

        public void editItem(string key)
        {
            var editor = TezcatFramework.instance.createWidget<TezLocalizationDescriptionEditor>("DescriptionEditor", this.window.overlay);
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


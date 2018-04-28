﻿using System.Collections.Generic;
using tezcat.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezLocalizationNameList : TezArea
    {
        [Header("Prefab")]
        [SerializeField]
        TezLocalizationNameItem m_Prefab = null;
        [SerializeField]
        TezLocalizationNameEditor m_PrefabEditor = null;

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
        TezLocalizationNameItem m_SearchResult = null;
        TezLocalizationNameItem m_SelectItem = null;

        List<TezLocalizationNameItem> m_ItemList = new List<TezLocalizationNameItem>();

        protected override void Awake()
        {
            base.Awake();

            ///
            m_Add.onClick += onAddClick;
            m_Remove.onClick += onRemoveClick;
            m_LoadProperty.onClick += onLoadPropertyClick;

            ///
            m_SearchKey.onEndEdit.AddListener(this.onSearch);
            m_Search.onClick += onSearchClick;
            m_ClearSearch.onClick += onClearSearchClick;

            ///
            m_PageController.countPerPage = m_CountPerPage;
            m_PageController.setListener(this.onPageChanged);
            m_PageUp.onClick += onPageUpClick;
            m_PageDown.onClick += onPageDownClick;
            m_Page.onValueChanged.AddListener(this.onPageSet);
            m_Page.contentType = InputField.ContentType.IntegerNumber;

        }

        protected override void Start()
        {
            base.Start();
            this.dirty = true;
        }

        protected override void onRefresh()
        {
            m_PageController.calculateMaxPage(TezLocalization.nameCount);
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

            TezLocalization.foreachName(
                this.createItem,
                begin,
                end);
        }

        private void onPageDownClick(PointerEventData.InputButton button)
        {
            m_PageController.pageDown();
        }

        private void onPageUpClick(PointerEventData.InputButton button)
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

        private void onAddClick(PointerEventData.InputButton button)
        {
            var editor = this.window.createPopup(m_PrefabEditor);
            editor.newItem();
            editor.open();
        }

        private void onRemoveClick(PointerEventData.InputButton button)
        {
            TezLocalization.removeName(m_SelectItem.index);
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

        private void onSearch(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                string value = null;
                int index = -1;
                if (TezLocalization.getName(key, out value, out index))
                {
                    this.hideAllItem();
                    if (m_SearchResult != null)
                    {
                        m_SearchResult.set(index);
                    }
                    else
                    {
                        m_SearchResult = Instantiate(m_Prefab, m_Content, false);
                        m_SearchResult.listArea = this;
                        m_SearchResult.set(index);
                        m_SearchResult.open();
                    }
                }

                m_PageGO.SetActive(false);
            }
        }

        private void onSearchClick(PointerEventData.InputButton button)
        {
            if (button == PointerEventData.InputButton.Left)
            {
                this.onSearch(m_SearchKey.text);
            }
        }

        private void onLoadPropertyClick(PointerEventData.InputButton button)
        {
            if (button == PointerEventData.InputButton.Left)
            {
                TezPropertyManager.foreachProperty((TezPropertyName name) =>
                {
                    TezLocalization.tryAddName(name.key_name, name.key_name);
                });

                this.dirty = true;
            }
        }

        public void onFocus(TezLocalizationNameItem item)
        {
            m_SelectItem = item;
            m_Vernier.gameObject.SetActive(true);
            m_Vernier.SetParent(m_SelectItem.transform, false);
            TezUILayout.setLayout(m_Vernier, -4, -4, 4, 4);
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

        public void edit(TezLocalizationNameItem item, int index)
        {
            var editor = this.window.createPopup(m_PrefabEditor);
            editor.set(index);
            editor.handleItem = item;
            editor.open();
        }

        private void createItem(int index, string name, string value)
        {
            var item = Instantiate(m_Prefab, m_Content, false);
            item.listArea = this;
            item.set(index);
            item.open();
            m_ItemList.Add(item);
        }
    }
}


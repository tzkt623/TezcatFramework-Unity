using System.Collections.Generic;
using tezcat.Core;
using tezcat.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezLocalizationNameList : TezArea
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
            m_PageController.setListener(this.onPageChanged);
            m_PageUp.onClick += onPageUpClick;
            m_PageDown.onClick += onPageDownClick;
            m_Page.onValueChanged.AddListener(this.onPageSet);
            m_Page.contentType = InputField.ContentType.IntegerNumber;
        }

        public override void clear()
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

            base.clear();
        }

        protected override void onRefresh()
        {
            m_PageController.calculateMaxPage(TezTranslater.nameCount);
            m_PageController.setPage(m_PageController.currentPage);
        }

        protected override void onShow()
        {

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

            TezTranslater.foreachName(this.createItem, begin, end);
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
            if(button == PointerEventData.InputButton.Left)
            {
                var editor = TezcatFramework.instance.createWidget<TezLocalizationNameEditor>("NameEditor", this.window.overlay);
                editor.listArea = this;
                editor.transform.SetAsLastSibling();
                editor.newItem();
                editor.open();
            }
        }

        private void onRemoveClick(PointerEventData.InputButton button)
        {
            if(button == PointerEventData.InputButton.Left)
            {
                if(TezTranslater.removeName(m_SelectItem.key))
                {
                    m_Vernier.SetParent(this.transform, false);
                    m_Vernier.gameObject.SetActive(false);
                    this.dirty = true;
                }
            }
        }

        private void onClearSearchClick(PointerEventData.InputButton button)
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
                if (TezTranslater.translateName(key, out value))
                {
                    if (m_SearchResult != null)
                    {
                        m_SearchResult.set(key, value);
                    }
                    else
                    {
                        m_SearchResult = TezcatFramework.instance.createWidget<TezLocalizationNameItem>("NameItem", m_Content);
                        m_SearchResult.listArea = this;
                        m_SearchResult.set(key, value);
                        m_SearchResult.open();
                    }
                }

                this.hideAllItem();
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
                    TezTranslater.tryAddName(name.name, name.name);
                });

                this.dirty = true;
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
            var editor = TezcatFramework.instance.createWidget<TezLocalizationNameEditor>("NameEditor", this.window.overlay);
            editor.listArea = this;
            editor.transform.SetAsLastSibling();
            editor.set(key);
            editor.open();
        }

        private void createItem(string key, string value)
        {
            var item = TezcatFramework.instance.createWidget<TezLocalizationNameItem>("NameItem", m_Content);
            item.listArea = this;
            item.set(key, value);
            item.open();

            m_ItemList.Add(item);
        }
    }
}


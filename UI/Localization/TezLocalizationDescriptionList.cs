using System.Collections.Generic;
using tezcat.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezLocalizationDescriptionList : TezArea
    {
        [SerializeField]
        TezLocalizationDescriptionItem m_Prefab = null;
        [SerializeField]
        TezLocalizationDescriptionEditor m_PrefabEditor = null;

        [SerializeField]
        RectTransform m_Content = null;

        [SerializeField]
        TezImageLabelButton m_Add = null;
        [SerializeField]
        TezImageLabelButton m_Remove = null;

        [SerializeField]
        TezImageLabelButton m_Search = null;
        [SerializeField]
        TezImageLabelButton m_ClearSearch = null;
        [SerializeField]
        InputField m_SearchKey = null;

        TezLocalizationDescriptionItem m_SearchResult = null;
        List<TezLocalizationDescriptionItem> m_ItemList = new List<TezLocalizationDescriptionItem>();

        protected override void Awake()
        {
            base.Awake();

            m_Add.onClick += onAddClick;
            m_Remove.onClick += onRemoveClick;

            m_Search.onClick += onSearchClick;
            m_ClearSearch.onClick += onClearSearchClick;
        }

        protected override void Start()
        {
            base.Start();
            this.dirty = true;
        }

        protected override void onRefresh()
        {
            foreach (var item in m_ItemList)
            {
                item.close();
            }
            m_ItemList.Clear();

            TezLocalization.foreachDescription(this.createItem);
        }

        private void onAddClick(PointerEventData.InputButton button)
        {
            var editor = this.window.createPopup(m_PrefabEditor);
            editor.newItem();
            editor.open();
        }

        private void onRemoveClick(PointerEventData.InputButton button)
        {

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
        }

        private void onSearchClick(PointerEventData.InputButton button)
        {
            if (button == PointerEventData.InputButton.Left)
            {
                var key = m_SearchKey.text;
                if (!string.IsNullOrEmpty(key))
                {
                    string value = null;
                    int index = -1;
                    if (TezLocalization.getDescription(key, out value, out index))
                    {
                        this.hideAllItem();
                        m_SearchResult = Instantiate(m_Prefab, m_Content, false);
                        m_SearchResult.set(index);
                        m_SearchResult.list = this;
                        m_SearchResult.open();
                    }
                }
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

        private void createItem(int index, string name, string value)
        {
            var item = Instantiate(m_Prefab, m_Content, false);
            item.set(index);
            item.list = this;
            item.open();
            m_ItemList.Add(item);
        }

        public void editItem(int index)
        {
            var editor = this.window.createPopup(m_PrefabEditor);
            editor.set(index);
            editor.open();
        }
    }
}


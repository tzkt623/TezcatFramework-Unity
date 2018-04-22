using UnityEngine;
using System.Collections.Generic;

using tezcat.Utility;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace tezcat.UI
{
    public class TezLocalizationNameList : TezArea
    {
        [SerializeField]
        TezLocalizationNameItem m_Prefab = null;
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

        TezLocalizationNameItem m_SearchResult = null;

        List<TezLocalizationNameItem> m_ItemList = new List<TezLocalizationNameItem>();

        protected override void Awake()
        {
            base.Awake();

            m_Add.onClick += onAddClick;
            m_Remove.onClick += onRemoveClick;

            m_SearchKey.onEndEdit.AddListener(this.onSearch);

            m_Search.onClick += onSearchClick;
            m_ClearSearch.onClick += onClearSearchClick;
        }



        private void onAddClick(PointerEventData.InputButton button)
        {

        }

        private void onRemoveClick(PointerEventData.InputButton button)
        {

        }

        private void onClearSearchClick(PointerEventData.InputButton button)
        {
            if(m_SearchResult != null)
            {
                m_SearchResult.close();
                m_SearchResult = null;
                this.showAllItem();
            }
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
                    m_SearchResult = Instantiate(m_Prefab, m_Content, false);
                    m_SearchResult.set(index);
                    m_SearchResult.open();
                }
            }
        }

        private void onSearchClick(PointerEventData.InputButton button)
        {
            if(button == PointerEventData.InputButton.Left)
            {
                this.onSearch(m_SearchKey.text);
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

        protected override void onRefresh()
        {
            foreach (var item in m_ItemList)
            {
                item.close();
            }
            m_ItemList.Clear();

            TezLocalization.foreachName(this.createItem);
        }

        private void createItem(int index, string name, string value)
        {
            var item = Instantiate(m_Prefab, m_Content, false);
            item.set(index);
            item.open();
            m_ItemList.Add(item);
        }
    }
}


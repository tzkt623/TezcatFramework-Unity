using System.Collections.Generic;
using tezcat.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezLocalizationNameList : TezArea
    {
        [SerializeField]
        TezLocalizationNameItem m_Prefab = null;
        [SerializeField]
        TezLocalizationNameEditor m_PrefabEditor = null;

        [SerializeField]
        RectTransform m_Content = null;
        [SerializeField]
        RectTransform m_Vernier = null;


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
        TezLocalizationNameItem m_SelectItem = null;

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

            TezLocalization.foreachName(this.createItem);
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
                    if(m_SearchResult != null)
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
            }
        }

        private void onSearchClick(PointerEventData.InputButton button)
        {
            if (button == PointerEventData.InputButton.Left)
            {
                this.onSearch(m_SearchKey.text);
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


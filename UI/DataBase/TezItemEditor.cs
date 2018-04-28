﻿using System;
using tezcat.DataBase;
using tezcat.Utility;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.UI
{
    public class TezItemEditor : TezBaseItemEditor
    {
        [Header("Prefab")]
        [SerializeField]
        TezPE_IFS m_PrefabPE = null;
        [SerializeField]
        TezPropertyView m_PrefabPV = null;

        [Header("Widget")]
        [SerializeField]
        TezImageLabelButton m_Confirm = null;
        [SerializeField]
        TezImageLabelButton m_Cancel = null;
        [SerializeField]
        TezImageLabelButton m_Save = null;

        [SerializeField]
        RectTransform m_Content = null;

        public override TezDatabase.CategoryType[] categoryTypes
        {
            get { throw new NotImplementedException(); }
        }

        TezItem m_NewItem = null;

        protected override void Awake()
        {
            base.Awake();

            m_Confirm.onClick += onConfirmClick;
            m_Cancel.onClick += onCancelClick;
            m_Save.onClick += onSaveClick;
        }

        protected override void Start()
        {
            base.Start();
            this.dirty = true;
        }

        private void onCancelClick(PointerEventData.InputButton button)
        {
            if (button == PointerEventData.InputButton.Left)
            {
                this.close();
            }
        }

        private void onConfirmClick(PointerEventData.InputButton button)
        {
            if (button == PointerEventData.InputButton.Left)
            {

            }
        }

        private void onSaveClick(PointerEventData.InputButton button)
        {
            if (button == PointerEventData.InputButton.Left)
            {
                TezDatabase.instance.registerInnateItem(m_NewItem);
                this.dirty = true;
            }
        }

        public override void bind(TezDatabase.CategoryType category_type)
        {
            m_NewItem = category_type.create();
            this.dirty = true;
        }

        private TezPropertyView createPV(string name, string value)
        {
            var pv = Instantiate(m_PrefabPV, m_Content, false);
            pv.setInfo(name, value);
            return pv;
        }

        private TezPE_IFS createPE(TezPropertyValue property)
        {
            var pe = Instantiate(m_PrefabPE, m_Content, false);
            pe.bind(property);
            return pe;
        }

        protected override void onRefresh()
        {
            foreach (RectTransform item in m_Content)
            {
                Destroy(item.gameObject);
            }

            if (m_NewItem != null)
            {
                var view = this.createPV(
                    TezLocalization.getName(TezReadOnlyString.Database.object_id, TezReadOnlyString.Database.object_id),
                    m_NewItem.objectID.ToString());
                view.open();

                view = this.createPV(
                    TezLocalization.getName(TezReadOnlyString.Database.GUID, TezReadOnlyString.Database.GUID),
                    m_NewItem.GUID.ToString());
                view.open();


                var properties = m_NewItem.properties;
                for (int i = 0; i < properties.Count; i++)
                {
                    var property = properties[i];
                    var editor = this.createPE(property);
                    editor.open();
                }
            }
        }
    }
}


using System.Collections.Generic;
using tezcat.Core;
using tezcat.DataBase;
using UnityEngine;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezTip
        : TezUIWidget
        , ITezTip
        , ITezPrefab
    {
        [SerializeField]
        Text m_Text = null;

        protected string m_ItemName;
        protected string m_Description;

        protected string m_Group;
        protected string m_Type;

        protected List<string> m_AttributeList = new List<string>();

        bool m_NeedName = false;
        bool m_NeedGroup = false;
        bool m_NeedType = false;

        bool m_NeedAttribute = false;
        bool m_NeedDescription = false;

        [SerializeField]
        int m_NameSize;
        [SerializeField]
        Color m_NameColor;

        [SerializeField]
        int m_GroupSize;
        [SerializeField]
        Color m_GroupColor;

        [SerializeField]
        int m_TypeSize;
        [SerializeField]
        Color m_TypeColor;

        [SerializeField]
        int m_AttributeTitleSize;
        [SerializeField]
        Color m_AttributeTitleColor;

        [SerializeField]
        int m_AttributeContentSize;
        [SerializeField]
        Color m_AttributeContentColor;

        [SerializeField]
        int m_AttributeSeparatorContentSize;
        [SerializeField]
        Color m_AttributeSeparatorContentColor;

        [SerializeField]
        int m_DescriptionSize;
        [SerializeField]
        Color m_DescriptionColor;

        RectTransform m_RectTransform = null;
        Vector2 m_Pivot = new Vector2(0, 1);

        protected override void initWidget()
        {
            base.initWidget();
            m_RectTransform = this.GetComponent<RectTransform>();
            this.gameObject.SetActive(false);
        }

        public void accept(TezTipController controller)
        {
            controller.nameSize = m_NameSize;
            controller.nameColor = m_NameColor;

            controller.groupSize = m_GroupSize;
            controller.groupColor = m_GroupColor;

            controller.typeSize = m_TypeSize;
            controller.typeColor = m_TypeColor;

            controller.attributeTitleSize = m_AttributeTitleSize;
            controller.attributeTitleColor = m_AttributeTitleColor;

            controller.attributeContentSize = m_AttributeContentSize;
            controller.attributeContentColor = m_AttributeContentColor;

            controller.attributeSeparatorSize = m_AttributeSeparatorContentSize;
            controller.attributeSeparatorColor = m_AttributeSeparatorContentColor;

            controller.descriptionSize = m_DescriptionSize;
            controller.descriptionColor = m_DescriptionColor;

            controller.setTip(this);
        }

        /*
         * (0, 1080)-------------(1920, 1080)
         * 
         * 
         * 
         * 
         * (0,    0)-------------(1920, 0)
         */
        void ITezTip.onShow(string tip)
        {
            m_Text.text = tip;
            this.open();
        }

        void ITezTip.onHide()
        {
            this.hide();
        }

        private void Update()
        {
            var position = Input.mousePosition;

            var rect = m_RectTransform.rect;
            var width = rect.width;
            var height = rect.height;

            if (position.x + width >= Screen.width)
            {
                m_Pivot.x = 1;
                position.x -= 8;
            }
            else
            {
                m_Pivot.x = 0;
                position.x += 32;
            }

            if (position.y - height <= 0)
            {
                m_Pivot.y = 0;
                position.y += 8;
            }
            else
            {
                m_Pivot.y = 1;
                position.y -= 24;
            }

            m_RectTransform.pivot = m_Pivot;
            m_RectTransform.position = position;
        }
    }
}
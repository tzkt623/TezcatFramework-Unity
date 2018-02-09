using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

namespace tezcat
{

    public class TezImageButton : TezButton
    {
        public event TezEventBus.Action<PointerEventData.InputButton> onClick;

        [SerializeField]
        Image m_Image;
        [SerializeField]
        Color m_PressColor;
        Color m_OrgColor;

        protected override void onAwake()
        {

        }

        protected override void onStart()
        {
            m_OrgColor = m_Image.color;
        }

        protected override void onDelete()
        {

        }

        protected override void onDisable()
        {

        }

        protected override void onEnable()
        {

        }

        protected override void onEnter(PointerEventData eventData)
        {
            m_Image.DOColor(new Color(1, 1, 1, 60 / 255.0f), 1.2f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetAutoKill(false);
        }

        protected override void onExit(PointerEventData eventData)
        {
            m_Image.DORewind(false);
            m_Image.DOKill();
        }

        protected override void onDown(PointerEventData eventData)
        {
            m_Image.DORewind(false);
            m_Image.DOKill();

            m_Image.color = m_PressColor;
        }

        protected override void onUp(PointerEventData eventData)
        {
            m_Image.color = m_OrgColor;
            onClick.launch(eventData.button);
        }
    }
}
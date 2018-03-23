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


        protected override void Start()
        {
            base.Start();
            m_OrgColor = m_Image.color;
        }

        public override void clear()
        {

        }

        protected override void onInteractable(bool value)
        {
            if(value)
            {
                m_Image.color = m_OrgColor;
            }
            else
            {
                m_Image.color = Color.gray;
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            m_Image.DOColor(new Color(1, 1, 1, 60 / 255.0f), 1.2f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetAutoKill(false);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            m_Image.DORewind(false);
            m_Image.DOKill();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            m_Image.DORewind(false);
            m_Image.DOKill();

            m_Image.color = m_PressColor;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            m_Image.color = m_OrgColor;
            onClick.launch(eventData.button);
        }
    }
}
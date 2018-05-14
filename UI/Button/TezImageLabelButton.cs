using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezImageLabelButton
        : TezButton
    {
        public event TezEventBus.Action<PointerEventData.InputButton> onClick;

        [SerializeField]
        Image m_BG = null;
        [SerializeField]
        TezText m_Label = null;
        [SerializeField]
        Color m_PressColor;

        Color m_LabelColor;
        Tweener m_Tweener = null;

        public string text
        {
            get { return m_Label.text; }
            set { m_Label.text = value; }
        }

        public Sprite bg
        {
            get { return m_BG.sprite; }
            set { m_BG.sprite = value; }
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            this.dirty = true;
        }

        protected override void onInteractable(bool value)
        {
            if(value)
            {
                m_Label.color = m_LabelColor;
            }
            else
            {
                m_Label.color = Color.gray;
            }
        }

        protected override void clear()
        {

        }

        protected override void onRefresh()
        {
            m_Label.dirty = true;
            m_LabelColor = m_Label.color;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            m_Label.color = m_PressColor;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    m_Label.color = m_LabelColor;
                    onClick.Invoke(eventData.button);
                    break;
                case PointerEventData.InputButton.Right:
                    break;
                case PointerEventData.InputButton.Middle:
                    break;
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            m_Tweener = m_BG.DOColor(ShipProject.Colors.button_hover, 1.2f);
            m_Tweener.SetAutoKill(false).SetLoops(-1, LoopType.Yoyo);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            m_Tweener.Rewind();
            m_Tweener.Kill();
        }
    }
}
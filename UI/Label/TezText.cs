using System.Collections.Generic;
using tezcat.Extension;
using UnityEngine;
using UnityEngine.UI;


namespace tezcat.UI
{
    [RequireComponent(typeof(Text))]
    public class TezText : TezWidget
    {
        #region Manager
        static LinkedList<TezText> m_Actived = new LinkedList<TezText>();
        static void add(TezText text)
        {
            m_Actived.AddLast(text.m_Node);
        }

        static void remove(TezText text)
        {
            m_Actived.Remove(text.m_Node);
        }

        public static void refreshAllText()
        {
            foreach (var item in m_Actived)
            {
                item.refreshAfterInit();
            }
        }
        #endregion

        LinkedListNode<TezText> m_Node = null;

        TezEventExtension.Function<string> m_Getter = null;
        public Text handler { get; private set; } = null;

        public Color color
        {
            get { return handler.color; }
            set { handler.color = value; }
        }

        public string text
        {
            get { return handler.text; }
            set
            {
                if (!handler)
                {
                    m_Getter = () => value;
                }
                else
                {
                    handler.text = value;
                }
            }
        }

        protected override void onInteractable(bool value)
        {
            handler.raycastTarget = value;
        }

        protected override void preInit()
        {
            handler = this.GetComponent<Text>();
            if (m_Getter == null)
            {
                m_Getter = () => handler.text;
            }
        }

        protected override void initWidget()
        {
            m_Node = new LinkedListNode<TezText>(this);
            add(this);
        }

        protected override void linkEvent()
        {

        }

        protected override void unLinkEvent()
        {

        }

        public void setGetter(TezEventExtension.Function<string> getter)
        {
            m_Getter = getter;
            this.refresh = RefreshPhase.System1;
        }

        protected override void onRefresh(RefreshPhase phase)
        {
            switch (phase)
            {
                case RefreshPhase.System1:
                    break;
                case RefreshPhase.System2:
                    break;
                case RefreshPhase.Custom1:
                    break;
                case RefreshPhase.Custom2:
                    break;
                case RefreshPhase.Custom3:
                    break;
                case RefreshPhase.Custom4:
                    break;
                case RefreshPhase.Custom5:
                    break;
                default:
                    break;
            }
        }

        protected override void refreshAfterInit()
        {
            handler.text = m_Getter();
        }

        public override void clear()
        {
            m_Getter = null;
            handler = null;
            m_Node = null;
        }

        protected override void onOpenAndRefresh()
        {
            add(this);
        }

        protected override void onHide()
        {
            remove(this);
        }

        public override void reset()
        {

        }
    }
}
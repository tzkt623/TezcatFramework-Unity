using System.Collections.Generic;
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
                item.onRefresh();
            }
        }
        #endregion

        LinkedListNode<TezText> m_Node = null;

        TezEventBus.Function<string> m_Getter = null;
        public Text handler { get; private set; } = null;

        public Color color
        {
            get { return handler.color; }
            set { handler.color = value; }
        }

        public string text
        {
            get { return handler.text; }
            set { handler.text = value; }
        }

        protected override void onInteractable(bool value)
        {

        }

        protected override void preInit()
        {
            handler = this.GetComponent<Text>();
            if(m_Getter == null)
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

        public void setGetter(TezEventBus.Function<string> getter)
        {
            m_Getter = getter;
            this.dirty = true;
        }

        protected override void onRefresh()
        {
            handler.text = m_Getter();
        }

        public override void clear()
        {
            m_Getter = null;
            handler = null;
            m_Node = null;
        }

        protected override void onShow()
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
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;
using UnityEngine;
using UnityEngine.UI;


namespace tezcat.Framework.UI
{
    [RequireComponent(typeof(Text))]
    public class TezText : TezUIWidget
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
                item.refreshData();
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

        public void setGetter(TezEventExtension.Function<string> getter)
        {
            m_Getter = getter;
            this.refreshMask = true;
        }

        protected override void onRefresh()
        {
            this.refreshData();
        }

        private void refreshData()
        {
            handler.text = m_Getter();
        }

        protected override void onClose(bool self_close)
        {
            m_Getter = null;
            handler = null;
            m_Node = null;
        }

        protected override void onShow()
        {
            TezText.add(this);
        }

        protected override void onHide()
        {
            TezText.remove(this);
        }

        public override void reset()
        {

        }
    }
}
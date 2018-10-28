using System.Collections.Generic;
using tezcat.Framework.Extension;
using UnityEngine;
using UnityEngine.UI;

namespace tezcat.Framework.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class TezTree : TezWidget
    {
        public event TezEventExtension.Action<TezTreeNode> onSelectNode;

        [SerializeField]
        TezTreeNode m_Prefab = null;
        public TezTreeNode prefab
        {
            get { return m_Prefab; }
        }

        [SerializeField]
        Sprite m_FlagOn = null;
        public Sprite flagOn { get { return m_FlagOn; } }

        [SerializeField]
        Sprite m_FlagOff = null;
        public Sprite flagOff { get { return m_FlagOff; } }


        [SerializeField]
        float m_VerticalOffset = 0;
        public float verticalOffset
        {
            get { return m_VerticalOffset; }
        }
        [SerializeField]
        float m_HorizontalOffset = 0;
        public float horizontalOffset
        {
            get { return m_HorizontalOffset; }
        }

        ScrollRect m_ScrollRect = null;
        List<TezTreeNode> m_Children = new List<TezTreeNode>();

        protected override void preInit()
        {
            m_ScrollRect = this.GetComponent<ScrollRect>();
        }

        protected override void initWidget()
        {

        }

        public override void reset()
        {
            foreach (var child in m_Children)
            {
                child.close();
            }
            m_Children.Clear();
        }

        protected override void linkEvent()
        {

        }

        protected override void unLinkEvent()
        {

        }

        public bool tryGet(ITezTreeData data, out TezTreeNode node)
        {
            for (int i = 0; i < m_Children.Count; i++)
            {
                if(m_Children[i].data.isEqual(data))
                {
                    node = m_Children[i];
                    return true;
                }
            }

            node = null;
            return false;
        }

        public TezTreeNode addData(ITezTreeData data)
        {
            var index = m_Children.Count;

            var child = Instantiate(this.prefab, m_ScrollRect.content);
            child.index = index;
            child.tree = this;
            child.setData(data);
            child.setParent(null);
            child.setPosition(index);
            m_Children.Add(child);


            child.gameObject.SetActive(true);

            this.addSpace(index, child.length + m_VerticalOffset);

            return child;
        }

        public void removeData(ITezTreeData data)
        {

        }

        public void addSpace(int index, float length)
        {
            for (int i = index + 1; i < m_Children.Count; i++)
            {
                var child = m_Children[i];
                var pos = child.rectTransform.anchoredPosition;
                pos.y -= length;
                child.rectTransform.anchoredPosition = pos;
            }

            m_ScrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_ScrollRect.content.rect.height + length);
        }

        public void removeSpace(int index, float length)
        {
            for (int i = index + 1; i < m_Children.Count; i++)
            {
                var child = m_Children[i];
                var pos = child.rectTransform.anchoredPosition;
                pos.y += length;
                child.rectTransform.anchoredPosition = pos;
            }

            m_ScrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_ScrollRect.content.rect.height - length);
        }

        public override void clear()
        {
            foreach (var child in m_Children)
            {
                child.close();
            }
            m_Children.Clear();
            m_Children = null;
        }

        public void selectNode(TezTreeNode node)
        {
            onSelectNode?.Invoke(node);
        }

        protected override void onRefresh(RefreshPhase phase)
        {

        }

        protected override void onHide()
        {

        }
    }
}
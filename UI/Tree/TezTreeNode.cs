using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{

    public class TezTreeNode
        : TezUINodeMB
        , IPointerDownHandler
        , IPointerUpHandler
    {
        List<TezTreeNode> m_Children = new List<TezTreeNode>();

        [SerializeField]
        Text m_Tag = null;
        Image m_BG = null;

        public int index { get; set; } = -1;

        public bool isRoot
        {
            get { return parent == null; }
        }

        public bool isLeaf
        {
            get { return m_Children.Count == 0; }
        }

        public int level { get; private set; } = 0;

        public string text
        {
            get { return m_Tag.text; }
        }
        public bool isOn { get; private set; } = false;

        public TezTreeNode parent { get; private set; } = null;
        public ITezTreeData data { get; private set; } = null;

        public RectTransform rectTransform
        {
            get { return (RectTransform)this.transform; }
        }

        public float length
        {
            get { return this.rectTransform.rect.height; }
        }

        public TezTree tree { get; set; } = null;

        protected override void Awake()
        {
            base.Awake();
            m_BG = this.GetComponent<Image>();
        }

        public void setPosition(int index)
        {
            this.rectTransform.anchoredPosition = new Vector2(this.level * tree.horizontalOffset, -index * (this.length + tree.verticalOffset));
        }

        protected override void Start()
        {
            base.Start();
            this.dirty = true;
        }

        public void setData(ITezTreeData data)
        {
            m_Tag.text = data.dataName;
            this.data = data;
        }

        public void setParent(TezTreeNode node)
        {
            parent = node;
            if (parent == null)
            {
                this.level = 0;
            }
            else
            {
                level = parent.level + 1;
            }
        }

        /// <summary>
        /// 展开
        /// 遍历查找打开状态的子节点
        /// 计算空间
        /// </summary>
        /// <returns>需要增加的空间</returns>
        private float calculateChildrenLength(bool avtive)
        {
            float result = 0;
            foreach (var child in m_Children)
            {
                child.gameObject.SetActive(avtive);
                result += child.length + tree.verticalOffset;
                if (child.isOn)
                {
                    result += child.calculateChildrenLength(avtive);
                }
            }

            return result;
        }

        private int getTopLevelIndex()
        {
            if (this.level > 0)
            {
                return parent.getTopLevelIndex();
            }

            return this.index;
        }

        public TezTreeNode addData(ITezTreeData data)
        {
            var index = m_Children.Count;
            var child = Instantiate(tree.prefab, this.rectTransform);
            child.index = index;
            child.tree = this.tree;
            child.setParent(this);
            child.setPosition(index + 1);
            child.setData(data);
            child.gameObject.SetActive(this.isOn);
            m_Children.Add(child);

            if (this.isOn)
            {
                tree.addSpace(this.getTopLevelIndex(), this.length + tree.verticalOffset);
            }

            return child;
        }

        public override void clear()
        {
            foreach (var item in m_Children)
            {
                Destroy(item.gameObject);
            }

            m_Children.Clear();
        }

        protected override void onRefresh()
        {

        }

        public void foldOrUnfold()
        {
            this.isOn = !this.isOn;
            var length = this.calculateChildrenLength(this.isOn);
            if (this.isOn)
            {
                this.tree.addSpace(this.getTopLevelIndex(), length);
            }
            else
            {
                this.tree.removeSpace(this.getTopLevelIndex(), length);
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {

            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                tree.selectNode(this);
            }
        }
    }
}
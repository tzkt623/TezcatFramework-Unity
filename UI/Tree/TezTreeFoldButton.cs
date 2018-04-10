using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezTreeFoldButton
        : UIBehaviour
        , IPointerUpHandler
        , IPointerDownHandler
    {
        Image m_Flag = null;

        TezTreeNode m_Node = null;

        protected override void Start()
        {
            base.Start();
            m_Node = this.transform.parent.GetComponent<TezTreeNode>();
            m_Flag = this.GetComponent<Image>();

            if(m_Node.isLeaf)
            {
                m_Flag.gameObject.SetActive(false);
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {

        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (!m_Node.isLeaf)
            {
                m_Node.foldOrUnfold();
                if (m_Node.isOn)
                {
                    m_Flag.sprite = m_Node.tree.flagOn;
                }
                else
                {
                    m_Flag.sprite = m_Node.tree.flagOff;
                }
            }
        }
    }
}
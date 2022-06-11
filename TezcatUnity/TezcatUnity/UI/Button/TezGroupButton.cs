using UnityEngine;
using UnityEngine.UI;

namespace tezcat.Framework.UI
{
    public class TezGroupButton : MonoBehaviour
    {
        [SerializeField]
        Image m_Flage = null;

        public void showFlag()
        {
            m_Flage.gameObject.SetActive(true);
        }

        public void hideFlag()
        {
            m_Flage.gameObject.SetActive(false);
        }
    }
}

using tezcat.DataBase;
using UnityEngine;

namespace tezcat.UI
{
    public class TezDatabaseWindow : TezWindow
    {
        [Header("Item Pool")]
        RectTransform m_Pool = null;

        public TezDatabase.GroupType selectGroup { get; set; }
        public TezDatabase.CategoryType selectCategory { get; set; }


        protected override void Start()
        {
            base.Start();
        }

        protected override void onRefresh()
        {
            base.onRefresh();
        }
    }
}
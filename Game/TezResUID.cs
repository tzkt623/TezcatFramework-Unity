namespace tezcat
{
    public class TezResUID
    {


        int m_GroupID = -1;
        int m_TypeID = -1;
        int m_ObjectID = -1;

        public int group_id
        {
            get { return m_GroupID; }
        }

        public int type_id
        {
            get { return m_TypeID; }
        }

        public int object_id
        {
            get { return m_ObjectID; }
        }

        public bool invalid
        {
            get { return m_GroupID == -1 || m_TypeID == -1 || m_ObjectID == -1; }
        }

        public void setGroupID(int group_id)
        {
            m_GroupID = group_id;
        }

        public void setTypeID(int type_id)
        {
            m_TypeID = type_id;
        }

        public void setObjectID(int object_id)
        {
            m_ObjectID = object_id;
        }
    }
}
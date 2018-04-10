namespace tezcat.UI
{
    public interface ITezTreeData
    {
        string dataName { get; }
        System.Type GetType();
        void onChoose();
        bool isEqual(ITezTreeData other);
    }

    public class TezTreeData : ITezTreeData
    {
        public string dataName { get; private set; }

        public TezTreeData(string name)
        {
            this.dataName = name;
        }

        public void onChoose()
        {

        }

        public bool isEqual(ITezTreeData data)
        {
            return data.dataName == this.dataName;
        }
    }
}
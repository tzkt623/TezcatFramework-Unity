namespace tezcat.Framework.Core
{
    public class TezBuildInName
    {
        #region ObjectInfo
        /// <summary>
        /// StackCount
        /// </summary>
        public const string ObjectInfo = "ObjectInfo";

        /// <summary>
        /// Class ID
        /// </summary>
        public const string CID = "CID";
        public const string ClassName = "ClassName";
        public const string ProtoIndex = "ProtoIndex";
        public const string ProtoName = "ProtoName";
        public const string IsProto = "IsProto";

        public const string Name = "Name";
        public const string Type = "Type";


        /// <summary>
        /// Tag
        /// </summary>
        public const string TAG = "TAG";

        /// <summary>
        /// 分类类型
        /// </summary>
        public const string Category = "Category";

        /// <summary>
        /// VersionID
        /// </summary>
        public const string Version = "Version";
        #endregion


        public static class SaveChunkName
        {
            public const string ObjectData = "ObjectData";
            public const string ProtoInfo = "ProtoInfo";
        }

        public static class ProtoInfo
        {
            public const string Name = "Name";

            public const string TID = "TID";
            public const string IID = "IID";
            public const string RDID = "RDID";
            public const string RTID = "RTID";
            public const string DBID = "DBID";

            public const string SharedType = "SharedType";
            public const string StackCount = "StackCount";
        }
    }
}
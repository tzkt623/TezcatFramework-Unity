using tezcat.String;

namespace tezcat
{
    public class TezAsset
    {
        public TezStaticString iconNormal { get; set; }
        public TezStaticString iconSamll { get; set; }
        public TezStaticString iconMiddle { get; set; }
        public TezStaticString iconLarge { get; set; }

        public void clear()
        {
            iconNormal = null;
            iconSamll = null;
            iconMiddle = null;
            iconLarge = null;
        }
    }
}
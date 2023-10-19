namespace tezcat.Framework.Game
{
    public abstract class TezTranslationSlot
    {
        public enum Category
        {
            AllInOne,
            Single
        }

        public abstract Category category { get; }
        public abstract void initContent(int count);
        public abstract string getContent(int index);
        public abstract void setContent(int index, string value);
    }

    public class TezTranslationSlot_AllInOne : TezTranslationSlot
    {
        string[] mContents;

        public override Category category => Category.AllInOne;

        public override string getContent(int index)
        {
            return mContents[index];
        }

        public override void initContent(int count)
        {
            mContents = new string[count];
        }

        public override void setContent(int index, string value)
        {
            mContents[index] = value;
        }
    }

    public class TezTranslationSlot_Single : TezTranslationSlot
    {
        string mContent;

        public override Category category => Category.Single;

        public override string getContent(int index)
        {
            return mContent;
        }

        public override void initContent(int count)
        {

        }

        public override void setContent(int index, string value)
        {
            mContent = value;
        }
    }
}
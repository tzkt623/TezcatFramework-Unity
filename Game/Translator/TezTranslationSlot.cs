using System;
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
        string[] m_Contents;

        public override Category category => Category.AllInOne;

        public override string getContent(int index)
        {
            return m_Contents[index];
        }

        public override void initContent(int count)
        {
            m_Contents = new string[count];
        }

        public override void setContent(int index, string value)
        {
            this.m_Contents[index] = value;
        }
    }

    public class TezTranslationSlot_Single : TezTranslationSlot
    {
        string m_Content;

        public override Category category => Category.Single;

        public override string getContent(int index)
        {
            return m_Content;
        }

        public override void initContent(int count)
        {

        }

        public override void setContent(int index, string value)
        {
            this.m_Content = value;
        }
    }
}
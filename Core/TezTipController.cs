using System.Text;
using tezcat.String;
using UnityEngine;

namespace tezcat.Core
{
    public interface ITezTip
    {
        void accept(TezTipController controller);
        void onShow(string content);
        void onHide();
    }

    public class TezTipController : ITezService
    {
        StringBuilder m_Builder = new StringBuilder();

        ITezTip m_Tip = null;
        bool m_IsTipShowed = false;

        int m_NameSize;
        public int nameSize
        {
            get { return m_NameSize; }
            set { m_NameSize = value; }
        }
        Color m_NameColor;
        public Color nameColor
        {
            get { return m_NameColor; }
            set { m_NameColor = value; }
        }

        int m_GroupSize;
        public int groupSize
        {
            get { return m_GroupSize; }
            set { m_GroupSize = value; }
        }
        Color m_GroupColor;
        public Color groupColor
        {
            get { return m_GroupColor; }
            set { m_GroupColor = value; }
        }

        int m_TypeSize;
        public int typeSize
        {
            get { return m_TypeSize; }
            set { m_TypeSize = value; }
        }
        Color m_TypeColor;
        public Color typeColor
        {
            get { return m_TypeColor; }
            set { m_TypeColor = value; }
        }

        int m_AttributeTitleSize;
        public int attributeTitleSize
        {
            get { return m_AttributeTitleSize; }
            set { m_AttributeTitleSize = value; }
        }
        Color m_AttributeTitleColor;
        public Color attributeTitleColor
        {
            get { return m_AttributeTitleColor; }
            set { m_AttributeTitleColor = value; }
        }

        int m_AttributeContentSize;
        public int attributeContentSize
        {
            get { return m_AttributeContentSize; }
            set { m_AttributeContentSize = value; }
        }
        Color m_AttributeContentColor;
        public Color attributeContentColor
        {
            get { return m_AttributeContentColor; }
            set { m_AttributeContentColor = value; }
        }

        int m_AttributeSeparatorContentSize;
        public int attributeSeparatorSize
        {
            get { return m_AttributeSeparatorContentSize; }
            set { m_AttributeSeparatorContentSize = value; }
        }
        Color m_AttributeSeparatorContentColor;
        public Color attributeSeparatorColor
        {
            get { return m_AttributeSeparatorContentColor; }
            set { m_AttributeSeparatorContentColor = value; }
        }

        int m_DescriptionSize;
        public int descriptionSize
        {
            get { return m_DescriptionSize; }
            set { m_DescriptionSize = value; }
        }
        Color m_DescriptionColor;
        public Color descriptionColor
        {
            get { return m_DescriptionColor; }
            set { m_DescriptionColor = value; }
        }

        int m_NameLength = 0;
        int m_GroupLength = 0;
        int m_TypeLength = 0;
        int m_DescriptionLength = 0;
        int m_OtherIndex = 0;

        #region Formatter
        const string m_FormatContent = "<color=#{0}><size={1}>{2}</size></color>";
        const string m_FormatContentWithFlag = "<color=#{0}><size={1}>{2}</size></color>{3}";
        const string m_FormatContentWithFlagAndLink = "<color=#{0}><size={1}>{2}</size></color>{3}{4}";
        const string m_FlagEnd = "\n";
        #endregion

        public void setTip(ITezTip tip)
        {
            m_Tip = tip;
        }

        public void show()
        {
            var tip = generateResult();
            if (string.IsNullOrEmpty(tip))
            {
                return;
            }

            m_IsTipShowed = true;
            m_Tip.onShow(tip);
        }

        public void hide()
        {
            if(m_IsTipShowed)
            {
                m_IsTipShowed = false;
                m_Tip.onHide();
            }
        }

        private string format(string content, ref int size, ref Color color)
        {
            return string.Format(m_FormatContent, ColorUtility.ToHtmlStringRGBA(color), size.ToString(), content);
        }

        private string format(string content, ref int size, ref Color color, string flag)
        {
            return string.Format(m_FormatContentWithFlag, ColorUtility.ToHtmlStringRGBA(color), size.ToString(), content, flag);
        }

        private string format(string content, ref int size, ref Color color, string flag, string link_content)
        {
            return string.Format(m_FormatContentWithFlagAndLink, ColorUtility.ToHtmlStringRGBA(color), size.ToString(), content, flag, link_content);
        }

        public TezTipController setName(string name)
        {
            var content = this.format(name, ref m_NameSize, ref m_NameColor, m_FlagEnd);
            m_NameLength = content.Length;
            m_Builder.Insert(0, content);
            return this;
        }

        public TezTipController setGroup(string group)
        {
            var content = this.format(group, ref m_GroupSize, ref m_GroupColor, m_FlagEnd);
            m_GroupLength = content.Length;
            m_Builder.Insert(m_NameLength, content);
            return this;
        }

        public TezTipController setType(string type)
        {
            var content = this.format(type, ref m_TypeSize, ref m_TypeColor, m_FlagEnd);
            m_TypeLength = content.Length;
            m_Builder.Insert(m_NameLength + m_GroupLength, content);
            return this;
        }

        public TezTipController setDescription(string description)
        {
            var content = format(description, ref m_DescriptionSize, ref m_DescriptionColor, m_FlagEnd);
            m_DescriptionLength = content.Length;
            m_Builder.Insert(m_NameLength + m_GroupLength + m_TypeLength, content);
            return this;
        }

        public TezTipController pushAttribute(string title, bool attribute)
        {
            this.pushAttribute(title, attribute.ToString());
            return this;
        }

        public TezTipController pushAttribute(string title, string attribute)
        {
            var content = format(title, ref m_AttributeTitleSize, ref m_AttributeTitleColor,
                " : ",
                format(attribute, ref m_AttributeContentSize, ref m_AttributeContentColor, m_FlagEnd));

            m_Builder.Insert(m_NameLength + m_GroupLength + m_TypeLength + m_DescriptionLength + m_OtherIndex , content);
            m_OtherIndex += content.Length;
            return this;
        }

        public TezTipController pushAttribute(string title, float attribute)
        {
            this.pushAttribute(title, string.Format("{0:N1}", attribute));
            return this;
        }

        public TezTipController pushAttribute(string title, int attribute)
        {
            this.pushAttribute(title, attribute.ToString());
            return this;
        }

        public TezTipController pushAttribute(TezValueWrapper property, bool display_basic = true)
        {
            switch (property.valueType)
            {
                case TezValueType.Float:
                    if(property.valueSubType == TezValueSubType.WithBasic && display_basic)
                    {
                        this.pushAttribute(TezTranslator.translateName(property.name),
                            string.Format("{0:N1}/{1:N1}", ((TezValueWithBasic<float>)property).value, ((TezValueWithBasic<float>)property).basic));
                    }
                    else
                    {
                        this.pushAttribute(TezTranslator.translateName(property.name), ((TezValueWrapper<float>)property).value);
                    }
                    break;
                case TezValueType.Int:
                    if(property.valueSubType == TezValueSubType.WithBasic && display_basic)
                    {
                        this.pushAttribute(TezTranslator.translateName(property.name),
                            string.Format("{0}/{1}", ((TezValueWithBasic<int>)property).value, ((TezValueWithBasic<int>)property).basic));
                    }
                    else
                    {
                        this.pushAttribute(TezTranslator.translateName(property.name), ((TezValueWrapper<int>)property).value);
                    }
                    break;
                case TezValueType.Bool:
                    break;
                case TezValueType.String:
                    this.pushAttribute(TezTranslator.translateName(property.name), ((TezValueWrapper<string>)property).value);
                    break;
                case TezValueType.Class:
                    break;
                case TezValueType.StaticString:
                    this.pushAttribute(TezTranslator.translateName(property.name), ((TezValueWrapper<TezStaticString>)property).value);
                    break;
                case TezValueType.Type:
                    this.pushAttribute(TezTranslator.translateName(property.name), TezTranslator.translateName(((TezPV_Type)property).baseValue.name));
                    break;
                default:
                    break;
            }

            return this;
        }

        public TezTipController pushAttributeSeparator()
        {
            return this;
        }

        private string generateResult()
        {
            string result = m_Builder.ToString();

            m_NameLength = 0;
            m_GroupLength = 0;
            m_TypeLength = 0;
            m_DescriptionLength = 0;
            m_OtherIndex = 0;

            if (result.Length == 0)
            {
                result = TezTranslator.translateDescription(string.Intern("#Error_MissingInfo"));
            }
            else
            {
                result = result.Remove(result.Length - 1);
            }

            m_Builder.Clear();

            return result;
        }

        public void close()
        {
            m_Builder.Clear();
            m_Builder = null;

            m_Tip = null;
        }
    }
}
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Game
{
    public interface ITezInventoryFilterData
    {
        TezItemObject item { get; }
        int filterIndex { get; set; }
    }

    public class TezInventoryFilter : ITezCloseable
    {
        public abstract class BaseFilter : ITezCloseable
        {
            protected string mName = null;
            public string name => mName;
            public int index { get; }

            public abstract bool calculate(ITezInventoryViewSlotData slotData);
            public abstract void setFunction(TezEventExtension.Function<bool, ITezInventoryViewSlotData> function);

            public BaseFilter(string name, int index)
            {
                mName = name;
                this.index = index;
            }

            void ITezCloseable.closeThis()
            {
                this.onClose();
            }

            protected virtual void onClose()
            {
                mName = null;
            }
        }

        public class Filter_Null : BaseFilter
        {
            public Filter_Null(string name) : base(name, 0)
            {
            }

            public sealed override bool calculate(ITezInventoryViewSlotData slotData)
            {
                return true;
            }

            public sealed override void setFunction(TezEventExtension.Function<bool, ITezInventoryViewSlotData> function)
            {

            }
        }

        public class Filter_Custom : BaseFilter
        {
            TezEventExtension.Function<bool, ITezInventoryViewSlotData> mFunction = null;

            public Filter_Custom(string name, int index) : base(name, index)
            {
            }

            public sealed override bool calculate(ITezInventoryViewSlotData slotData)
            {
                return mFunction(slotData);
            }

            protected override void onClose()
            {
                base.onClose();
                mFunction = null;
            }

            public sealed override void setFunction(TezEventExtension.Function<bool, ITezInventoryViewSlotData> function)
            {
                mFunction = function;
            }
        }

        #region Tool
        public const string DefaultFilter = "Default";
        static List<BaseFilter> sFilters = new List<BaseFilter>()
        {
            new Filter_Null(DefaultFilter)
        };


        public static int createFilter(string filterName, TezEventExtension.Function<bool, ITezInventoryViewSlotData> function)
        {
            var result = sFilters.Find((BaseFilter filter) =>
            {
                return filter.name == filterName;
            });

            if (result == null)
            {
                result = new Filter_Custom(filterName, sFilters.Count);
                result.setFunction(function);
                sFilters.Add(result);
                return result.index;
            }

            return -1;
        }
        #endregion

        public event TezEventExtension.Action evtFilterChanged;

        BaseFilter mCurrentFilter = null;

        public TezInventoryFilter()
        {
            mCurrentFilter = sFilters[0];
        }

        public void setFilter(int index)
        {
            mCurrentFilter = sFilters[index];
            evtFilterChanged?.Invoke();
        }

        public void setFilter(string name)
        {
            mCurrentFilter = sFilters.Find((BaseFilter filter) => filter.name == name);
            evtFilterChanged?.Invoke();
        }

        public bool filter(ITezInventoryViewSlotData slotData)
        {
            return mCurrentFilter.calculate(slotData);
        }

        void ITezCloseable.closeThis()
        {
            mCurrentFilter = null;
            evtFilterChanged = null;
        }
    }
}
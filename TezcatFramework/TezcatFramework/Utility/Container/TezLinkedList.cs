using System;
using tezcat.Framework.Core;

namespace tezcat.Framework.Utility
{
    public interface ITezLinkedListNode<T> : ITezCloseable
    {
        TezLinkedList<T> list { get; set; }
        ITezLinkedListNode<T> prev { get; set; }
        ITezLinkedListNode<T> next { get; set; }
        T value { get; set; }

        void linkPrev(ITezLinkedListNode<T> prevNode);
        void linkNext(ITezLinkedListNode<T> nextNode);
        void removeSlef();
    }

    public class TezLinkedListNode<T> : ITezLinkedListNode<T>
    {
        public TezLinkedList<T> list { get; set; } = null;
        public ITezLinkedListNode<T> prev { get; set; } = null;
        public ITezLinkedListNode<T> next { get; set; } = null;
        public T value { get; set; }

        void ITezLinkedListNode<T>.linkPrev(ITezLinkedListNode<T> prevNode)
        {
            prevNode.prev = this.prev;
            prevNode.next = this;

            this.prev.next = prevNode;
            this.prev = prevNode;
        }

        void ITezLinkedListNode<T>.linkNext(ITezLinkedListNode<T> nextNode)
        {
            nextNode.next = this.next;
            nextNode.prev = this;

            this.next.prev = nextNode;
            this.next = nextNode;
        }

        void ITezLinkedListNode<T>.removeSlef()
        {
            this.prev.next = this.next;
            this.next.prev = this.prev;

            this.prev = null;
            this.next = null;
            this.list = null;
        }

        void ITezCloseable.closeThis()
        {
            this.list = null;
            this.prev = null;
            this.next = null;
            this.value = default;
        }
    }

    public class TezLinkedList<T> : ITezCloseable
    {
        class MarkNodeBegin : ITezLinkedListNode<T>
        {
            public ITezLinkedListNode<T> prev
            {
                get { return null; }
                set { }
            }
            public ITezLinkedListNode<T> next
            {
                get;
                set;
            } = null;

            public T value
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            TezLinkedList<T> ITezLinkedListNode<T>.list { get; set; }

            void ITezLinkedListNode<T>.linkPrev(ITezLinkedListNode<T> prevNode)
            {
                throw new NotImplementedException();
            }

            void ITezLinkedListNode<T>.linkNext(ITezLinkedListNode<T> nextNode)
            {
                nextNode.next = this.next;
                nextNode.prev = this;

                if (this.next != null)
                {
                    this.next.prev = nextNode;
                }
                this.next = nextNode;
            }

            void ITezLinkedListNode<T>.removeSlef()
            {
                throw new NotImplementedException();
            }

            void ITezCloseable.closeThis()
            {

            }
        }

        class MarkNodeEnd : ITezLinkedListNode<T>
        {
            public ITezLinkedListNode<T> prev
            {
                get;
                set;
            } = null;

            public ITezLinkedListNode<T> next
            {
                get { return null; }
                set { }
            }

            public T value
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            TezLinkedList<T> ITezLinkedListNode<T>.list { get; set; }

            void ITezLinkedListNode<T>.linkPrev(ITezLinkedListNode<T> prevNode)
            {
                prevNode.prev = this.prev;
                prevNode.next = this;

                if (this.prev != null)
                {
                    this.prev.next = prevNode;
                }
                this.prev = prevNode;
            }

            void ITezLinkedListNode<T>.linkNext(ITezLinkedListNode<T> nextNode)
            {
                throw new NotImplementedException();
            }

            void ITezLinkedListNode<T>.removeSlef()
            {
                throw new NotImplementedException();
            }

            void ITezCloseable.closeThis()
            {

            }
        }

        ITezLinkedListNode<T> mFrontMark = new MarkNodeBegin();
        ITezLinkedListNode<T> mBackMark = new MarkNodeEnd();

        int mCount = 0;
        public int count => mCount;

        public TezLinkedList()
        {
            mFrontMark.next = mBackMark;
            mBackMark.prev = mFrontMark;
        }

        public ITezLinkedListNode<T> addFront(T value)
        {
            var node = new TezLinkedListNode<T>
            {
                value = value
            };
            this.addFront(node);

            return node;
        }

        public void addFront(ITezLinkedListNode<T> node)
        {
            node.list = this;
            mFrontMark.linkNext(node);
            mCount++;
        }

        public ITezLinkedListNode<T> addBack(T value)
        {
            var node = new TezLinkedListNode<T>
            {
                value = value
            };
            this.addBack(node);

            return node;
        }

        public void addBack(ITezLinkedListNode<T> node)
        {
            node.list = this;
            mBackMark.linkPrev(node);
            mCount++;
        }

        public void insertBefore(ITezLinkedListNode<T> markNode, T value)
        {
            this.insertBefore(markNode, new TezLinkedListNode<T> { value = value });
        }

        public void insertAfter(ITezLinkedListNode<T> markNode, T value)
        {
            this.insertAfter(markNode, new TezLinkedListNode<T> { value = value });
        }

        public void insertBefore(ITezLinkedListNode<T> markNode, ITezLinkedListNode<T> value)
        {
            value.list = this;
            markNode.linkPrev(value);
            mCount++;
        }

        public void insertAfter(ITezLinkedListNode<T> markNode, ITezLinkedListNode<T> value)
        {
            value.list = this;
            markNode.linkNext(value);
            mCount++;
        }

        public bool tryGetFrontValue(out T value)
        {
            if (mCount == 0)
            {
                value = default;
                return false;
            }

            value = mFrontMark.next.value;
            return true;
        }

        public ITezLinkedListNode<T> getFrontNode()
        {
            return mFrontMark.next;
        }

        public bool tryGetBackValue(out T value)
        {
            if (mCount == 0)
            {
                value = default;
                return false;
            }

            value = mBackMark.prev.value;
            return true;
        }

        public ITezLinkedListNode<T> getBackNode()
        {
            return mBackMark.prev;
        }

        public ITezLinkedListNode<T> getBegin()
        {
            return mFrontMark;
        }

        public ITezLinkedListNode<T> getEnd()
        {
            return mBackMark;
        }

        public void foreachList(Action<T> function)
        {
            ITezLinkedListNode<T> current = mFrontMark.next;
            while (current != mBackMark)
            {
                function(current.value);
                current = current.next;
            }
        }

        public void foreachList(Action<ITezLinkedListNode<T>> function)
        {
            ITezLinkedListNode<T> current = mFrontMark.next;
            ITezLinkedListNode<T> temp;
            while (current != mBackMark)
            {
                temp = current;
                current = current.next;

                function(temp);
            }
        }

        public ITezLinkedListNode<T> popFront()
        {
            if (mCount == 0)
            {
                return null;
            }

            var temp = mFrontMark.next;
            temp.removeSlef();
            mCount--;
            return temp;
        }

        public ITezLinkedListNode<T> popBack()
        {
            if (mCount == 0)
            {
                return null;
            }

            var temp = mBackMark.prev;
            temp.removeSlef();
            mCount--;
            return temp;
        }

        public bool remove(T value)
        {
            ITezLinkedListNode<T> current = mFrontMark.next;
            while (current != mBackMark)
            {
                if (current.value.Equals(value))
                {
                    current.removeSlef();
                    return true;
                }

                current = current.next;
            }

            return false;
        }

        public bool removeAt(ITezLinkedListNode<T> node)
        {
            if (node.list != this)
            {
                return false;
            }

            node.removeSlef();
            mCount--;

            return true;
        }

        private void deleteList()
        {
            ITezLinkedListNode<T> current = mFrontMark.next;
            ITezLinkedListNode<T> temp = null;
            while (current != mBackMark)
            {
                temp = current;
                current = current.next;

                temp.close();
            }
        }

        public void clear()
        {
            this.deleteList();

            mFrontMark.next = mBackMark;
            mBackMark.prev = mFrontMark;
        }

        void ITezCloseable.closeThis()
        {
            this.deleteList();

            mFrontMark = null;
            mBackMark = null;
        }
    }
}

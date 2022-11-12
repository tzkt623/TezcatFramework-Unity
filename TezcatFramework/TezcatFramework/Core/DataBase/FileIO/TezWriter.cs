using System;
using System.Collections.Generic;

namespace tezcat.Framework.Database
{
    public abstract class TezWriter
    {
        class Checker
        {
            public string name = null;
            public int index = -1;

            public void close()
            {
                this.name = null;
            }
        }
        private Stack<Checker> mChecker = new Stack<Checker>();

        public abstract void save(string path);

        #region Array
        /// <summary>
        /// 开始写入一个数组
        /// </summary>
        public void beginArray(string key)
        {
            mChecker.Push(new Checker() { name = key });
            this.onBeginArray(key);
        }

        protected abstract void onBeginArray(string key);

        /// <summary>
        /// 结束当前数组的写入
        /// </summary>
        public void endArray(string key)
        {
            var checker = mChecker.Pop();
            if (!string.IsNullOrEmpty(key) && checker.name == key)
            {
                this.onEndArray(checker.name);
            }
            else
            {
                throw new System.ArgumentException(string.Format("Error End Array : Current:{0} | Save:{1}", key, checker.name));
            }
        }

        protected abstract void onEndArray(string key);


        //===============================================//

        /// <summary>
        /// 开始写入一个数组
        /// </summary>
        public void beginArray(int key)
        {
            mChecker.Push(new Checker() { index = key });
            this.onBeginArray(key);
        }

        protected abstract void onBeginArray(int key);

        /// <summary>
        /// 结束当前数组的写入
        /// </summary>
        public void endArray(int key)
        {
            var checker = mChecker.Pop();
            if (checker.index == key)
            {
                this.onEndArray(checker.index);
            }
            else
            {
                throw new System.ArgumentException(string.Format("Error End Array : Current:{0} | Save:{1}", key, checker.index));
            }
        }

        protected abstract void onEndArray(int key);
        #endregion

        #region Object
        /// <summary>
        /// 开始写入一个对象
        /// </summary>
        public void beginObject(string key)
        {
            mChecker.Push(new Checker() { name = key });
            this.onBeginObject(key);
        }

        protected abstract void onBeginObject(string key);

        /// <summary>
        /// 结束当前对象的写入
        /// </summary>
        public void endObject(string key)
        {
            var checker = mChecker.Pop();
            if (!string.IsNullOrEmpty(key) && checker.name == key)
            {
                this.onEndObject(checker.name);
            }
            else
            {
                throw new System.ArgumentException(string.Format("Error End Object : Current:{0} | Save:{1}", key, checker.name));
            }
        }

        protected abstract void onEndObject(string key);

        //===============================================//

        /// <summary>
        /// 开始写入一个对象
        /// </summary>
        public void beginObject(int key)
        {
            mChecker.Push(new Checker() { index = key });
            this.onBeginObject(key);
        }

        protected abstract void onBeginObject(int key);

        /// <summary>
        /// 结束当前对象的写入
        /// </summary>
        public void endObject(int key)
        {
            var checker = mChecker.Pop();
            if (checker.index == key)
            {
                this.onEndObject(checker.index);
            }
            else
            {
                throw new System.ArgumentException(string.Format("Error End Object : Current:{0} | Save:{1}", key, checker.index));
            }
        }

        protected abstract void onEndObject(int key);
        #endregion

        /*
         * 以固定格式写入数据 
         */
        public abstract void write(bool value);
        public abstract void write(int value);
        public abstract void write(float value);
        public abstract void write(string value);


        /*
         * 以键值对的方式写入数据
         */
        public abstract void write(string name, bool value);
        public abstract void write(string name, int value);
        public abstract void write(string name, float value);
        public abstract void write(string name, string value);

        public void clear()
        {
            foreach (var checker in mChecker)
            {
                checker.close();
            }
            mChecker.Clear();
        }

        public void close()
        {
            this.clear();
            mChecker = null;
        }
    }
}


using System;
using tezcat.Framework.Database;
using tezcat.Framework.ECS;

namespace tezcat.Framework.Test
{
    class TestDatabase
    {
        MyDatabase TotalDB = new MyDatabase();


        const int ID_DB1 = 0;
        const int ID_DB2 = 1;
        MyDatabase1 DB1 = new MyDatabase1(ID_DB1);
        MyDatabase2 DB2 = new MyDatabase2(ID_DB2);


        public void test()
        {
            TotalDB.registerItem(new MyItem());

            DB1.registerItem(new MyItem1());
            DB2.registerItem(new MyItem2());
        }
    }

    #region Signal
    class MyItem : TezDatabaseGameItem
    {

    }

    class MyDatabase : TezDatabase
    {

    }
    #endregion


    #region Mult
    class MyItem1 : TezDatabaseGameItem
    {
        protected override TezDataComponent createDataComponent()
        {
            var data = new MyData1();
            data.initWithData(this);
            return data;
        }
    }

    class MyItem2 : TezDatabaseGameItem
    {

    }


    class MyDatabase1 : TezMultDatabase<MyItem1>
    {
        public MyDatabase1(int uid) : base(uid)
        {

        }
    }

    class MyDatabase2 : TezMultDatabase<MyItem2>
    {
        public MyDatabase2(int uid) : base(uid)
        {

        }
    }
    #endregion


}
using System;
using tezcat.Framework.Core;
using tezcat.Framework.Game;

namespace tezcat.Framework.Test
{

    public class TestSaveController : TezBaseTest
    {
        TezSaveController.Reader mReader = null;
        TezSaveController.Writer mWriter = null;

        public TestSaveController() : base("SaveController")
        {
            
        }

        public override void init()
        {
            mReader = new TezSaveController.Reader();
            mWriter = new TezSaveController.Writer();
        }

        private void show<T>(int offset, string id)
        {
            string str = "";
            for (int i = 0; i < offset; i++)
            {
                str += " ";
            }
            str += $"{id}:{mReader.readValue<T>(id)}";
            Console.WriteLine(str);
        }

        private void show<T>(int offset, int id)
        {
            string str = "";
            for (int i = 0; i < offset; i++)
            {
                str += " ";
            }
            str += $"{id}:{mReader.readValue<T>(id)}";
            Console.WriteLine(str);
        }

        public override void run()
        {
            Console.WriteLine("READ TEST......");
            this.read();

            Console.WriteLine("WRITE TEST......");
            this.write();
        }

        private void read()
        {
            mReader.load($"{Path.root}Res/Config/CategoryConfig.json");
            mReader.beginReadObject();
            {
                this.show<string>(0, "Namespace");
                this.show<string>(0, "WrapperClass");

                mReader.enterObject("Root");
                {
                    mReader.enterObject("Useable");
                    {
                        mReader.enterArray("Potion");
                        this.show<string>(1, 0);
                        this.show<string>(1, 1);
                        mReader.exitArray("Potion");
                    }
                    mReader.exitObject("Useable");


                    mReader.enterObject("Equipment");
                    {
                        mReader.enterArray("Weapon");
                        this.show<string>(1, 0);
                        this.show<string>(1, 1);
                        this.show<string>(1, 2);
                        mReader.exitArray("Weapon");

                        mReader.enterArray("Armor");
                        this.show<string>(2, 0);
                        this.show<string>(2, 1);
                        this.show<string>(2, 2);
                        mReader.exitArray("Armor");
                    }
                    mReader.exitObject("Equipment");

                    mReader.enterArray("Unit");
                    {
                        this.show<string>(1, 0);
                        this.show<string>(1, 1);
                    }
                    mReader.exitArray("Unit");
                }
                mReader.exitObject("Root");
            }
            mReader.endReadObject();
        }

        private void write()
        {
            mWriter.beginWriteObject($"{Path.root}Res/Config/WriterTest.json");
            {
                mWriter.write("Namespace", "tezcat.Framework.Core");
                mWriter.write("WrapperClass", "MyCategory");

                mWriter.enterObject("Root");
                {
                    mWriter.enterObject("Useable");
                    {
                        mWriter.enterArray("Potion");
                        {
                            mWriter.write("HealthPotion");
                            mWriter.write("MagicPotion");
                        }
                        mWriter.exitArray("Potion");
                    }
                    mWriter.exitObject("Useable");

                    mWriter.enterObject("Equipment");
                    {
                        mWriter.enterArray("Weapon");
                        {
                            mWriter.write("Gun");
                            mWriter.write("Axe");
                            mWriter.write("Missle");
                        }
                        mWriter.exitArray("Weapon");

                        mWriter.enterArray("Armor");
                        {
                            mWriter.write("Helmet");
                            mWriter.write("Breastplate");
                            mWriter.write("Leg");
                        }
                        mWriter.exitArray("Armor");
                    }
                    mWriter.exitObject("Equipment");

                    mWriter.enterArray("Unit");
                    {
                        mWriter.write("Character");
                        mWriter.write("Ship");
                    }
                    mWriter.exitArray("Unit");
                }
                mWriter.exitObject("Root");
            }
            mWriter.endWriteObject();
        }

        protected override void onClose()
        {
            mReader.close();
            mReader = null;
        }
    }
}

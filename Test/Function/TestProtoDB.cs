namespace tezcat.Framework.Test
{
    class TestProtoDB
    {
        public void run()
        {
            //             reader = new TezJsonReader();
            //             if (reader.load(Path.root + "Res/Config/ItemConfig.json"))
            //             {
            //                 TezItemID.loadConfigFile(reader);
            //                 TezCategorySystem.registerTypeIDFrom(reader);
            //             }
            //             reader.close();

            TezcatFramework.protoDB.loadConfigFile($"{Path.root}Res/Config/ProtoConfig.json");
            TezcatFramework.protoDB.loadProtoFile($"{Path.root}Res/Proto/Item/");
        }
    }
}
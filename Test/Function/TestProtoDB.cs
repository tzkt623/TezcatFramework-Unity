﻿using tezcat.Framework.Core;

namespace tezcat.Framework.Test
{
    class TestProtoDB
    {
        public void run()
        {
            TezJsonReader reader = new TezJsonReader();
            if (reader.load(Path.root + "Res/Config/ProtoConfig.json"))
            {
                TezProtoIDManager.loadConfigFile(reader);
            }
            reader.close();

            reader = new TezJsonReader();
            if (reader.load(Path.root + "Res/Config/ItemConfig.json"))
            {
                TezItemID.loadConfigFile(reader);
                TezCategorySystem.registerTypeIDFrom(reader);
            }
            reader.close();

            TezcatFramework.protoDB.load(Path.root + "Res/Proto/Item/");
        }
    }
}
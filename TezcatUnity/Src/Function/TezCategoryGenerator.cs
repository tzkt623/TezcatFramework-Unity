using tezcat.Framework.Core;
using UnityEngine;

namespace tezcat.Unity.Tool
{
    /// <summary>
    /// 
    /// </summary>
    public class TezCategoryGenerator : MonoBehaviour
    {
        public string inCategoryFilePath;
        public string outCShapPath;
        public string outItemConfigPath;

        public void generateCShapFile()
        {
            TezJsonReader reader = new TezJsonReader();
            if(!reader.load(inCategoryFilePath))
            {
                Debug.LogError($"Load File Error : {inCategoryFilePath}");
            }
            TezCategorySystem.generateCShapFile(outCShapPath, reader);
            reader.close();
        }

        public void generateItemConfigFile()
        {
            TezJsonReader reader = new TezJsonReader();
            if (!reader.load(inCategoryFilePath))
            {
                Debug.LogError($"Load File Error : {inCategoryFilePath}");
            }
            TezCategorySystem.generateItemConfigFile(outItemConfigPath, reader);
            reader.close();
        }
    }
}
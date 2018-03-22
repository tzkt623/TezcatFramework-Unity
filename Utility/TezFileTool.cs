using System.IO;

namespace tezcat
{
    public class TezFileTool
    {
        public static bool fileExist(string file_path)
        {
            return File.Exists(file_path);
        }

        public FileStream createFile(string path)
        {
            return File.Create(path);
        }

        public static bool directoryExist(string dir_path)
        {
            return Directory.Exists(dir_path);
        }
    }
}
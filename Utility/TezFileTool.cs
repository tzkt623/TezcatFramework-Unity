using System.IO;

namespace tezcat.Utility
{
    public class TezFileTool
    {
        public static bool fileExist(string file_path)
        {
            return File.Exists(file_path);
        }

        public static FileStream createFile(string path)
        {
            return File.Create(path);
        }

        public static bool directoryExist(string dir_path)
        {
            return Directory.Exists(dir_path);
        }

        public static DirectoryInfo createDirectory(string path)
        {
            return Directory.CreateDirectory(path);
        }
    }
}
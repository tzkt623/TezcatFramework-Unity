using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace tezcat.Framework.Core
{
    public class TezFilePath
    {
        private static Regex mRelativePathCleaner = new Regex("\\/[^\\/]*[^:]\\/\\.\\.");

        static string mRootPath = null;
        public static string rootPath => mRootPath;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Application.dataPath</param>
        public static void setUnityPath(string path)
        {
            mRootPath = TezFilePath.cleanPath(path + "/..");
        }

        public static string cleanPath(string path, bool full = true)
        {
            if (path == null)
            {
                throw new ArgumentNullException("Path is null");
            }
            path = path.Replace('\\', '/');

            string text = path;
            do
            {
                path = text;
                text = mRelativePathCleaner.Replace(path, string.Empty);
            }
            while (full && text != path);

            if (text.EndsWith(":"))
            {
                text += '/';
            }

            return text;
        }

        public static string combineFullPath(string path)
        {
            if (Path.IsPathRooted(path))
            {
                return path;
            }
            return TezFilePath.rootPath + "/" + path;
        }

        public static string[] getDirs(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("directory");
            }

            var dirs = Directory.GetDirectories(path);
            for (int i = 0; i < dirs.Length; i++)
            {
                dirs[i] = TezFilePath.cleanPath(dirs[i]);
            }

            return dirs;
        }

        public static string[] getFiles(string directory, bool recursively = false)
        {
            if (string.IsNullOrEmpty(directory))
            {
                throw new ArgumentNullException("directory");
            }

            string[] files = null;

            if (recursively)
            {
                List<string> list = new List<string>(1);
                TezFilePath.getFilesRecursively(directory, ref list);
                files = list.ToArray();
            }
            else
            {
                files = Directory.GetFiles(directory);
            }

            for (int i = 0; i < files.Length; i++)
            {
                files[i] = TezFilePath.cleanPath(files[i]);
            }

            return files;
        }

        private static void getFilesRecursively(string directory, ref List<string> container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            string[] files = Directory.GetFiles(directory);
            container.Capacity += files.Length;
            container.AddRange(files);

            string[] directories = Directory.GetDirectories(directory);
            for (int i = 0; i < directories.Length; i++)
            {
                TezFilePath.getFilesRecursively(directories[i], ref container);
            }
        }

        public static bool fileExist(string file_path)
        {
            return File.Exists(file_path);
        }

        public static FileStream createFile(string path)
        {
            return File.Create(path);
        }

        public static StreamWriter createTextFile(string path)
        {
            return File.CreateText(path);
        }

        public static bool directoryExist(string path)
        {
            return Directory.Exists(path);
        }

        public static DirectoryInfo createDirectory(string path)
        {
            return Directory.CreateDirectory(path);
        }

        public static string getProjectPath()
        {
            return Directory.GetCurrentDirectory();
        }
    }
}
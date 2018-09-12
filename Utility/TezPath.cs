using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace tezcat.Utility
{
    public class TezPath
    {
        private static Regex m_RelativePathCleaner = new Regex("\\/[^\\/]*[^:]\\/\\.\\.");

        static string m_FullPath = null;
        public static string fullPath
        {
            get
            {
                if (string.IsNullOrEmpty(m_FullPath))
                {
                    m_FullPath = TezPath.cleanPath(Application.dataPath + "/..");
                }
                return m_FullPath;
            }
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
                text = m_RelativePathCleaner.Replace(path, string.Empty);
            }
            while (full && text != path);

            if (text.EndsWith(":"))
            {
                text += '/';
            }

            return path;
        }

        public static string combineFullPath(string path)
        {
            if (Path.IsPathRooted(path))
            {
                return path;
            }
            return TezPath.fullPath + "/" + path;
        }

        public static string[] getFiles(string directory, bool recursively = false)
        {
            if(string.IsNullOrEmpty(directory))
            {
                throw new ArgumentNullException("directory");
            }

            if(recursively)
            {
                List<string> list = new List<string>(1);
                TezPath.getFilesRecursively(directory, ref list);
                return list.ToArray();
            }

            return Directory.GetFiles(directory);
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
                TezPath.getFilesRecursively(directories[i], ref container);
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
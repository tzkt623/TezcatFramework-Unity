using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace tezcat.Framework.Utility
{
    public class TezPath
    {
        private static Regex m_RelativePathCleaner = new Regex("\\/[^\\/]*[^:]\\/\\.\\.");

        static string m_RootPath = null;
        public static string rootPath
        {
            get
            {
                if (string.IsNullOrEmpty(m_RootPath))
                {
                    m_RootPath = TezPath.cleanPath(Application.dataPath + "/..");
                }
                return m_RootPath;
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
            return TezPath.rootPath + "/" + path;
        }

        public static string[] getFiles(string directory, bool recursively = false)
        {
            if(string.IsNullOrEmpty(directory))
            {
                throw new ArgumentNullException("directory");
            }

            string[] files = null; 

            if(recursively)
            {
                List<string> list = new List<string>(1);
                TezPath.getFilesRecursively(directory, ref list);
                files = list.ToArray();
            }
            else
            {
                files = Directory.GetFiles(directory);
            }

            for (int i = 0; i < files.Length; i++)
            {
                files[i] = TezPath.cleanPath(files[i]);
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

        public static bool directoryExist(string path)
        {
            return Directory.Exists(path);
        }

        public static DirectoryInfo createDirectory(string path)
        {
            return Directory.CreateDirectory(path);
        }
    }
}
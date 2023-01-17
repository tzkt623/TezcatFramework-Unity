using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace mu
{
    class Rebuild
    {
        class Data
        {
            public string idGene;
            public string chrom = ".";
            public string chromStart = ".";
            public string chromEnd = ".";
            public string strand = ".";
            public string value = "NA";

            public override string ToString()
            {
                //                return string.Format("idGene:{0} chrom:{1}|chromStart:{2}|strand:{3}|value:{4}", idGene, chrom, chromStart, strand, value);
                return string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", idGene, chrom, chromStart, chromEnd, strand, value);
            }
        }

        static Regex m_RelativePathCleaner = new Regex("\\/[^\\/]*[^:]\\/\\.\\.");

        public static string cleanPath(string path, bool full = true)
        {
            if (path == null)
            {
                throw new ArgumentNullException("Path is null");
            }

            if (path[0] == '"')
            {
                path = path.Remove(0, 1);
            }

            if (path[path.Length - 1] == '"')
            {
                path = path.Remove(path.Length - 1);
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


        public void run()
        {
            int cursor_top = 0;
            int invalid_count = 0;

            Console.WriteLine("拖入红色ID文件,并按下回车");
            var samplePath = Console.ReadLine();
            samplePath = cleanPath(samplePath);
            if (!File.Exists(samplePath))
            {
                Console.WriteLine("文件错误!");
                return;
            }

            Console.WriteLine("处理中请稍后......");
            Dictionary<string, Data> dict = new Dictionary<string, Data>();
            var reader = File.OpenText(samplePath);
            while (!reader.EndOfStream)
            {
                var datas = reader.ReadLine().Split(' ', '\t');
                if (string.IsNullOrEmpty(datas[0]))
                {
                    invalid_count++;
                }
                else
                {
                    dict.Add(datas[0], new Data()
                    {
                        idGene = datas[0],
                        value = datas[1]
                    });
                }
            }
            reader.Close();
            Console.WriteLine(string.Format("已处理红色文件ID[{0}]个", dict.Count));


            Console.WriteLine("拖入紫色ID文件,并按下回车");
            invalid_count = 0;
            var dataPath = Console.ReadLine();
            dataPath = cleanPath(dataPath);
            if (!File.Exists(dataPath))
            {
                Console.WriteLine("文件错误!");
                return;
            }

            Console.WriteLine("使用快速合并模式(Y/N)");
            bool fast = Console.ReadKey(false).Key == ConsoleKey.Y;

            Console.WriteLine("处理中请稍后......文件将保存在同目录下");
            cursor_top = Console.CursorTop;

            int mage_count = 0;
            int unmage_count = 0;
            var savePath = Path.GetPathRoot(".");
            var file = File.CreateText(savePath + "MegaData.txt");
            var contents = File.ReadAllLines(dataPath);
            for (int i = 0; i < contents.Length; i++)
            {
                if(!fast)
                {
                    Console.SetCursorPosition(0, cursor_top);
                    Console.WriteLine(string.Format("已处理{0}/{1}, 合并ID[{2}]个, 无效ID[{3}]个", i + 1, contents.Length, mage_count, invalid_count));
                }

                var data_strings = contents[i].Split(' ', '\t');
                if (string.IsNullOrEmpty(data_strings[0]))
                {
                    invalid_count++;
                    continue;
                }

                if (dict.TryGetValue(data_strings[0], out var data))
                {
                    mage_count++;
                    data.chrom = data_strings[1];
                    data.chromStart = data_strings[2];
                    data.chromEnd = data_strings[3];
                    data.strand = data_strings[4];
                    file.WriteLine(data.ToString());
                    //                    Console.WriteLine(data.ToString());
                }
                else
                {
                    unmage_count++;
                }
            }

            file.Flush();
            file.Close();

            Console.WriteLine(string.Format("检测到未匹配ID[{0}]个", Math.Abs(contents.Length - invalid_count - dict.Count)));
            Console.WriteLine("数据合并完毕,按任意键退出");
            Console.Read();
        }
    }
}

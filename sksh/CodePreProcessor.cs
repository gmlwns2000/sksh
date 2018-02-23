using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sksh
{
    public class CodePreProcessor
    {
        static char[] Trims = new char[] { ' ', '\t' };

        public List<string> Content { get; set; }
        public List<string> Defines { get; set; } = new List<string>();

        public string FilePath { get; set; }

        public CodePreProcessor(string filename)
        {
            var file = filename.Replace('/', '\\');
            if (!Path.IsPathRooted(file))
            {
                file = Path.Combine(Environment.CurrentDirectory, file);
            }

            if (!File.Exists(file))
                throw new Exception($"File is not found. {file}");

            Content = File.ReadAllLines(file).ToList();
            FilePath = file;

            Console.WriteLine($"Opened : {FilePath}");
        }

        public CodePreProcessor(List<string> content, List<string> defines, string filepath)
        {
            Content = content;
            Defines = defines;
            FilePath = filepath;
        }

        public void Run()
        {
            bool skipline = false;
            for (int i = 0; i < Content.Count; i++)
            {
                var line = Content[i].Trim(Trims);

                if (line.StartsWith("#"))
                {
                    line = line.TrimStart('#');
                    line = line.TrimStart(Trims);

                    Content.RemoveAt(i);

                    var spl = line.Split(new[] { ' ' }, 2);
                    var keyword = spl[0].ToLower();

                    if (keyword == "import" && !skipline)
                    {
                        Import(spl[1], i);
                        Run();
                        return;
                    }
                    else if (keyword == "if")
                    {
                        if (!IsDefined(spl[1]))
                        {
                            skipline = true;
                        }
                    }
                    else if (keyword == "endif")
                    {
                        skipline = false;
                    }
                    else if (keyword == "define")
                    {
                        AddDefine(spl[1].Trim(Trims));
                    }
                    else if (keyword == "undef")
                    {
                        RemoveDefine(spl[1].Trim(Trims));
                    }
                    else
                    {
                        Console.WriteLine($"=! WARN [{Path.GetFileName(FilePath)}:{i}?]: Unknown Keyword \"{keyword}\". Is it mis-typed?");
                    }

                    i--;
                }

                if (skipline)
                {
                    Content.RemoveAt(i);
                    i--;

                    if (i + 1 >= Content.Count)
                    {
                        Console.WriteLine($"=! WARN [{Path.GetFileName(FilePath)}:{i}?]: #IF is not closed properly.");
                    }
                }
            }
        }

        public string GetContent()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in Content)
            {
                builder.AppendLine(item);
            }
            return builder.ToString();
        }

        void AddDefine(string arg)
        {
            foreach (var item in Defines)
            {
                if (arg.ToLower() == item.ToLower())
                    return;
            }
            Defines.Add(arg);
        }

        void RemoveDefine(string arg)
        {
            Defines.Remove(arg);
        }

        bool IsDefined(string name)
        {
            name = name.ToLower();
            foreach (var item in Defines)
            {
                if (item.ToLower() == name)
                    return true;
            }
            return false;
        }

        void Import(string argument, int index)
        {
            Console.WriteLine($"Importing {argument} at {index}...");
            if (argument.StartsWith("\"") && argument.EndsWith("\""))
            {
                var filename = argument.Trim('"');
                if (!Path.IsPathRooted(filename))
                {
                    filename = Path.Combine(Path.GetDirectoryName(FilePath), filename);
                }
                var proc = new CodePreProcessor(filename);
                proc.Defines = Defines;
                proc.Run();
                var temp = proc.Content;
                Content.InsertRange(index, temp);
            }
            else
            {
                throw new Exception($"Wrong Syntax [{Path.GetFileName(FilePath)}:{index}]: Incomplited String");
            }
        }
    }
}

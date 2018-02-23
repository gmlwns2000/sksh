using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sksh
{
    class Program
    {
        static string GetAbsolute(string path)
        {
            if (Path.IsPathRooted(path))
                return path;
            return Path.Combine(Environment.CurrentDirectory, path.Replace('/', '\\'));
        }

        static void Main(string[] args)
        {
            string filename = null;

            bool voutFlag = false;
            string voutArg = null;

            bool foutFlag = false;
            string foutArg = null;

            bool helpFlag = false;

            bool defFlag = false;
            List<string> Defines = new List<string>();

            foreach (var item in args)
            {
                if (voutFlag)
                {
                    voutArg = item;
                    voutFlag = false;
                }
                else if (foutFlag)
                {
                    foutArg = item;
                    foutFlag = false;
                }
                else if (defFlag)
                {
                    Defines.Add(item.Trim());
                    defFlag = false;
                }
                else if (item.StartsWith("-") || item.StartsWith("/"))
                {
                    var keyword = item.TrimStart('-', '/').Trim().ToLower();
                    switch (keyword)
                    {
                        case "vout":
                            voutFlag = true;
                            break;
                        case "fout":
                            foutFlag = true;
                            break;
                        case "def":
                            defFlag = true;
                            break;
                        case "help":
                            helpFlag = true;
                            break;
                        case "?":
                            helpFlag = true;
                            break;

                    }
                }
                else
                {
                    filename = item;
                }
            }

            var header = new[]
            {
                "Spiky Shader Language Pre-Processor (sksh)",
                "Version 0.1",
                "Project Spiky (c) 2018",
                "-- Spiky ShaderLang (.SKSH) is file format that is an extension of GLSL (OpenGL Shading language). --",
                "",
            };

            foreach (var item in header)
            {
                Console.WriteLine(item);
            }

            if (helpFlag)
            {
                var helps = new[]
                {
                    "= Helps ======================================",
                    "= help    | Show Help Messages",
                    "= vout    | Vertex Shader Output Name",
                    "= fout    | Fragment Shader Output Name",
                    "= def     | Defines Custom Keywords",
                    "",
                    "= Usages =====================================",
                    "= $ sksh.exe main.sksh -vout mainv.sksh -fout mainf.sksh -def SOMENICE -def NEWDEFINES",
                    "",
                    "= Reserved Defines ===========================",
                    "= Vertex Shader   | VERTEX, GEO, GEOMETRY",
                    "= Fragment Shader | PIXEL, FRAG, FRAGMENT",
                    ""
                };

                foreach (var item in helps)
                {
                    Console.WriteLine(item);
                }
            }

            if (foutArg != null)
            {
                Console.WriteLine("Fragment Shader Processing...");
                try
                {
                    var code = new CodePreProcessor(filename);
                    code.Defines.Add("PIXEL");
                    code.Defines.Add("FRAG");
                    code.Defines.Add("FRAGMENT");
                    code.Defines.AddRange(Defines);
                    code.Run();
                    var content = code.GetContent();
                    var outfile = GetAbsolute(foutArg);
                    Console.WriteLine($"Output File Path: {outfile}");
                    File.WriteAllText(outfile, content);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n=! Error is occur while fragment shader processing! \n=! {ex.Message}\n");
                    #if DEBUG
                    throw ex;
                    #endif
                }
            }

            if (voutArg != null)
            {
                Console.WriteLine("Vertex Shader Processing...");
                try
                {
                    var code = new CodePreProcessor(filename);
                    code.Defines.Add("VERTEX");
                    code.Defines.Add("GEO");
                    code.Defines.Add("GEOMETRY");
                    code.Defines.AddRange(Defines);
                    code.Run();
                    var content = code.GetContent();
                    var outfile = GetAbsolute(voutArg);
                    Console.WriteLine($"Output File Path: {outfile}");
                    File.WriteAllText(outfile, content);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n=! Error is occur while vertex shader processing! \n=! {ex.Message}\n");
                    #if DEBUG
                    throw ex;
                    #endif
                }
            }

#if DEBUG
            Console.Write("Press Enter To Exit");
            Console.ReadLine();
#endif
        }
    }
}

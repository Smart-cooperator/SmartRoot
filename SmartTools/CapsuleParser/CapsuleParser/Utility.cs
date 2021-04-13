using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CapsuleParser
{
    class Utility
    {

        static public List<string> ListDirectory()
        {
            string dirPath = @"C:\Documents\PrototypeTools\CapsuleParser\CapsuleParser\bin\Debug\Capsules";

            var dirs = from dir in
                       Directory.EnumerateDirectories(dirPath, "*")
                       select dir;

            // Show results.
            foreach (var dir in dirs)
            {
                // Remove path information from string.
                Console.WriteLine("{0}",
                    dir.Substring(dir.LastIndexOf("\\") + 1));
            }
            Console.WriteLine("{0} directories found.",
                dirs.Count<string>().ToString());

            // Optionally create a List collection.
            List<string> workDirs = new List<string>(dirs);

            return workDirs;
        }

        // Process all files in the directory passed in, recurse on any directories
        // that are found, and process the files they contain.
        public static void ProcessDirectory(string targetDirectory, List<string> infArray, List<string> binArray)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName, infArray, binArray);

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory, infArray, binArray);
        }

        // Insert logic for processing found files here.
        public static void ProcessFile(string path, List<string> infArray, List<string> binArray)
        {
            Console.WriteLine("Processed file '{0}'.", path);

            if (path.EndsWith("inf") == true)
            {
                infArray.Add(path);
            }

            if (path.EndsWith("bin") == true)
            {
                binArray.Add(path);
            }
        }

        /// <summary>
        /// Lightweight command line parser taken from from https://http://toolbox/ArgumentParser
        /// Takes whatever is found on the command line and throws it into a dictionary.
        /// Switches are supported, but positional arguments aren't.
        /// </summary>
        /// <param name="args">Arguments passed in to the command line.</param>
        /// <returns></returns>
        public static Dictionary<string, string> Parse(string[] args)
        {
            Dictionary<string, string> argTable = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            if (null == args)
                return argTable;

            string previous = string.Empty;

            for (int i = 0; i < args.Length; i++)
            {
                string s = args[i];

                if (s == "?")
                {
                    if (!argTable.ContainsKey(s))
                    {
                        argTable.Add(s, "true");
                        continue;
                    }
                }

                if (s.Equals("-Cmd", StringComparison.OrdinalIgnoreCase))
                {
                    string t = "";
                    for (i = i + 1; i < args.Length; i++)
                    {
                        t += args[i] + " ";
                    }
                    argTable.Add("Cmd", t.Trim());
                    break;
                }

                if (s.StartsWith("-") || s.StartsWith("/"))
                {
                    string snew = s.Substring(1);

                    if (!argTable.ContainsKey(snew))
                    {
                        argTable.Add(snew, "true");
                    }
                    else
                    {
                        argTable[snew] = "true";
                    }
                    previous = snew;
                }
                else
                {
                    if (argTable.Count > 0)
                    {
                        if (0 == String.Compare(argTable[previous], "true", true))
                        {
                            argTable[previous] = s;
                        }
                    }
                }
            }

            return argTable;
        }
    }
}

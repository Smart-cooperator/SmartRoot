using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace CapsuleParser
{
    class TextParser
    {
        static List<string> infArray = new List<string>();
        static List<string> binArray = new List<string>();
        static Dictionary<string, string> mapInfBinFile = new Dictionary<string, string>();
        static StringBuilder fileSaver = new StringBuilder();
        public static void ParseCapsules(Dictionary<string, string> argTable)
        {
            string configFile = "CapsuleInfoConfiguration.xml";

            if (argTable.ContainsKey("ConfigFile"))
            {
                configFile = argTable["ConfigFile"];
            }

            if (argTable.ContainsKey("CapsuleFolder"))
            {
                string dirPath = argTable["CapsuleFolder"];
                GenerateInfBinFileMap(dirPath);
                UpdateCapsuleVersion(configFile, mapInfBinFile);

                using (FileStream fs = new FileStream(configFile, FileMode.Create, FileAccess.Write, FileShare.Write))
                {
                    using (StreamWriter writer = new StreamWriter(fs, new UTF8Encoding(false)))
                    //using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                    {
                        writer.Write(fileSaver.ToString());
                    }
                }
            }
            else if (argTable.ContainsKey("InfFile") && argTable.ContainsKey("BinFile"))
            {
                mapInfBinFile.Add(argTable["InfFile"], argTable["BinFile"]);

                UpdateCapsuleVersion(configFile, mapInfBinFile);

                using (FileStream fs = new FileStream(configFile, FileMode.Create, FileAccess.Write, FileShare.Write))
                {
                    using (StreamWriter writer = new StreamWriter(fs, new UTF8Encoding(false)))
                    //using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                    {
                        writer.Write(fileSaver.ToString());
                    }
                }
            }
            else
            {
                throw new Exception("Not supported/Valid argements!");
            }

        }

        private static void GenerateInfBinFileMap(string dirPath)
        {
            Utility.ProcessDirectory(dirPath, infArray, binArray);

            foreach (string infFile in infArray)
            {
                foreach (string binFile in binArray)
                {
                    if (Path.GetDirectoryName(infFile).ToUpper().CompareTo(Path.GetDirectoryName(binFile).ToUpper()) == 0)
                    {
                        // string infFileName = Path.GetFileName(infFile);
                        // string binFileName = Path.GetFileName(binFile);
                        mapInfBinFile.Add(infFile, binFile);
                    }


                }
            }
        }

        private static void UpdateCapsuleVersion(string configFile, Dictionary<string, string> mapInfBinFile)
        {
            StreamReader rd = File.OpenText(configFile);

            string oneLineText = rd.ReadLine();

            while (oneLineText != null)
            {
                if (oneLineText.ToLower().Contains("<capsulePath>".ToLower()) == true)
                {
                    string infFilePath = ParseInfFilePath(oneLineText.Trim());

                    fileSaver.AppendLine(oneLineText);

                    foreach (var item in mapInfBinFile)
                    {
                        if (item.Key.ToLower().Contains(infFilePath.ToLower()))
                        {
                            Dictionary<string, string> versionDic = ParseVersionFromBinFile(item.Value);

                            UpdateVersionLine(rd, versionDic);
                        }
                    }

                }
                else
                {
                    fileSaver.AppendLine(oneLineText);
                }
                oneLineText = rd.ReadLine();
            }

            rd.Close();
        }

        public static Dictionary<string, string> ParseVersionFromBinFile(string binFile)
        {
            Dictionary<string, string> versionMap = new Dictionary<string, string>();
            string nameonly = Path.GetFileName(binFile);
            string verstr = nameonly.Substring(nameonly.LastIndexOf('_') + 1);
            string[] verparts = verstr.Split('.');

            if (verparts.Length == 4)
            {
                versionMap.Add("major", verparts[0]);
                versionMap.Add("minor", verparts[1]);
                versionMap.Add("build", verparts[2]);
                versionMap.Add("revision", "0");
            }
            else if (verparts.Length == 5)
            {
                versionMap.Add("major", verparts[0]);
                versionMap.Add("minor", verparts[1]);
                versionMap.Add("build", verparts[2]);
                versionMap.Add("revision", verparts[3]);
            }

            return versionMap;

        }
        public static string ParseInfFilePath(string inputString)
        {
            string infFilePath = string.Empty;

            int start = inputString.IndexOf("<capsulePath>");
            int end = inputString.IndexOf("</capsulePath>");
            infFilePath = inputString.Substring(start + 13, end - 13);

            return infFilePath;
        }

        public static void UpdateVersionLine(StreamReader rd, Dictionary<string, string> versionDic)
        {
            string oneLineText = rd.ReadLine();

            while (oneLineText != null)
            {
                string textToRepace = string.Empty;

                if (oneLineText.ToLower().Contains("<major>") == true)
                {
                    textToRepace = "<major>" + versionDic["major"] + "</major>";
                    textToRepace = oneLineText.Substring(0, oneLineText.IndexOf("<major>")) + textToRepace;
                    fileSaver.AppendLine(textToRepace);
                }
                else if (oneLineText.ToLower().Contains("<minor>") == true)
                {
                    textToRepace = "<minor>" + versionDic["minor"] + "</minor>";
                    textToRepace = oneLineText.Substring(0, oneLineText.IndexOf("<minor>")) + textToRepace;
                    fileSaver.AppendLine(textToRepace);
                }
                else if (oneLineText.ToLower().Contains("<build>") == true)
                {
                    textToRepace = "<build>" + versionDic["build"] + "</build>";
                    textToRepace = oneLineText.Substring(0, oneLineText.IndexOf("<build>")) + textToRepace;
                    fileSaver.AppendLine(textToRepace);

                }
                else if (oneLineText.ToLower().Contains("<revision>") == true)
                {
                    textToRepace = "<revision>" + versionDic["revision"] + "</revision>";
                    textToRepace = oneLineText.Substring(0, oneLineText.IndexOf("<revision>")) + textToRepace;
                    fileSaver.AppendLine(textToRepace);
                    break;
                }
                else
                {
                    fileSaver.AppendLine(oneLineText);
                }

                oneLineText = rd.ReadLine();
            }

        }
    }
}

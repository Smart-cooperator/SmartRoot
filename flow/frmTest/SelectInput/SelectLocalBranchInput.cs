using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Utilities;

namespace ProvisioningBuildTools.SelectInput
{
    public class SelectLocalBranchInput
    {
        private string[] m_LocalBranches;
        public string[] LocalBranches => m_LocalBranches;

        private string[] m_LocalFolders;
        public string[] LocalFolders => m_LocalFolders;

        private Dictionary<string, Tuple<string, string, Action<string>>> Dict = new Dictionary<string, Tuple<string, string, Action<string>>>();

        public SelectLocalBranchInput()
        {
            m_LocalFolders = Directory.GetDirectories(Command.REPOSFOLDER).Where(dir => File.Exists(Path.Combine(dir, Command.REPOSSLN))).ToArray();
            m_LocalBranches = m_LocalFolders.Select(dir => new DirectoryInfo(dir).Name).ToArray();
        }

        public Tuple<string, string, Action<string>> GetByProject(string projectName, ILogNotify logNotify)
        {
            if (!Dict.ContainsKey(projectName))
            {
                Tuple<string, string, Action<string>> value = null;

                try
                {
                    string packageConfig = Path.Combine(Command.REPOSFOLDER, projectName, Command.PACKAGESCONFIG);

                    XmlDocument packageDoc = new XmlDocument();
                    packageDoc.Load(packageConfig);

                    XmlNode packageProvisioningTools = packageDoc.DocumentElement.ChildNodes.Cast<XmlNode>().FirstOrDefault(
                        node =>
                        {
                            try
                            {
                                return Regex.IsMatch(node.Attributes["id"].Value, Command.PROVISIONINGTOOLSFILTER);
                            }
                            catch (Exception)
                            {
                                return false;
                            }
                        }
                        );

                    if (packageProvisioningTools != null)
                    {
                        string id = packageProvisioningTools.Attributes["id"].Value;
                        string destination = packageProvisioningTools.Attributes["destination"].Value;
                        string oldVersion = packageProvisioningTools.Attributes["Version"].Value;

                        if (Directory.Exists(Path.Combine(Command.REPOSFOLDER, projectName, destination)))
                        {
                            Action<string> action = new Action<string>(
                                (version) =>
                                {
                                    //packageProvisioningTools.Attributes["Version"].Value = version;
                                    //packageDoc.Save(packageConfig);

                                    string[] lines = File.ReadLines(packageConfig).ToArray();

                                    bool idFind = false;
                                    bool versionFind = false;

                                    for (int i = 0; i < lines.Length; i++)
                                    {
                                        if (!idFind)
                                        {
                                            if (lines[i].Contains($"id=\"{id}\""))
                                            {
                                                idFind = true;
                                            }
                                        }
                                        else
                                        {
                                            if (lines[i].Contains($"Version=\"{oldVersion}\""))
                                            {
                                                lines[i] = lines[i].Replace($"Version=\"{oldVersion}\"", $"Version=\"{version}\"");
                                                versionFind = true;
                                                break;
                                            }
                                        }
                                    }

                                    if (!versionFind)
                                    {
                                        throw new Exception($"Version of {id} not found in {packageConfig}");
                                    }

                                    //File.WriteAllLines(packageConfig, lines);

                                    using (FileStream fs=new FileStream(packageConfig,FileMode.Create, FileAccess.Write))
                                    {
                                        using (StreamWriter sr=new StreamWriter(fs))
                                        {
                                            for (int i = 0; i < lines.Length; i++)
                                            {
                                                if (i!= lines.Length-1)
                                                {
                                                    sr.WriteLine(lines[i]);
                                                }
                                                else
                                                {
                                                    sr.Write(lines[i]);
                                                }
                                            }
                                        }
                                    }

                                    logNotify.WriteLog($"Version of {id} change to {version} from {oldVersion} in {packageConfig}");
                                }
                                );

                            value = new Tuple<string, string, Action<string>>(id, destination, action);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logNotify.WriteLog(ex);
                }

                Dict[projectName] = value;
            }

            return Dict[projectName];
        }

    }
}

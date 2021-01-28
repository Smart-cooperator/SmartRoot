using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Utilities;
using System.Configuration;

namespace ProvisioningBuildTools.SelectInput
{
    public class SelectLocalBranchInput
    {
        private ILogNotify logNotify;

        private string[] m_LocalBranches;
        public string[] LocalBranches => m_LocalBranches;

        private Dictionary<string, LocalProjectInfo> localProjectInfoDict = new Dictionary<string, LocalProjectInfo>();

        private Dictionary<string, Tuple<string, string, Action<string>>> provisioningToolsDict = new Dictionary<string, Tuple<string, string, Action<string>>>();

        public SelectLocalBranchInput(ILogNotify logNotify)
        {
            ConfigurationManager.RefreshSection("projectSettings");

            ProjectSection projectSection = ProjectSection.Instance;

            this.logNotify = logNotify;

            string name;

            string[] m_LocalFolders = Directory.GetDirectories(Command.REPOSFOLDER).Where(dir => File.Exists(Path.Combine(dir, Command.REPOSSLN))).ToArray();

            LocalProjectInfo[] configedLocalProjectInfo = projectSection.Settings.Cast<ProjectConfigurationElement>().Select(project => new LocalProjectInfo(project.Name, project.SourceFolder, project.ProvisioningToolsFolder, project.BuildScriptsFolder, true)).ToArray();

            m_LocalFolders = m_LocalFolders.Where(folder => !configedLocalProjectInfo.Select(project => project.SourceFolder).ToArray().Contains(folder)).ToArray();

            foreach (var folder in m_LocalFolders)
            {
                name = new DirectoryInfo(folder).Name;

                if (name != "DeviceProvisioning")
                {
                    localProjectInfoDict[name] = new LocalProjectInfo(name, folder, null, null);
                }
            }

            foreach (LocalProjectInfo project in configedLocalProjectInfo)
            {
                if (!string.IsNullOrEmpty(project.Name))
                {
                    if (localProjectInfoDict.ContainsKey(project.Name))
                    {                      
                        logNotify.WriteLog($"Duplicate configuration for {project.Name}, overwrite the default one",true);
                    }

                    localProjectInfoDict[project.Name] = project;
                }
            }

            m_LocalBranches = localProjectInfoDict.Keys.OrderBy(new Func<string, string>(key => key)).ToArray();
        }

        public Tuple<string, string, Action<string>> GetByProject(string projectName, ILogNotify logNotify)
        {
            LocalProjectInfo localProjectInfo = GetProjectInfo(projectName);

            if (!provisioningToolsDict.ContainsKey(projectName))
            {
                Tuple<string, string, Action<string>> value = null;

                try
                {
                    string packageConfig = Path.Combine(localProjectInfo.SourceFolder, Command.PACKAGESCONFIG);

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
                        string destination = localProjectInfo.ProvisioningToolsFolder;
                        string oldVersion = packageProvisioningTools.Attributes["Version"].Value;

                        if (Directory.Exists(destination))
                        {
                            Action<string> action = new Action<string>(
                                (version) =>
                                {
                                    //packageProvisioningTools.Attributes["Version"].Value = version;
                                    //packageDoc.Save(packageConfig);

                                    string[] lines = File.ReadLines(packageConfig).ToArray();

                                    bool idFind = false;
                                    bool versionFind = false;
                                    bool marked = false;

                                    for (int i = 0; i < lines.Length; i++)
                                    {
                                        if (!marked)
                                        {
                                            if (lines[i].TrimStart().StartsWith("<!--"))
                                            {
                                                marked = true;
                                            }
                                            else
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
                                        }
                                        else
                                        {
                                            if (lines[i].TrimEnd().EndsWith("-->"))
                                            {
                                                marked = false;
                                            }
                                        }
                                    }

                                    if (!versionFind)
                                    {
                                        throw new Exception($"Version of {id} not found in {packageConfig}");
                                    }

                                    //File.WriteAllLines(packageConfig, lines);

                                    using (FileStream fs = new FileStream(packageConfig, FileMode.Create, FileAccess.Write))
                                    {
                                        using (StreamWriter sr = new StreamWriter(fs))
                                        {
                                            for (int i = 0; i < lines.Length; i++)
                                            {
                                                if (i != lines.Length - 1)
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
                        else
                        {
                            throw new Exception($"{destination} not found!!!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    logNotify.WriteLog(ex);
                }

                provisioningToolsDict[projectName] = value;
            }

            return provisioningToolsDict[projectName];
        }

        public LocalProjectInfo GetProjectInfo(string name)
        {
            return localProjectInfoDict[name];
        }
    }

    public class LocalProjectInfo
    {
        private string m_Name;
        public string Name => m_Name;

        private bool IsConfiguration;
        private bool FirstTimeSource = true;
        private bool FirstTimeProvisioningTools = true;
        private bool FirstTimeBuildScripts = true;

        private string m_SourceFolder;
        public string SourceFolder
        {
            get
            {
                if (string.IsNullOrEmpty(m_SourceFolder))
                {
                    m_SourceFolder = Path.Combine(Command.REPOSFOLDER, Name);
                }
                else if (string.IsNullOrEmpty(Path.GetPathRoot(m_SourceFolder)))
                {
                    m_SourceFolder = GetFullPath(m_SourceFolder, new string[] { Path.Combine(Command.REPOSFOLDER, Name) });
                }

                if (FirstTimeSource)
                {
                    FirstTimeSource = false;

                    m_SourceFolder = GetRealPath(m_SourceFolder);
                }

                return m_SourceFolder;
            }
        }

        private string m_ProvisioningToolsFolder;
        public string ProvisioningToolsFolder
        {
            get
            {
                if (string.IsNullOrEmpty(m_ProvisioningToolsFolder))
                {
                    string packageConfig = Path.Combine(SourceFolder, Command.PACKAGESCONFIG);

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
                        string destination = packageProvisioningTools.Attributes["destination"].Value;
                        m_ProvisioningToolsFolder = Path.Combine(SourceFolder, destination);
                    }
                }
                else if (string.IsNullOrEmpty(Path.GetPathRoot(m_ProvisioningToolsFolder)))
                {
                    m_ProvisioningToolsFolder = GetFullPath(m_ProvisioningToolsFolder, new string[] { SourceFolder });
                }

                if (FirstTimeProvisioningTools)
                {
                    FirstTimeProvisioningTools = false;

                    m_ProvisioningToolsFolder = GetRealPath(m_ProvisioningToolsFolder);
                }

                return m_ProvisioningToolsFolder;
            }
        }

        private string m_BuildScriptsFolder;
        public string BuildScriptsFolder
        {
            get
            {
                if (string.IsNullOrEmpty(m_BuildScriptsFolder))
                {
                    m_BuildScriptsFolder = Path.Combine(SourceFolder, Command.BUILDSCRIPTS);
                }
                else if (string.IsNullOrEmpty(Path.GetPathRoot(m_BuildScriptsFolder)))
                {
                    m_BuildScriptsFolder = GetFullPath(m_BuildScriptsFolder, new string[] { SourceFolder });
                }

                if (FirstTimeBuildScripts)
                {
                    FirstTimeBuildScripts = false;

                    m_BuildScriptsFolder = GetRealPath(m_BuildScriptsFolder);
                }

                return m_BuildScriptsFolder;
            }
        }

        private string GetFullPath(string relativePath, string[] relatedPathes = null)
        {
            string currentDirectory = Directory.GetCurrentDirectory();

            string fullPath = Path.Combine(currentDirectory, relativePath);

            string tempPath = null;

            if (relatedPathes!=null)
            {
                foreach (var relatedPath in relatedPathes)
                {
                    tempPath = Path.Combine(relatedPath, relativePath);

                    if (Directory.Exists(tempPath))
                    {
                        fullPath = tempPath;
                        break;
                    }
                }
            }
           
            return fullPath;
        }

        private string GetRealPath(string path)
        {
            if (IsConfiguration && Directory.Exists(path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);

                return Directory.GetDirectories(directoryInfo.Parent.FullName, directoryInfo.Name)[0];
            }
            else
            {
                return path;
            }
        }

        public LocalProjectInfo(string name, string sourceFolder, string provisioningToolsFolder, string buildScriptsFolder, bool isConfiguration = false)
        {
            m_Name = name;
            m_SourceFolder = sourceFolder;
            m_ProvisioningToolsFolder = provisioningToolsFolder;
            m_BuildScriptsFolder = buildScriptsFolder;
            IsConfiguration = isConfiguration;
        }

        public void UpdateProjectInfo(string sourceFolder, string provisioningToolsFolder, string buildScriptsFolder)
        {
            IsConfiguration = true;

            if (!string.IsNullOrEmpty(sourceFolder))
            {
                m_SourceFolder = sourceFolder;
            }

            if (!string.IsNullOrEmpty(provisioningToolsFolder))
            {
                m_ProvisioningToolsFolder = provisioningToolsFolder;
            }

            if (!string.IsNullOrEmpty(buildScriptsFolder))
            {
                m_BuildScriptsFolder = buildScriptsFolder;
            }
        }
    }
}

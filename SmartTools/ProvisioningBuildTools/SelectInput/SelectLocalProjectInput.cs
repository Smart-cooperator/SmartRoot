using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using ProvisioningBuildTools;
using System.Configuration;

namespace ProvisioningBuildTools.SelectInput
{
    public class SelectLocalProjectInput
    {
        private ILogNotify logNotify;

        private string[] m_LocalBranches;
        public string[] LocalBranches => m_LocalBranches;

        private Dictionary<string, LocalProjectInfo> localProjectInfoDict = new Dictionary<string, LocalProjectInfo>();

        public SelectLocalProjectInput(ILogNotify logNotify)
        {
            ProjectSection projectSection = ProjectSection.Instance;

            this.logNotify = logNotify;

            string name;

            string[] m_LocalFolders = Directory.GetDirectories(Command.REPOSFOLDER).Where(dir => File.Exists(Path.Combine(dir, Command.REPOSSLN))).ToArray();

            LocalProjectInfo[] configedLocalProjectInfo = projectSection?.Settings.Cast<ProjectConfigurationElement>().Select(project => new LocalProjectInfo(logNotify, project)).ToArray() ?? Enumerable.Empty<LocalProjectInfo>().ToArray();

            m_LocalFolders = m_LocalFolders.Where(folder => !configedLocalProjectInfo.Select(project => project.SourceFolder.ToUpper()).ToArray().Contains(folder.ToUpper())).ToArray();

            foreach (var folder in m_LocalFolders)
            {
                name = new DirectoryInfo(folder).Name;

                if (name.ToUpper() != "DeviceProvisioning".ToUpper())
                {
                    localProjectInfoDict[name.ToUpper()] = new LocalProjectInfo(logNotify, name, folder);
                }
            }

            foreach (LocalProjectInfo project in configedLocalProjectInfo)
            {
                if (!string.IsNullOrEmpty(project.Name))
                {
                    if (localProjectInfoDict.ContainsKey(project.Name.ToUpper()))
                    {
                        logNotify.WriteLog($"Duplicate configuration for {project.Name}, overwrite the default one", true);
                    }

                    localProjectInfoDict[project.Name.ToUpper()] = project;
                }
            }

            m_LocalBranches = localProjectInfoDict.Values.Select(value => value.Name).OrderBy(new Func<string, string>(key => key)).ToArray();
        }

        public LocalProjectInfo GetProjectInfo(string name)
        {
            if (localProjectInfoDict.ContainsKey(name.ToUpper()))
            {
                return localProjectInfoDict[name.ToUpper()];
            }
            else
            {
                return null;
            }
        }
    }

    public class LocalProjectInfo
    {
        public ILogNotify LogNotify { get; }

        private string m_Name;
        public string Name => m_Name;

        private bool IsConfiguration;
        private bool FirstTimeSource = true;
        private bool FirstTimeProvisioningTools = true;
        private bool FirstTimeBuildScripts = true;
        private bool FirstTimeProvisioningPackage = true;
        private bool FirstTimeCapsule = true;
        private bool FirstTimeCapsuleInfoConfiguration = true;
        private bool FirstTimeInputGenealogy = true;

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
                    m_SourceFolder = GetFullFolderPath(m_SourceFolder, new string[] { Path.Combine(Command.REPOSFOLDER, Name) });
                }

                if (FirstTimeSource)
                {
                    FirstTimeSource = false;

                    m_SourceFolder = GetRealFolderPath(m_SourceFolder);
                }

                return m_SourceFolder;
            }
        }

        private string m_ProvisioningToolsFolder;

        public string GetProvisioningToolsFolder(ILogNotify logNotify)
        {
            if (FirstTimeProvisioningTools)
            {
                FirstTimeProvisioningTools = false;
                GetTotalPakages(logNotify);
            }

            return m_ProvisioningToolsFolder;
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
                    m_BuildScriptsFolder = GetFullFolderPath(m_BuildScriptsFolder, new string[] { SourceFolder });
                }

                if (FirstTimeBuildScripts)
                {
                    FirstTimeBuildScripts = false;

                    m_BuildScriptsFolder = GetRealFolderPath(m_BuildScriptsFolder);
                }

                return m_BuildScriptsFolder;
            }
        }

        private string m_provisioningPackageFolder;
        public string ProvisioningPackageFolder
        {
            get
            {
                if (string.IsNullOrEmpty(m_provisioningPackageFolder))
                {
                    m_provisioningPackageFolder = Path.Combine(SourceFolder, Command.PROVISIONINGPACKAGE);
                }
                else if (string.IsNullOrEmpty(Path.GetPathRoot(m_provisioningPackageFolder)))
                {
                    m_provisioningPackageFolder = GetFullFolderPath(m_provisioningPackageFolder, new string[] { Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Name, Command.PROVISIONINGPACKAGE), Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Name), Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Command.PROVISIONINGPACKAGE, Name), Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Command.PROVISIONINGPACKAGE), AppDomain.CurrentDomain.BaseDirectory }, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Name));
                }

                if (FirstTimeProvisioningPackage)
                {
                    FirstTimeProvisioningPackage = false;

                    m_provisioningPackageFolder = GetRealFolderPath(m_provisioningPackageFolder);
                }

                return m_provisioningPackageFolder;
            }
        }

        private string m_CapsuleFolder;
        public string CapsuleFolder
        {
            get
            {
                if (string.IsNullOrEmpty(m_CapsuleFolder))
                {
                    m_CapsuleFolder = Path.Combine(GetProvisioningToolsFolder(LogNotify), "Capsules");
                }
                else if (string.IsNullOrEmpty(Path.GetPathRoot(m_CapsuleFolder)))
                {
                    m_CapsuleFolder = GetFullFolderPath(m_CapsuleFolder, new string[] { GetProvisioningToolsFolder(LogNotify), SourceFolder });
                }

                if (FirstTimeCapsule)
                {
                    FirstTimeCapsule = false;

                    m_CapsuleFolder = GetRealFolderPath(m_CapsuleFolder);
                }

                return m_CapsuleFolder;
            }
        }

        private string[] m_CapsuleInfoConfigurationPaths;

        public string[] CapsuleInfoConfigurationPaths
        {
            get
            {
                IEnumerable<string> temps = Enumerable.Empty<string>();
                IEnumerable<string> reals = Enumerable.Empty<string>();

                if (m_CapsuleInfoConfigurationPaths == null)
                {
                    temps = temps.Concat(Directory.EnumerateFiles(Path.Combine(SourceFolder, @"Source\ProvisioningClient\Config"), "CapsuleInfoConfiguration.xml", SearchOption.AllDirectories));
                }
                else
                {
                    temps = temps.Concat(m_CapsuleInfoConfigurationPaths);
                }

                foreach (var item in temps)
                {
                    string temp = item;

                    if (string.IsNullOrEmpty(Path.GetPathRoot(temp)))
                    {
                        temp = GetFullFilePath(temp, new string[] { Path.Combine(SourceFolder, @"Source\ProvisioningClient\Config"), SourceFolder }, Path.Combine(SourceFolder, @"Source\ProvisioningClient\Config"));
                    }

                    if (FirstTimeCapsuleInfoConfiguration)
                    {
                        temp = GetRealFilePath(temp);
                    }

                    reals = reals.Append(temp);
                }

                if (FirstTimeCapsuleInfoConfiguration)
                {
                    FirstTimeCapsuleInfoConfiguration = false;
                }

                m_CapsuleInfoConfigurationPaths = reals.ToArray();

                return m_CapsuleInfoConfigurationPaths;
            }
        }

        private string m_InputGenealogyPath;

        public string InputGenealogyPath
        {
            get
            {
                if (string.IsNullOrEmpty(m_InputGenealogyPath))
                {

                }
                else if (string.IsNullOrEmpty(Path.GetPathRoot(m_InputGenealogyPath)))
                {
                    m_InputGenealogyPath = GetFullFilePath(m_InputGenealogyPath, new string[] { Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Name, "InputGenealogy"), Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Name), Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InputGenealogy", Name), Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InputGenealogy"), AppDomain.CurrentDomain.BaseDirectory }, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Name, "InputGenealogy"));
                }

                if (FirstTimeInputGenealogy)
                {
                    FirstTimeInputGenealogy = false;

                    m_InputGenealogyPath = GetRealFilePath(m_InputGenealogyPath);
                }

                if (!File.Exists(m_InputGenealogyPath))
                {
                    m_InputGenealogyPath = string.Empty;
                }

                return m_InputGenealogyPath;
            }
        }

        public string TaskOpCodeList { get; }
        public string ProvisioningTesterPreArgFormat { get; }
        public string TestEnvironmentType { get; }
        public string InputGenealogyFileName { get; }
        public string NodeNameForSN { get; }
        public bool UseExternalProvisioningTester { get; }

        private Dictionary<string, Tuple<string, string, Action<string>>> PackagesInfo = new Dictionary<string, Tuple<string, string, Action<string>>>();

        private IEnumerable<string> m_TotalPakages;

        private string m_SelectePackagedList;

        public string SelectePackagedList => m_SelectePackagedList;

        public string[] GetTotalPakages(ILogNotify logNotify)
        {
            if (m_TotalPakages == null)
            {
                m_TotalPakages = Enumerable.Empty<string>();

                string packageConfig = Path.Combine(SourceFolder, Command.PACKAGESCONFIG);
                string allowedUploadListStr = ConfigurationManager.AppSettings["allowedUploadList"];
                string[] allowedUploadList = !string.IsNullOrEmpty(allowedUploadListStr) ? allowedUploadListStr.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { "ProvisioningTools", "DeviceBridge", "DBProxySdk" };


                if (File.Exists(packageConfig))
                {
                    XmlDocument packageDoc = new XmlDocument();
                    packageDoc.Load(packageConfig);

                    IEnumerable<XmlNode> packagesInfo = packageDoc.DocumentElement.ChildNodes.Cast<XmlNode>().Where(
                         node =>
                         {
                             try
                             {
                                 return !string.IsNullOrEmpty(allowedUploadList.FirstOrDefault(str => !string.IsNullOrEmpty(str) && node.Attributes["id"].Value.ToUpper().StartsWith(str.ToUpper())));
                             }
                             catch (Exception)
                             {
                                 return false;
                             }
                         }
                         );

                    if (packagesInfo != null && packagesInfo.Count() != 0)
                    {

                        foreach (var packageInfo in packagesInfo)
                        {
                            string id = packageInfo.Attributes["id"].Value;
                            string destination = packageInfo.Attributes["destination"].Value;

                            if (id.ToUpper().StartsWith("ProvisioningTools".ToUpper()))
                            {
                                if (!string.IsNullOrEmpty(m_ProvisioningToolsFolder))
                                {
                                    if (string.IsNullOrEmpty(Path.GetPathRoot(m_ProvisioningToolsFolder)))
                                    {
                                        m_ProvisioningToolsFolder = GetFullFolderPath(m_ProvisioningToolsFolder, new string[] { SourceFolder });
                                    }
                                }
                                else
                                {
                                    m_ProvisioningToolsFolder = Path.Combine(GetFullFolderPath(destination, new string[] { SourceFolder }));
                                }

                                if (FirstTimeProvisioningTools)
                                {
                                    FirstTimeProvisioningTools = false;

                                    m_ProvisioningToolsFolder = GetRealFolderPath(m_ProvisioningToolsFolder);
                                }

                                destination = m_ProvisioningToolsFolder;
                            }
                            else
                            {
                                destination = Path.Combine(GetFullFolderPath(destination, new string[] { SourceFolder }));
                            }

                            string oldVersion = packageInfo.Attributes["Version"].Value;

                            if (Directory.Exists(destination))
                            {
                                Action<string> action = new Action<string>(
                                    (version) =>
                                    {
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

                                                    if (lines[i].TrimEnd().EndsWith("-->"))
                                                    {
                                                        marked = false;
                                                    }
                                                }
                                                else
                                                {
                                                    if (!idFind)
                                                    {
                                                        if (lines[i].Contains($"id=\"{id}\""))
                                                        {
                                                            idFind = true;

                                                            if (lines[i].Contains($"Version=\"{oldVersion}\""))
                                                            {
                                                                lines[i] = lines[i].Replace($"Version=\"{oldVersion}\"", $"Version=\"{version}\"");
                                                                versionFind = true;
                                                                break;
                                                            }
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

                                PackagesInfo[id] = new Tuple<string, string, Action<string>>(id, destination, action);

                            }
                        }

                        m_TotalPakages = PackagesInfo.Keys.ToArray();
                    }
                }
                else
                {
                    logNotify.WriteLog($"packages info not found in {packageConfig}!!!", true);
                }

            }
            return m_TotalPakages.ToArray();
        }

        public List<Tuple<string, string, Action<string>>> GetSelectedPackagesInfo(string[] selectedList)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var selected in selectedList)
            {
                stringBuilder.Append(selected).Append(" + ");
            }

            m_SelectePackagedList = stringBuilder.ToString().TrimEnd(new char[] { '+', ' ' });

            if (selectedList != null && selectedList.Length > 0)
            {
                return PackagesInfo.Where(packageInfo => selectedList.Contains(packageInfo.Key)).Select(packageInfo => packageInfo.Value).ToList();
            }
            else
            {
                return null;
            }
        }

        private string GetFullFolderPath(string relativePath, string[] relatedPathes = null, string defaultRelatedPath = null)
        {
            string currentDirectory = Directory.GetCurrentDirectory();

            string fullPath = null;

            if (string.IsNullOrWhiteSpace(defaultRelatedPath))
            {
                fullPath = Path.Combine(currentDirectory, relativePath);
            }
            else
            {
                fullPath = Path.Combine(defaultRelatedPath, relativePath);
            }


            string tempPath = null;

            if (relatedPathes != null)
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

        private string GetFullFilePath(string relativePath, string[] relatedPathes = null, string defaultRelatedPath = null)
        {
            string currentDirectory = Directory.GetCurrentDirectory();

            string fullPath = null;

            if (string.IsNullOrWhiteSpace(defaultRelatedPath))
            {
                fullPath = Path.Combine(currentDirectory, relativePath);
            }
            else
            {
                fullPath = Path.Combine(defaultRelatedPath, relativePath);
            };

            string tempPath = null;

            if (relatedPathes != null)
            {
                foreach (var relatedPath in relatedPathes)
                {
                    tempPath = Path.Combine(relatedPath, relativePath);

                    if (File.Exists(tempPath))
                    {
                        fullPath = tempPath;
                        break;
                    }
                }
            }

            return fullPath;
        }

        private string GetRealFolderPath(string path)
        {
            if (IsConfiguration && Directory.Exists(path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(new string(path.TrimEnd(new char[] { '\\', '/' }).Append('\\').ToArray()));

                if (directoryInfo.Parent != null)
                {
                    return Directory.GetDirectories(directoryInfo.Parent.FullName, directoryInfo.Name)[0];
                }
                else
                {
                    return path;
                }
            }
            else
            {
                return path;
            }
        }

        private string GetRealFilePath(string path)
        {
            if (IsConfiguration && File.Exists(path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(new string(Path.GetDirectoryName(path).TrimEnd(new char[] { '\\', '/' }).Append('\\').ToArray()));

                return Path.Combine(Directory.GetFiles(directoryInfo.FullName, Path.GetFileName(path))[0]);
            }
            else
            {
                return path;
            }
        }

        public LocalProjectInfo(ILogNotify logNotify, string name, string sourceFolder)
        {
            LogNotify = logNotify;
            m_Name = name;
            m_SourceFolder = sourceFolder;
            InputGenealogyFileName = "InputGenealogy*.xml";
            NodeNameForSN = "SystemDutSn";
            IsConfiguration = false;
            UseExternalProvisioningTester = ToBoolean(ConfigurationManager.AppSettings["useExternalProvisioningTester"]);
        }

        public LocalProjectInfo(ILogNotify logNotify, ProjectConfigurationElement project)
        {
            LogNotify = logNotify;
            m_Name = project.Name;
            m_SourceFolder = project.SourceFolder;
            m_ProvisioningToolsFolder = project.ProvisioningToolsFolder;
            m_BuildScriptsFolder = project.BuildScriptsFolder;
            m_provisioningPackageFolder = project.ProvisioningPackageFolder;
            m_CapsuleFolder = project.CapsuleFolder;
            if (!string.IsNullOrEmpty(project.CapsuleInfoConfigurationPath))
            {
                m_CapsuleInfoConfigurationPaths = project.CapsuleInfoConfigurationPath.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(str => str.Trim()).ToArray();
            }
            else
            {
                m_CapsuleInfoConfigurationPaths = null;
            }
            m_InputGenealogyPath = project.InputGenealogyPath;
            TaskOpCodeList = project.TaskOpCodeList;
            ProvisioningTesterPreArgFormat = project.ProvisioningTesterPreArgFormat;
            TestEnvironmentType = project.TestEnvironmentType;
            InputGenealogyFileName = Path.GetFileName(project.InputGenealogyFileName);
            NodeNameForSN = project.NodeNameForSN;
            UseExternalProvisioningTester = ToBoolean(project.UseExternalProvisioningTester, ToBoolean(ConfigurationManager.AppSettings["useExternalProvisioningTester"]));
            IsConfiguration = true;
        }

        private bool ToBoolean(string vaule, bool defaultVaule = false)
        {
            if (!string.IsNullOrEmpty(vaule))
            {
                bool.TryParse(vaule, out defaultVaule);
            }

            return defaultVaule;
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ProvisioningBuildTools.SelectInput
{
    public class SelectProvisioningTesterInfoInput : SelectLocalProjectInput
    {
        private Dictionary<string, ProvisioningTesterInfo> provisioningTesterInfoDict = new Dictionary<string, ProvisioningTesterInfo>(StringComparer.InvariantCultureIgnoreCase);

        public SelectProvisioningTesterInfoInput(ILogNotify logNotify) : base(logNotify)
        {

        }

        public ProvisioningTesterInfo GetProvisioningTesterInfo(string project)
        {
            if (!provisioningTesterInfoDict.ContainsKey(project))
            {
                provisioningTesterInfoDict[project] = new ProvisioningTesterInfo(this.GetProjectInfo(project));
            }

            return provisioningTesterInfoDict[project];
        }
    }

    public class ProvisioningTesterInfo
    {
        public string Project { get; }

        public Dictionary<string, Lazy<ProvisioningPackageInfo>> ProvisioningPackageList { get; }

        public LocalProjectInfo LocalProjectInfo { get; }

        public ProvisioningTesterInfo(LocalProjectInfo localProjectInfo)
        {
            LocalProjectInfo = localProjectInfo;

            Project = localProjectInfo.Name;

            ProvisioningPackageList = new Dictionary<string, Lazy<ProvisioningPackageInfo>>();

            if (Directory.Exists(LocalProjectInfo.ProvisioningPackageFolder))
            {
                string[] provisioningPackages = Directory.EnumerateFiles(LocalProjectInfo.ProvisioningPackageFolder, Command.ProvisioningTester, SearchOption.AllDirectories).Select(file => Path.GetDirectoryName(file)).ToArray();

                foreach (var provisioningPackage in provisioningPackages)
                {
                    ProvisioningPackageList.Add(provisioningPackage.Replace(LocalProjectInfo.ProvisioningPackageFolder, string.Empty).Trim(new char[] { '\\', '/' }), new Lazy<ProvisioningPackageInfo>(() => new ProvisioningPackageInfo(provisioningPackage, this)));
                }
            }
        }
    }

    public class ProvisioningPackageInfo
    {
        public string CurrentGenealogyFile { get; }
        public string NodeNameForSN { get; }

        public string ProvisioningPackage { get; }

        public List<string> SerialNumberList { get; }

        public List<string> TaskList { get; }

        private bool mUseExternalProvisioningTester;

        public bool UseExternalProvisioningTester => mUseExternalProvisioningTester;

        public ProvisioningTesterInfo ProvisioningTesterInfo { get; }

        private string ProvisioningTesterPreArgFormat = @"-SN {SN} -IP {IP} -Slot {Slot}";

        private Dictionary<string, string> ProvisioningTesterPreArgDict = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        public ProvisioningPackageInfo(string provisioningPackage, ProvisioningTesterInfo provisioningTesterInfo)
        {
            using (GetTaskCodeManager getTaskCodeManager = new GetTaskCodeManager())
            {
                try
                {
                    ProvisioningPackage = provisioningPackage;

                    ProvisioningTesterInfo = provisioningTesterInfo;

                    mUseExternalProvisioningTester = ProvisioningTesterInfo.LocalProjectInfo.UseExternalProvisioningTester;

                    IEnumerable<string> genealogies = Enumerable.Empty<string>();

                    IEnumerable<string> inputGenealogyFileNames = new string[] { ProvisioningTesterInfo.LocalProjectInfo.InputGenealogyFileName, "InputGenealogy*.xml", "Genealogy*.xml" }.Distinct(StringComparer.InvariantCultureIgnoreCase);

                    foreach (var inputGenealogyFileName in inputGenealogyFileNames)
                    {
                        if (string.IsNullOrWhiteSpace(inputGenealogyFileName))
                        {
                            continue;
                        }

                        genealogies = Directory.EnumerateFiles(provisioningPackage, inputGenealogyFileName, SearchOption.TopDirectoryOnly);

                        if (genealogies.Count() != 0)
                        {
                            break;
                        }
                    }

                    string selectedGenealogy = null;

                    if (File.Exists(ProvisioningTesterInfo.LocalProjectInfo.InputGenealogyPath))
                    {
                        if (genealogies.Count() == 1)
                        {
                            selectedGenealogy = genealogies.First();

                            if (string.Compare(ProvisioningTesterInfo.LocalProjectInfo.InputGenealogyPath, selectedGenealogy, true) != 0)
                            {
                                File.Copy(ProvisioningTesterInfo.LocalProjectInfo.InputGenealogyPath, selectedGenealogy, true);
                            }

                        }
                        else
                        {
                            selectedGenealogy = Path.Combine(ProvisioningPackage, Path.GetFileName(ProvisioningTesterInfo.LocalProjectInfo.InputGenealogyPath));

                            if (string.Compare(ProvisioningTesterInfo.LocalProjectInfo.InputGenealogyPath, selectedGenealogy, true) != 0)
                            {
                                File.Copy(ProvisioningTesterInfo.LocalProjectInfo.InputGenealogyPath, selectedGenealogy, true);
                            }
                        }
                    }
                    else
                    {
                        if (genealogies.Count() == 1)
                        {
                            selectedGenealogy = genealogies.First();
                        }
                        else if (genealogies.Count() >= 1)
                        {
                            selectedGenealogy = genealogies.First();
                            ProvisioningTesterInfo.LocalProjectInfo.LogNotify.WriteLog($"Multi InputGenealogy file in {ProvisioningPackage}, use first one!!!", true);
                        }
                        else
                        {
                            ProvisioningTesterInfo.LocalProjectInfo.LogNotify.WriteLog($"InputGenealogy file not found in {ProvisioningPackage}", true);
                        }
                    }

                    if (!string.IsNullOrEmpty(selectedGenealogy) && File.Exists(selectedGenealogy))
                    {
                        CurrentGenealogyFile = selectedGenealogy;

                        IEnumerable<string> nodeNameForSNs = new string[] { ProvisioningTesterInfo.LocalProjectInfo.NodeNameForSN, "SystemDutSn", "ComputeDutSn", "SerialNumber" }.Distinct();

                        try
                        {
                            XDocument xDocument = XDocument.Load(selectedGenealogy);

                            foreach (var nodeNameForSN in nodeNameForSNs)
                            {
                                if (string.IsNullOrWhiteSpace(nodeNameForSN))
                                {
                                    continue;
                                }

                                SerialNumberList = xDocument.Descendants(nodeNameForSN).Select(element => element.Value).ToList();

                                if (SerialNumberList.Count != 0)
                                {
                                    NodeNameForSN = nodeNameForSN;
                                    break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            SerialNumberList = new List<string>();
                            ProvisioningTesterInfo.LocalProjectInfo.LogNotify.WriteLog($"a bad input genealogy file:{Environment.NewLine}{selectedGenealogy}{Environment.NewLine}{ex.Message}",true);
                        }
                    }
                    else
                    {
                        SerialNumberList = new List<string>();
                    }

                    string[] taskCodes = null;

                    if (!string.IsNullOrWhiteSpace(ProvisioningTesterInfo.LocalProjectInfo.TaskOpCodeList))
                    {
                        taskCodes = ProvisioningTesterInfo.LocalProjectInfo.TaskOpCodeList.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                    }
                    else if (!UseExternalProvisioningTester)
                    {
                        taskCodes = getTaskCodeManager.GetTaskCode(provisioningPackage, Path.Combine(provisioningPackage, Command.ProvisioningTester));
                    }

                    if (UseExternalProvisioningTester || taskCodes == null || taskCodes.Length == 0)
                    {
                        mUseExternalProvisioningTester = true;

                        try
                        {
                            taskCodes = Command.Run(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ExternalProvisioningTester"), "ExternalProvisioningTester.exe -TaskOpList").StandOutput.Split(new char[] { ',' }.Union(Environment.NewLine.ToArray()).ToArray(), StringSplitOptions.RemoveEmptyEntries);

                        }
                        catch (Exception)
                        {
                            ProvisioningTesterInfo.LocalProjectInfo.LogNotify.WriteLog("Get task list error: call ExternalProvisioningTester.exe -TaskOpList fail!!!", true);
                        }
                    }

                    if (taskCodes == null)
                    {
                        TaskList = new List<string>();
                    }
                    else
                    {
                        TaskList = taskCodes.ToList();
                    }
                }
                catch (Exception ex)
                {
                    ProvisioningTesterInfo.LocalProjectInfo.LogNotify.WriteLog(ex);
                }
            }
        }

        public string GenerateProvisioningTesterArg(string sn, string slot, string task)
        {
            string arg = null;

            ProvisioningTesterPreArgDict["SN"] = sn;
            ProvisioningTesterPreArgDict["IP"] = $"192.168.1.{51 + int.Parse(slot)}";
            ProvisioningTesterPreArgDict["Slot"] = slot;

            Regex regex = new Regex(@"{\S+}", RegexOptions.IgnoreCase);

            using (GetTaskCodeManager getTaskCodeManager = new GetTaskCodeManager())
            {
                if (!string.IsNullOrWhiteSpace(ProvisioningTesterInfo.LocalProjectInfo.ProvisioningTesterPreArgFormat))
                {
                    ProvisioningTesterPreArgFormat = ProvisioningTesterInfo.LocalProjectInfo.ProvisioningTesterPreArgFormat.TrimEnd();
                }
                else if (getTaskCodeManager.IssueAppendEnv(ProvisioningPackage, Path.Combine(ProvisioningPackage, Command.ProvisioningTester)))
                {
                    ProvisioningTesterPreArgFormat = ProvisioningTesterPreArgFormat.TrimEnd() + " -Env {Env}";
                }
                if (ProvisioningTesterPreArgFormat.ToUpper().Contains("{Env}".ToUpper()))
                {
                    if (!ProvisioningTesterPreArgDict.ContainsKey("Env"))
                    {
                        if (!string.IsNullOrWhiteSpace(ProvisioningTesterInfo.LocalProjectInfo.TestEnvironmentType))
                        {
                            ProvisioningTesterPreArgDict["Env"] = ProvisioningTesterInfo.LocalProjectInfo.TestEnvironmentType.Trim();
                        }
                        else
                        {
                            ProvisioningTesterPreArgDict["Env"] = getTaskCodeManager.GetEnv(ProvisioningPackage, Path.Combine(ProvisioningPackage, Command.ProvisioningTester));
                        }
                    }
                }
            }

            MatchEvaluator matchEvaluator = new MatchEvaluator(
                (match) =>
                {
                    string key = match.Value.Trim('{', '}');

                    if (ProvisioningTesterPreArgDict.ContainsKey(key))
                    {
                        return ProvisioningTesterPreArgDict[key];
                    }
                    else
                    {
                        throw new Exception($"found not supported key:{key} for {Command.ProvisioningTester} pre arguments:{ProvisioningTesterPreArgFormat}");
                    }
                });

            arg = regex.Replace(ProvisioningTesterPreArgFormat, matchEvaluator).TrimEnd();

            arg = $"{arg} {task}".TrimEnd();

            return arg;
        }
    }

    [Serializable]
    public class GetTaskCodeManager : MarshalByRefObject, IDisposable
    {
        private bool disposed = false;

        ~GetTaskCodeManager()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        /// <param name="disposing">Disposing</param>
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // clean up managed resources
                    if (appDomainProxy != null)
                    {
                        AppDomain.Unload(appDomainProxy);
                        appDomainProxy = null;
                    }
                }

                this.disposed = true;
            }
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        private AppDomain appDomainProxy;
        private GetTaskCodeProxy getTaskCodeProxy;

        public GetTaskCodeManager()
        {
            appDomainProxy = AppDomain.CreateDomain("appDomainProxy", AppDomain.CurrentDomain.Evidence);
            getTaskCodeProxy = (GetTaskCodeProxy)appDomainProxy.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(GetTaskCodeProxy).FullName);
        }

        public string[] GetTaskCode(string provisioningPackage, string provisioningTester)
        {
            try
            {
                return getTaskCodeProxy.GetTaskCode(provisioningPackage, provisioningTester);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool IssueAppendEnv(string provisioningPackage, string provisioningTester)
        {
            try
            {
                return getTaskCodeProxy.IssueAppendEnv(provisioningPackage, provisioningTester);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetEnv(string provisioningPackage, string provisioningTester)
        {
            try
            {
                return getTaskCodeProxy.GetEnv(provisioningPackage, provisioningTester);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    [Serializable]
    public class GetTaskCodeProxy : MarshalByRefObject
    {
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public string[] GetTaskCode(string provisioningPackage, string provisioningTester)
        {
            try
            {
                ResolveEventHandler resolveEventHandler = (s, e) => this.OnAssemblyResolve(e, new DirectoryInfo(provisioningPackage));
                AppDomain.CurrentDomain.AssemblyResolve += resolveEventHandler;

                Assembly assembly = Assembly.LoadFile(provisioningTester);

                Type[] types = assembly.GetTypes();

                Type codeType = types.FirstOrDefault(type => type.Name.ToUpper().Contains("OpCode".ToUpper()));

                string[] codes = new string[] { };
                string[] codesCommon = new string[] { };
                string[] codesSpecific = new string[] { };

                if (codeType != null)
                {
                    codes = Enum.GetNames(codeType);
                }

                string commTestDll = Directory.EnumerateFiles(provisioningPackage, "*ProvisioningTestingCommon*.dll", SearchOption.AllDirectories).FirstOrDefault();

                if (!string.IsNullOrEmpty(commTestDll) && File.Exists(commTestDll))
                {
                    assembly = Assembly.LoadFile(commTestDll);

                    types = assembly.GetTypes();

                    codeType = types.FirstOrDefault(type => type.Name.ToUpper().Contains("OpCode".ToUpper()));

                    if (codeType != null)
                    {
                        codesCommon = Enum.GetNames(codeType);
                    }

                }

                codeType = types.FirstOrDefault(type => type.Name.ToUpper().Contains("Program".ToUpper()));

                if (codeType != null)
                {
                    FieldInfo fileInfo = codeType.GetField("ValidOperations", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
                    object value = fileInfo?.GetValue(null);

                    if (value is List<string> && value != null)
                    {
                        codesSpecific = ((List<string>)value).ToArray();
                    }
                }

                AppDomain.CurrentDomain.AssemblyResolve -= resolveEventHandler;

                List<string> totals = new List<string>();
                totals.AddRange(codesSpecific);
                totals.AddRange(codesCommon);
                totals.AddRange(codes);
                totals = totals.Distinct(StringComparer.InvariantCultureIgnoreCase).ToList();

                return totals.ToArray();

            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool IssueAppendEnv(string provisioningPackage, string provisioningTester)
        {
            try
            {
                bool ret = false;

                ResolveEventHandler resolveEventHandler = (s, e) => this.OnAssemblyResolve(e, new DirectoryInfo(provisioningPackage));
                AppDomain.CurrentDomain.AssemblyResolve += resolveEventHandler;

                Assembly assembly = Assembly.LoadFile(provisioningTester);

                Type[] types = assembly.GetTypes();

                Type codeType = types.FirstOrDefault(type => type.Name.ToUpper().Contains("Program".ToUpper()));

                if (codeType != null)
                {
                    FieldInfo fileInfo = codeType.GetField("environmentName", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.IgnoreCase);

                    if (fileInfo != null)
                    {
                        ret = true;
                    }
                }

                AppDomain.CurrentDomain.AssemblyResolve -= resolveEventHandler;

                return ret;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetEnv(string provisioningPackage, string provisioningTester)
        {
            try
            {
                string env = null;

                ResolveEventHandler resolveEventHandler = (s, e) => this.OnAssemblyResolve(e, new DirectoryInfo(provisioningPackage));
                AppDomain.CurrentDomain.AssemblyResolve += resolveEventHandler;

                string provisioningClientDll = Directory.EnumerateFiles(provisioningPackage, "*ProvisioningClient*.dll", SearchOption.AllDirectories).FirstOrDefault();

                if (!string.IsNullOrEmpty(provisioningClientDll) && File.Exists(provisioningClientDll))
                {
                    Assembly assembly = Assembly.LoadFile(provisioningClientDll);

                    Type[] types = assembly.GetTypes();

                    Type envType = types.FirstOrDefault(type => type.Name.ToUpper().Contains("OA3Config".ToUpper()));

                    if (envType != null)
                    {
                        MethodInfo methodInfo = envType.GetMethod("getTestEnvironmentType", BindingFlags.Static | BindingFlags.Public | BindingFlags.IgnoreCase);

                        if (methodInfo != null)
                        {
                            object value = methodInfo.Invoke(null, null);

                            if (value != null)
                            {
                                env = value.ToString();
                            }
                        }
                    }

                }

                AppDomain.CurrentDomain.AssemblyResolve -= resolveEventHandler;

                return env;

            }
            catch (Exception)
            {
                return null;
            }
        }

        private Assembly OnAssemblyResolve(ResolveEventArgs args, DirectoryInfo directoryInfo)
        {
            var loadedAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(asm => string.Equals(asm.FullName, args.Name, StringComparison.OrdinalIgnoreCase)) ??
                AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies().FirstOrDefault(asm => string.Equals(asm.FullName, args.Name, StringComparison.OrdinalIgnoreCase));
            if (loadedAssembly != null)
            {
                return loadedAssembly;
            }

            var assemblyName = new AssemblyName(args.Name);
            string dependentAssemblyFilename = Path.Combine(directoryInfo.FullName, assemblyName.Name + ".dll");
            if (File.Exists(dependentAssemblyFilename))
            {
                return Assembly.LoadFrom(dependentAssemblyFilename);
            }

            dependentAssemblyFilename = Path.Combine(directoryInfo.FullName, assemblyName.Name + ".exe");
            if (File.Exists(dependentAssemblyFilename))
            {
                return Assembly.LoadFrom(dependentAssemblyFilename);
            }

            return null;
        }
    }
}

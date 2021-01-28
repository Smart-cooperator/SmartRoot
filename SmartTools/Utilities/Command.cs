using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Xml;
using System.Configuration;

namespace Utilities
{
    public class Command
    {
        public const string CMDPATH = @"cmd.exe";
        public static readonly string EWDKPATH = @"C:\17134.1.3";
        public static readonly string EWDKCMD = @"LaunchBuildEnv.cmd";
        public const string CMD = "cmd";
        public const string NUGETPACKAGECONFIGCMD = @"Nuget Install {0} -ConfigFile Nuget.config -OutputDirectory .\packages";
        public static readonly string REPOSFOLDER = $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\source\repos\";
        public const string BUILDSCRIPTS = "BuildScripts";
        public static readonly string OPENREPOSSLN = @"""C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv.exe"" Provisioning.sln";
        public static readonly string REPOSSLN = @"Provisioning.sln";
        public const string PSOTBUILDCMD = @"PostBuildPackageGeneration.cmd";
        public const string PSOTBUILDPS1 = @"PostBuildPackageGeneration.ps1";
        public static readonly string REBUILDX86 = @"""C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv"" Provisioning.sln /ReBuild ""Debug|x86""";
        public static readonly string REBUILDPLATFORM64 = @"""C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv"" Provisioning.sln /ReBuild ""Debug|{0}""";
        public const string DEBUG = @"Debug";
        public const string EXTERNALDROPS = @"ExternalDrops";
        public const string DEBUGX86BIN = @"Debug\x86\bin";
        public const string DEBUGPLATFORM64BIN = @"Debug\{0}\bin";
        public const string CREATEPACKAGE = @"CreatePackage.cmd Debug";
        public const string CREATEPACKAGECMD = @"CreatePackage.cmd";
        public const string INITCMD = "init.cmd";
        public const string UpdateExternalDropsCMD = "UpdateExternalDrops.cmd";
        public static readonly string CLONEREPOS = @"git clone https://dev.azure.com/MSFTDEVICES/Vulcan/_git/DeviceProvisioning {0}";
        public const string LISTREMOTEBRANCH = "git branch -r";
        public const string GETBRANCHLOG = "git log --decorate=full {0}";
        public const string CHECKOUTBRANCH = "git checkout {0}";
        public const string FETCHBRANCH = "git fetch";
        public const string FETCHALL = "git fetch --all";
        public const string PULLBRANCH = "git pull";
        public static readonly string CREATEPERSONALBRANCH = $"git checkout -b personal/{new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)).Name}/{{0}}";
        public static readonly string PERSONAL = $"personal/{new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)).Name}/{{0}}";
        public const string PRODUCTBRANCHFILTER = "origin/product/{0}/";
        public static readonly string DESMOBUILDFOLDER = @"\\desmo\release\Vulcan\CI_DeviceProvisioning\refs\heads\";
        public const string RSX = @"RS*";
        public const string RSXX86DEBUG = @"Drop_RS*_x86_Debug";
        public const string RSXPLATFORM64DEBUG = @"Drop_RS*_*64_Debug";
        public const string PROVISIONINGPACKAGE = @"ProvisioningPackage";
        public const string POSTBUILDPACKAGENAME = @"Provisioning_{0}_Debug";
        public const string CREATEHASHANDVERSION = @"CreateHashAndVersion.cmd";
        public const string PUBLISHLOCALBINARIES = @"PublishLocalBinaries.cmd";
        public const string PACKAGESCONFIG = "Packages.config";
        public const string PROVISIONINGTOOLSFILTER = @"(ProvisioningTools)_(\S+)_(\S+)";
        public static readonly string GLOBALPACKAGEFOLDER = $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\.packager";
        public const string INSTALLSURFACEPACKAGESCMD = @"InstallSurfacePackages.cmd";
        public const string INSTALLSURFACEPACKAGESPS1 = @"InstallSurfacePackages.ps1";
        public const string MANIFESTRESOURCEPATH = "Utilities.InstallSurfacePackages.";
        public const string GENERATEDFILESFOLDER = @"Source\ProvisioningClient\obj\GeneratedFiles";

        static Command()
        {
            string eWDKPath = ConfigurationManager.AppSettings["eWDKPath"];
            string devenvExePath = ConfigurationManager.AppSettings["devenvExePath"];
            string devenvComPath = ConfigurationManager.AppSettings["devenvComPath"];
            string provisioningSolution = ConfigurationManager.AppSettings["provisioningSolution"];
            string provisioningRepository = ConfigurationManager.AppSettings["provisioningRepository"];
            string desmoBuildFolder = ConfigurationManager.AppSettings["desmoBuildFolder"];
            string defaultSourceFolder = ConfigurationManager.AppSettings["defaultSourceFolder"];
            string arg0 = "{0}";

            if (!string.IsNullOrEmpty(eWDKPath) && File.Exists(eWDKPath))
            {
                EWDKPATH = Path.GetDirectoryName(eWDKPath);
                EWDKCMD = Path.GetFileName(eWDKPath);
            }

            if (!string.IsNullOrEmpty(provisioningSolution))
            {
                REPOSSLN = provisioningSolution;
            }

            if (!string.IsNullOrEmpty(devenvExePath) && File.Exists(devenvExePath) && !string.IsNullOrEmpty(provisioningSolution))
            {
                OPENREPOSSLN = $@"""{devenvExePath}"" {provisioningSolution}";
            }

            if (!string.IsNullOrEmpty(devenvComPath) && File.Exists(devenvComPath) && !string.IsNullOrEmpty(provisioningSolution))
            {
                REBUILDX86 = $@"""{devenvComPath}"" {provisioningSolution} /ReBuild ""Debug|x86""";
                REBUILDPLATFORM64 = $@"""{devenvComPath}"" {provisioningSolution} /ReBuild ""Debug|{arg0}""";
            }

            if (!string.IsNullOrEmpty(provisioningRepository))
            {
                CLONEREPOS = $@"git clone {provisioningRepository} {arg0}";
            }

            if (!string.IsNullOrEmpty(desmoBuildFolder))
            {
                DESMOBUILDFOLDER = desmoBuildFolder;
            }

            if (!string.IsNullOrEmpty(defaultSourceFolder))
            {
                REPOSFOLDER = defaultSourceFolder.Replace("{UserProfile}", new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)).Name);
                Directory.SetCurrentDirectory(REPOSFOLDER);
            }
        }


        public static int Run(string workDirectory, string cmd, string[] input, out int exitCode, out string standOutput, out string errorOutput, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            string exitFlag = null;

            if (input != null && input.Length > 0 && input.Contains("exit") && !string.IsNullOrEmpty(exitFlag = input.FirstOrDefault(str => str.Trim().StartsWith("cd"))))
            {
                exitFlag = $"{exitFlag.Trim().Substring(2).Trim()}>exit";
            }

            if (cancellationTokenSource != null && cancellationTokenSource.Token != CancellationToken.None)
            {
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
            }

            exitCode = int.MinValue;
            standOutput = null;
            errorOutput = null;

            int tempExitCode = int.MinValue;
            StringBuilder tempStandOutput = new StringBuilder();
            StringBuilder tempErrorOutput = new StringBuilder();

            ManualResetEvent exitPutWaitHandle = new ManualResetEvent(false);
            ManualResetEvent standOutputWaitHandle = new ManualResetEvent(false);
            ManualResetEvent errorOutputWaitHandle = new ManualResetEvent(false);

            using (Process p = new Process())
            {
                p.StartInfo.FileName = CMDPATH;
                p.StartInfo.UseShellExecute = false;

                if (cmd != null)
                {
                    p.StartInfo.Arguments = $"/c {cmd}";
                }

                //if (cmd == null && input != null && input.Length != 0 && !string.IsNullOrWhiteSpace(input.FirstOrDefault(str => !string.IsNullOrWhiteSpace(str))))
                if (input != null && input.Length != 0 && !string.IsNullOrWhiteSpace(input.FirstOrDefault(str => !string.IsNullOrWhiteSpace(str))))
                {
                    p.StartInfo.RedirectStandardInput = true;
                }
                else
                {
                    p.StartInfo.RedirectStandardInput = false;
                }

                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;


                p.StartInfo.Verb = "RunAs";

                if (workDirectory != null)
                {
                    if (Directory.Exists(workDirectory))
                    {
                        p.StartInfo.WorkingDirectory = workDirectory;
                    }
                    else
                    {
                        throw new DirectoryNotFoundException($"{workDirectory} not found");
                    }
                }

                p.EnableRaisingEvents = true;

                p.OutputDataReceived += new DataReceivedEventHandler(
                    (object sender, DataReceivedEventArgs e) =>
                    {
                        bool exit = false;

                        if (e.Data == null)
                        {
                            standOutputWaitHandle.Set();
                            return;
                        }

                        if (!string.IsNullOrEmpty(exitFlag))
                        {
                            if (e.Data.Equals(exitFlag))
                            {
                                exit = true;
                            }
                        }


                        if (cmd != null && cmd.Contains(string.Format(GETBRANCHLOG, string.Empty)) && tempStandOutput.ToString().Contains("commit") && tempStandOutput.ToString().Contains("Date:"))
                        {
                            return;
                        }

                        commandNotify?.WriteOutPut(((Process)sender).Id, e.Data); tempStandOutput.AppendLine(e.Data);

                        if (exit)
                        {
                            standOutputWaitHandle.Set();
                            errorOutputWaitHandle.Set();
                        }
                    });

                p.ErrorDataReceived += new DataReceivedEventHandler(
                    (object sender, DataReceivedEventArgs e) =>
                    {
                        if (e.Data == null)
                        {
                            errorOutputWaitHandle.Set();
                            return;
                        }

                        commandNotify?.WriteErrorOutPut(((Process)sender).Id, e.Data);
                        tempErrorOutput.AppendLine(e.Data);
                    });

                p.Exited += new EventHandler(
                    (object sender, EventArgs e) =>
                    {
                        //Thread.Sleep(5);

                        tempExitCode = ((Process)sender).ExitCode;
                        commandNotify?.Exit(((Process)sender).Id, ((Process)sender).ExitCode);

                        exitPutWaitHandle.Set();
                    });


                p.Start();//启动程序

                if (cmd != null)
                {
                    commandNotify?.WriteOutPut(p.Id, $"{workDirectory}>{cmd}");
                }

                p.BeginOutputReadLine();
                p.BeginErrorReadLine();

                if (p.StartInfo.RedirectStandardInput)
                {
                    foreach (var item in input)
                    {
                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            p.StandardInput.WriteLine(item);
                        }
                    }
                }

                bool killed = false;

                //p.WaitForExit();
                if (cancellationTokenSourceForKill != null && cancellationTokenSourceForKill.Token != CancellationToken.None)
                {
                    while (!exitPutWaitHandle.WaitOne(100))
                    {
                        if (cancellationTokenSourceForKill.Token.IsCancellationRequested)
                        {
                            p.Kill();
                            killed = true;
                        }
                    }
                }

                if (!killed)
                {
                    exitPutWaitHandle.WaitOne();
                    standOutputWaitHandle.WaitOne();
                    errorOutputWaitHandle.WaitOne();
                }

                p.CancelErrorRead();
                p.CancelOutputRead();

                standOutput = tempStandOutput.ToString();
                errorOutput = tempErrorOutput.ToString();
                exitCode = p.ExitCode;
                int id = p.Id;

                p.Close();

                return id;
            }
        }

        public static int Run(string workDirectory, string cmd, out int exitCode, out string standOutput, out string errorOutput, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            return Run(workDirectory, cmd, null, out exitCode, out standOutput, out errorOutput, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
        }

        public static int RunOneWDK(string workDirectory, string cmd, out int exitCode, out string standOutput, out string errorOutput, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            // List<string> input = new List<string>() { EWDKCMD };
            List<string> input = new List<string>() { };

            if (!File.Exists(Path.Combine(EWDKPATH, EWDKCMD)))
            {
                throw new FileNotFoundException($"{EWDKCMD} not found", Path.Combine(EWDKPATH, EWDKCMD));
            }

            if (!string.IsNullOrEmpty(workDirectory) && workDirectory != EWDKPATH)
            {
                if (Directory.Exists(workDirectory))
                {
                    if (Path.GetPathRoot(workDirectory) != Path.GetPathRoot(EWDKPATH))
                    {
                        input.Add($"{Path.GetPathRoot(workDirectory)}：");
                    }

                    input.Add($"cd {workDirectory}");
                }
                else
                {
                    throw new DirectoryNotFoundException($"{workDirectory} not found");
                }
            }

            input.AddRange(new string[] { cmd, "exit", "exit" });

            //return Run(EWDKPATH, null, input.ToArray(), out exitCode, out standOutput, out errorOutput, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
            return Run(EWDKPATH, EWDKCMD, input.ToArray(), out exitCode, out standOutput, out errorOutput, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
        }

        public static CommandResult Run(string workDirectory, string cmd, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null, bool runOneWDK = false)
        {
            if (cancellationTokenSource != null && cancellationTokenSource.Token != CancellationToken.None)
            {
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
            }

            MethodBase method = new StackTrace().GetFrame(1).GetMethod();

            //WriteFuctionName2Log(method.Name, logNotify);

            int id;
            int exitCode;
            string standOutput;
            string errorOutput;

            if (!runOneWDK)
            {
                id = Run(workDirectory, cmd, out exitCode, out standOutput, out errorOutput, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
            }
            else
            {
                id = RunOneWDK(workDirectory, cmd, out exitCode, out standOutput, out errorOutput, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
            }

            CommandResult commandResult = new CommandResult(method.Name, exitCode, standOutput, errorOutput, id);

            // WriteFuctionName2Log(method.Name, logNotify, false);

            return commandResult;
        }

        public static CommandResult OpenReposSln(string projectPath, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            string slnPath = Path.Combine(projectPath, REPOSSLN);

            if (!File.Exists(slnPath))
            {
                throw new FileNotFoundException($"{REPOSSLN} not found", slnPath);
            }

            return Run(projectPath, OPENREPOSSLN, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill, true);
        }

        public static CommandResult NugetPackageConfig(string projectPath, string packageConfig, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            string slnPath = Path.Combine(projectPath, REPOSSLN);

            if (!File.Exists(slnPath))
            {
                throw new FileNotFoundException($"{REPOSSLN} not found", slnPath);
            }

            return Run(projectPath, string.Format(NUGETPACKAGECONFIGCMD, packageConfig), commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
        }

        public static CommandResult RebulidX86(string projectPath, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            string slnPath = Path.Combine(projectPath, REPOSSLN);

            if (!File.Exists(slnPath))
            {
                throw new FileNotFoundException($"{REPOSSLN} not found", slnPath);
            }

            if (Directory.Exists(Path.Combine(projectPath, DEBUGX86BIN)))
            {
                Directory.Delete(Path.Combine(projectPath, DEBUGX86BIN), true);
            }

            if (Directory.Exists(Path.Combine(projectPath, GENERATEDFILESFOLDER)))
            {
                Directory.Delete(Path.Combine(projectPath, GENERATEDFILESFOLDER), true);
            }

            return Run(projectPath, REBUILDX86, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill, true);
        }


        public static CommandResult RebulidPlatform64(string projectPath, string platform64, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            string slnPath = Path.Combine(projectPath, REPOSSLN);

            if (!File.Exists(slnPath))
            {
                throw new FileNotFoundException($"{REPOSSLN} not found", slnPath);
            }

            string platform64DebugBinFolder = string.Format(Path.Combine(projectPath, DEBUGPLATFORM64BIN), platform64);

            if (Directory.Exists(platform64DebugBinFolder))
            {
                Directory.Delete(platform64DebugBinFolder, true);
            }

            return Run(projectPath, string.Format(REBUILDPLATFORM64, platform64), commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill, true);
        }

        public static CommandResult RebulidAll(string projectPath, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            CommandResult commandResult;

            string slnPath = Path.Combine(projectPath, REPOSSLN);

            if (!File.Exists(slnPath))
            {
                throw new FileNotFoundException($"{REPOSSLN} not found", slnPath);
            }

            if (Directory.Exists(Path.Combine(projectPath, DEBUG)))
            {
                Directory.Delete(Path.Combine(projectPath, DEBUG), true);
            }

            commandResult = RebulidX86(projectPath, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

            if (commandResult.ExitCode == 0)
            {
                string platform64 = GetPlatform64(projectPath);

                if (string.IsNullOrEmpty(platform64))
                {
                    return commandResult;
                }
                else
                {
                    return RebulidPlatform64(projectPath, platform64, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
                }
            }
            else
            {
                return commandResult;
            }
        }

        private static string GetPlatform64(string projectPath)
        {
            //string vcProject = Directory.EnumerateFiles(Path.Combine(projectPath, "Source"), "*.vcxproj", SearchOption.AllDirectories).FirstOrDefault();
            string vcProject = Path.Combine(projectPath, CREATEPACKAGECMD);
            if (File.Exists(vcProject))
            {
                //string pattern = @"<Platform>(?<Platform>\S+?)</Platform>";
                string pattern = @"\\(?<Platform>[^\\\s]+?64)\\";
                Regex regex = new Regex(pattern);

                Match match = regex.Match(File.ReadAllText(vcProject));

                if (match.Success)
                {
                    return match.Groups["Platform"].Value;
                }
                else
                {
                    throw new Exception($"Platform not found in {vcProject}");
                }
            }
            else
            {
                return null;
            }
        }

        public static CommandResult CreatePackage(string projectPath, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            if (!File.Exists(Path.Combine(projectPath, CREATEPACKAGECMD)))
            {
                throw new FileNotFoundException($"{CREATEPACKAGECMD} not found", Path.Combine(projectPath, CREATEPACKAGECMD));
            }

            string platform64 = GetPlatform64(projectPath);
            string platform64DebugBinFolder = string.Format(Path.Combine(projectPath, DEBUGPLATFORM64BIN), platform64);

            if (!Directory.Exists(platform64DebugBinFolder) || Directory.GetFiles(platform64DebugBinFolder).Length == 0)
            {
                throw new FileNotFoundException($"{string.Format(DEBUGPLATFORM64BIN, platform64)} not found", platform64DebugBinFolder);
            }

            if (!Directory.Exists(Path.Combine(projectPath, DEBUGX86BIN)) || Directory.GetFiles(Path.Combine(projectPath, DEBUGX86BIN)).Length == 0)
            {
                throw new FileNotFoundException($"{DEBUGX86BIN} not found", Path.Combine(projectPath, DEBUGX86BIN));
            }

            if (Directory.Exists(Path.Combine(projectPath, PROVISIONINGPACKAGE)))
            {
                Directory.Delete(Path.Combine(projectPath, PROVISIONINGPACKAGE), true);
            }

            return Run(projectPath, CREATEPACKAGE, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
        }

        public static CommandResult Init(string projectPath, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            if (!File.Exists(Path.Combine(projectPath, INITCMD)))
            {
                throw new FileNotFoundException($"{INITCMD} not found", Path.Combine(projectPath, INITCMD));
            }

            return Run(projectPath, INITCMD, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
        }

        public static CommandResult UpdateExternalDrops(string projectPath, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            if (!File.Exists(Path.Combine(projectPath, UpdateExternalDropsCMD)))
            {
                throw new FileNotFoundException($"{UpdateExternalDropsCMD} not found", Path.Combine(projectPath, UpdateExternalDropsCMD));
            }

            string packageConfig = Path.Combine(projectPath, PACKAGESCONFIG);

            XmlDocument packageDoc = new XmlDocument();
            packageDoc.Load(packageConfig);

            IEnumerable<XmlElement> packages = packageDoc.DocumentElement.ChildNodes.Cast<XmlNode>().Where(node => node is XmlElement).Cast<XmlElement>().Where(element => element.HasAttribute("Version"));

            string id;
            Version version;
            string destination;
            string nugetspec;


            packages.ToList().ForEach(
                (package) =>
                            {
                                id = package.GetAttribute("id");
                                version = new Version(package.GetAttribute("Version"));

                                if (version.Build == -1)
                                {
                                    version = new Version(version.Major, version.Minor, 0);
                                }

                                if (version.Revision == -1)
                                {
                                    version = new Version(version.Major, version.Minor, version.Build, 0);
                                }

                                destination = Path.Combine(projectPath, package.GetAttribute("destination"));

                                string versionStr = version.Revision == 0 ? version.ToString(3) : version.ToString(4);

                                nugetspec = Path.Combine(destination, $"{id}.{versionStr}.nuspec");

                                if (Directory.Exists(destination) && !File.Exists(nugetspec))
                                {
                                    Directory.Delete(destination, true);
                                }
                            }
                );

            CommandResult commandResult = Run(projectPath, UpdateExternalDropsCMD, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

            if (!string.IsNullOrEmpty(commandResult.ErrorOutput))
            {
                string pattern = @"Get-Item : (?<Error>Package '[\S\s]+?' is not found)";

                Regex regex = new Regex(pattern);

                Match match = regex.Match(commandResult.ErrorOutput);

                while (match.Success)
                {
                    logNotify.WriteLog(match.Groups["Error"].Value, true);
                    match = match.NextMatch();
                }

                commandResult = new CommandResult(commandResult.CommandName, -1, commandResult.StandOutput, commandResult.ErrorOutput, commandResult.ProcessId);
            }

            return commandResult;
        }

        public static CommandResult GitColne(string projectPath, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            if (Directory.Exists(projectPath) && (Directory.GetFiles(projectPath).Length != 0 || Directory.GetDirectories(projectPath).Length != 0))
            {
                throw new Exception($"{projectPath} is not an empty folder");
            }

            return Run(Path.GetDirectoryName(projectPath), string.Format(CLONEREPOS, new DirectoryInfo(projectPath).Name), commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
        }

        public static CommandResult GitRemoteBranchList(string projectPath, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            if (!Directory.Exists(projectPath) || !Directory.GetDirectories(projectPath).Select(dir => (new DirectoryInfo(dir)).Name).Contains(".git"))
            {
                throw new Exception($"{projectPath} is not an repos folder");
            }

            return Run(projectPath, LISTREMOTEBRANCH, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
        }

        public static CommandResult GitLog(string projectPath, string branchName, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            if (!Directory.Exists(projectPath) || !Directory.GetDirectories(projectPath).Select(dir => (new DirectoryInfo(dir)).Name).Contains(".git"))
            {
                throw new Exception($"{projectPath} is not an repos folder");
            }

            return Run(projectPath, string.Format(GETBRANCHLOG, branchName), commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
        }

        public static CommandResult GitCheckOut(string projectPath, string branchName, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            if (!Directory.Exists(projectPath) || !Directory.GetDirectories(projectPath).Select(dir => (new DirectoryInfo(dir)).Name).Contains(".git"))
            {
                throw new Exception($"{projectPath} is not an repos folder");
            }

            return Run(projectPath, string.Format(CHECKOUTBRANCH, branchName), commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
        }

        public static CommandResult GitFetch(string projectPath, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            if (!Directory.Exists(projectPath) || !Directory.GetDirectories(projectPath).Select(dir => (new DirectoryInfo(dir)).Name).Contains(".git"))
            {
                throw new Exception($"{projectPath} is not an repos folder");
            }

            return Run(projectPath, FETCHBRANCH, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
        }

        public static CommandResult GitFetchAll(string projectPath, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            if (!Directory.Exists(projectPath) || !Directory.GetDirectories(projectPath).Select(dir => (new DirectoryInfo(dir)).Name).Contains(".git"))
            {
                throw new Exception($"{projectPath} is not an repos folder");
            }

            return Run(projectPath, FETCHALL, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
        }

        public static CommandResult GitPull(string projectPath, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            if (!Directory.Exists(projectPath) || !Directory.GetDirectories(projectPath).Select(dir => (new DirectoryInfo(dir)).Name).Contains(".git"))
            {
                throw new Exception($"{projectPath} is not an repos folder");
            }

            return Run(projectPath, PULLBRANCH, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
        }

        public static CommandResult GitCreatePersonalBranch(string projectPath, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            if (!Directory.Exists(projectPath) || !Directory.GetDirectories(projectPath).Select(dir => (new DirectoryInfo(dir)).Name).Contains(".git"))
            {
                throw new Exception($"{projectPath} is not an repos folder");
            }

            return Run(projectPath, string.Format(CREATEPERSONALBRANCH, new DirectoryInfo(projectPath).Name), commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
        }

        public static CommandResult CheckOutLatestBranch(string projectName, string newBranchName = null, Tuple<string, DateTime?, Version> specificBranch = null, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {

            //WriteFuctionName2Log(MethodBase.GetCurrentMethod().Name, logNotify);


            string projectPath = Path.Combine(REPOSFOLDER, string.IsNullOrWhiteSpace(newBranchName) ? projectName : $"{projectName}_{newBranchName}");

            CommandResult commandResult;

            commandResult = GitColne(projectPath, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

            if (commandResult.ExitCode != 0)
            {
                throw new Exception(string.Format($"Project:{projectPath} Action:{CLONEREPOS} failed!!! Error:{commandResult.ErrorOutput}", projectPath));
            }


            commandResult = GitFetchAll(projectPath, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

            if (commandResult.ExitCode != 0)
            {
                throw new Exception($"Project:{projectPath} Action:{FETCHALL} failed!!! Error:{commandResult.ErrorOutput}");
            }

            commandResult = GitRemoteBranchList(projectPath, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

            if (commandResult.ExitCode != 0)
            {
                throw new Exception($"Project:{projectPath} Action:{LISTREMOTEBRANCH} failed!!! Error:{commandResult.ErrorOutput}");
            }

            string[] branches = commandResult.StandOutput.Split(Environment.NewLine.ToCharArray()).Where(branch => branch.Contains(string.Format(PRODUCTBRANCHFILTER, projectName))).Select(str => str.Trim()).ToArray();

            Tuple<string, DateTime?, Version> lastestBranch = new Tuple<string, DateTime?, Version>(null, null, null);

            Tuple<string, DateTime?, Version> tempBranch;

            if (specificBranch != null && specificBranch.Item1 != null && branches.Contains($"origin/{specificBranch.Item1.Trim()}"))
            {
                lastestBranch = specificBranch;
            }
            else
            {
                for (int i = 0; i < branches.Length; i++)
                {
                    commandResult = GitLog(projectPath, branches[i], commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

                    if (commandResult.ExitCode != 0)
                    {
                        throw new Exception(string.Format($"Project:{projectPath} Action:{GETBRANCHLOG} failed!!! Error:{commandResult.ErrorOutput}", branches[i]));
                    }
                    string tempBranchName = branches[i].Replace("origin/", string.Empty).Replace("/", "_").Trim();

                    //string firstLine = commandResult.StandOutput.Split(Environment.NewLine.ToCharArray()).FirstOrDefault(line => line.ToLower().Contains("tag: refs/tags/"));
                    string firstLine = commandResult.StandOutput.Split(Environment.NewLine.ToCharArray()).FirstOrDefault(line => line.ToLower().Contains("commit"));

                    Version tag = null;

                    if (!string.IsNullOrEmpty(firstLine))
                    {
                        string matchPattern = $"{Regex.Escape("tag: refs/tags/")}[0-9]+{Regex.Escape(".")}[0-9]+{Regex.Escape(".")}[0-9]+{Regex.Escape($"-{tempBranchName}")}";

                        Match ma = Regex.Match(firstLine, matchPattern, RegexOptions.IgnoreCase);

                        if (ma.Success)
                        {
                            tag = new Version(ma.Value.ToUpper().Replace("tag: refs/tags/".ToUpper(), string.Empty).Replace($"-{tempBranchName}".ToUpper(), string.Empty));
                        }
                    }

                    string dateTimeLine = commandResult.StandOutput.Split(Environment.NewLine.ToCharArray()).FirstOrDefault(line => line.ToLower().Contains("Date:".ToLower())).Replace("Date:", string.Empty).Trim();

                    DateTime? dateTime = null;

                    try
                    {
                        dateTime = dateTimeLine == null ? null : (DateTime?)Convert.ToDateTime(dateTimeLine);
                    }
                    catch (Exception)
                    {
                        if (dateTimeLine != null)
                        {
                            string[] dateTimeArrary = dateTimeLine.Split(' ');

                            dateTime = (DateTime?)Convert.ToDateTime($"{dateTimeArrary[0]}, {dateTimeArrary[1]} {dateTimeArrary[2]} {dateTimeArrary[4]} {dateTimeArrary[3]} {dateTimeArrary[5]}");
                        }
                    }


                    tempBranch = new Tuple<string, DateTime?, Version>(branches[i].Replace("origin/", string.Empty).Trim(), dateTime, tag);

                    if (lastestBranch.Item1 == null)
                    {
                        lastestBranch = tempBranch;
                    }
                    else if (lastestBranch.Item2 != null && tempBranch.Item2 != null)
                    {
                        if (tempBranch.Item2 > lastestBranch.Item2)
                        {
                            lastestBranch = tempBranch;
                        }
                    }
                }
            }

            if (lastestBranch.Item1 != null)
            {
                commandResult = GitCheckOut(projectPath, lastestBranch.Item1, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception(string.Format($"Project:{projectPath} Action:{CHECKOUTBRANCH} failed!!! Error:{commandResult.ErrorOutput}", lastestBranch.Item1));
                }

                commandResult = GitFetch(projectPath, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception($"Project:{projectPath} Action:{FETCHBRANCH} failed!!! Error:{commandResult.ErrorOutput}");
                }

                commandResult = GitPull(projectPath, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception($"Project:{projectPath} Action:{PULLBRANCH} failed!!! Error:{commandResult.ErrorOutput}");
                }

                commandResult = GitCheckOut(projectPath, lastestBranch.Item1, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception(string.Format($"Project:{projectPath} Action:{CHECKOUTBRANCH} failed!!! Error:{commandResult.ErrorOutput}", lastestBranch.Item1));
                }

                commandResult = GitCreatePersonalBranch(projectPath, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception(string.Format($"Project:{projectPath} Action:{CREATEPERSONALBRANCH} failed!!! Error:{commandResult.ErrorOutput}", projectPath));
                }

                commandResult = Init(projectPath, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception($"Project:{projectPath} Action:{INITCMD} failed!!! Error:{commandResult.ErrorOutput}");
                }

                foreach (var packageConfig in Directory.EnumerateFiles(Path.Combine(projectPath, "Source"), "packages.config", SearchOption.AllDirectories))
                {
                    commandResult = NugetPackageConfig(projectPath, packageConfig, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

                    if (commandResult.ExitCode != 0)
                    {
                        throw new Exception($"Project:{projectPath} Action:NugetPackageConfig config:{packageConfig} failed!!! Error:{commandResult.ErrorOutput}");
                    }
                }

                commandResult = UpdateExternalDrops(projectPath, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception($"Project:{projectPath} Action:{UpdateExternalDropsCMD} failed!!! Error:{commandResult.ErrorOutput}");
                }

                commandResult = RebulidAll(projectPath, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception($"Project:{projectPath} Action:ReBuildAll failed!!! Error:{commandResult.ErrorOutput}");
                }

                commandResult = CreatePackage(projectPath, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception($"Project:{projectPath} Action:CreatePackage failed!!! Error:{commandResult.ErrorOutput}");
                }

                commandResult = OpenReposSln(projectPath, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception($"Project:{projectPath} Action:{OPENREPOSSLN} failed!!! Error:{commandResult.ErrorOutput}");
                }

                CheckOutedLatestBranch checkOutedLatestBranch = new CheckOutedLatestBranch(true, lastestBranch.Item1, string.Format(PERSONAL, projectPath), lastestBranch.Item2, lastestBranch.Item3);

                //WriteFuctionName2Log(MethodBase.GetCurrentMethod().Name, logNotify, false);

                logNotify?.WriteLog($"Checkout Remote Product Branch={checkOutedLatestBranch.Product} Tag={checkOutedLatestBranch.Tag} LastModifiedTime={checkOutedLatestBranch.LastModifiedTime} into Local Personal Branch={checkOutedLatestBranch.Personal}");

                return new CommandResult(MethodBase.GetCurrentMethod().Name, 0, $"Success={checkOutedLatestBranch.Success} Product={checkOutedLatestBranch.Product} Personal={checkOutedLatestBranch.Personal} Tag={checkOutedLatestBranch.Tag} LastModifiedTime={checkOutedLatestBranch.LastModifiedTime}", null, commandResult.ProcessId);
            }
            else
            {
                throw new Exception($"Project:{projectPath} Action:Got latest branch failed!!! Error:latest branch");
            }
        }

        public static CommandResult PostBuild(string localFolder, Tuple<string, string, Version, Func<ICommandNotify, ILogNotify, CancellationTokenSource, CancellationTokenSource, Version>> remoteBranchInfo, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            if (remoteBranchInfo.Item3 == null && remoteBranchInfo.Item4 != null)
            {
                remoteBranchInfo = new Tuple<string, string, Version, Func<ICommandNotify, ILogNotify, CancellationTokenSource, CancellationTokenSource, Version>>(remoteBranchInfo.Item1, remoteBranchInfo.Item2, remoteBranchInfo.Item4(commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill), remoteBranchInfo.Item4);
            }

            if (remoteBranchInfo.Item3 == null)
            {
                throw new Exception($@"the tag for {remoteBranchInfo.Item2} is null !!!");
            }

            if (!File.Exists(Path.Combine(localFolder, PSOTBUILDCMD)))
            {
                throw new FileNotFoundException($"{ PSOTBUILDCMD} not found", Path.Combine(localFolder, PSOTBUILDCMD));
            }

            if (!File.Exists(Path.Combine(localFolder, PSOTBUILDPS1)))
            {
                throw new FileNotFoundException($"{ PSOTBUILDPS1} not found", Path.Combine(localFolder, PSOTBUILDPS1));
            }

            string deviceName = remoteBranchInfo.Item1;

            string branch = remoteBranchInfo.Item2;

            string version = $"{remoteBranchInfo.Item3}-{branch.Replace('/', '_')}";

            string dropPath = Path.Combine(DESMOBUILDFOLDER, branch, version).Replace('/', '\\');

            string postBuildPS1Conext = File.ReadAllText(Path.Combine(localFolder, PSOTBUILDPS1));

            CommandResult commandResult;

            bool hasServerBuild = false;
            string rsxFolder = null;
            string rsxx86Folder = null;
            string rsxPlatform64Folder = null;
            string dropPath64 = null;
            string dropPath86 = null;


            if (Directory.Exists(dropPath))
            {
                rsxFolder = Directory.EnumerateDirectories(dropPath, RSX, SearchOption.TopDirectoryOnly).FirstOrDefault();

                if (!string.IsNullOrEmpty(rsxFolder))
                {
                    rsxx86Folder = Directory.EnumerateDirectories(rsxFolder, RSXX86DEBUG, SearchOption.TopDirectoryOnly).FirstOrDefault();
                    rsxPlatform64Folder = Directory.EnumerateDirectories(rsxFolder, RSXPLATFORM64DEBUG, SearchOption.TopDirectoryOnly).FirstOrDefault();

                    if (!string.IsNullOrEmpty(rsxx86Folder) && !string.IsNullOrEmpty(rsxx86Folder))
                    {
                        hasServerBuild = true;
                    }
                }
            }

            if (!hasServerBuild)
            {
                throw new FileNotFoundException($"Server Build not found", dropPath);
            }

            string pattern86 = @"\$ManagedItemsPath\s+=\s+\$DropPath\s+\+\s+""(?<86>\S+86_Debug)";
            string pattern64 = @"\$NativeItemsPath\s+=\s+\$DropPath\s+\+\s+""(?<64>\S+64_Debug)";

            Regex regex86 = new Regex(pattern86);
            Regex regex64 = new Regex(pattern64);

            Match match86 = regex86.Match(postBuildPS1Conext);
            Match match64 = regex64.Match(postBuildPS1Conext);

            if (match86.Success)
            {
                int index86 = rsxx86Folder.IndexOf(match86.Groups["86"].Value.Replace('/', '\\'));
                if (index86 > 0)
                {
                    dropPath86 = rsxx86Folder.Substring(0, index86);
                }
            }

            if (match64.Success)
            {
                int index64 = rsxPlatform64Folder.IndexOf(match64.Groups["64"].Value.Replace('/', '\\'));
                if (index64 > 0)
                {
                    dropPath64 = rsxPlatform64Folder.Substring(0, index64);
                }
            }

            if (string.IsNullOrEmpty(dropPath64) || string.IsNullOrEmpty(dropPath86) || dropPath86 != dropPath64)
            {
                throw new Exception($"Pls check NativeItemsPath and ManagedItemsPath in {Path.Combine(localFolder, PSOTBUILDPS1)} mismatch with {rsxx86Folder} and {rsxPlatform64Folder} !!!");
            }

            if (Directory.Exists(Path.Combine(localFolder, string.Format(POSTBUILDPACKAGENAME, deviceName))))
            {
                Directory.Delete(Path.Combine(localFolder, string.Format(POSTBUILDPACKAGENAME, deviceName)), true);
            }

            string zipFileName = $"{version}_{string.Format(POSTBUILDPACKAGENAME, deviceName)}.zip";

            if (File.Exists(Path.Combine(localFolder, zipFileName)))
            {
                File.Delete(Path.Combine(localFolder, zipFileName));
            }

            commandResult = Run(localFolder, $"{PSOTBUILDCMD} {deviceName} {version} {dropPath86}", commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

            if (commandResult.ExitCode == 0)
            {
                if (!Directory.Exists(Path.Combine(localFolder, string.Format(POSTBUILDPACKAGENAME, deviceName))))
                {
                    throw new FileNotFoundException($"{string.Format(POSTBUILDPACKAGENAME, deviceName)} not found", Path.Combine(localFolder, string.Format(POSTBUILDPACKAGENAME, deviceName)));
                }

                ZipFile.CreateFromDirectory(Path.Combine(localFolder, string.Format(POSTBUILDPACKAGENAME, deviceName)), Path.Combine(localFolder, zipFileName));

                logNotify.WriteLog($"Please share: {zipFileName} in {localFolder}");
            }

            return commandResult;
        }

        public static CommandResult UploadProvisioningTools2Nuget(string projectPath,string buildScriptsPath, string id, string destination, Action<string> updateVersion, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            string buildScriptsFolder = buildScriptsPath;

            if (!Directory.Exists(buildScriptsFolder))
            {
                throw new FileNotFoundException($"{buildScriptsFolder} not found", buildScriptsFolder);
            }

            string CreateHashAndVersionCmd = Path.Combine(buildScriptsFolder, CREATEHASHANDVERSION);
            string PublishLocalBinariesCmd = Path.Combine(buildScriptsFolder, PUBLISHLOCALBINARIES);

            if (!File.Exists(CreateHashAndVersionCmd))
            {
                throw new FileNotFoundException($"{CREATEHASHANDVERSION} not found", CreateHashAndVersionCmd);
            }

            if (!File.Exists(PublishLocalBinariesCmd))
            {
                throw new FileNotFoundException($"{PUBLISHLOCALBINARIES} not found", PublishLocalBinariesCmd);
            }

            if (!Directory.Exists(destination) || Directory.GetDirectories(destination).Length == 0)
            {
                throw new FileNotFoundException($"provisioningTools folder not found or is an empty folder", destination);
            }

            Directory.GetFiles(destination, "*.nuspec", SearchOption.TopDirectoryOnly).ToList().ForEach(fileName => File.Delete(fileName));

            CommandResult commandResult = Run(Path.Combine(buildScriptsFolder), $@"{CREATEHASHANDVERSION} {destination}", commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

            if (commandResult.ExitCode == 0)
            {
                string newVersion = commandResult.StandOutput.Split(Environment.NewLine.ToCharArray()).Last(line => line.Contains("New Version:")).Split(':').Last().Trim();
                string[] idSplits = id.Split('_');
                string cmd = $@"{PUBLISHLOCALBINARIES} -Type {idSplits[0]} -Project {idSplits[1]} -Platform {idSplits[2]} -DropPath {destination}";

                commandResult = Run(Path.Combine(buildScriptsFolder), cmd, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

                if (commandResult.ExitCode == 0)
                {
                    updateVersion(newVersion);

                    Directory.Delete(destination, true);
                }
            }

            return commandResult;
        }

        public static CommandResult InstallSurfacePackage(Func<string, string> generatePackageConfig, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            Guid guid = Guid.NewGuid();
            string tempFolder = Path.Combine(GLOBALPACKAGEFOLDER, guid.ToString());

            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }

            string[] manifestResourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            for (int i = 0; i < manifestResourceNames.Length; i++)
            {
                if (manifestResourceNames[i].Contains(MANIFESTRESOURCEPATH))
                {
                    string fileName = manifestResourceNames[i].Replace(MANIFESTRESOURCEPATH, string.Empty);
                    using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(manifestResourceNames[i]))
                    {
                        string context;

                        using (StreamReader sr = new StreamReader(stream))
                        {
                            context = sr.ReadToEnd();
                        }

                        switch (fileName)
                        {
                            case PACKAGESCONFIG:
                            case INSTALLSURFACEPACKAGESCMD:
                                break;
                            case INSTALLSURFACEPACKAGESPS1:
                                context = string.Format(context, guid.ToString());
                                break;
                            default:
                                break;
                        }

                        using (FileStream fs = new FileStream(Path.Combine(tempFolder, fileName), FileMode.Create, FileAccess.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(fs))
                            {
                                sw.Write(context);
                            }
                        }
                    }
                }
            }

            string shareResult = generatePackageConfig(Path.Combine(tempFolder, PACKAGESCONFIG));

            CommandResult commandResult = Run(GLOBALPACKAGEFOLDER, $@".\{guid}\{INSTALLSURFACEPACKAGESCMD}", commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

            Directory.Delete(tempFolder, true);

            string pattern = @"Get-Item : (?<Error>Package '[\S\s]+?' is not found)";

            if (!string.IsNullOrEmpty(commandResult.ErrorOutput))
            {
                Regex regex = new Regex(pattern);

                Match match = regex.Match(commandResult.ErrorOutput);

                while (match.Success)
                {
                    logNotify.WriteLog(match.Groups["Error"].Value, true);
                    match = match.NextMatch();
                }

                commandResult = new CommandResult(commandResult.CommandName, -1, commandResult.StandOutput, commandResult.ErrorOutput, commandResult.ProcessId);
            }
            else
            {
                if (commandResult.ExitCode == 0)
                {
                    logNotify.WriteLog(shareResult);
                }
            }

            return commandResult;
        }

        //public static void WriteFuctionName2Log(string Name, ILogNotify logNotify, bool isStart = true)
        //{
        //    logNotify?.WriteLog(isStart ? $"{LOGSTASTR}{Name}{LOGSTASTR}" : $"{LOGENDSTR}{Name}{LOGENDSTR}");
        //}
    }

    public struct CommandResult
    {
        public CommandResult(string commandName, int exitCode, string standOutput, string errorOutput, int processId)
        {
            m_ExitCode = exitCode;
            m_StandOutput = standOutput;
            m_ErrorOutput = errorOutput;
            m_processId = processId;
            m_CommandName = commandName;
        }

        private int m_ExitCode;
        private string m_StandOutput;
        private string m_ErrorOutput;
        private int m_processId;
        private string m_CommandName;
        public int ExitCode => m_ExitCode;
        public int ProcessId => m_processId;
        public string StandOutput => m_StandOutput;

        public string ErrorOutput => m_ErrorOutput;

        public string CommandName => m_CommandName;
    }

    public struct CheckOutedLatestBranch
    {
        public CheckOutedLatestBranch(bool success, string product, string personal, DateTime? lastModifiedTime, Version tag)
        {
            m_Success = success;
            m_Product = product;
            m_Personal = personal;
            m_Tag = tag;
            m_LastModifiedTime = lastModifiedTime;
        }

        private bool m_Success;
        private string m_Product;

        private string m_Personal;

        private Version m_Tag;

        private DateTime? m_LastModifiedTime;

        public bool Success => m_Success;
        public string Product => m_Product;

        public string Personal => m_Personal;

        public Version Tag => m_Tag;

        public DateTime? LastModifiedTime => m_LastModifiedTime;
    }

    public interface ICommandNotify
    {
        void WriteOutPut(int processId, string outputLine);

        void WriteErrorOutPut(int processId, string errorLine);

        void Exit(int processId, int exitCode);
    }


    public interface ILogNotify
    {
        void WriteLog(string logLine, bool hasError = false);
        void WriteLog(Exception ex);
    }
}

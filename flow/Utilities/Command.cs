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

namespace Utilities
{
    public class Command
    {
        public const string CMDPATH = @"cmd.exe";
        //public const string CMDTITLE = @"title Command Prompt";
        public const string EWDKPATH = @"C:\17134.1.3";
        public const string EWDKCMD = @"LaunchBuildEnv.cmd";
        public const string CMD = "cmd";
        public const string NUGETPROVISIONINGCLIENTCMD = @"Nuget Install .\Source\ProvisioningClient\packages.config -ConfigFile Nuget.config -OutputDirectory .\packages";
        // public const string MAINWINDOWSTIELE = @"Administrator:  ""Vs2017 & WDK Build Env WDKContentRoot: C:\17134.1.3\Program Files\Windows Kits\10\""";
        public static readonly string REPOSFOLDER = $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\source\repos\";
        public static readonly string BUILDSCRIPTSFOLDER = Path.Combine($@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\source\repos\", "{0}", "BuildScripts");
        public const string OPENREPOSSLN = @"""C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv.exe"" Provisioning.sln";
        public const string REPOSSLN = @"Provisioning.sln";
        public const string PSOTBUILDCMD = @"PostBuildPackageGeneration.cmd";
        public const string PSOTBUILDPS1 = @"PostBuildPackageGeneration.ps1";
        //public const string BUILDX86 = @"""C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv"" Provisioning.sln /Build ""Debug|x86""";
        //public const string BUILDX64 = @"""C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv"" Provisioning.sln /Build ""Debug|x64""";
        public const string REBUILDX86 = @"""C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv"" Provisioning.sln /ReBuild ""Debug|x86""";
        public const string REBUILDX64 = @"""C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv"" Provisioning.sln /ReBuild ""Debug|x64""";
        public const string DEBUG = @"Debug";
        public const string EXTERNALDROPS = @"ExternalDrops";
        public const string DEBUGX86BIN = @"Debug\x86\bin";
        public const string DEBUGX64BIN = @"Debug\x64\bin";
        public const string CREATEPACKAGE = @"CreatePackage.cmd Debug";
        public const string CREATEPACKAGECMD = @"CreatePackage.cmd";
        public const string INITCMD = "init.cmd";
        public const string UpdateExternalDropsCMD = "UpdateExternalDrops.cmd";
        //public const string LOGSTASTR = "++++++++++++++++++++++++++++++++++++++++";
        //public const string LOGENDSTR = "----------------------------------------";
        public const string CLONEREPOS = @"git clone https://dev.azure.com/MSFTDEVICES/Vulcan/_git/DeviceProvisioning {0}";
        public const string LISTREMOTEBRANCH = "git branch -r";
        public const string GETBRANCHLOG = "git log --decorate=full {0}";
        public const string CHECKOUTBRANCH = "git checkout {0}";
        public const string FETCHBRANCH = "git fetch";
        public const string FETCHALL = "git fetch --all";
        public const string PULLBRANCH = "git pull";
        public static readonly string CREATEPERSONALBRANCH = $"git checkout -b personal/{new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)).Name}/{{0}}";
        public static readonly string PERSONAL = $"personal/{new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)).Name}/{{0}}";
        public const string PRODUCTBRANCHFILTER = "origin/product/{0}/";
        public const string DESMOBUILDFOLDER = @"\\desmo\release\Vulcan\CI_DeviceProvisioning\refs\heads\";
        public const string RS4 = @"RS4";
        public const string RS4X86DEBUG = @"Drop_RS4_x86_Debug";
        public const string RS4X64DEBUG = @"Drop_RS4_x64_Debug";
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

        static Command()
        {

        }


        public static int Run(string workDirectory, string cmd, string[] input, out int exitCode, out string standOutput, out string errorOutput, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
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

            AutoResetEvent outPutWaitHandle = new AutoResetEvent(false);

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
                        if (e.Data == null)
                        {
                            return;
                        }

                        if (cmd != null && cmd.Contains(string.Format(GETBRANCHLOG, string.Empty)) && tempStandOutput.ToString().Contains("commit") && tempStandOutput.ToString().Contains("Date:"))
                        {
                            return;
                        }

                        commandNotify?.WriteOutPut(((Process)sender).Id, e.Data); tempStandOutput.AppendLine(e.Data);
                    });

                p.ErrorDataReceived += new DataReceivedEventHandler(
                    (object sender, DataReceivedEventArgs e) =>
                    {
                        if (e.Data == null)
                        {
                            return;
                        }

                        commandNotify?.WriteErrorOutPut(((Process)sender).Id, e.Data);
                        tempErrorOutput.AppendLine(e.Data);
                    });

                p.Exited += new EventHandler(
                    (object sender, EventArgs e) =>
                    {
                        Thread.Sleep(5);

                        tempExitCode = ((Process)sender).ExitCode;
                        commandNotify?.Exit(((Process)sender).Id, ((Process)sender).ExitCode);

                        outPutWaitHandle.Set();
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

                //p.WaitForExit();
                if (cancellationTokenSourceForKill != null && cancellationTokenSourceForKill.Token != CancellationToken.None)
                {
                    while (!outPutWaitHandle.WaitOne(100))
                    {
                        if (cancellationTokenSourceForKill.Token.IsCancellationRequested)
                        {
                            p.Kill();
                        }
                    }
                }
                else
                {
                    outPutWaitHandle.WaitOne();
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

        public static CommandResult OpenReposSln(string projectName, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            if (!File.Exists(Path.Combine(REPOSFOLDER, projectName, REPOSSLN)))
            {
                throw new FileNotFoundException($"{REPOSSLN} not found", Path.Combine(REPOSFOLDER, projectName, REPOSSLN));
            }

            return Run(Path.Combine(REPOSFOLDER, projectName), OPENREPOSSLN, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill, true);
        }

        public static CommandResult NugetProvisioningClient(string projectName, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            if (!File.Exists(Path.Combine(REPOSFOLDER, projectName, REPOSSLN)))
            {
                throw new FileNotFoundException($"{REPOSSLN} not found", Path.Combine(REPOSFOLDER, projectName, REPOSSLN));
            }

            return Run(Path.Combine(REPOSFOLDER, projectName), NUGETPROVISIONINGCLIENTCMD, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
        }

        public static CommandResult RebulidX86(string projectName, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            if (!File.Exists(Path.Combine(REPOSFOLDER, projectName, REPOSSLN)))
            {
                throw new FileNotFoundException($"{REPOSSLN} not found", Path.Combine(REPOSFOLDER, projectName, REPOSSLN));
            }

            if (Directory.Exists(Path.Combine(REPOSFOLDER, projectName, DEBUGX86BIN)))
            {
                Directory.Delete(Path.Combine(REPOSFOLDER, projectName, DEBUGX86BIN), true);
            }

            return Run(Path.Combine(REPOSFOLDER, projectName), REBUILDX86, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill, true);
        }

        public static CommandResult RebulidX64(string projectName, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            if (!File.Exists(Path.Combine(REPOSFOLDER, projectName, REPOSSLN)))
            {
                throw new FileNotFoundException($"{ REPOSSLN} not found", Path.Combine(REPOSFOLDER, projectName, REPOSSLN));
            }

            if (Directory.Exists(Path.Combine(REPOSFOLDER, projectName, DEBUGX64BIN)))
            {
                Directory.Delete(Path.Combine(REPOSFOLDER, projectName, DEBUGX64BIN), true);
            }

            return Run(Path.Combine(REPOSFOLDER, projectName), REBUILDX64, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill, true);
        }

        public static CommandResult RebulidAll(string projectName, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            CommandResult commandResult;

            if (!File.Exists(Path.Combine(REPOSFOLDER, projectName, REPOSSLN)))
            {
                throw new FileNotFoundException($"{REPOSSLN} not found", Path.Combine(REPOSFOLDER, projectName, REPOSSLN));
            }

            if (Directory.Exists(Path.Combine(REPOSFOLDER, projectName, DEBUG)))
            {
                Directory.Delete(Path.Combine(REPOSFOLDER, projectName, DEBUG), true);
            }

            commandResult = RebulidX86(projectName, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

            if (commandResult.ExitCode == 0)
            {
                return RebulidX64(projectName, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
            }
            else
            {
                return commandResult;
            }
        }

        public static CommandResult CreatePackage(string projectName, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            if (!File.Exists(Path.Combine(REPOSFOLDER, projectName, CREATEPACKAGECMD)))
            {
                throw new FileNotFoundException($"{CREATEPACKAGECMD} not found", Path.Combine(REPOSFOLDER, projectName, CREATEPACKAGECMD));
            }

            if (!Directory.Exists(Path.Combine(REPOSFOLDER, projectName, DEBUGX64BIN)) || Directory.GetFiles(Path.Combine(REPOSFOLDER, projectName, DEBUGX64BIN)).Length == 0)
            {
                throw new FileNotFoundException($"{DEBUGX64BIN} not found", Path.Combine(REPOSFOLDER, projectName, DEBUGX64BIN));
            }

            if (!Directory.Exists(Path.Combine(REPOSFOLDER, projectName, DEBUGX86BIN)) || Directory.GetFiles(Path.Combine(REPOSFOLDER, projectName, DEBUGX86BIN)).Length == 0)
            {
                throw new FileNotFoundException($"{DEBUGX86BIN} not found", Path.Combine(REPOSFOLDER, projectName, DEBUGX86BIN));
            }

            if (Directory.Exists(Path.Combine(REPOSFOLDER, projectName, PROVISIONINGPACKAGE)))
            {
                Directory.Delete(Path.Combine(REPOSFOLDER, projectName, PROVISIONINGPACKAGE), true);
            }

            return Run(Path.Combine(REPOSFOLDER, projectName), CREATEPACKAGE, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
        }

        public static CommandResult Init(string projectName, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            if (!File.Exists(Path.Combine(REPOSFOLDER, projectName, INITCMD)))
            {
                throw new FileNotFoundException($"{INITCMD} not found", Path.Combine(REPOSFOLDER, projectName, INITCMD));
            }

            return Run(Path.Combine(REPOSFOLDER, projectName), INITCMD, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
        }

        public static CommandResult UpdateExternalDrops(string projectName, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            if (!File.Exists(Path.Combine(REPOSFOLDER, projectName, UpdateExternalDropsCMD)))
            {
                throw new FileNotFoundException($"{UpdateExternalDropsCMD} not found", Path.Combine(REPOSFOLDER, projectName, UpdateExternalDropsCMD));
            }

            //if (Directory.Exists(Path.Combine(REPOSFOLDER, projectName, EXTERNALDROPS)))
            //{
            //    Directory.Delete(Path.Combine(REPOSFOLDER, projectName, EXTERNALDROPS), true);
            //}

            string packageConfig = Path.Combine(REPOSFOLDER, projectName, PACKAGESCONFIG);

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

                                if (version.Revision == -1)
                                {
                                    version = new Version(version.Major, version.Minor, 0);
                                }

                                destination = Path.Combine(REPOSFOLDER, projectName, package.GetAttribute("destination"));
                                nugetspec = Path.Combine(destination, $"{id}.{version.ToString(3)}.nuspec");

                                if (Directory.Exists(destination) && !File.Exists(nugetspec))
                                {
                                    Directory.Delete(destination, true);
                                }
                            }
                );

            return Run(Path.Combine(REPOSFOLDER, projectName), UpdateExternalDropsCMD, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
        }

        public static CommandResult GitColne(string projectName, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            if (Directory.Exists(Path.Combine(REPOSFOLDER, projectName)) && (Directory.GetFiles(Path.Combine(REPOSFOLDER, projectName)).Length != 0 || Directory.GetDirectories(Path.Combine(REPOSFOLDER, projectName)).Length != 0))
            {
                throw new Exception($"{Path.Combine(REPOSFOLDER, projectName)} is not an empty folder");
            }

            return Run(REPOSFOLDER, string.Format(CLONEREPOS, projectName), commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
        }

        public static CommandResult GitRemoteBranchList(string projectName, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            if (!Directory.Exists(Path.Combine(REPOSFOLDER, projectName)) || !Directory.GetDirectories(Path.Combine(REPOSFOLDER, projectName)).Select(dir => (new DirectoryInfo(dir)).Name).Contains(".git"))
            {
                throw new Exception($"{Path.Combine(REPOSFOLDER, projectName)} is not an repos folder");
            }

            return Run(Path.Combine(REPOSFOLDER, projectName), LISTREMOTEBRANCH, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
        }

        public static CommandResult GitLog(string projectName, string branchName, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            if (!Directory.Exists(Path.Combine(REPOSFOLDER, projectName)) || !Directory.GetDirectories(Path.Combine(REPOSFOLDER, projectName)).Select(dir => (new DirectoryInfo(dir)).Name).Contains(".git"))
            {
                throw new Exception($"{Path.Combine(REPOSFOLDER, projectName)} is not an repos folder");
            }

            return Run(Path.Combine(REPOSFOLDER, projectName), string.Format(GETBRANCHLOG, branchName), commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
        }

        public static CommandResult GitCheckOut(string projectName, string branchName, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            if (!Directory.Exists(Path.Combine(REPOSFOLDER, projectName)) || !Directory.GetDirectories(Path.Combine(REPOSFOLDER, projectName)).Select(dir => (new DirectoryInfo(dir)).Name).Contains(".git"))
            {
                throw new Exception($"{Path.Combine(REPOSFOLDER, projectName)} is not an repos folder");
            }

            return Run(Path.Combine(REPOSFOLDER, projectName), string.Format(CHECKOUTBRANCH, branchName), commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
        }

        public static CommandResult GitFetch(string projectName, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            if (!Directory.Exists(Path.Combine(REPOSFOLDER, projectName)) || !Directory.GetDirectories(Path.Combine(REPOSFOLDER, projectName)).Select(dir => (new DirectoryInfo(dir)).Name).Contains(".git"))
            {
                throw new Exception($"{Path.Combine(REPOSFOLDER, projectName)} is not an repos folder");
            }

            return Run(Path.Combine(REPOSFOLDER, projectName), FETCHBRANCH, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
        }

        public static CommandResult GitFetchAll(string projectName, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            if (!Directory.Exists(Path.Combine(REPOSFOLDER, projectName)) || !Directory.GetDirectories(Path.Combine(REPOSFOLDER, projectName)).Select(dir => (new DirectoryInfo(dir)).Name).Contains(".git"))
            {
                throw new Exception($"{Path.Combine(REPOSFOLDER, projectName)} is not an repos folder");
            }

            return Run(Path.Combine(REPOSFOLDER, projectName), FETCHALL, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
        }

        public static CommandResult GitPull(string projectName, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            if (!Directory.Exists(Path.Combine(REPOSFOLDER, projectName)) || !Directory.GetDirectories(Path.Combine(REPOSFOLDER, projectName)).Select(dir => (new DirectoryInfo(dir)).Name).Contains(".git"))
            {
                throw new Exception($"{Path.Combine(REPOSFOLDER, projectName)} is not an repos folder");
            }

            return Run(Path.Combine(REPOSFOLDER, projectName), PULLBRANCH, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
        }

        public static CommandResult GitCreatePersonalBranch(string projectName, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            if (!Directory.Exists(Path.Combine(REPOSFOLDER, projectName)) || !Directory.GetDirectories(Path.Combine(REPOSFOLDER, projectName)).Select(dir => (new DirectoryInfo(dir)).Name).Contains(".git"))
            {
                throw new Exception($"{Path.Combine(REPOSFOLDER, projectName)} is not an repos folder");
            }

            return Run(Path.Combine(REPOSFOLDER, projectName), string.Format(CREATEPERSONALBRANCH, projectName), commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
        }

        public static CommandResult CheckOutLatestBranch(string projectName, string newBranchName = null, Tuple<string, DateTime?, Version> specificBranch = null, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {

            //WriteFuctionName2Log(MethodBase.GetCurrentMethod().Name, logNotify);


            string newProjectName = string.IsNullOrWhiteSpace(newBranchName) ? projectName : $"{projectName}_{newBranchName}";

            CommandResult commandResult;

            commandResult = GitColne(newProjectName, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

            if (commandResult.ExitCode != 0)
            {
                throw new Exception(string.Format($"Project:{newProjectName} Action:{CLONEREPOS} failed!!! Error:{commandResult.ErrorOutput}", newProjectName));
            }


            commandResult = GitFetchAll(newProjectName, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

            if (commandResult.ExitCode != 0)
            {
                throw new Exception($"Project:{newProjectName} Action:{FETCHALL} failed!!! Error:{commandResult.ErrorOutput}");
            }

            commandResult = GitRemoteBranchList(newProjectName, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

            if (commandResult.ExitCode != 0)
            {
                throw new Exception($"Project:{newProjectName} Action:{LISTREMOTEBRANCH} failed!!! Error:{commandResult.ErrorOutput}");
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
                    commandResult = GitLog(newProjectName, branches[i], commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

                    if (commandResult.ExitCode != 0)
                    {
                        throw new Exception(string.Format($"Project:{newProjectName} Action:{GETBRANCHLOG} failed!!! Error:{commandResult.ErrorOutput}", branches[i]));
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
                commandResult = GitCheckOut(newProjectName, lastestBranch.Item1, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception(string.Format($"Project:{newProjectName} Action:{CHECKOUTBRANCH} failed!!! Error:{commandResult.ErrorOutput}", lastestBranch.Item1));
                }

                commandResult = GitFetch(newProjectName, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception($"Project:{newProjectName} Action:{FETCHBRANCH} failed!!! Error:{commandResult.ErrorOutput}");
                }

                commandResult = GitPull(newProjectName, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception($"Project:{newProjectName} Action:{PULLBRANCH} failed!!! Error:{commandResult.ErrorOutput}");
                }

                commandResult = GitCheckOut(newProjectName, lastestBranch.Item1, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception(string.Format($"Project:{newProjectName} Action:{CHECKOUTBRANCH} failed!!! Error:{commandResult.ErrorOutput}", lastestBranch.Item1));
                }

                commandResult = GitCreatePersonalBranch(newProjectName, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception(string.Format($"Project:{newProjectName} Action:{CREATEPERSONALBRANCH} failed!!! Error:{commandResult.ErrorOutput}", newProjectName));
                }

                commandResult = Init(newProjectName, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception($"Project:{newProjectName} Action:{INITCMD} failed!!! Error:{commandResult.ErrorOutput}");
                }

                commandResult = UpdateExternalDrops(newProjectName, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception($"Project:{newProjectName} Action:{UpdateExternalDropsCMD} failed!!! Error:{commandResult.ErrorOutput}");
                }

                if (File.Exists(Path.Combine(REPOSFOLDER, projectName, @"Source\ProvisioningClient\packages.config")))
                {
                    commandResult = NugetProvisioningClient(newProjectName, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

                    if (commandResult.ExitCode != 0)
                    {
                        throw new Exception($"Project:{newProjectName} Action:NugetProvisioningClient failed!!! Error:{commandResult.ErrorOutput}");
                    }
                }

                commandResult = RebulidAll(newProjectName, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception($"Project:{newProjectName} Action:ReBuildAll failed!!! Error:{commandResult.ErrorOutput}");
                }

                commandResult = CreatePackage(newProjectName, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception($"Project:{newProjectName} Action:CreatePackage failed!!! Error:{commandResult.ErrorOutput}");
                }

                commandResult = OpenReposSln(newProjectName, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception($"Project:{newProjectName} Action:{OPENREPOSSLN} failed!!! Error:{commandResult.ErrorOutput}");
                }

                CheckOutedLatestBranch checkOutedLatestBranch = new CheckOutedLatestBranch(true, lastestBranch.Item1, string.Format(PERSONAL, newProjectName), lastestBranch.Item2, lastestBranch.Item3);

                //WriteFuctionName2Log(MethodBase.GetCurrentMethod().Name, logNotify, false);

                logNotify?.WriteLog($"Checkout Remote Product Branch={checkOutedLatestBranch.Product} Tag={checkOutedLatestBranch.Tag} LastModifiedTime={checkOutedLatestBranch.LastModifiedTime} into Local Personal Branch={checkOutedLatestBranch.Personal}");

                return new CommandResult(MethodBase.GetCurrentMethod().Name, 0, $"Success={checkOutedLatestBranch.Success} Product={checkOutedLatestBranch.Product} Personal={checkOutedLatestBranch.Personal} Tag={checkOutedLatestBranch.Tag} LastModifiedTime={checkOutedLatestBranch.LastModifiedTime}", null, commandResult.ProcessId);
            }
            else
            {
                throw new Exception($"Project:{newProjectName} Action:Got latest branch failed!!! Error:latest branch");
            }
        }

        public static CommandResult PostBuild(string localFolder, Tuple<string, string, Version> remoteBranchInfo, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
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

            if (!Directory.Exists(Path.Combine(dropPath, RS4, RS4X86DEBUG)) || !Directory.Exists(Path.Combine(dropPath, RS4, RS4X64DEBUG)))
            {
                throw new FileNotFoundException($"Server Build not found", Path.Combine(dropPath, RS4));
            }

            CommandResult commandResult;

            if (postBuildPS1Conext.Contains(Path.Combine(RS4, RS4X86DEBUG)) && postBuildPS1Conext.Contains(Path.Combine(RS4, RS4X64DEBUG)))
            {
                commandResult = Run(localFolder, $"{PSOTBUILDCMD} {deviceName} {version} {dropPath}\\", commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
            }
            else if (postBuildPS1Conext.Contains(RS4X86DEBUG) && postBuildPS1Conext.Contains(RS4X64DEBUG))
            {
                commandResult = Run(localFolder, $"{PSOTBUILDCMD} {deviceName} {version} {Path.Combine(dropPath, RS4)}\\", commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);
            }
            else
            {
                throw new Exception($"Pls check NativeItemsPath and ManagedItemsPath in {Path.Combine(localFolder, PSOTBUILDPS1)} path not include {RS4}");
            }

            if (commandResult.ExitCode == 0)
            {
                if (!Directory.Exists(Path.Combine(localFolder, string.Format(POSTBUILDPACKAGENAME, deviceName))))
                {
                    throw new FileNotFoundException($"{string.Format(POSTBUILDPACKAGENAME, deviceName)} not found", Path.Combine(localFolder, string.Format(POSTBUILDPACKAGENAME, deviceName)));
                }

                string zipFileName = $"{version}_{string.Format(POSTBUILDPACKAGENAME, deviceName)}.zip";

                ZipFile.CreateFromDirectory(Path.Combine(localFolder, string.Format(POSTBUILDPACKAGENAME, deviceName)), Path.Combine(localFolder, zipFileName));

                logNotify.WriteLog($"Please share: {zipFileName} in {localFolder}");
            }

            return commandResult;
        }

        public static CommandResult UploadProvisioningTools2Nuget(string project, string id, string destination, Action<string> updateVersion, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            string CreateHashAndVersionCmd = Path.Combine(string.Format(BUILDSCRIPTSFOLDER, project), CREATEHASHANDVERSION);
            string PublishLocalBinariesCmd = Path.Combine(string.Format(BUILDSCRIPTSFOLDER, project), PUBLISHLOCALBINARIES);
            string provisioningToolsFolder = Path.Combine(REPOSFOLDER, project, destination);

            if (!File.Exists(CreateHashAndVersionCmd))
            {
                throw new FileNotFoundException($"{CREATEHASHANDVERSION} not found", CreateHashAndVersionCmd);
            }

            if (!File.Exists(PublishLocalBinariesCmd))
            {
                throw new FileNotFoundException($"{PUBLISHLOCALBINARIES} not found", PublishLocalBinariesCmd);
            }

            if (!Directory.Exists(provisioningToolsFolder) || Directory.GetDirectories(provisioningToolsFolder).Length == 0)
            {
                throw new FileNotFoundException($"provisioningTools folder not found or is an empty folder", provisioningToolsFolder);
            }

            Directory.GetFiles(provisioningToolsFolder, "*.nuspec", SearchOption.TopDirectoryOnly).ToList().ForEach(fileName => File.Delete(fileName));

            CommandResult commandResult = Run(Path.Combine(string.Format(BUILDSCRIPTSFOLDER, project)), $@"{CREATEHASHANDVERSION} ..\{destination}", commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

            if (commandResult.ExitCode == 0)
            {
                string newVersion = commandResult.StandOutput.Split(Environment.NewLine.ToCharArray()).Last(line => line.Contains("New Version:")).Split(':').Last().Trim();
                string[] idSplits = id.Split('_');
                string cmd = $@"{PUBLISHLOCALBINARIES} -Type {idSplits[0]} -Project {idSplits[1]} -Platform {idSplits[2]} -DropPath ..\{destination}";

                commandResult = Run(Path.Combine(string.Format(BUILDSCRIPTSFOLDER, project)), cmd, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

                if (commandResult.ExitCode == 0)
                {
                    updateVersion(newVersion);

                    Directory.Delete(provisioningToolsFolder, true);
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

            CommandResult commandResult = Run(GLOBALPACKAGEFOLDER, Path.Combine(tempFolder, INSTALLSURFACEPACKAGESCMD), commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

            Directory.Delete(tempFolder, true);

            if (commandResult.ExitCode == 0)
            {
                logNotify.WriteLog(shareResult);
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
        void WriteLog(string logLine, bool showMessageBoxs = false);
        void WriteLog(Exception ex);
    }
}

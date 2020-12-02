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

namespace Utilities
{
    public class Command
    {
        public const string CMDPATH = @"cmd.exe";
        //public const string CMDTITLE = @"title Command Prompt";
        public const string EWDKPATH = @"C:\17134.1.3";
        public const string EWDKCMD = @"LaunchBuildEnv.cmd";
        public const string CMD = "cmd";
        // public const string MAINWINDOWSTIELE = @"Administrator:  ""Vs2017 & WDK Build Env WDKContentRoot: C:\17134.1.3\Program Files\Windows Kits\10\""";
        public static readonly string REPOSFOLDER = $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\source\repos\";
        public const string OPENREPOSSLN = @"""C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv.exe"" Provisioning.sln";
        public const string REPOSSLN = @"Provisioning.sln";
        public const string BUILDX86 = @"""C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv"" Provisioning.sln /Build ""Debug|x86""";
        public const string BUILDX64 = @"""C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv"" Provisioning.sln /Build ""Debug|x64""";
        public const string REBUILDX86 = @"""C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv"" Provisioning.sln /ReBuild ""Debug|x86""";
        public const string REBUILDX64 = @"""C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv"" Provisioning.sln /ReBuild ""Debug|x64""";
        public const string DEBUGX86BIN = @"Debug\x86\bin";
        public const string DEBUGX64BIN = @"Debug\x64\bin";
        public const string CREATEPACAKGE = @"CreatePackage.cmd Debug";
        public const string CREATEPACAKGECMD = @"CreatePackage.cmd";
        public const string INITCMD = "init.cmd";
        public const string UpdateExternalDropsCMD = "UpdateExternalDrops.cmd";
        //public const string LOGSTASTR = "++++++++++++++++++++++++++++++++++++++++";
        //public const string LOGENDSTR = "----------------------------------------";
        public const string CLONEREPOS = @"git clone https://dev.azure.com/MSFTDEVICES/Vulcan/_git/DeviceProvisioning {0}";
        public const string LISTREMOTEBRANCH = "git branch -r";
        public const string GETBRANCHLOG = "git log --decorate=full {0}";
        public const string CHECKOUTBRANCH = "git checkout {0}";
        public const string FETCHBRANCH = "git fetch";
        public const string PULLBRANCH = "git pull";
        public static readonly string CREATEPERSONALBRANCH = $"git checkout -b personal/{new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)).Name}/{{0}}";
        public static readonly string PERSONAL = $"personal/{new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)).Name}/{{0}}";
        public const string PRODUCTBRANCHFILTER = "origin/product/{0}/";
        private static Process eWDKProcess;
        private static string eWDKProcessWorkDirectory;

        static Command()
        {

        }


        public static int Run(string workDirectory, string cmd, out int exitCode, out string standOutput, out string errorOutput, bool output = true, bool exitWithOpenForm = false, WaitHandle waitHandle = null, bool waitForCloe = false, OutPutEnum outPutEnum = OutPutEnum.All, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null)
        {
            if (cancellationTokenSource != null && cancellationTokenSource.Token != CancellationToken.None)
            {
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
            }

            exitCode = int.MinValue;
            standOutput = null;
            errorOutput = null;

            int tempExitCode = int.MinValue;
            string tempStandOutput = null;
            string tempErrorOutput = null;

            //if (output == false && outPutEnum != OutPutEnum.None)
            //{
            //    throw new Exception("output is false but outPutEnum is not None");
            //}

            using (Process p = new Process())
            {
                p.StartInfo.FileName = CMDPATH;
                p.StartInfo.UseShellExecute = false;         //是否使用操作系统shell启动
                //p.StartInfo.Arguments=$" / k {cmd}";

                if (exitWithOpenForm)
                {
                    p.StartInfo.Arguments = $"/c start {cmd}";
                }
                else
                {
                    if (output)
                    {
                        p.StartInfo.Arguments = $"/c {cmd}";
                    }
                    else
                    {
                        p.StartInfo.Arguments = $"/k {cmd}";
                    }
                }

                if (output)
                {
                    p.StartInfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息
                    p.StartInfo.RedirectStandardOutput = true;  //由调用程序获取输出信息
                    p.StartInfo.RedirectStandardError = true;   //重定向标准错误输出
                    p.StartInfo.CreateNoWindow = true;          //不显示程序窗口        
                    //p.StartInfo.StandardErrorEncoding = Encoding.Default;
                    //p.StartInfo.StandardErrorEncoding = Encoding.Default;
                }
                else
                {
                    p.StartInfo.CreateNoWindow = false;          //不显示程序窗口
                }

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

                if (output && outPutEnum == OutPutEnum.Single)
                {
                    //p.EnableRaisingEvents = true;
                    p.OutputDataReceived += new DataReceivedEventHandler((object sender, DataReceivedEventArgs e) =>
                    {
                        if (cmd.Contains(string.Format(GETBRANCHLOG,string.Empty)) && tempStandOutput?.Count(c=>c=='\n')==3)
                        {
                            return;
                        }

                        commandNotify?.WriteOutPut(((Process)sender).Id, e.Data); tempStandOutput = e.Data == tempStandOutput ? null : $"{tempStandOutput}{e.Data}{Environment.NewLine}";
                    });
                    p.ErrorDataReceived += new DataReceivedEventHandler((object sender, DataReceivedEventArgs e) => { commandNotify?.WriteErrorOutPut(((Process)sender).Id, e.Data); tempErrorOutput = e.Data == null ? tempErrorOutput : $"{tempErrorOutput}{e.Data}{Environment.NewLine}"; });
                    p.Exited += new EventHandler((object sender, EventArgs e) => { tempExitCode = ((Process)sender).ExitCode; commandNotify?.Exit(((Process)sender).Id, ((Process)sender).ExitCode); });
                }

                p.Start();//启动程序

                if (output && outPutEnum == OutPutEnum.Single)
                {
                    commandNotify?.WriteOutPut(p.Id, $"{workDirectory}>{cmd}");
                }


                if (output)
                {
                    if (outPutEnum == OutPutEnum.All)
                    {
                        standOutput = $"{workDirectory}>{cmd}{Environment.NewLine}{p.StandardOutput.ReadToEnd()}";
                        errorOutput = p.StandardError.ReadToEnd();
                    }
                    else
                    {
                        p.BeginOutputReadLine();//开始读取输出数据
                        p.BeginErrorReadLine();//开始读取错误数据，重要！                  
                    }

                    p.WaitForExit();//等待程序执行完退出进程

                    if (outPutEnum == OutPutEnum.Single)
                    {
                        standOutput = tempStandOutput;
                        errorOutput = tempErrorOutput;
                    }

                    exitCode = p.ExitCode;
                }
                else
                {
                    if (waitForCloe)
                    {
                        //SpinWait.SpinUntil(() =>
                        //{
                        //    try
                        //    {
                        //        return Process.GetProcessById(p.Id) == null;
                        //    }
                        //    catch (Exception)
                        //    {
                        //        return true;
                        //    }
                        //});

                        p.WaitForExit();//等待程序执行完退出进程
                    }

                    waitHandle?.WaitOne();
                }

                int id = p.Id;

                p.Close();

                return id;
            }
        }

        public static CommandResult Run(string workDirectory, string cmd, bool output = true, WaitHandle waitHandle = null, bool waitForCloe = false, OutPutEnum outPutEnum = OutPutEnum.All, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null)
        {
            if (cancellationTokenSource != null && cancellationTokenSource.Token != CancellationToken.None)
            {
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
            }

            MethodBase method = new StackTrace().GetFrame(1).GetMethod();

            //WriteFuctionName2Log(method.Name, logNotify);

            int id = Run(workDirectory, cmd, out int exitCode, out string standOutput, out string errorOutput, output, false, waitHandle, waitForCloe, outPutEnum, commandNotify, logNotify, cancellationTokenSource);
            CommandResult commandResult = new CommandResult(exitCode, standOutput, errorOutput, id);

            // WriteFuctionName2Log(method.Name, logNotify, false);

            return commandResult;
        }

        public static CommandResult RunOneWDK(string workDirectory, string cmd, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null)
        {
            if (cancellationTokenSource != null && cancellationTokenSource.Token != CancellationToken.None)
            {
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
            }

            MethodBase method = new StackTrace().GetFrame(1).GetMethod();

            //WriteFuctionName2Log(method.Name, logNotify);

            if (eWDKProcess == null)
            {
                throw new Exception("please run eWDK first!!!");
            }

            if (!string.IsNullOrEmpty(workDirectory) && workDirectory != eWDKProcessWorkDirectory)
            {
                if (Directory.Exists(workDirectory))
                {
                    if (Path.GetPathRoot(workDirectory) != Path.GetPathRoot(eWDKProcessWorkDirectory))
                    {
                        eWDKProcess.StandardInput.WriteLine($"{Path.GetPathRoot(workDirectory)}：");
                    }

                    eWDKProcess.StandardInput.WriteLine($"cd {workDirectory}");

                    eWDKProcessWorkDirectory = workDirectory;
                }
                else
                {
                    throw new DirectoryNotFoundException($"{workDirectory} not found");
                }

            }

            eWDKProcess.StandardInput.WriteLine(cmd);

            //WriteFuctionName2Log(method.Name, logNotify, false);

            return new CommandResult(0, null, null, eWDKProcess.Id);
        }

        public static CommandResult RuneWDK(ICommandNotify commandNotify = null, ILogNotify logNotify = null, IMainWindowsTitleNotify mainWindowsTitleNotify = null, CancellationTokenSource cancellationTokenSource = null)
        {
            //WriteFuctionName2Log(MethodBase.GetCurrentMethod().Name, logNotify);

            if (!File.Exists(Path.Combine(EWDKPATH, EWDKCMD)))
            {
                throw new FileNotFoundException($"{EWDKCMD} not found", Path.Combine(EWDKPATH, EWDKCMD));
            }

            if (eWDKProcess != null)
            {
                eWDKProcess.Kill();
                eWDKProcess.Close();
                eWDKProcess = null;
            }

            eWDKProcess = new Process();
            Process p = eWDKProcess;

            p.StartInfo.FileName = CMDPATH;
            p.StartInfo.UseShellExecute = false;         //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;  //由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;   //重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;          //不显示程序窗口        
            p.StartInfo.Verb = "RunAs";
            p.StartInfo.WorkingDirectory = EWDKPATH;
            //p.EnableRaisingEvents = true;
            p.OutputDataReceived += new DataReceivedEventHandler((object sender, DataReceivedEventArgs e) =>
            {
                try
                {
                    commandNotify?.WriteOutPut(((Process)sender).Id, e.Data);
                }
                catch (Exception)
                {
                }
            });

            p.ErrorDataReceived += new DataReceivedEventHandler((object sender, DataReceivedEventArgs e) =>
            {
                try
                {
                    commandNotify?.WriteErrorOutPut(((Process)sender).Id, e.Data);
                }
                catch (Exception)
                {
                }
            });

            p.Exited += new EventHandler((object sender, EventArgs e) =>
            {
                try
                {
                    commandNotify?.Exit(((Process)sender).Id, ((Process)sender).ExitCode);
                }
                catch (Exception)
                {

                    throw;
                }
            });

            p.Start();//启动程序

            p.BeginOutputReadLine();//开始读取输出数据
            p.BeginErrorReadLine();//开始读取错误数据，重要！                  

            p.StandardInput.AutoFlush = true;
            p.StandardInput.WriteLine(EWDKCMD);

            eWDKProcessWorkDirectory = EWDKPATH;

            //Thread.Sleep(1000);

            mainWindowsTitleNotify.Write(p.MainWindowTitle);

            //WriteFuctionName2Log(MethodBase.GetCurrentMethod().Name, logNotify, false);

            return new CommandResult(0, null, null, eWDKProcess.Id);
        }

        public static CommandResult OpenReposSln(string projectName, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null)
        {
            if (!File.Exists(Path.Combine(REPOSFOLDER, projectName, REPOSSLN)))
            {
                throw new FileNotFoundException($"{REPOSSLN} not found", Path.Combine(REPOSFOLDER, projectName, REPOSSLN));
            }

            return RunOneWDK(Path.Combine(REPOSFOLDER, projectName), OPENREPOSSLN, logNotify, cancellationTokenSource);
        }

        public static CommandResult RebulidX86(string projectName, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null)
        {
            if (!File.Exists(Path.Combine(REPOSFOLDER, projectName, REPOSSLN)))
            {
                throw new FileNotFoundException($"{REPOSSLN} not found", Path.Combine(REPOSFOLDER, projectName, REPOSSLN));
            }

            return RunOneWDK(Path.Combine(REPOSFOLDER, projectName), REBUILDX86, logNotify, cancellationTokenSource);
        }

        public static CommandResult RebulidX64(string projectName, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null)
        {
            if (!File.Exists(Path.Combine(REPOSFOLDER, projectName, REPOSSLN)))
            {
                throw new FileNotFoundException($"{ REPOSSLN} not found", Path.Combine(REPOSFOLDER, projectName, REPOSSLN));
            }

            return RunOneWDK(Path.Combine(REPOSFOLDER, projectName), REBUILDX64, logNotify, cancellationTokenSource);
        }

        public static CommandResult RebulidAll(string projectName, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null)
        {
            if (!File.Exists(Path.Combine(REPOSFOLDER, projectName, REPOSSLN)))
            {
                throw new FileNotFoundException($"{REPOSSLN} not found", Path.Combine(REPOSFOLDER, projectName, REPOSSLN));
            }

            RebulidX86(projectName, logNotify, cancellationTokenSource);

            return RebulidX64(projectName, logNotify, cancellationTokenSource);
        }

        public static CommandResult CreatePacakge(string projectName, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null)
        {
            if (!File.Exists(Path.Combine(REPOSFOLDER, projectName, CREATEPACAKGECMD)))
            {
                throw new FileNotFoundException($"{CREATEPACAKGECMD} not found", Path.Combine(REPOSFOLDER, projectName, CREATEPACAKGECMD));
            }

            //if (!Directory.Exists(Path.Combine(REPOSFOLDER, projectName, DEBUGX64BIN)) || Directory.GetFiles(Path.Combine(REPOSFOLDER, projectName, DEBUGX64BIN)).Length == 0)
            //{
            //    throw new FileNotFoundException($"{DEBUGX64BIN} not found", Path.Combine(REPOSFOLDER, projectName, DEBUGX64BIN));
            //}

            //if (!Directory.Exists(Path.Combine(REPOSFOLDER, projectName, DEBUGX86BIN)) || Directory.GetFiles(Path.Combine(REPOSFOLDER, projectName, DEBUGX86BIN)).Length == 0)
            //{
            //    throw new FileNotFoundException($"{DEBUGX86BIN} not found", Path.Combine(REPOSFOLDER, projectName, DEBUGX86BIN));
            //}

            return RunOneWDK(Path.Combine(REPOSFOLDER, projectName), CREATEPACAKGE, logNotify, cancellationTokenSource);
        }

        public static CommandResult Init(string projectName, bool output = true, WaitHandle waitHandle = null, bool waitForCloe = false, OutPutEnum outPutEnum = OutPutEnum.All, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null)
        {
            if (!File.Exists(Path.Combine(REPOSFOLDER, projectName, INITCMD)))
            {
                throw new FileNotFoundException($"{INITCMD} not found", Path.Combine(REPOSFOLDER, projectName, INITCMD));
            }

            return Run(Path.Combine(REPOSFOLDER, projectName), INITCMD, output, waitHandle, waitForCloe, outPutEnum, commandNotify, logNotify, cancellationTokenSource);
        }

        public static CommandResult UpdateExternalDrops(string projectName, bool output = true, WaitHandle waitHandle = null, bool waitForCloe = false, OutPutEnum outPutEnum = OutPutEnum.All, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null)
        {
            if (!File.Exists(Path.Combine(REPOSFOLDER, projectName, UpdateExternalDropsCMD)))
            {
                throw new FileNotFoundException($"{UpdateExternalDropsCMD} not found", Path.Combine(REPOSFOLDER, projectName, UpdateExternalDropsCMD));
            }

            return Run(Path.Combine(REPOSFOLDER, projectName), UpdateExternalDropsCMD, output, waitHandle, waitForCloe, outPutEnum, commandNotify, logNotify, cancellationTokenSource);
        }

        public static CommandResult GitColne(string projectName, bool output = true, WaitHandle waitHandle = null, bool waitForCloe = false, OutPutEnum outPutEnum = OutPutEnum.All, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null)
        {
            if (Directory.Exists(Path.Combine(REPOSFOLDER, projectName)) && (Directory.GetFiles(Path.Combine(REPOSFOLDER, projectName)).Length != 0 || Directory.GetDirectories(Path.Combine(REPOSFOLDER, projectName)).Length != 0))
            {
                throw new Exception($"{Path.Combine(REPOSFOLDER, projectName)} is not an empty folder");
            }

            return Run(REPOSFOLDER, string.Format(CLONEREPOS, projectName), output, waitHandle, waitForCloe, outPutEnum, commandNotify, logNotify, cancellationTokenSource);
        }

        public static CommandResult GitRemoteBranchList(string projectName, bool output = true, WaitHandle waitHandle = null, bool waitForCloe = false, OutPutEnum outPutEnum = OutPutEnum.All, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null)
        {
            if (!Directory.Exists(Path.Combine(REPOSFOLDER, projectName)) || !Directory.GetDirectories(Path.Combine(REPOSFOLDER, projectName)).Select(dir => (new DirectoryInfo(dir)).Name).Contains(".git"))
            {
                throw new Exception($"{Path.Combine(REPOSFOLDER, projectName)} is not an repos folder");
            }

            return Run(Path.Combine(REPOSFOLDER, projectName), LISTREMOTEBRANCH, output, waitHandle, waitForCloe, outPutEnum, commandNotify, logNotify, cancellationTokenSource);
        }

        public static CommandResult GitLog(string projectName, string branchName, bool output = true, WaitHandle waitHandle = null, bool waitForCloe = false, OutPutEnum outPutEnum = OutPutEnum.All, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null)
        {
            if (!Directory.Exists(Path.Combine(REPOSFOLDER, projectName)) || !Directory.GetDirectories(Path.Combine(REPOSFOLDER, projectName)).Select(dir => (new DirectoryInfo(dir)).Name).Contains(".git"))
            {
                throw new Exception($"{Path.Combine(REPOSFOLDER, projectName)} is not an repos folder");
            }

            return Run(Path.Combine(REPOSFOLDER, projectName), string.Format(GETBRANCHLOG, branchName), output, waitHandle, waitForCloe, outPutEnum, commandNotify, logNotify, cancellationTokenSource);
        }

        public static CommandResult GitCheckOut(string projectName, string branchName, bool output = true, WaitHandle waitHandle = null, bool waitForCloe = false, OutPutEnum outPutEnum = OutPutEnum.All, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null)
        {
            if (!Directory.Exists(Path.Combine(REPOSFOLDER, projectName)) || !Directory.GetDirectories(Path.Combine(REPOSFOLDER, projectName)).Select(dir => (new DirectoryInfo(dir)).Name).Contains(".git"))
            {
                throw new Exception($"{Path.Combine(REPOSFOLDER, projectName)} is not an repos folder");
            }

            return Run(Path.Combine(REPOSFOLDER, projectName), string.Format(CHECKOUTBRANCH, branchName), output, waitHandle, waitForCloe, outPutEnum, commandNotify, logNotify, cancellationTokenSource);
        }

        public static CommandResult GitFetch(string projectName, bool output = true, WaitHandle waitHandle = null, bool waitForCloe = false, OutPutEnum outPutEnum = OutPutEnum.All, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null)
        {
            if (!Directory.Exists(Path.Combine(REPOSFOLDER, projectName)) || !Directory.GetDirectories(Path.Combine(REPOSFOLDER, projectName)).Select(dir => (new DirectoryInfo(dir)).Name).Contains(".git"))
            {
                throw new Exception($"{Path.Combine(REPOSFOLDER, projectName)} is not an repos folder");
            }

            return Run(Path.Combine(REPOSFOLDER, projectName), FETCHBRANCH, output, waitHandle, waitForCloe, outPutEnum, commandNotify, logNotify, cancellationTokenSource);
        }

        public static CommandResult GitPull(string projectName, bool output = true, WaitHandle waitHandle = null, bool waitForCloe = false, OutPutEnum outPutEnum = OutPutEnum.All, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null)
        {
            if (!Directory.Exists(Path.Combine(REPOSFOLDER, projectName)) || !Directory.GetDirectories(Path.Combine(REPOSFOLDER, projectName)).Select(dir => (new DirectoryInfo(dir)).Name).Contains(".git"))
            {
                throw new Exception($"{Path.Combine(REPOSFOLDER, projectName)} is not an repos folder");
            }

            return Run(Path.Combine(REPOSFOLDER, projectName), PULLBRANCH, output, waitHandle, waitForCloe, outPutEnum, commandNotify, logNotify, cancellationTokenSource);
        }

        public static CommandResult GitCreatePersonalBranch(string projectName, bool output = true, WaitHandle waitHandle = null, bool waitForCloe = false, OutPutEnum outPutEnum = OutPutEnum.All, ICommandNotify commandNotify = null, ILogNotify logNotify = null, CancellationTokenSource cancellationTokenSource = null)
        {
            if (!Directory.Exists(Path.Combine(REPOSFOLDER, projectName)) || !Directory.GetDirectories(Path.Combine(REPOSFOLDER, projectName)).Select(dir => (new DirectoryInfo(dir)).Name).Contains(".git"))
            {
                throw new Exception($"{Path.Combine(REPOSFOLDER, projectName)} is not an repos folder");
            }

            return Run(Path.Combine(REPOSFOLDER, projectName), string.Format(CREATEPERSONALBRANCH, projectName), output, waitHandle, waitForCloe, outPutEnum, commandNotify, logNotify, cancellationTokenSource);
        }

        public static CommandResult CheckOutLatestBranch(string projectName, ICommandNotify commandNotify = null, ILogNotify logNotify = null, string newBranchName = null, Tuple<string, DateTime?, Version> specificBranch = null, CancellationTokenSource cancellationTokenSource = null)
        {

            //WriteFuctionName2Log(MethodBase.GetCurrentMethod().Name, logNotify);

            bool output = true;
            WaitHandle waitHandle = null;
            bool waitForCloe = false;
            OutPutEnum outPutEnum = OutPutEnum.All;

            if (commandNotify != null)
            {
                outPutEnum = OutPutEnum.Single;
            }

            string newProjectName = string.IsNullOrWhiteSpace(newBranchName) ? projectName : $"{projectName}_{newBranchName}";

            CommandResult commandResult;

            commandResult = GitColne(newProjectName, output, waitHandle, waitForCloe, outPutEnum, commandNotify, logNotify, cancellationTokenSource);

            if (commandResult.ExitCode != 0)
            {
                throw new Exception(string.Format($"Project:{newProjectName} Action:{CLONEREPOS} failed!!! Error:{commandResult.ErrorOutput}", newProjectName));
            }

            commandResult = GitRemoteBranchList(newProjectName, output, waitHandle, waitForCloe, outPutEnum, commandNotify, logNotify, cancellationTokenSource);

            if (commandResult.ExitCode != 0)
            {
                throw new Exception($"Project:{newProjectName} Action:{LISTREMOTEBRANCH} failed!!! Error:{commandResult.ErrorOutput}");
            }

            string[] branches = commandResult.StandOutput.Split(Environment.NewLine.ToCharArray()).Where(branch => branch.Contains(string.Format(PRODUCTBRANCHFILTER, projectName))).ToArray();

            Tuple<string, DateTime?, Version> lastestBranch = new Tuple<string, DateTime?, Version>(null, null, null);

            Tuple<string, DateTime?, Version> tempBranch;


            if (specificBranch != null && specificBranch.Item1 != null && branches.Contains($"origin/{specificBranch.Item1}"))
            {
                lastestBranch = specificBranch;
            }
            else
            {
                for (int i = 0; i < branches.Length; i++)
                {
                    commandResult = GitLog(newProjectName, branches[i], output, waitHandle, waitForCloe, outPutEnum, commandNotify, logNotify, cancellationTokenSource);

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
                            if (!(lastestBranch.Item3 != null && tempBranch.Item3 != null && tempBranch.Item3 < lastestBranch.Item3))
                            {
                                lastestBranch = tempBranch;
                            }
                        }
                    }
                    else
                    {
                        if (lastestBranch.Item3 != null && tempBranch.Item3 != null && tempBranch.Item3 > lastestBranch.Item3)
                        {
                            lastestBranch = tempBranch;
                        }
                    }

                }
            }

            if (lastestBranch.Item1 != null)
            {
                commandResult = GitCheckOut(newProjectName, lastestBranch.Item1, output, waitHandle, waitForCloe, outPutEnum, commandNotify, logNotify, cancellationTokenSource);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception(string.Format($"Project:{newProjectName} Action:{CHECKOUTBRANCH} failed!!! Error:{commandResult.ErrorOutput}", lastestBranch.Item1));
                }

                commandResult = GitFetch(newProjectName, output, waitHandle, waitForCloe, outPutEnum, commandNotify, logNotify, cancellationTokenSource);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception($"Project:{newProjectName} Action:{FETCHBRANCH} failed!!! Error:{commandResult.ErrorOutput}");
                }

                commandResult = GitPull(newProjectName, output, waitHandle, waitForCloe, outPutEnum, commandNotify, logNotify, cancellationTokenSource);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception($"Project:{newProjectName} Action:{PULLBRANCH} failed!!! Error:{commandResult.ErrorOutput}");
                }

                commandResult = GitCheckOut(newProjectName, lastestBranch.Item1, output, waitHandle, waitForCloe, outPutEnum, commandNotify, logNotify, cancellationTokenSource);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception(string.Format($"Project:{newProjectName} Action:{CHECKOUTBRANCH} failed!!! Error:{commandResult.ErrorOutput}", lastestBranch.Item1));
                }

                commandResult = GitCreatePersonalBranch(newProjectName, output, waitHandle, waitForCloe, outPutEnum, commandNotify, logNotify, cancellationTokenSource);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception(string.Format($"Project:{newProjectName} Action:{CREATEPERSONALBRANCH} failed!!! Error:{commandResult.ErrorOutput}", newProjectName));
                }

                commandResult = Init(newProjectName, output, waitHandle, waitForCloe, outPutEnum, commandNotify, logNotify, cancellationTokenSource);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception($"Project:{newProjectName} Action:{INITCMD} failed!!! Error:{commandResult.ErrorOutput}");
                }

                commandResult = UpdateExternalDrops(newProjectName, output, waitHandle, waitForCloe, outPutEnum, commandNotify, logNotify, cancellationTokenSource);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception($"Project:{newProjectName} Action:{UpdateExternalDropsCMD} failed!!! Error:{commandResult.ErrorOutput}");
                }

                commandResult = OpenReposSln(newProjectName, logNotify, cancellationTokenSource);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception($"Project:{newProjectName} Action:{OPENREPOSSLN} failed!!! Error:{commandResult.ErrorOutput}");
                }

                logNotify?.WriteLog("Pls ReBuild Debug|x86 in opened solution,close solution after failed and then click ok button!!!", true);

                commandResult = RebulidAll(newProjectName, logNotify, cancellationTokenSource);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception($"Project:{newProjectName} Action:ReBuildAll failed!!! Error:{commandResult.ErrorOutput}");
                }

                commandResult = CreatePacakge(newProjectName, logNotify, cancellationTokenSource);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception($"Project:{newProjectName} Action:CreatePacakge failed!!! Error:{commandResult.ErrorOutput}");
                }

                commandResult = OpenReposSln(newProjectName, logNotify, cancellationTokenSource);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception($"Project:{newProjectName} Action:{OPENREPOSSLN} failed!!! Error:{commandResult.ErrorOutput}");
                }

                CheckOutedLatestBranch checkOutedLatestBranch = new CheckOutedLatestBranch(true, lastestBranch.Item1, string.Format(PERSONAL, newProjectName), lastestBranch.Item2, lastestBranch.Item3);

                //WriteFuctionName2Log(MethodBase.GetCurrentMethod().Name, logNotify, false);

                logNotify?.WriteLog($"Checkout Remote Product Branch={checkOutedLatestBranch.Product} Tag={checkOutedLatestBranch.Tag} LastModifiedTime={checkOutedLatestBranch.LastModifiedTime} into Local Personal Branch={checkOutedLatestBranch.Personal}");

                return new CommandResult(0, $"Success={checkOutedLatestBranch.Success} Product={checkOutedLatestBranch.Product} Personal={checkOutedLatestBranch.Personal} Tag={checkOutedLatestBranch.Tag} LastModifiedTime={checkOutedLatestBranch.LastModifiedTime}", null, commandResult.ProcessId);
            }
            else
            {
                throw new Exception($"Project:{newProjectName} Action:Got latest branch failed!!! Error:latest branch");
            }
        }

        //public static void WriteFuctionName2Log(string Name, ILogNotify logNotify, bool isStart = true)
        //{
        //    logNotify?.WriteLog(isStart ? $"{LOGSTASTR}{Name}{LOGSTASTR}" : $"{LOGENDSTR}{Name}{LOGENDSTR}");
        //}
    }

    public struct CommandResult
    {
        public CommandResult(int exitCode, string standOutput, string errorOutput, int processId)
        {
            m_ExitCode = exitCode;
            m_StandOutput = standOutput;
            m_ErrorOutput = errorOutput;
            m_processId = processId;
        }

        private int m_ExitCode;
        private string m_StandOutput;
        private string m_ErrorOutput;
        private int m_processId;
        public int ExitCode => m_ExitCode;
        public int ProcessId => m_processId;
        public string StandOutput => m_StandOutput;

        public string ErrorOutput => m_ErrorOutput;
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

    public interface IMainWindowsTitleNotify
    {
        void Write(string title);
    }

    public interface ILogNotify
    {
        void WriteLog(string logLine, bool showMessageBoxs = false);
        void WriteLog(Exception ex);
    }

    public enum OutPutEnum
    {
        None,
        All,
        Single
    }
}

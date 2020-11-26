using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

namespace Utilities
{
    public class Command
    {
        private const string CMDPATH = @"cmd.exe";
        //private const string CMDTITLE = @"title Command Prompt";
        private const string EWDKPATH = @"C:\17134.1.3";
        private const string EWDKCMD = @"LaunchBuildEnv.cmd";
        private const string CMD = "cmd";
        private const string MAINWINDOWSTIELE = @"Administrator:  ""Vs2017 & WDK Build Env WDKContentRoot: C:\17134.1.3\Program Files\Windows Kits\10\""";
        private static readonly string REPOSFOLDER = $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\source\repos\";
        private const string OPENREPOSSLN = @"""C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv.exe"" Provisioning.sln";
        private const string REPOSSLN = @"Provisioning.sln";
        private const string CREATEPACAKGE = @"CreatePackage.cmd Debug";
        private const string CREATEPACAKGECMD = @"CreatePackage.cmd";
        private const string INITCMD = "init.cmd";
        private const string UpdateExternalDropsCMD = "UpdateExternalDrops.cmd";
        private const string LOGSTASTR = "++++++++++++++++++++++++++++++++++++++++";
        private const string LOGENDSTR = "----------------------------------------";
       

        public static int Run(string workDirectory, string cmd, out int exitCode, out string standOutput, out string errorOutput, bool output = true, bool exitWithOpenForm = false, WaitHandle waitHandle = null, bool waitForCloe = false, OutPutEnum outPutEnum = OutPutEnum.All, ICommandNotify commandNotify = null)
        {
            exitCode = int.MinValue;
            standOutput = null;
            errorOutput = null;

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
                    p.OutputDataReceived += new DataReceivedEventHandler((object sender, DataReceivedEventArgs e) => { commandNotify.WriteOutPut(((Process)sender).Id, e.Data); });
                    p.ErrorDataReceived += new DataReceivedEventHandler((object sender, DataReceivedEventArgs e) => { commandNotify.WriteError(((Process)sender).Id, e.Data); });
                    p.Exited += new EventHandler((object sender, EventArgs e) => { commandNotify.Exit(((Process)sender).Id, ((Process)sender).ExitCode); });
                }

                p.Start();//启动程序

                if (output && outPutEnum == OutPutEnum.All)
                {
                    standOutput = p.StandardOutput.ReadToEnd();
                    errorOutput = p.StandardError.ReadToEnd();

                    p.WaitForExit();//等待程序执行完退出进程

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

        public static CommandResult Run(string workDirectory, string cmd, bool output = true, WaitHandle waitHandle = null, bool waitForCloe = false)
        {
            int id = Run(workDirectory, cmd, out int exitCode, out string standOutput, out string errorOutput, output, false, waitHandle, waitForCloe);
            CommandResult commandResult = new CommandResult(exitCode, standOutput, errorOutput, id);
            return commandResult;
        }

        public static int RunWitheWDK(string workDirectory, string cmd, out int exitCode, out string standOutput, out string errorOutput, bool output = true, bool exitWithOpenForm = false, WaitHandle waitHandle = null, bool waitForCloe = false, OutPutEnum outPutEnum = OutPutEnum.All, ICommandNotify commandNotify = null, ILogNotify logNotify = null)
        {

            RuneWDK(logNotify);

            return Run(workDirectory, cmd, out exitCode, out standOutput, out errorOutput, output, exitWithOpenForm, waitHandle, waitForCloe, outPutEnum, commandNotify);
        }

        public static CommandResult RunWitheWDK(string workDirectory, string cmd, bool output = true, WaitHandle waitHandle = null, bool waitForCloe = false, OutPutEnum outPutEnum = OutPutEnum.All, ICommandNotify commandNotify = null, ILogNotify logNotify = null)
        {
            int id = RunWitheWDK(workDirectory, cmd, out int exitCode, out string standOutput, out string errorOutput, output, false, waitHandle, waitForCloe, outPutEnum, commandNotify, logNotify);
            CommandResult commandResult = new CommandResult(exitCode, standOutput, errorOutput, id);
            return commandResult;
        }

        public static void RuneWDK(ILogNotify logNotify)
        {

            WriteFuctionName2Log(MethodBase.GetCurrentMethod().Name, logNotify);

            if (!File.Exists(Path.Combine(EWDKPATH, EWDKCMD)))
            {
                throw new FileNotFoundException($"{EWDKCMD} not found", Path.Combine(EWDKPATH, EWDKCMD));
            }


            int id;
            
            using (Process eWDKProcess = Process.GetProcessesByName(CMD).FirstOrDefault(p => p.MainWindowTitle == MAINWINDOWSTIELE))
            {               
                if (eWDKProcess == null)
                {
                    id = Run(EWDKPATH, EWDKCMD, out int exitCode, out string standOutput, out string errorOutput, false);
                }
                else
                {
                    id = eWDKProcess.Id;
                }                              
            }

            logNotify?.WriteLog($"eWDK ran by process:{id}");

            WriteFuctionName2Log(MethodBase.GetCurrentMethod().Name, logNotify, false);
        }

        public static int OpenReposSln(string projectName, ILogNotify logNotify = null)
        {
            if (!File.Exists(Path.Combine(REPOSFOLDER, projectName, REPOSSLN)))
            {
                throw new FileNotFoundException($"{REPOSSLN} not found", Path.Combine(REPOSFOLDER, projectName, REPOSSLN));
            }

            return RunWitheWDK(Path.Combine(REPOSFOLDER, projectName), OPENREPOSSLN, out int exitCode, out string standOutput, out string errorOutput, false, true, null, false, OutPutEnum.None, null, logNotify);
        }

        public static CommandResult CreatePacakge(string projectName, bool output = true, WaitHandle waitHandle = null, bool waitForCloe = false, OutPutEnum outPutEnum = OutPutEnum.All, ICommandNotify commandNotify = null, ILogNotify logNotify = null)
        {
            if (!File.Exists(Path.Combine(REPOSFOLDER, projectName, CREATEPACAKGECMD)))
            {
                throw new FileNotFoundException($"{CREATEPACAKGECMD} not found", Path.Combine(REPOSFOLDER, projectName, CREATEPACAKGECMD));
            }

            return RunWitheWDK(Path.Combine(REPOSFOLDER, projectName), CREATEPACAKGE, output, waitHandle, waitForCloe, outPutEnum, commandNotify, logNotify);
        }

        public static CommandResult Init(string projectName, bool output = true, WaitHandle waitHandle = null, bool waitForCloe = false, OutPutEnum outPutEnum = OutPutEnum.All, ICommandNotify commandNotify = null, ILogNotify logNotify = null)
        {
            if (!File.Exists(Path.Combine(REPOSFOLDER, projectName, INITCMD)))
            {
                throw new FileNotFoundException($"{INITCMD} not found", Path.Combine(REPOSFOLDER, projectName, INITCMD));
            }

            return RunWitheWDK(Path.Combine(REPOSFOLDER, projectName), INITCMD, output, waitHandle, waitForCloe, outPutEnum, commandNotify, logNotify);
        }

        public static CommandResult UpdateExternalDrops(string projectName, bool output = true, WaitHandle waitHandle = null, bool waitForCloe = false, OutPutEnum outPutEnum = OutPutEnum.All, ICommandNotify commandNotify = null, ILogNotify logNotify = null)
        {
            if (!File.Exists(Path.Combine(REPOSFOLDER, projectName, UpdateExternalDropsCMD)))
            {
                throw new FileNotFoundException($"{UpdateExternalDropsCMD} not found", Path.Combine(REPOSFOLDER, projectName, UpdateExternalDropsCMD));
            }

            return RunWitheWDK(Path.Combine(REPOSFOLDER, projectName), UpdateExternalDropsCMD, output, waitHandle, waitForCloe, outPutEnum, commandNotify, logNotify);
        }

        public static void WriteFuctionName2Log(string Name, ILogNotify logNotify, bool isStart = true)
        {
            logNotify?.WriteLog(isStart ? $"{LOGSTASTR}{Name}{LOGSTASTR}" : $"{LOGENDSTR}{Name}{LOGENDSTR}");
        }
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

    public interface ICommandNotify
    {
        void WriteOutPut(int processId, string outputLine);

        void WriteError(int processId, string errorLine);

        void Exit(int processId, int exitCode);
    }

    public interface ILogNotify
    {
        void WriteLog(string logLine);
    }

    public enum OutPutEnum
    {
        None,
        All,
        Single
    }
}

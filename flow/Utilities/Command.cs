using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static void Run(string workDirectory, string cmd, out int exitCode, out string standOutput, bool output = true, bool exitWithOpenForm = false)
        {
            exitCode = int.MinValue;
            standOutput = null;

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

                p.Start();//启动程序

                if (output)
                {
                    standOutput = p.StandardOutput.ReadToEnd();
                    string error = p.StandardError.ReadToEnd();

                    if (!string.IsNullOrEmpty(error))
                    {
                        error = Environment.NewLine + error;
                    }

                    standOutput += error;

                    p.WaitForExit();//等待程序执行完退出进程

                    exitCode = p.ExitCode;
                }

                p.Close();
            }
        }

        public static CommandResult Run(string workDirectory, string cmd, bool output = true)
        {
            Run(workDirectory, cmd, out int exitCode, out string standOutput, output);
            CommandResult commandResult = new CommandResult(exitCode, standOutput);
            return commandResult;
        }

        public static void RunWitheWDK(string workDirectory, string cmd, out int exitCode, out string standOutput, bool output = true, bool exitWithOpenForm = false)
        {

            RuneWDK();

            Run(workDirectory, cmd, out exitCode, out standOutput, output, exitWithOpenForm);
        }

        public static CommandResult RunWitheWDK(string workDirectory, string cmd, bool output = true)
        {
            RunWitheWDK(workDirectory, cmd, out int exitCode, out string standOutput, output);
            CommandResult commandResult = new CommandResult(exitCode, standOutput);
            return commandResult;
        }

        public static void RuneWDK()
        {
            Process eWDKProcess = Process.GetProcessesByName(CMD).FirstOrDefault(p => p.MainWindowTitle == MAINWINDOWSTIELE);

            if (eWDKProcess == null)
            {
                Run(EWDKPATH, EWDKCMD, out int exitCode, out string standOutput, false);
            }
        }

        public static void OpenReposSln(string projectName)
        {
            if (!File.Exists(Path.Combine(REPOSFOLDER, projectName, REPOSSLN)))
            {
                throw new FileNotFoundException($"{REPOSSLN} not found", Path.Combine(REPOSFOLDER, projectName, REPOSSLN));
            }

            RunWitheWDK(Path.Combine(REPOSFOLDER, projectName), OPENREPOSSLN, out int exitCode, out string standOutput, false, true);
        }

        public static CommandResult CreatePacakge(string projectName, bool output = true)
        {
            return RunWitheWDK(Path.Combine(REPOSFOLDER, projectName), CREATEPACAKGE, output);
        }

        public struct CommandResult
        {
            public CommandResult(int exitCode, string standOutput)
            {
                m_ExitCode = exitCode;
                m_StandOutput = standOutput;
                m_ErrorOutput = null;
            }

            public CommandResult(int exitCode, string standOutput, string errorOutput)
            {
                m_ExitCode = exitCode;
                m_StandOutput = standOutput;
                m_ErrorOutput = errorOutput;
            }

            private int m_ExitCode;
            private string m_StandOutput;
            private string m_ErrorOutput;
            public int ExitCode => m_ExitCode;
            public string StandOutput => m_StandOutput;

            public string ErrorOutput => m_ErrorOutput;
        }
    }
}

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
        private const string CMDPATH = @"C:\Windows\System32\cmd.exe";

        public static void Run(string workDirectory, string cmd, out int exitCode, out string standOutput)
        {
            exitCode = int.MinValue;
            standOutput = null;

            using (Process p = new Process())
            {
                p.StartInfo.FileName = CMDPATH;
                p.StartInfo.UseShellExecute = false;        //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;  //由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;   //重定向标准错误输出
                p.StartInfo.CreateNoWindow = false;          //不显示程序窗口
                p.StartInfo.Verb = "RunAs";

                p.Start();//启动程序


                if (Directory.Exists(workDirectory))
                {
                    p.StandardInput.WriteLine($"cd {workDirectory}");
                }
                else if (workDirectory != null)
                {
                    throw new DirectoryNotFoundException($"{workDirectory} not found");
                }

                //向cmd窗口写入命令
                p.StandardInput.WriteLine(cmd);

                p.StandardInput.WriteLine("exit");

                p.StandardInput.AutoFlush = true;

                //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
                //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令

                //获取cmd窗口的输出信息
                standOutput = p.StandardOutput.ReadToEnd();

                p.WaitForExit();//等待程序执行完退出进程

                exitCode = p.ExitCode;

                p.Close();
            }
        }
    }
}

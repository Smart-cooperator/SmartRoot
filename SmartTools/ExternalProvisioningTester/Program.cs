using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ExternalProvisioningTester
{
    class Program : IProvisioningTesterHandler
    {
        /// <summary>
        /// <summary>
        /// Command line args
        /// </summary>
        static CommandLineArgs commandLineArgs;
        static TaskRunManager taskRunManager;
        static Enhancelog log;
        bool hasError = false;

        static void Main(string[] args)
        {
            try
            {
                log = new Enhancelog();
                log.Start();

                if (args == null || args.Length == 0)
                {
                    string debug = @"ExternalProvisioningTester.exe -ProvisioningTesterPath C:\Users\v-fengzhou\source\repos\bayside\ProvisioningPackage\Debug\ProvisioningTester.exe -SN 033423602553 -IP 192.168.1.56 -Slot 5 -Task Rollback,Provision";
                    args = Regex
                                  .Matches(debug, @"(?<match>[^\s""]+)|""(?<match>([^\s""]|\s)*)""")
                                 .Cast<Match>()
                                 .Select(m => m.Groups["match"].Value)
                                 .ToArray();
                }

                if (args.Contains("-TaskOpList", StringComparer.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine(string.Join(",", Enum.GetNames(typeof(TaskOpCode))));
                    Environment.Exit(0);
                }
                else
                {
                    CommandLineParser commandLineParser = new CommandLineParser();
                    Program.commandLineArgs = commandLineParser.Parse(args);
                    taskRunManager = new TaskRunManager(commandLineArgs.TasksToRun);

                    taskRunManager.SetRequiredValue("SN", commandLineArgs.SystemDutSN, false);
                    taskRunManager.SetRequiredValue("IP", commandLineArgs.IPAddress, false);
                    taskRunManager.SetRequiredValue("Env", commandLineArgs.TestEnvironmentType, string.IsNullOrWhiteSpace(commandLineArgs.TestEnvironmentType));
                    int exitCode = new ProvisioningTesterWrapper(new Program(), log).Start(commandLineArgs.ProvisioningTesterPath);
                    log.Exit();
                    Environment.Exit(exitCode);
                }
            }
            catch (Exception ex)
            {
                log.AppendException(ex);

                log.Exit();

                if (ex.Data.Contains("ErrorNumber"))
                {
                    Environment.Exit(Convert.ToInt32(ex.Data["ErrorNumber"]));
                }
                else if (ex.HResult != 0)
                {
                    Environment.Exit(ex.HResult);
                }
                else
                {
                    Environment.Exit(1);
                }
            }
        }

        private static void KillProcessAndChildren(int pid)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }
            try
            {
                Process proc = Process.GetProcessById(pid);
                //Console.WriteLine(pid);
                proc.Kill();
            }
            catch (Exception)
            {
                /* process already exited */
            }
        }

        public Process ProvisioningTesterProcess { get; set; }

        public void HandelErrorOutput(string data)
        {
            log.Append(data, true);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void HandelStandOutput(string data)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data))
                {
                    log.Append(data);
                }
                else
                {
                    //data = taskRunManager.GetUnmarkedData(data);

                    if (!string.IsNullOrEmpty(data))
                    {
                        log.Append(data);
                    }
                }

                data = data.TrimEnd(Environment.NewLine.ToCharArray());

                if (!string.IsNullOrEmpty(data))
                {
                    List<Tuple<string, string>> tuples;

                    if (taskRunManager.AnalysisData(data, out tuples))
                    {
                        Action<List<Tuple<string, string>>> action = new Action<List<Tuple<string, string>>>(WriteLine);

                        action.BeginInvoke(tuples, null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                if (!hasError)
                {
                    Action action = () =>
                    {
                        Thread.Sleep(20);
                        log.AppendException(ex);
                        log.Exit();
                        KillProvisioningTesterProcess();
                    };

                    hasError = true;
                    action.BeginInvoke(null, null);
                }
            }
        }

        public void KillProvisioningTesterProcess()
        {
            if (ProvisioningTesterProcess != null && !ProvisioningTesterProcess.HasExited)
            {
                KillProcessAndChildren(ProvisioningTesterProcess.Id);
            }
        }

        public void WriteLine(List<Tuple<string, string>> tuples)
        {
            if (ProvisioningTesterProcess != null && !ProvisioningTesterProcess.HasExited)
            {
                foreach (var item in tuples)
                {
                    Thread.Sleep(10);

                    log.AppendLine(item.Item2);

                    ProvisioningTesterProcess.StandardInput.WriteLine(item.Item1);
                }
            }
        }

        public void StartReadLine()
        {
            Action action = () =>
            {
                try
                {
                    while (true)
                    {
                        if (ProvisioningTesterProcess != null && !ProvisioningTesterProcess.HasExited)
                        {
                            string inputLine = Console.ReadLine();
                            ProvisioningTesterProcess.StandardInput.WriteLine(inputLine);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                catch (Exception)
                {

                }
            };

            action.BeginInvoke(null, null);
        }
    }
}

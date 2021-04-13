using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace ProvisioningBuildTools.CLI
{
    public class CLIWrapper : IEnhancelogNotify
    {
        private CLIMode m_CurrentMode = CLIMode.NonProcessing;
        private Process m_CurrentProcess = null;
        private object CurrentModeLock = new object();
        private object CurrentProcessLock = new object();
        public CLIMode CurrentMode
        {
            get
            {
                lock (CurrentModeLock)
                {
                    return m_CurrentMode;
                }
            }
            set
            {
                lock (CurrentModeLock)
                {
                    m_CurrentMode = value;
                }
            }
        }
        public Process CurrentProcess
        {
            get
            {
                lock (CurrentProcessLock)
                {
                    return m_CurrentProcess;
                }
            }
            set
            {
                lock (CurrentProcessLock)
                {
                    m_CurrentProcess = value;
                }
            }
        }

        public List<string> NonProcessingHistory { get; set; } = new List<string>();
        public List<string> ProcessingHistory { get; set; } = new List<string>();

        public int NonProcessingHistoryIndex { get; set; } = -1;
        public int ProcessingHistoryIndex { get; set; } = -1;

        public ICLIWrapperNotify Notify { get; set; }
        public ILogNotify ILogNotify { get; set; }

        public const int HistoryCountLimit = 65536;

        public CLIFactory Factory { get; set; }

        public CLIWrapper(ICLIWrapperNotify notify, ILogNotify iLogNotify)
        {
            Notify = notify;
            ILogNotify = iLogNotify;
            Factory = new CLIFactory(iLogNotify);
        }

        public string GetHistory(bool upOrDown)
        {
            if (CurrentMode == CLIMode.NonProcessing)
            {
                if (upOrDown && NonProcessingHistoryIndex > 0)
                {
                    NonProcessingHistoryIndex--;
                }
                else if (!upOrDown && NonProcessingHistoryIndex < NonProcessingHistory.Count - 1)
                {
                    NonProcessingHistoryIndex++;
                }

                if (0 <= NonProcessingHistoryIndex && NonProcessingHistoryIndex <= NonProcessingHistory.Count - 1)
                {
                    string temp = NonProcessingHistory[NonProcessingHistoryIndex];

                    return temp;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (upOrDown && ProcessingHistoryIndex > 0)
                {
                    ProcessingHistoryIndex--;
                }
                else if (!upOrDown && ProcessingHistoryIndex < ProcessingHistory.Count - 1)
                {
                    ProcessingHistoryIndex++;
                }

                if (0 <= ProcessingHistoryIndex && ProcessingHistoryIndex <= ProcessingHistory.Count - 1)
                {
                    string temp = ProcessingHistory[ProcessingHistoryIndex];

                    return temp;
                }
                else
                {
                    return null;
                }
            }
        }

        public void HandleInput(string command)
        {
            AddHistory(command);

            if (CurrentMode == CLIMode.NonProcessing)
            {
                if (string.IsNullOrWhiteSpace(command))
                {
                    Notify.HandleNonProcessingSkip();
                }
                else
                {
                    AbCLIExecInstance cliExecInstance = Factory.CreateCLIExecInstance(command);

                    if (cliExecInstance == null)
                    {
                        Notify.HandleNonProcessingError(Factory.CreateResult);
                    }
                    else if (cliExecInstance.CLIExecEnum == CLIExecEnum.Help)
                    {
                        if (!cliExecInstance.IsHelp)
                        {
                            Notify.HandleNonProcessingHelp(Factory.CreateResult);
                        }
                        else
                        {
                            Notify.HandleNonProcessingHelp(cliExecInstance.ParseResult);
                        }
                    }
                    else if (cliExecInstance.CLIExecEnum == CLIExecEnum.Clear)
                    {
                        Notify.HandleNonProcessingClear();
                    }
                    else
                    {
                        cliExecInstance.Parse();

                        if (!cliExecInstance.ParseSuccess)
                        {
                            Notify.HandleNonProcessingError(cliExecInstance.ParseResult);
                        }
                        else if (cliExecInstance.IsHelp)
                        {
                            Notify.HandleNonProcessingHelp(cliExecInstance.ParseResult);
                        }
                        else
                        {
                            switch (cliExecInstance.CLIExecEnum)
                            {
                                case CLIExecEnum.Help:
                                    break;
                                case CLIExecEnum.Clear:
                                    break;
                                case CLIExecEnum.ChangeProject:
                                    Notify.HandleNonProcessingChangeProject(cliExecInstance.ParseResult);
                                    break;
                                case CLIExecEnum.OpenLocalProject:
                                case CLIExecEnum.BuildLocalProject:
                                case CLIExecEnum.CreatePackage:
                                case CLIExecEnum.UpdateExternalDrops:
                                case CLIExecEnum.GetRemoteProject:
                                case CLIExecEnum.GetProvisioningArtifact:
                                case CLIExecEnum.InstallSurfacePackage:
                                case CLIExecEnum.UploadNugetPackage:
                                case CLIExecEnum.CapsuleParser:
                                case CLIExecEnum.ProvisioningTester:
                                    Notify.HandleNonProcessingExec(cliExecInstance);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            else
            {
                if (CurrentProcess != null && !CurrentProcess.HasExited && CurrentProcess.StartInfo.RedirectStandardInput)
                {
                    CurrentProcess.StandardInput.WriteLine(command);
                }
            }
        }

        public AbCLIExecInstance FindInstance(string name)
        {
            return Factory.FindInstance(name);
        }

        public void AddHistory(string history)
        {
            if (!string.IsNullOrEmpty(history))
            {
                List<string> historyList;

                if (CurrentMode == CLIMode.NonProcessing)
                {
                    historyList = NonProcessingHistory;
                }
                else
                {
                    historyList = ProcessingHistory;
                }

                if (historyList.Contains(history))
                {
                    historyList.Remove(history);
                }

                if (historyList.Count >= HistoryCountLimit)
                {
                    historyList.RemoveAt(0);
                }

                historyList.Add(history);

                if (CurrentMode == CLIMode.NonProcessing)
                {
                    NonProcessingHistoryIndex = historyList.Count;
                }
                else
                {
                    ProcessingHistoryIndex = historyList.Count;
                }
            }
        }
    }

    public enum CLIMode
    {
        Processing,
        NonProcessing
    }

    public enum CLIExecEnum
    {
        Help,
        ChangeProject,
        Clear,
        OpenLocalProject,
        BuildLocalProject,
        CreatePackage,
        UpdateExternalDrops,
        GetRemoteProject,
        GetProvisioningArtifact,
        InstallSurfacePackage,
        UploadNugetPackage,
        CapsuleParser,
        ProvisioningTester,
    }

    public interface ICLIWrapperNotify
    {
        void HandleNonProcessingError(string errorMessgae);
        void HandleNonProcessingHelp(string help);
        void HandleNonProcessingException(Exception ex);
        void HandleNonProcessingSkip();
        void HandleNonProcessingClear();
        void HandleNonProcessingChangeProject(string project);
        void HandleNonProcessingExec(AbCLIExecInstance instance);
    }
}

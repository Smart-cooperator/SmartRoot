using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ProvisioningBuildTools;

namespace ProvisioningBuildTools.SelectInput
{
    public class SelectRemoteProjectInput
    {
        private Dictionary<string, string[]> m_Projects;
        public Dictionary<string, string[]> Projects => m_Projects;

        private Dictionary<string, BranchInfo> m_BranchInfos = new Dictionary<string, BranchInfo>();

        private string projectPath = null;
        private ICommandNotify commandNotify;
        private ILogNotify logNotify;
        private CommandResult commandResult;
        private CancellationTokenSource cancellationTokenSource = null;
        private CancellationTokenSource cancellationTokenSourceForKill = null;
        private Action startInvoke;
        private Action endInvoke;
        private BackGroundCommand backGroundCommand = new BackGroundCommand();

        private SelectLocalProjectInput LocalBranchInput;

        public SelectRemoteProjectInput(SelectLocalProjectInput localBranchInput, ICommandNotify commandNotify, ILogNotify logNotify, Action startInvoke, Action endInvoke)
        {
            this.commandNotify = commandNotify;
            this.logNotify = logNotify;
            this.startInvoke = startInvoke;
            this.endInvoke = endInvoke;

            LocalBranchInput = localBranchInput;

            Action actRun = new Action(GetAllRemoteAct);

            backGroundCommand.AsyncRun(actRun, startInvoke, endInvoke, cancellationTokenSource, logNotify, cancellationTokenSourceForKill);
        }


        public BranchInfo GetBranchInfo(string branchName, bool ifNotExistThrowException = false)
        {
            if (!m_BranchInfos.ContainsKey(branchName))
            {
                if (ifNotExistThrowException)
                {
                    throw new Exception($"couldn't find {branchName} information!!!");
                }
                else
                {
                    Action actRun = new Action(() => GetBranchInfoAct(branchName));

                    backGroundCommand.AsyncRun(actRun, startInvoke, endInvoke, cancellationTokenSource, logNotify, cancellationTokenSourceForKill);
                }
                return null;
            }
            else
            {
                return m_BranchInfos[branchName];
            }
        }

        public void WaitAndGetTag(string branchName)
        {
            Action actRun = new Action(() => WaitAndGetTagAct(branchName));

            backGroundCommand.AsyncRun(actRun, startInvoke, endInvoke, cancellationTokenSource, logNotify, cancellationTokenSourceForKill);
        }

        private void GetAllRemoteAct()
        {
            projectPath = LocalBranchInput.LocalBranches?.Where(name => name.ToUpper().Trim() == Global.GlobalValue.Root.SelectedProject?.ToUpper().Trim()).Select(name => LocalBranchInput.GetProjectInfo(name).SourceFolder).FirstOrDefault(folder => Utility.IsGitProjectPath(folder));
            projectPath = projectPath ?? LocalBranchInput.LocalBranches?.Select(name => LocalBranchInput.GetProjectInfo(name).SourceFolder).FirstOrDefault(folder => Utility.IsGitProjectPath(folder));

            if (string.IsNullOrEmpty(projectPath) || !Directory.Exists(projectPath))
            {
                projectPath = Path.Combine(Command.REPOSFOLDER, "DeviceProvisioning");

                if (!Utility.IsGitProjectPath(projectPath))
                {
                    commandResult = Command.GitColne(projectPath, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

                    if (commandResult.ExitCode != 0)
                    {
                        throw new Exception(string.Format($"Project:{projectPath} Action:{Command.CLONEREPOS} failed!!! Error:{commandResult.ErrorOutput}", projectPath));
                    }
                }
            }

            commandResult = Command.GitFetchAll(projectPath, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

            if (commandResult.ExitCode != 0)
            {
                throw new Exception($"Project:{projectPath} Action:{Command.FETCHALL} failed!!! Error:{commandResult.ErrorOutput}");
            }

            commandResult = Command.GitRemoteBranchList(projectPath, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

            if (commandResult.ExitCode != 0)
            {
                throw new Exception($"Project:{projectPath} Action:{Command.LISTREMOTEBRANCH} failed!!! Error:{commandResult.ErrorOutput}");
            }

            IGrouping<string, string>[] groupBranches = commandResult.StandOutput.Split(Environment.NewLine.ToCharArray()).Where(branch => branch.Contains(string.Format(Command.PRODUCTBRANCHFILTER.TrimEnd('/'), string.Empty))).GroupBy(branch => branch.Split('/')[2]).ToArray();

            m_Projects = groupBranches.ToDictionary<IGrouping<string, string>, string, string[]>(gorupItem => gorupItem.Key, gorupItem => gorupItem.Select(str => str.Trim()).ToArray());
        }

        private void GetBranchInfoAct(string branchName)
        {
            commandResult = Command.GitLog(projectPath, branchName, commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill);

            if (commandResult.ExitCode != 0)
            {
                throw new Exception(string.Format($"Project:{projectPath} Action:{Command.GETBRANCHLOG} failed!!! Error:{commandResult.ErrorOutput}", branchName));
            }
            string tempBranchName = branchName.Replace("origin/", string.Empty).Replace("/", "_").Trim();

            //string firstLine = commandResult.StandOutput.Split(Environment.NewLine.ToCharArray()).FirstOrDefault(line => line.ToLower().Contains("tag: refs/tags/"));
            string firstLine = commandResult.StandOutput.Split(Environment.NewLine.ToCharArray()).FirstOrDefault(line => line.ToLower().Contains("commit"));

            Version tag = null;

            if (!string.IsNullOrEmpty(firstLine))
            {
                string matchPattern = $"{Regex.Escape("tag: refs/tags/")}[0-9]+{Regex.Escape(".")}[0-9]+{Regex.Escape(".")}[0-9]+{Regex.Escape($"-{tempBranchName}")}[,)]";

                Match ma = Regex.Match(firstLine, matchPattern, RegexOptions.IgnoreCase);

                if (ma.Success)
                {
                    tag = new Version(ma.Value.ToUpper().Replace("tag: refs/tags/".ToUpper(), string.Empty).Replace($"-{tempBranchName}".ToUpper(), string.Empty).TrimEnd(new char[] { ',', ')' }));
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

            m_BranchInfos[branchName] = new BranchInfo(branchName.Replace("origin/", string.Empty).Trim(), dateTime, tag);
        }

        private void WaitAndGetTagAct(string branchName)
        {
            int sleepLoopCount = 0;

            //TimeSpan timeSpan = TimeSpan.FromMinutes(15).Subtract(DateTime.Now.Subtract((DateTime)m_BranchInfos[branchName].LastModifiedTime));
            TimeSpan timeSpan = TimeSpan.FromMinutes(8).Subtract(DateTime.Now.Subtract((DateTime)m_BranchInfos[branchName].LastModifiedTime));

            double preSleepMinutes = 0;

            if (timeSpan.TotalMinutes > 0)
            {
                preSleepMinutes = timeSpan.TotalMinutes;
            }
            else if (-timeSpan.TotalMinutes > TimeSpan.FromDays(1).TotalMinutes)
            {
                logNotify.WriteLog($"{branchName} Lastmodified time {(DateTime)m_BranchInfos[branchName].LastModifiedTime} over due 1 day, give up getting tag!!!", true);
                return;
            }

            logNotify.WriteLog($"Start to wait {branchName} Tag Created, delay {preSleepMinutes} minutes for server build.");

            sleepLoopCount = (int)(TimeSpan.FromMinutes(preSleepMinutes).TotalSeconds * 10);

            for (int i = 0; i < sleepLoopCount; i++)
            {
                if (cancellationTokenSource != null && cancellationTokenSource.IsCancellationRequested)
                {
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();
                }

                if (cancellationTokenSourceForKill != null && cancellationTokenSourceForKill.IsCancellationRequested)
                {
                    cancellationTokenSourceForKill.Token.ThrowIfCancellationRequested();
                }

                Thread.Sleep(100);
            }

            for (int i = 0; i < 15; i++)
            {
                logNotify.WriteLog($"try to get {branchName} Tag,current retried count is {i}, limit is 15");

                GetAllRemoteAct();

                GetBranchInfoAct(branchName);

                if (m_BranchInfos[branchName].Tag != null)
                {
                    logNotify.WriteLog($"Got {branchName} Tag {m_BranchInfos[branchName].Tag}, done");
                    break;
                }

                if (i == 14)
                {
                    logNotify.WriteLog($"Timed out to get {branchName} Tag, retried 15 count", true);
                }
                else
                {
                    logNotify.WriteLog($"Not got {branchName} Tag,current retried count is {i + 1}, limit is 15");
                    logNotify.WriteLog($"Delay 1 minute for next retry to get {branchName} Tag");

                    sleepLoopCount = (int)(TimeSpan.FromMinutes(1).TotalSeconds * 10);

                    for (int j = 0; j < sleepLoopCount; j++)
                    {
                        if (cancellationTokenSource != null && cancellationTokenSource.IsCancellationRequested)
                        {
                            cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        }

                        if (cancellationTokenSourceForKill != null && cancellationTokenSourceForKill.IsCancellationRequested)
                        {
                            cancellationTokenSourceForKill.Token.ThrowIfCancellationRequested();
                        }

                        Thread.Sleep(100);
                    }
                }
            }
        }

        public Func<ICommandNotify, ILogNotify, CancellationTokenSource, CancellationTokenSource, Version> GenerateWaitGetTagFunc(string branchName)
        {
            return new Func<ICommandNotify, ILogNotify, CancellationTokenSource, CancellationTokenSource, Version>(
                (commandNotify, logNotify, cancellationTokenSource, cancellationTokenSourceForKill) =>
                {
                    this.commandNotify = commandNotify;
                    this.logNotify = logNotify;
                    this.cancellationTokenSource = cancellationTokenSource;
                    this.cancellationTokenSourceForKill = cancellationTokenSourceForKill;

                    WaitAndGetTagAct(branchName);

                    return m_BranchInfos[branchName].Tag;
                }
                );
        }
    }
    public class BranchInfo
    {
        private string m_BranchName;
        private DateTime? m_LastModifiedTime;
        private Version m_Tag;

        public string BranchName => m_BranchName;
        public DateTime? LastModifiedTime => m_LastModifiedTime;
        public Version Tag => m_Tag;

        public BranchInfo(string branchName, DateTime? lastModifiedTime, Version tag)
        {
            m_BranchName = branchName;
            m_LastModifiedTime = lastModifiedTime;
            m_Tag = tag;
        }
    }

}

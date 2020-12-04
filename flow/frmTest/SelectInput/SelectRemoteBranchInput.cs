using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Utilities;

namespace ProvisioningBuildTools.SelectInput
{
    public class SelectRemoteBranchInput
    {
        private Dictionary<string, string[]> m_Projects;
        public Dictionary<string, string[]> Projects => m_Projects;

        private Dictionary<string, BranchInfo> m_BranchInfos = new Dictionary<string, BranchInfo>();

        private string newProjectName = null;
        private ICommandNotify commandNotify;
        private ILogNotify logNotify;
        private CommandResult commandResult;
        private CancellationTokenSource cancellationTokenSource = null;
        private Action startInvoke;
        private Action endInvoke;
        private BackGroundCommand backGroundCommand = new BackGroundCommand();

        public SelectRemoteBranchInput(ICommandNotify commandNotify, ILogNotify logNotify, Action startInvoke, Action endInvoke)
        {
            this.commandNotify = commandNotify;
            this.logNotify = logNotify;
            this.startInvoke = startInvoke;
            this.endInvoke = endInvoke;

            Action actRun = new Action(GetAllRemoteAct);

            backGroundCommand.AsyncRun(actRun, startInvoke, endInvoke, cancellationTokenSource, logNotify);
        }


        public BranchInfo GetBranchInfo(string branchName, bool ifNotExistThrowException = false)
        {
            if (!m_BranchInfos.ContainsKey(branchName))
            {
                if (ifNotExistThrowException)
                {
                    throw new Exception($"couldn't find {branchName} information!!!");
                }

                Action actRun = new Action(() => GetBranchInfoAct(branchName));

                backGroundCommand.AsyncRun(actRun, startInvoke, endInvoke, cancellationTokenSource, logNotify);

                return null;
            }
            else
            {
                return m_BranchInfos[branchName];
            }
        }

        private void GetAllRemoteAct()
        {
            SelectLocalBranchInput selectLocalBranchInput = new SelectLocalBranchInput();

            if (selectLocalBranchInput.LocalBranches == null || selectLocalBranchInput.LocalBranches.Length == 0)
            {
                newProjectName = "DeviceProvisioning";

                commandResult = Command.GitColne(newProjectName, commandNotify, logNotify, cancellationTokenSource);

                if (commandResult.ExitCode != 0)
                {
                    throw new Exception(string.Format($"Project:{newProjectName} Action:{Command.CLONEREPOS} failed!!! Error:{commandResult.ErrorOutput}", newProjectName));
                }
            }
            else
            {
                newProjectName = selectLocalBranchInput.LocalBranches[0];
            }

            commandResult = Command.GitRemoteBranchList(newProjectName, commandNotify, logNotify, cancellationTokenSource);

            if (commandResult.ExitCode != 0)
            {
                throw new Exception($"Project:{newProjectName} Action:{Command.LISTREMOTEBRANCH} failed!!! Error:{commandResult.ErrorOutput}");
            }

            IGrouping<string, string>[] groupBranches = commandResult.StandOutput.Split(Environment.NewLine.ToCharArray()).Where(branch => branch.Contains(string.Format(Command.PRODUCTBRANCHFILTER.TrimEnd('/'), string.Empty))).GroupBy(branch => branch.Split('/')[2]).ToArray();

            m_Projects = groupBranches.ToDictionary<IGrouping<string, string>, string, string[]>(gorupItem => gorupItem.Key, gorupItem => gorupItem.Select(str=>str.Trim()).ToArray());
        }

        private void GetBranchInfoAct(string branchName)
        {
            commandResult = Command.GitLog(newProjectName, branchName, commandNotify, logNotify, cancellationTokenSource);

            if (commandResult.ExitCode != 0)
            {
                throw new Exception(string.Format($"Project:{newProjectName} Action:{Command.GETBRANCHLOG} failed!!! Error:{commandResult.ErrorOutput}", branchName));
            }
            string tempBranchName = branchName.Replace("origin/", string.Empty).Replace("/", "_").Trim();

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

            m_BranchInfos[branchName] = new BranchInfo(branchName.Replace("origin/", string.Empty).Trim(), dateTime, tag);
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

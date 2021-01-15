using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProvisioningBuildTools.SelectOutput
{
    public class SelectRemoteBranchOutputPostBuild
    {
        private string m_SelectProject;
        public string SelectProject => m_SelectProject;

        private string m_SelectRemoteBranch;
        public string SelectRemoteBranch => m_SelectRemoteBranch;


        private string m_LocalBuildScriptsFolder;
        public string LocalBuildScriptsFolder => m_LocalBuildScriptsFolder;

        private Version m_Tag;
        public Version Tag => m_Tag;

        public SelectRemoteBranchOutputPostBuild(object selectProject, object selectRemoteBranch, string tag, object localBuildScriptsFolder)
        {
            m_SelectProject = selectProject?.ToString();

            if (m_SelectProject != null)
            {
                m_SelectProject = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(m_SelectProject);
            }

            m_SelectRemoteBranch = selectRemoteBranch?.ToString().Replace("origin/", string.Empty);
            m_Tag = string.IsNullOrWhiteSpace(tag) ? null : new Version(tag);
            m_LocalBuildScriptsFolder = localBuildScriptsFolder?.ToString();
        }
        public SelectRemoteBranchOutputPostBuild()
        {

        }
    }
}

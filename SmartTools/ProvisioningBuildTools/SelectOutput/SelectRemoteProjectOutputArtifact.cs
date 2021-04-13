using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ProvisioningBuildTools;

namespace ProvisioningBuildTools.SelectOutput
{
    public class SelectRemoteProjectOutputArtifact
    {
        private string m_SelectProject;
        public string SelectProject => m_SelectProject;

        private string m_SelectRemoteBranch;
        public string SelectRemoteBranch => m_SelectRemoteBranch;


        private string m_ProvisioningPackageFolder;
        public string ProvisioningPackageFolder => m_ProvisioningPackageFolder;

        private Version m_Tag;
        public Version Tag => m_Tag;

        private Func<ICommandNotify, ILogNotify, CancellationTokenSource, CancellationTokenSource, Version> mWaitGetTag;

        public Func<ICommandNotify, ILogNotify, CancellationTokenSource, CancellationTokenSource, Version> WaitGetTag => mWaitGetTag;

        public SelectRemoteProjectOutputArtifact(object selectProject, object selectRemoteBranch, string tag, string provisioningPackageFolder, Func<ICommandNotify, ILogNotify, CancellationTokenSource, CancellationTokenSource, Version> waitGetTag)
        {
            m_SelectProject = selectProject?.ToString();

            if (m_SelectProject != null)
            {
                m_SelectProject = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(m_SelectProject);
            }

            m_SelectRemoteBranch = selectRemoteBranch?.ToString().Replace("origin/", string.Empty);
            m_Tag = string.IsNullOrWhiteSpace(tag) ? null : new Version(tag);
            m_ProvisioningPackageFolder = provisioningPackageFolder;
            mWaitGetTag = waitGetTag;
        }

        public SelectRemoteProjectOutputArtifact()
        {

        }
    }
}

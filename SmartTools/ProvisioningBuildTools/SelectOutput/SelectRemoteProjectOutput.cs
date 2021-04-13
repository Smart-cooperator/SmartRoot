using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvisioningBuildTools.SelectOutput
{
    public class SelectRemoteProjectOutput
    {
        private string m_SelectProject;
        public string SelectProject => m_SelectProject;

        private string m_SelectRemoteBranch;
        public string SelectRemoteBranch => m_SelectRemoteBranch;

        private string m_NewBranchName;
        public string NewBranchName => m_NewBranchName;

        private Version m_Tag;
        public Version Tag => m_Tag;

        private DateTime? m_LastModifiedTime;
        public DateTime? LastModifiedTime => m_LastModifiedTime;

        public SelectRemoteProjectOutput(object selectProject, object selectRemoteBranch, string tag, string lastModifiedTime, string newBranchName)
        {
            m_SelectProject = selectProject?.ToString();
            m_SelectRemoteBranch = selectRemoteBranch?.ToString().Replace("origin/", string.Empty);
            m_Tag = string.IsNullOrWhiteSpace(tag) ? null : new Version(tag);
            m_LastModifiedTime = string.IsNullOrWhiteSpace(lastModifiedTime) ? null : (DateTime?)Convert.ToDateTime(lastModifiedTime);
            m_NewBranchName = newBranchName.Trim();
        }
        public SelectRemoteProjectOutput()
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvisioningBuildTools.SelectOutput
{
    public class SelectLocalBranchOutput
    {
        private string m_SelectedLocalBranch;
        public string SelectedLocalBranch => m_SelectedLocalBranch;

        private string m_ProvisioningToolsPackageId;
        public string ProvisioningToolsPackageId => m_ProvisioningToolsPackageId;

        private string m_ProvisioningToolsPackageDestination;
        public string ProvisioningToolsPackageDestination => m_ProvisioningToolsPackageDestination;

        private Action<string> m_UpdateNewVersionAction;
        public Action<string> UpdateNewVersionAction => m_UpdateNewVersionAction;

        public SelectLocalBranchOutput(string selectedLocalBranch)
        {
            m_SelectedLocalBranch = selectedLocalBranch;
        }

        public SelectLocalBranchOutput(string selectedLocalBranch,string provisioningToolsPackageId,string provisioningToolsPackageDestination,Action<string> updateNewVersionAction)
        {
            m_SelectedLocalBranch = selectedLocalBranch;
            m_ProvisioningToolsPackageId = provisioningToolsPackageId;
            m_ProvisioningToolsPackageDestination = provisioningToolsPackageDestination;
            m_UpdateNewVersionAction = updateNewVersionAction;
        }

        public SelectLocalBranchOutput()
        {

        }
    }
}

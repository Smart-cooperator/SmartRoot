using ProvisioningBuildTools.SelectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvisioningBuildTools.SelectOutput
{
    public class SelectLocalBranchOutput
    {
        private LocalProjectInfo m_SelectedLocalProjectInfo;
        public LocalProjectInfo SelectedLocalProjectInfo => m_SelectedLocalProjectInfo;

        private string m_ProvisioningToolsPackageId;
        public string ProvisioningToolsPackageId => m_ProvisioningToolsPackageId;

        private string m_ProvisioningToolsPackageDestination;
        public string ProvisioningToolsPackageDestination => m_ProvisioningToolsPackageDestination;

        private Action<string> m_UpdateNewVersionAction;
        public Action<string> UpdateNewVersionAction => m_UpdateNewVersionAction;

        public SelectLocalBranchOutput(LocalProjectInfo selectedLocalProjectInfo)
        {
            m_SelectedLocalProjectInfo = selectedLocalProjectInfo;
        }

        public SelectLocalBranchOutput(LocalProjectInfo selectedLocalProjectInfo, string provisioningToolsPackageId,string provisioningToolsPackageDestination,Action<string> updateNewVersionAction)
        {
            m_SelectedLocalProjectInfo = selectedLocalProjectInfo;
            m_ProvisioningToolsPackageId = provisioningToolsPackageId;
            m_ProvisioningToolsPackageDestination = provisioningToolsPackageDestination;
            m_UpdateNewVersionAction = updateNewVersionAction;
        }

        public SelectLocalBranchOutput()
        {

        }
    }
}

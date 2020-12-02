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

        public SelectLocalBranchOutput(string selectedLocalBranch)
        {
            m_SelectedLocalBranch = selectedLocalBranch;
        }
        public SelectLocalBranchOutput()
        {

        }
    }
}

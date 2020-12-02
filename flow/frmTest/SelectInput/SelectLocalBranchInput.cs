using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace ProvisioningBuildTools.SelectInput
{
    public class SelectLocalBranchInput
    {
        private string[] m_LocalBranches;
        public string[] LocalBranches => m_LocalBranches;

        public SelectLocalBranchInput()
        {
            m_LocalBranches = Directory.GetDirectories(Command.REPOSFOLDER).Where(dir => File.Exists(Path.Combine(dir, Command.REPOSSLN))).Select(dir => new DirectoryInfo(dir).Name).ToArray();
        }
    }
}

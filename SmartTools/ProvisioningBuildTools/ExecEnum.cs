using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvisioningBuildTools
{
    public enum ExecEnum
    {
        OpenLocalBranch,
        BuildLocalBranch,
        UpdateExternalDrops,
        DropRemoteBranch,
        PostBuildPackage,
        InstallSurfacePackage,
        UploadProvisionTools,        
    }
}

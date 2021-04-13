using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvisioningBuildTools
{
    public enum ExecEnum
    {
        OpenLocalProject,    
        BuildLocalProject,
        CreatePackage,
        UpdateExternalDrops,
        GetRemoteProject,
        GetProvisioningArtifact,
        //PostBuildPackage,
        InstallSurfacePackage,
        UploadNugetPackage,
        CapsuleParser,
        ProvisioningTester,
    }
}

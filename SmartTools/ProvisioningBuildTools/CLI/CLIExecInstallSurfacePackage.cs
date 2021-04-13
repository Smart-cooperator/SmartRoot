using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvisioningBuildTools.CLI
{
    public class CLIExecInstallSurfacePackage : AbCLIExecInstance
    {
        public CLIExecInstallSurfacePackage(CLIFactory cliFactory, CLIExecEnum cliExecEnum) : base(cliFactory, cliExecEnum)
        {
            Alias = new string[] { "isp", "ip", "is" };
            RequiredParas = new string[] { "function", "package" };
            SupportedParas = new string[] { "help", "function", "package" };

            ParameterAlias["package"] = new string[] { "id" };
            Samples = new string[] { $"{cliExecEnum} \"Install-Package Devices.Cardinal.Driver.SurfaceME.RS5.Test -version 11.8.70.3626\"",
                                     $"{cliExecEnum} \"Install-SurfacePackage Devices.Cruz.Driver.SurfaceSAM.RSx.Test.RollBack -version 241.6.139.0 -Source Cruz\"",
                                     $"{cliExecEnum} \"Install-Package Microsoft.Shared.eWDK.RS4_RELEASE -version 17134.1.3\"",
                                     $@"{cliExecEnum} [f:Cruz][p:Devices.Cruz.Driver.SurfaceSAM.RSx.Test.RollBack][v:241.6.139.0][a:content\amd64\release\drv\SurfaceSAM.inf",
                                     $"{cliExecEnum} \"Devices.Cardinal.Driver.SurfaceME.RS5.Test Cardinal 11.8.70.3626\"",
                                     $"{cliExecEnum} \"...,...,...\"",};
            CommandLineFormat = $"{cliExecEnum} {{package}}";
            DefaultParameter = "package";

            DistinctParameterAlias();
        }

        public override void Parse()
        {
            base.Parse();
            string[] args = CLIFactory.Args;

            if (ParseSuccess && args != null && args.Length == 2 && GetParameterValue("package") == null)
            {
                Parameters["package"] = args[1];
            }
        }
    }
}

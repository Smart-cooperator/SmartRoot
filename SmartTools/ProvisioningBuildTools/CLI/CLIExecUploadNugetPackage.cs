using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvisioningBuildTools.CLI
{
    public class CLIExecUploadNugetPackage : AbCLIExecInstance
    {
        public CLIExecUploadNugetPackage(CLIFactory cliFactory, CLIExecEnum cliExecEnum) : base(cliFactory, cliExecEnum)
        {
            Alias = new string[] { "u2", "un", "up", "unp" };
            RequiredParas = new string[] { "function", "project" };
            SupportedParas = new string[] { "help", "function", "project", "package", "append", "force" };
            ParameterAlias["package"] = new string[] { "id","p2"};
            Samples = new string[] { $"{cliExecEnum} paloma -package ProvisioningTools,DeviceBridge,DBProxySdk -append true -force true" };
            CommandLineFormat = $"{cliExecEnum} {{project}} -package {{package}} -append {{append}} -force {{force}}";

            DistinctParameterAlias();
        }
    }
}

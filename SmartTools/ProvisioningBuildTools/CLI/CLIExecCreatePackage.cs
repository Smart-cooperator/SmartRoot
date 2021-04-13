using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvisioningBuildTools.CLI
{
    public class CLIExecCreatePackage : AbCLIExecInstance
    {
        public CLIExecCreatePackage(CLIFactory cliFactory, CLIExecEnum cliExecEnum) : base(cliFactory, cliExecEnum)
        {
            Alias = new string[] { "c2", "cpackage", "cpac", "cpack" };
            RequiredParas = new string[] { "function", "project" };
            SupportedParas = new string[] { "help", "function", "project" };
            Samples = new string[] { $"{cliExecEnum} paloma" };
            CommandLineFormat = $"{cliExecEnum} {{project}}";

            DistinctParameterAlias();
        }
    }
}

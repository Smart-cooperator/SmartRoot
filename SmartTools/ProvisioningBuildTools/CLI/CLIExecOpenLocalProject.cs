using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvisioningBuildTools.CLI
{
    public class CLIExecOpenLocalProject : AbCLIExecInstance
    {
        public CLIExecOpenLocalProject(CLIFactory cliFactory, CLIExecEnum cliExecEnum) : base(cliFactory, cliExecEnum)
        {
            Alias = new string[] { "ol", "op", "olp" };
            RequiredParas = new string[] { "function", "project" };
            SupportedParas = new string[] { "help", "function", "project" };
            Samples = new string[] { $"{cliExecEnum} paloma" };
            CommandLineFormat = $"{cliExecEnum} {{project}}";

            DistinctParameterAlias();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvisioningBuildTools.CLI
{
    public class CLIExecUpdateExternalDrops : AbCLIExecInstance
    {
        public CLIExecUpdateExternalDrops(CLIFactory cliFactory, CLIExecEnum cliExecEnum) : base(cliFactory, cliExecEnum)
        {
            Alias = new string[] { "u","u1", "ue", "ud", "ued" };
            RequiredParas = new string[] { "function", "project" };
            SupportedParas = new string[] { "help", "function", "project", "append" };
            Samples = new string[] { $"{cliExecEnum} paloma -append true" };
            CommandLineFormat = $"{cliExecEnum} {{project}} -append {{append}}";

            DistinctParameterAlias();
        }
    }
}

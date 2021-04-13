using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvisioningBuildTools.CLI
{
    public class CLIExecHelp : AbCLIExecInstance
    {
        public CLIExecHelp(CLIFactory cliFactory, CLIExecEnum cliExecEnum) : base(cliFactory, cliExecEnum)
        {
            Alias = new string[] {"?"};
            RequiredParas = new string[] { "function"};
            SupportedParas = new string[] { "help", "function" };
            Samples = new string[] { $"{cliExecEnum}" };

            DistinctParameterAlias();
        }
    }
}

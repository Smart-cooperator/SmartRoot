using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvisioningBuildTools.CLI
{
    public class CLIExecBuildLocalProject : AbCLIExecInstance
    {
        public CLIExecBuildLocalProject(CLIFactory cliFactory, CLIExecEnum cliExecEnum) : base(cliFactory, cliExecEnum)
        {
            Alias = new string[] { "bl", "bp", "blp" };
            RequiredParas = new string[] { "function", "project" };
            SupportedParas = new string[] { "help", "function", "project", "append" };
            Samples = new string[] { $"{cliExecEnum} paloma -append true" };
            CommandLineFormat = $"{cliExecEnum} {{project}} -append {{append}}";

            DistinctParameterAlias();
        }
    }
}

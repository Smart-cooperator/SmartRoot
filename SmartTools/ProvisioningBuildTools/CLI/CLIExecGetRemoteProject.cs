using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvisioningBuildTools.CLI
{
    public class CLIExecGetRemoteProject : AbCLIExecInstance
    {
        public CLIExecGetRemoteProject(CLIFactory cliFactory, CLIExecEnum cliExecEnum) : base(cliFactory, cliExecEnum)
        {
            Alias = new string[] { "g2", "gr", "gp", "grp" };
            RequiredParas = new string[] { "function", "project", "branch" };
            SupportedParas = new string[] { "help", "function", "project", "branch", "newbranchname" };
            ParameterAlias["branch"] = Enumerable.Empty<string>().ToArray();
            ParameterAlias["newbranchname"] = new string[] { "name" };
            Samples = new string[] { $"{cliExecEnum} paloma -branch main",
                                     $"{cliExecEnum} paloma -branch main -newbranchname main" };
            CommandLineFormat = $"{cliExecEnum} {{project}} -branch {{branch}} -newbranchname {{newbranchname}}";

            DistinctParameterAlias();
        }
    }
}

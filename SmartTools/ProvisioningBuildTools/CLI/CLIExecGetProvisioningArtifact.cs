using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvisioningBuildTools.CLI
{
    public class CLIExecGetProvisioningArtifact : AbCLIExecInstance
    {
        public CLIExecGetProvisioningArtifact(CLIFactory cliFactory, CLIExecEnum cliExecEnum) : base(cliFactory, cliExecEnum)
        {
            Alias = new string[] {"g", "g1", "gpa", "pa", "ga", "getartifact", "downloadpacakage" };
            RequiredParas = new string[] { "function", "project", "branch" };
            SupportedParas = new string[] { "help", "function", "project", "branch" };
            ParameterAlias["branch"] = Enumerable.Empty<string>().ToArray();
            Samples = new string[] { $"{cliExecEnum} paloma -branch main" };
            CommandLineFormat = $"{cliExecEnum} {{project}} -branch {{branch}}";

            DistinctParameterAlias();
        }
    }
}

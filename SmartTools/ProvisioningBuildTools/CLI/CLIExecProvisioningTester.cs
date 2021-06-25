using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvisioningBuildTools.CLI
{
    public class CLIExecProvisioningTester : AbCLIExecInstance
    {
        public CLIExecProvisioningTester(CLIFactory cliFactory, CLIExecEnum cliExecEnum) : base(cliFactory, cliExecEnum)
        {
            Alias = new string[] { "t", "pt", "test" };
            RequiredParas = new string[] { "function", "project", "package", "serialnumber", "slot", "task" };
            SupportedParas = new string[] { "help", "function", "project", "package", "serialnumber", "slot", "task", "loopcount","sku", "promisecity", "force" };
            ParameterAlias["package"] = new string[] { "id", "p2" };
            ParameterAlias["serialnumber"] = new string[] { "sn" };
            ParameterAlias["slot"] = new string[] { "s" };
            ParameterAlias["task"] = Enumerable.Empty<string>().ToArray();
            Samples = new string[] { $"{cliExecEnum} paloma -package Debug -serialnumber 003217191757 -slot 0 -task Provision,Rollback -promisecity NULL -sku 9.94.3_16,43,9.94.6_172 -loopcount 1 -force true" };
            CommandLineFormat = $"{cliExecEnum} {{project}} -package {{package}} -serialnumber {{serialnumber}} -slot {{slot}} -task {{task}} -promisecity {{promisecity}} -sku {{sku}} -loopcount {{loopcount}} -force {{force}}";

            DistinctParameterAlias();
        }
    }
}

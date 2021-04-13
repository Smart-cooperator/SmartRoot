using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvisioningBuildTools.CLI
{
    public class CLIExecChangeProject : AbCLIExecInstance
    {
        public CLIExecChangeProject(CLIFactory cliFactory, CLIExecEnum cliExecEnum) : base(cliFactory, cliExecEnum)
        {
            Alias = new string[] { "cd", "project", "cproject" };
            RequiredParas = new string[] { "function", "project" };
            SupportedParas = new string[] { "help", "function", "project" };
            Samples = new string[] { $"{cliExecEnum} paloma" };

            DistinctParameterAlias();
        }

        public override void Parse()
        {
            base.Parse();

            if (ParseSuccess)
            {
                string project = Parameters["project"];

                string[] existProjects = CLIFactory.Projects;

                project = FindItem(existProjects, project);

                if (string.IsNullOrEmpty(project))
                {
                    ParseSuccess = false;
                    ParseResult = $"'{Parameters["project"]}' is not recognized as an internal project name for {CLIExecEnum},{Environment.NewLine}please use existed projects: '{string.Join(",", existProjects)}'";
                }
                else
                {
                    ParseResult = project;
                }
            }
        }
    }
}

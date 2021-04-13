using ProvisioningBuildTools.SelectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ProvisioningBuildTools.Global;

namespace ProvisioningBuildTools.CLI
{
    public class CLIFactory
    {
        public string[] Args { get; set; }

        public bool CreateSuccess { get; set; }

        public string CreateResult { get; set; }

        public Dictionary<string, AbCLIExecInstance> CLIExecInstances = new Dictionary<string, AbCLIExecInstance>(StringComparer.InvariantCultureIgnoreCase);

        public string[] Projects { get; set; }

        public ILogNotify LogNotify { get; set; }

        public string SelectedProject { get; set; }

        public bool IsHelp { get; set; }

        public CLIFactory(ILogNotify logNotify)
        {
            LogNotify = logNotify;
            Type enumType = typeof(CLIExecEnum);

            StringBuilder totalAlias = new StringBuilder(",");

            foreach (var name in Enum.GetNames(enumType))
            {
                Type type = Type.GetType($"ProvisioningBuildTools.CLI.CLIExec{name},ProvisioningBuildTools");

                if (type != null)
                {
                    AbCLIExecInstance instance = Activator.CreateInstance(type, this, Enum.Parse(enumType, name)) as AbCLIExecInstance;

                    if (instance != null)
                    {
                        CLIExecInstances[name] = instance;

                        IEnumerable<string> actualAlias = Enumerable.Empty<string>();
                        CLIExecInstances[name].Alias = (CLIExecInstances[name].Alias ?? Enumerable.Empty<string>().ToArray()).Distinct(StringComparer.InvariantCultureIgnoreCase).ToArray();

                        foreach (var alia in CLIExecInstances[name].Alias)
                        {
                            if (!totalAlias.ToString().ToUpper().Contains($",{alia},".ToUpper()))
                            {
                                actualAlias = actualAlias.Append(alia);
                                totalAlias.Append(alia);
                                totalAlias.Append(",");
                            }
                        }

                        CLIExecInstances[name].Alias = actualAlias.ToArray();
                    }
                }
            }
        }


        private void Clear()
        {
            CreateSuccess = false;
            CreateResult = string.Empty;
            Projects = null;
            SelectedProject = null;
            IsHelp = false;
        }

        public AbCLIExecInstance CreateCLIExecInstance(string commandLine)
        {
            Clear();

            AbCLIExecInstance instance = null;

            CreateSuccess = false;

            CreateResult = "Unknown error";

            Projects = new SelectLocalProjectInput(LogNotify).LocalBranches ?? Enumerable.Empty<string>().ToArray();

            SelectedProject = GlobalValue.Root.SelectedProject ?? string.Empty;

            if (!Projects.Contains(SelectedProject, StringComparer.InvariantCultureIgnoreCase))
            {
                SelectedProject = null;
            }

            Args = Regex
                        .Matches(commandLine, @"(?<match>[^\s""]+)|""(?<match>([^\s""]|\s)*)""")
                        .Cast<Match>()
                        .Select(m => m.Groups["match"].Value)
                        .ToArray();

            if (Args == null || Args.Length == 0)
            {
                CreateSuccess = false;

                CreateResult = "Not enough arguments";
            }
            else
            {
                instance = FindInstance(Args[0]);

                if (instance != null)
                {
                    if (instance.CLIExecEnum == CLIExecEnum.Help)
                    {
                        CreateSuccess = true;
                        IsHelp = true;

                        instance.Parse();

                        if (!instance.Parameters.ContainsKey("help"))
                        {
                            StringBuilder sb = new StringBuilder($"Help:{Environment.NewLine}");
                            sb.AppendLine($"Total supported commands:{string.Join(",", CLIExecInstances.Keys)}");
                            sb.AppendLine(string.Empty);

                            foreach (var item in CLIExecInstances.Values)
                            {
                                sb.AppendLine(string.Empty.PadRight(100, '+'));
                                sb.AppendLine(item.GetHelpInfo());
                                sb.AppendLine(string.Empty.PadRight(100, '-'));
                                sb.AppendLine(string.Empty);
                            }

                            CreateResult = sb.ToString();
                        }
                    }
                }
                else
                {
                    CreateSuccess = false;

                    CreateResult = $"'{Args[0]}' is not recognized as an internal command,{Environment.NewLine}please use 'help' to see all supported commands";
                }
            }

            return instance;
        }

        public AbCLIExecInstance FindInstance(string name)
        {
            string actualName = name;
            bool found = false;
            string temp;
            string[] temps;

            if (!string.IsNullOrEmpty(name))
            {
                if (CLIExecInstances.Keys.Contains(name, StringComparer.InvariantCultureIgnoreCase))
                {
                    actualName = name;
                    found = true;
                }
                else if (!string.IsNullOrEmpty(temp = CLIExecInstances.AsEnumerable().FirstOrDefault(pair => pair.Value.Alias.Contains(name, StringComparer.InvariantCultureIgnoreCase)).Key))
                {
                    actualName = temp;
                    found = true;
                }
                else
                {
                    temps = CLIExecInstances.Keys.Where(key => key.ToUpper().StartsWith(name.ToUpper())).ToArray();

                    if (temps != null && temps.Length == 1)
                    {
                        actualName = temps[0];
                        found = true;
                    }
                }
            }
        
            if (found)
            {
                AbCLIExecInstance instance = CLIExecInstances[actualName];
                instance.Clear();
                return instance;
            }
            else
            {
                return null;
            }
        }
    }
}

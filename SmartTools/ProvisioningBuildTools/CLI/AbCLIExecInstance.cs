using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProvisioningBuildTools.CLI
{
    public abstract class AbCLIExecInstance
    {
        protected string[] Samples { get; set; }

        public string[] Alias { get; set; }

        public CLIExecEnum CLIExecEnum { get; set; }

        protected Dictionary<string, string[]> ParameterAlias { get; set; }

        public CLIFactory CLIFactory { get; set; }

        public Dictionary<string, string> Parameters { get; set; }

        public bool ParseSuccess { get; set; }

        public string ParseResult { get; set; }

        protected string[] RequiredParas { get; set; }
        protected string[] SupportedParas { get; set; }

        protected string DefaultParameter { get; set; } = "project";

        public bool RequiredCompleted { get; set; }
        public bool FromCLI { get; set; }
        public bool IsHelp { get; set; }

        protected string CommandLineFormat { get; set; }
        public Dictionary<string, string> CommandLineFormatParas { get; set; }

        protected List<string> boolParaList { get; set; } = new List<string>() { "append", "force" };

        public AbCLIExecInstance(CLIFactory cliFactory, CLIExecEnum cliExecEnum)
        {
            CLIExecEnum = cliExecEnum;
            CLIFactory = cliFactory;
            ParameterAlias = new Dictionary<string, string[]>(StringComparer.InvariantCultureIgnoreCase);
            Parameters = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            ParameterAlias["help"] = new string[] { "?" };
            ParameterAlias["project"] = new string[] { "p" };
            ParameterAlias["force"] = Enumerable.Empty<string>().ToArray();
            ParameterAlias["append"] = new string[] { "n", "na", "nappend", "noappend", "notappend", "nonappend" };
            Alias = Enumerable.Empty<string>().ToArray();
            CommandLineFormatParas = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            Clear();
        }

        protected void DistinctParameterAlias()
        {
            StringBuilder totalAlias = new StringBuilder(",");

            ParameterAlias = ParameterAlias.Where(alias => SupportedParas.Contains(alias.Key, StringComparer.InvariantCultureIgnoreCase)).Select(alias =>
               {
                   string key = alias.Key;
                   string[] value = alias.Value;

                   IEnumerable<string> actualAlias = Enumerable.Empty<string>();
                   value = (value ?? Enumerable.Empty<string>().ToArray()).Distinct(StringComparer.InvariantCultureIgnoreCase).ToArray();

                   foreach (var alia in value)
                   {
                       if (!totalAlias.ToString().ToUpper().Contains($",{alia},".ToUpper()))
                       {
                           actualAlias = actualAlias.Append(alia);
                           totalAlias.Append(alia);
                           totalAlias.Append(",");
                       }
                   }

                   value = actualAlias.ToArray();
                   KeyValuePair<string, string[]> keyValuePair = new KeyValuePair<string, string[]>(key, value);
                   return keyValuePair;
               }).ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value);
        }

        public void Clear()
        {
            CommandLineFormatParas.Clear();
            Parameters.Clear();
            Parameters["function"] = CLIExecEnum.ToString();
            ParseSuccess = false;
            ParseResult = null;
            RequiredCompleted = false;
            FromCLI = false;
            IsHelp = false;
        }

        public virtual void Parse()
        {
            Clear();

            FromCLI = true;

            string[] args = CLIFactory.Args;
            string selectedProject = CLIFactory.SelectedProject;
            string[] existProjects = CLIFactory.Projects;

            if (RequiredParas.Contains("project", StringComparer.InvariantCultureIgnoreCase) && string.Compare(DefaultParameter, "project", true) == 0)
            {
                Parameters["project"] = FindItemForProject(existProjects, selectedProject) ?? string.Empty;
            }

            string previous = DefaultParameter ?? string.Empty;
            string previousOrigin = DefaultParameter ?? string.Empty;

            for (int i = 1; i < args.Length; i++)
            {
                string s = args[i];

                if (i == 1 && !(s.StartsWith("-") || s.StartsWith("/")))
                {
                    if (RequiredParas.Contains("project", StringComparer.InvariantCultureIgnoreCase) && string.Compare(DefaultParameter, "project", true) == 0)
                    {
                        Parameters["project"] = FindItemForProject(existProjects, s) ?? string.Empty;
                    }
                    else
                    {
                        Parameters[previous] = s;
                    }

                    previous = string.Empty;
                }
                else if (s.StartsWith("-") || s.StartsWith("/"))
                {
                    string snew = s.Substring(1);

                    string paraName = FindItem(ParameterAlias, snew);

                    if (!string.IsNullOrEmpty(paraName))
                    {
                        if (string.Compare(paraName, "project", ignoreCase: true) == 0)
                        {

                        }
                        else
                        {
                            if (boolParaList != null && boolParaList.Contains(paraName, StringComparer.InvariantCultureIgnoreCase))
                            {
                                if (snew.ToUpper().StartsWith("N") && !paraName.ToUpper().StartsWith("N"))
                                {
                                    Parameters[paraName] = "false";
                                }
                                else
                                {
                                    Parameters[paraName] = "true";
                                }
                            }
                            else
                            {
                                Parameters[paraName] = string.Empty;
                            }
                        }

                        previous = paraName;
                        previousOrigin = snew;
                    }
                    else
                    {
                        ParseSuccess = false;
                        ParseResult = $"'-{snew}' is not recognized as an internal parameter for {CLIExecEnum},{Environment.NewLine}please use '{CLIExecEnum} -help' to see all supported parameters for {CLIExecEnum}";
                        return;
                    }
                }
                else if (!string.IsNullOrEmpty(previous) && Parameters.Count > 0)
                {
                    if (previous.ToLower() == "project".ToLower())
                    {
                        Parameters["project"] = FindItemForProject(existProjects, s) ?? string.Empty;
                    }
                    else
                    {
                        if (boolParaList != null && boolParaList.Contains(previous, StringComparer.InvariantCultureIgnoreCase))
                        {
                            bool val = s.ToLower() == "true" || "true".StartsWith(s.ToLower());

                            if (previousOrigin.ToUpper().StartsWith("N") && !previous.ToUpper().StartsWith("N"))
                            {
                                Parameters[previous] = (!val).ToString().ToLower();
                            }
                            else
                            {
                                Parameters[previous] = val.ToString().ToLower();
                            }
                        }
                        else
                        {
                            Parameters[previous] = s;
                        }
                    }

                    previous = string.Empty;
                    previousOrigin = string.Empty;
                }
                else
                {
                    ParseSuccess = false;
                    ParseResult = $"missing parameter name before value:'{s}' for {CLIExecEnum},{Environment.NewLine}please use '{CLIExecEnum} -help' to see all supported parameters for {CLIExecEnum}";
                    return;
                }
            }

            if (Parameters.ContainsKey("help"))
            {
                ParseSuccess = true;
                IsHelp = true;
                ParseResult = GetHelpInfo();
            }
            else
            {
                ParseSuccess = true;
                RequiredCompleted = RequiredParas.Except(Parameters.Keys).Count() == 0;
            }
        }


        private string FindItemForProject(string[] sources, string des)
        {
            return FindItem(sources, des, true);
        }

        protected string FindItem(string[] sources, string des, bool forProject = false)
        {
            string ret = forProject ? des : null;

            if (sources != null && sources.Count() > 0 && !string.IsNullOrEmpty(des))
            {
                string temp = sources.FirstOrDefault(item => item.ToUpper() == des.ToUpper());

                if (!string.IsNullOrEmpty(temp))
                {
                    ret = temp;
                }
                else
                {
                    string[] temps = sources.Where(item => item.ToUpper().StartsWith(des.ToUpper()))?.ToArray();

                    if (temps != null && temps.Length == 1)
                    {
                        ret = temps[0];
                    }
                }
            }

            return ret;
        }
        private string FindItem(Dictionary<string, string[]> sources, string des)
        {
            string ret = null;

            if (sources != null && sources.Count() > 0 && !string.IsNullOrEmpty(des))
            {
                string temp = sources.Keys.FirstOrDefault(item => item.ToUpper() == des.ToUpper());

                if (!string.IsNullOrEmpty(temp))
                {
                    ret = temp;
                }
                else
                {
                    temp = sources.AsEnumerable().FirstOrDefault(pair => pair.Value.Contains(des, StringComparer.InvariantCultureIgnoreCase)).Key;

                    if (!string.IsNullOrEmpty(temp))
                    {
                        ret = temp;
                    }
                    else
                    {
                        string[] temps = sources.Keys.Where(item => item.ToUpper().StartsWith(des.ToUpper()))?.ToArray();

                        if (temps != null && temps.Length == 1)
                        {
                            ret = temps[0];
                        }
                    }
                }
            }

            return ret;
        }

        public virtual string GetHelpInfo()
        {
            StringBuilder sb = new StringBuilder($"{CLIExecEnum} -Help:{Environment.NewLine}");

            sb.AppendLine("Samples:");
            for (int i = 0; i < Samples.Length; i++)
            {
                sb.AppendLine($"Sample{i + 1}:{Samples[i]}");
            }
            sb.AppendLine(string.Empty);

            sb.AppendLine($"Alias:{string.Join(",", Alias)}");
            sb.AppendLine(string.Empty);

            sb.AppendLine($"Parameter Alias:");
            foreach (var item in ParameterAlias)
            {
                if (SupportedParas.Contains(item.Key, StringComparer.InvariantCultureIgnoreCase))
                {
                    sb.AppendLine($"-{item.Key} Alias:{string.Join(",", item.Value.Select(value => $"-{value}"))}");
                }
            }

            return sb.ToString();
        }

        public virtual string GetCommandLine()
        {
            Regex regex = new Regex(@"{\S+}", RegexOptions.IgnoreCase);

            MatchEvaluator matchEvaluator = new MatchEvaluator(
               (match) =>
               {
                   string key = match.Value.Trim('{', '}');

                   if (CommandLineFormatParas.ContainsKey(key))
                   {
                       return CommandLineFormatParas[key];
                   }
                   else
                   {
                       return "NULL";
                   }
               });

            return regex.Replace(CommandLineFormat, matchEvaluator).TrimEnd();
        }

        public virtual string GetParameterValue(string para, string defaultValue = null)
        {
            string value = defaultValue;

            if (Parameters != null && para != null && Parameters.ContainsKey(para))
            {
                string tmp = Parameters[para];

                if (tmp != null && tmp.ToUpper() != "NULL")
                {
                    value = tmp;
                }
            }

            return value;
        }
        public virtual bool GetParameterValueBool(string para, bool defaultValue = false)
        {
            string value = GetParameterValue(para, defaultValue.ToString().ToLower());
            defaultValue = string.IsNullOrEmpty(value) ? defaultValue : (value.ToLower() == "true" || "true".StartsWith(value.ToLower()));
            return defaultValue;
        }
    }
}

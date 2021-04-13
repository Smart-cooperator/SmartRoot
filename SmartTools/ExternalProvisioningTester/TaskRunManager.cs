using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalProvisioningTester
{
    public class TaskRunManager
    {
        private Dictionary<string, bool> RequiredCompletedDict = new Dictionary<string, bool>(StringComparer.InvariantCultureIgnoreCase);
        private Dictionary<string, string> RequiredValueDict = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        private MappingSection MappingSection = MappingSection.Instance;
        private Dictionary<string, int> OptionIndexDict = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
        private bool OptionUsed = false;
        private bool OptionCompleted = false;
        private bool waitEnv = false;
        private string targetStage = string.Empty;
        private string currentStage = string.Empty;
        private bool taskStart = false;
        private List<string> taskList = new List<string>();
        private int taskIndex = -1;
        private string topLevelTask;


        public TaskRunManager(int[] taskCodes)
        {
            taskList.AddRange(taskCodes.Select(code => ((TaskOpCode)code).ToString()));
            targetStage = "IP";
        }

        public void SetRequiredValue(string key, string value, bool completed)
        {
            RequiredCompletedDict[key.Trim()] = completed;
            RequiredValueDict[key.Trim()] = value;
        }

        public string GetUnmarkedData(string data)
        {
            string ret = data;

            bool flag = true;

            while (flag)
            {
                foreach (MarkedMapping markedMapping in MappingSection.MarkedMappings)
                {
                    string value = markedMapping.Value;

                    if (!string.IsNullOrEmpty(value) && ret.StartsWith(value))
                    {
                        ret = new string(ret.Skip(value.Length).ToArray());

                        flag = true;
                        break;
                    }
                    else
                    {
                        flag = false;
                    }
                }
            }

            return ret;
        }

        public bool AnalysisData(string data, out List<Tuple<string, string>> tuples)
        {
            bool hasInput = false;
            tuples = new List<Tuple<string, string>>();
            string key = null;
            string optionVaule;
            int optionIndex;
            bool isOption;

            if (waitEnv)
            {
                Func<string, bool> useFunc = (value) =>
                {
                    bool ret = false;

                    if (ret = value.Trim().Equals(RequiredValueDict["Env"].Trim(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        hasInput = true;
                    }

                    return ret;
                };

                isOption = IssueOption(data, out optionVaule, out optionIndex, useFunc);

                if (hasInput)
                {
                    string tmp = optionIndex.ToString();

                    //tuples.Add(new Tuple<string, string>(tmp, $"Choose test environment by typing a number:{tmp}"));
                    tuples.Add(new Tuple<string, string>(tmp, tmp));
                    waitEnv = false;
                }

                if (OptionCompleted && !OptionUsed)
                {
                    throw new Exception($"the vaule {RequiredValueDict["Env"]} of Env not found in options {string.Join(",", OptionIndexDict.Keys.Cast<string>())}");
                }
            }
            else
            {
                bool isRequiredCompleted = IsRequiredCompleted();

                Func<string, bool> useFunc = (value) =>
                {
                    bool ret = false;

                    if (!isRequiredCompleted)
                    {
                        if (ret = value.Trim().Equals("IP", StringComparison.CurrentCultureIgnoreCase))
                        {
                            hasInput = true;
                        }
                    }
                    else
                    {
                        if (!taskStart)
                        {
                            if (ret = value.Trim().Equals(GetNextStage().Trim(), StringComparison.CurrentCultureIgnoreCase))
                            {
                                hasInput = true;
                            }
                        }
                        else
                        {
                            if (new string[] { "GoBack", "Exit" }.Contains(value.Trim().Trim(), StringComparer.InvariantCultureIgnoreCase))
                            {
                                taskStart = false;

                                if (ret = OptionIndexDict.ContainsKey(GetNextStage().Trim()))
                                {
                                    optionIndex = OptionIndexDict[GetNextStage().Trim()];
                                    hasInput = true;
                                }
                            }
                        }
                    }

                    return ret;
                };

                isOption = IssueOption(data, out optionVaule, out optionIndex, useFunc, true);

                if (!isRequiredCompleted && !isOption)
                {
                    if (SearchKey(data, out key, !isRequiredCompleted))
                    {
                        if (key.Trim().Equals("SN", StringComparison.CurrentCultureIgnoreCase))
                        {
                            string tmp = RequiredValueDict[key.Trim()];

                            tuples.Add(new Tuple<string, string>(tmp, tmp));
                            hasInput = true;
                        }
                        else if (key.Trim().Equals("Env", StringComparison.CurrentCultureIgnoreCase))
                        {
                            waitEnv = true;
                            hasInput = false;
                        }
                    }
                }
                else if (isOption)
                {
                    if (hasInput)
                    {
                        if (!isRequiredCompleted)
                        {
                            if (optionVaule.Trim().Equals("IP", StringComparison.CurrentCultureIgnoreCase))
                            {
                                string tmp = optionIndex.ToString();

                                //tuples.Add(new Tuple<string, string>(tmp, $"Choose task by typing a number:{tmp}"));
                                tuples.Add(new Tuple<string, string>(tmp, tmp));

                                tmp = RequiredValueDict["IP"].Trim();

                                //tuples.Add(new Tuple<string, string>(tmp, $"IP address:{tmp}"));
                                tuples.Add(new Tuple<string, string>(tmp, tmp));

                                currentStage = string.Empty;
                                targetStage = string.Empty;
                            }
                        }
                        else
                        {
                            string tmp = optionIndex.ToString();

                            string nextStep = GetNextStage();

                            if (nextStep == "GoBack" || (!string.IsNullOrEmpty(currentStage) && targetStage.Contains("-")))
                            {
                                //tuples.Add(new Tuple<string, string>(tmp, $"Choose stage by type a number:{tmp}"));
                                tuples.Add(new Tuple<string, string>(tmp, tmp));
                            }
                            else
                            {
                                //tuples.Add(new Tuple<string, string>(tmp, $"Choose task by typing a number:{tmp}"));
                                tuples.Add(new Tuple<string, string>(tmp, tmp));
                            }

                            ResetStage();
                        }
                    }
                    else
                    {
                        if (OptionCompleted && !OptionUsed)
                        {
                            throw new Exception($"{GetNextStage()} not found in options {string.Join(",", OptionIndexDict.Keys.Cast<string>())}, please add commentMapping in ExternalProvisioningTester.exe.config");
                        }
                    }
                }
            }

            return hasInput;
        }

        public void ResetStage()
        {
            string nextStage = GetNextStage();

            if (!string.IsNullOrEmpty(nextStage))
            {
                if (nextStage == "GoBack")
                {
                    int index = currentStage.LastIndexOf('-');

                    if (index > 0)
                    {
                        currentStage = currentStage.Substring(0, index);
                    }
                    else
                    {
                        currentStage = string.Empty;
                    }
                }
                else
                {
                    currentStage = $"{currentStage}-{nextStage}".Trim('-');
                }
            }


            if (!string.IsNullOrEmpty(currentStage) && currentStage == targetStage)
            {
                taskStart = true;
                targetStage = string.Empty;

                int index = currentStage.LastIndexOf('-');

                if (index > 0)
                {
                    currentStage = currentStage.Substring(0, index);
                }
                else
                {
                    currentStage = string.Empty;
                }

            }

            if (string.IsNullOrEmpty(targetStage))
            {
                taskIndex++;

                if (taskIndex < taskList.Count)
                {
                    if (!SearchTarget(taskList[taskIndex].Trim(), out targetStage))
                    {
                        throw new Exception($"{taskList[taskIndex].Trim()} not found in taskOpMappings, please add taskOpMapping in ExternalProvisioningTester.exe.config");
                    }
                }
                else
                {
                    targetStage = "Exit";
                }
            }
        }

        public string GetNextStage()
        {
            string nextStage;

            if (string.IsNullOrEmpty(currentStage))
            {
                nextStage = targetStage.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            }
            else if (currentStage == targetStage)
            {
                nextStage = string.Empty;
            }
            else if (targetStage.Contains(currentStage))
            {
                nextStage = targetStage.Substring(currentStage.Length).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            }
            else
            {
                nextStage = "GoBack";
            }

            return nextStage?.Trim();
        }

        private bool IsRequiredCompleted()
        {
            bool isCompleted = false;

            foreach (var item in RequiredCompletedDict)
            {
                if (!(isCompleted = item.Value))
                {
                    break;
                }
            }

            return isCompleted;
        }

        private bool SearchKeyByName(string vaule, out string key, List<string> names)
        {
            bool found = false;
            key = null;

            foreach (CommentMapping commentMapping in MappingSection.CommentMappings)
            {
                if (names.Contains(commentMapping.Name.Trim(), StringComparer.InvariantCultureIgnoreCase))
                {
                    if (commentMapping.AllowedComments.Cast<AllowedComment>().Select(allowedComment => allowedComment.Value.Trim()).Contains(vaule.Trim(), StringComparer.InvariantCultureIgnoreCase))
                    {
                        found = true;
                        key = commentMapping.Name.Trim();

                        break;
                    }
                }
            }

            return found;
        }

        private bool SearchKey(string vaule, out string key, bool required)
        {
            bool found = false;
            key = null;

            foreach (CommentMapping commentMapping in MappingSection.CommentMappings)
            {
                //if (required && RequiredCompletedDict.Keys.Contains(commentMapping.Name.Trim(), StringComparer.InvariantCultureIgnoreCase)
                if (required
                    || !required && !RequiredCompletedDict.Keys.Contains(commentMapping.Name.Trim(), StringComparer.InvariantCultureIgnoreCase))
                {
                    if (required && RequiredCompletedDict.Keys.Contains(commentMapping.Name.Trim(), StringComparer.InvariantCultureIgnoreCase) && RequiredCompletedDict[commentMapping.Name.Trim()])
                    {
                        continue;
                    }

                    if (commentMapping.AllowedComments.Cast<AllowedComment>().Select(allowedComment => allowedComment.Value.Trim()).Contains(vaule.Trim(), StringComparer.InvariantCultureIgnoreCase))
                    {
                        found = true;
                        key = commentMapping.Name.Trim();

                        if (required && RequiredCompletedDict.Keys.Contains(commentMapping.Name.Trim(), StringComparer.InvariantCultureIgnoreCase))
                        {
                            RequiredCompletedDict[key] = true;
                        }

                        break;
                    }
                }
            }

            return found;
        }

        private bool SearchTarget(string key, out string target)
        {
            bool found = false;
            target = null;

            foreach (TaskOpMapping taskOpMapping in MappingSection.TaskOpMappings)
            {
                if (taskOpMapping.Name.Trim().Equals(key.Trim(), StringComparison.InvariantCultureIgnoreCase))
                {
                    found = true;

                    string[] childValues = taskOpMapping.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);


                    foreach (var childValue in childValues)
                    {
                        if (!string.IsNullOrEmpty(childValue) && topLevelTask.ToUpper().Trim().Contains(new string(childValue.TakeWhile(c => c != '-').ToArray()).ToUpper()))
                        {
                            if (string.IsNullOrEmpty(target))
                            {
                                target = childValue.Trim();
                            }
                            else
                            {
                                if (childValue.Count(c => c == '-') < target.Count(c => c == '-'))
                                {
                                    target = childValue.Trim();
                                }
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(target) && childValues != null && childValues.Length > 0)
                    {
                        target = childValues[0].Trim();
                    }

                    break;
                }
            }

            return found;
        }
        private bool IssueOption(string value, out string optionVaule, out int optionIndex, Func<string, bool> useAction, bool search = false)
        {
            bool issue = false;
            optionVaule = null;
            optionIndex = int.MinValue;
            int maxOptionIndex = OptionIndexDict.Values.Count == 0 ? int.MinValue : OptionIndexDict.Values.Max();
            int index;
            bool searchResult = !search;

            if ((index = value.IndexOf('.')) > 0)
            {
                optionVaule = new string(value.Substring(index).Skip(1).ToArray());

                if (search)
                {
                    string temp;
                    searchResult = SearchKey(optionVaule, out temp, !IsRequiredCompleted());

                    if (searchResult)
                    {
                        optionVaule = temp;
                    }
                }

                issue = int.TryParse(value.Substring(0, index), out optionIndex);
            }

            if (issue)
            {
                if (optionIndex > maxOptionIndex)
                {
                    if (searchResult)
                    {
                        OptionIndexDict[optionVaule.Trim()] = optionIndex;
                    }

                    if (searchResult && !OptionUsed)
                    {
                        OptionUsed = useAction(optionVaule.Trim());
                    }

                    if (search && searchResult)
                    {
                        OptionCompleted = new string[] { "GoBack", "Exit" }.Contains(optionVaule.Trim(), StringComparer.InvariantCultureIgnoreCase);
                    }
                }
                else
                {
                    OptionCompleted = true;
                }
            }
            else
            {
                if (OptionIndexDict.Count > 0)
                {
                    OptionCompleted = true;
                }
            }

            if (OptionCompleted && OptionUsed)
            {
                bool addOption = false;

                if (issue && !OptionIndexDict.ContainsKey(optionVaule.Trim()))
                {
                    addOption = true;
                }

                if (string.IsNullOrEmpty(topLevelTask) && IsRequiredCompleted() && !waitEnv)
                {
                    topLevelTask = string.Join(",", OptionIndexDict.Keys);

                    ResetStage();
                }

                OptionIndexDict.Clear();
                OptionUsed = false;
                OptionCompleted = false;

                if (addOption)
                {
                    OptionIndexDict[optionVaule.Trim()] = optionIndex;
                }
            }

            return issue;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ExternalProvisioningTester
{
	public class CommandLineParser
	{
		private enum EmptyProductOpCode
		{

		}

		public const int OpCodesReservedForCommon = 32768;

		protected Enum productOpCode = (EmptyProductOpCode)0;

		protected string genealogyFile = null;

		public const string Usage = "\r\nUsage:\r\n------\r\nProvisioningTester [-IP <IP address> | -Label <1-based slot label> | -Slot <0-based slot number>]\r\n                   [-Adapter <adapter name>]\r\n                   [-SN <serial number>]\r\n                   [-Task <task to run>]\r\n                   [-Cmd <DOS command to run>]\r\n                   [-? | - Help]\r\n\r\nExamples:\r\n---------\r\nProvisioningTester\r\nProvisioningTester -IP 192.168.1.51 -SN 123123123\r\nProvisioningTester -Label 1\r\nProvisioningTester -SN 123123123 -Task Rollback\r\nProvisioningTester.exe -label 23 -sn 123123123 -task RenameVolumeImaging,Provision\r\nProvisioningTester.exe -label 23 -sn 123123123 -task shell\r\nProvisioningTester.exe -label 23 -sn 123123123 -cmd shutdown -s -t 0\r\n\r\nNotes:\r\n------\r\n- You may provide either IP adress, label # or slot #.  label = slot + 1\r\n- The -Task argument executes one of the tasks shown on the user menus.\r\n- The available tasks will vary by product.  To get a list of tasks, use\r\n  ProvisioningTester -Task\r\n- The -Cmd argument must be last.  All text after -Cmd will be executed on the DUT.";

		public CommandLineParser()
		{
		}

		public CommandLineParser(string genealogyFile, Enum productOpCode = null)
		{
			this.genealogyFile = genealogyFile;
			if (productOpCode != null)
			{
				this.productOpCode = productOpCode;
			}
		}

		public virtual Dictionary<string, string> PopulateArgTable(string[] args)
		{
			Dictionary<string, string> argTable = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
			string previous = string.Empty;
			if (args == null)
			{
				return argTable;
			}
			for (int i = 0; i < args.Length; i++)
			{
				string s = args[i];
				if (s == "?" && !argTable.ContainsKey(s))
				{
					argTable.Add(s, "true");
					continue;
				}
				if (s.Equals("-Cmd", StringComparison.OrdinalIgnoreCase))
				{
					string t = "";
					for (i++; i < args.Length; i++)
					{
						t = t + args[i] + " ";
					}
					argTable.Add("Cmd", t.Trim());
					break;
				}
				if (s.StartsWith("-") || s.StartsWith("/"))
				{
					string snew = s.Substring(1);
					if (!argTable.ContainsKey(snew))
					{
						argTable.Add(snew, "true");
					}
					else
					{
						argTable[snew] = "true";
					}
					previous = snew;
				}
				else if (argTable.Count > 0 && string.Compare(argTable[previous], "true", ignoreCase: true) == 0)
				{
					argTable[previous] = s;
				}
			}
			return argTable;
		}

		public virtual CommandLineArgs Parse(string[] args)
		{
			CommandLineArgs argInfo = new CommandLineArgs(genealogyFile);
			Dictionary<string, string> argTable = PopulateArgTable(args);
			if (argTable.ContainsKey("?") || argTable.ContainsKey("Help") || argTable.ContainsKey("-Help"))
			{
				Console.WriteLine("\r\nUsage:\r\n------\r\nProvisioningTester [-IP <IP address> | -Label <1-based slot label> | -Slot <0-based slot number>]\r\n                   [-Adapter <adapter name>]\r\n                   [-SN <serial number>]\r\n                   [-Task <task to run>]\r\n                   [-Cmd <DOS command to run>]\r\n                   [-? | - Help]\r\n\r\nExamples:\r\n---------\r\nProvisioningTester\r\nProvisioningTester -IP 192.168.1.51 -SN 123123123\r\nProvisioningTester -Label 1\r\nProvisioningTester -SN 123123123 -Task Rollback\r\nProvisioningTester.exe -label 23 -sn 123123123 -task RenameVolumeImaging,Provision\r\nProvisioningTester.exe -label 23 -sn 123123123 -task shell\r\nProvisioningTester.exe -label 23 -sn 123123123 -cmd shutdown -s -t 0\r\n\r\nNotes:\r\n------\r\n- You may provide either IP adress, label # or slot #.  label = slot + 1\r\n- The -Task argument executes one of the tasks shown on the user menus.\r\n- The available tasks will vary by product.  To get a list of tasks, use\r\n  ProvisioningTester -Task\r\n- The -Cmd argument must be last.  All text after -Cmd will be executed on the DUT.");
				Environment.Exit(0);
			}
			if (argTable.ContainsKey("SN"))
			{
				argInfo.SystemDutSN = argTable["SN"];
			}
			if (argTable.ContainsKey("IP"))
			{
				argInfo.IPAddress = argTable["IP"];
			}
			else if (argTable.ContainsKey("Slot"))
			{
				int lowByte = 51 + int.Parse(argTable["Slot"]);
				argInfo.IPAddress = $"192.168.1.{lowByte}";
			}
			else if (argTable.ContainsKey("Label"))
			{
				int lowByte2 = 50 + int.Parse(argTable["Label"]);
				argInfo.IPAddress = $"192.168.1.{lowByte2}";
			}
			if (!IPAddress.TryParse(argInfo.IPAddress, out argInfo.IPAddressObj))
			{
				throw new ArgumentException("Invalid IP address.");
			}
			if (argTable.ContainsKey("Genes"))
			{
				argInfo.GenealogyFile = argTable["Genes"];
			}
			if (argTable.ContainsKey("GenealogyFile"))
			{
				argInfo.GenealogyFile = argTable["GenealogyFile"];
			}
			if (argTable.ContainsKey("Adapter"))
			{
				argInfo.AdapterName = argTable["Adapter"];
			}
			if (argTable.ContainsKey("Task"))
			{
				string[] taskArray = argTable["Task"].Split(',');
				List<int> tasks2 = new List<int>();
				string[] array = taskArray;
				foreach (string task in array)
				{
					if (Enum.TryParse<TaskOpCode>(task, ignoreCase: true, out var standardOp))
					{
						tasks2.Add((int)standardOp);
						continue;
					}
					try
					{
						Type productOpType = productOpCode.GetType();
						int productOp = (int)Enum.Parse(productOpType, task, ignoreCase: true);
						tasks2.Add(productOp);
					}
					catch
					{
						throw new ArgumentException($"{task} is not a valid operation.  Valid operations include:\n\n{DumpOperations()}");
					}
				}
				argInfo.TasksToRun = tasks2.ToArray();
			}
			if (argTable.ContainsKey("Cmd"))
			{
				argInfo.TasksToRun = argInfo.TasksToRun ?? new int[0];
				List<int> tasks = new List<int>(argInfo.TasksToRun);
				argInfo.TasksToRun = tasks.Append(11).ToArray();
				argInfo.CommandToRun = argTable["Cmd"];
			}
            if (argTable.ContainsKey("Env"))
            {
                argInfo.TestEnvironmentType = argTable["Env"];
            }
            if (argTable.ContainsKey("ProvisioningTesterPath"))
            {
                argInfo.ProvisioningTesterPath = argTable["ProvisioningTesterPath"];
            }
            return argInfo;
		}

		public virtual string DumpOperations()
		{
			StringBuilder sb = new StringBuilder();
			string[] names = Enum.GetNames(typeof(TaskOpCode));
			foreach (string op in names)
			{
				if (!op.Equals("Cmd"))
				{
					sb.AppendLine(op);
				}
			}
			string[] names2 = Enum.GetNames(productOpCode.GetType());
			foreach (string op2 in names2)
			{
				sb.AppendLine(op2);
			}
			return sb.ToString();
		}

		public virtual string OpCodeToTask(int opCode)
		{
			if (opCode < 32768)
			{
				return Enum.GetName(typeof(TaskOpCode), opCode);
			}
			return Enum.GetName(productOpCode.GetType(), opCode);
		}
	}
}

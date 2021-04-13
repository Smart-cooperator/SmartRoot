using System.Net;

namespace ExternalProvisioningTester
{
	public class CommandLineArgs
	{
		public string SystemDutSN = null;

		public int[] TasksToRun = null;

		public string CommandToRun = null;

		public string IPAddress = "192.168.1.51";

		public IPAddress IPAddressObj = null;

		public string GenealogyFile = null;

		public string AdapterName = "device_adapter";

        public string TestEnvironmentType = null;

        public string ProvisioningTesterPath = null;

        public CommandLineArgs()
		{

		}

		public CommandLineArgs(string genealogyFile)
		{
			GenealogyFile = genealogyFile;
		}
	}
}

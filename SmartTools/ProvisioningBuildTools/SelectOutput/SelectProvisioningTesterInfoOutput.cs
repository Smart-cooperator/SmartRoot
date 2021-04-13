using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvisioningBuildTools.SelectOutput
{
    public class SelectProvisioningTesterInfoOutput
    {
        public string SelectProject { get; }

        public string SelectProvisioningPackage { get; }

        public string SelectSerialNumber { get; }

        public string SelectSlot { get; }

        public string SelectTaskOpList { get; }

        public string SelectArgs { get; }

        public bool UseExternalProvisioningTester { get; }

        public SelectProvisioningTesterInfoOutput()
        {

        }
        public SelectProvisioningTesterInfoOutput(
            string selectProject,
            string selectProvisioningPackage,
            string selectSerialNumber,
            string selectSlot,
            string selectTaskOpList,
            string selectArgs,
            bool useExternalProvisioningTester)
        {
            SelectProject = selectProject;
            SelectProvisioningPackage = selectProvisioningPackage;
            SelectSerialNumber = selectSerialNumber;
            SelectSlot = selectSlot;
            SelectTaskOpList = selectTaskOpList;
            SelectArgs = selectArgs;
            UseExternalProvisioningTester= useExternalProvisioningTester;
        }
    }
}

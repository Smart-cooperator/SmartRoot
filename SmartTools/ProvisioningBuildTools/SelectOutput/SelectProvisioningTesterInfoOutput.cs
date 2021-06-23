using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        public Dictionary<string, XDocument> SelectSkuDocumentDict { get; }

        public string SelectGenealogyFile { get; }

        public int SelectLoopCount { get; }

        public string SelectPromiseCity { get; }

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
            bool useExternalProvisioningTester) :this(
             selectProject,
             selectProvisioningPackage,
             selectSerialNumber,
             selectSlot,
             selectTaskOpList,
             selectArgs,
             useExternalProvisioningTester,
             null,
             null,
             1,
             null
            )
        {
           
        }
        public SelectProvisioningTesterInfoOutput(
            string selectProject,
            string selectProvisioningPackage,
            string selectSerialNumber,
            string selectSlot,
            string selectTaskOpList,
            string selectArgs,
            bool useExternalProvisioningTester,
            Dictionary<string, XDocument> selectSkuDocumentDict,
            string selectGenealogyFile,
            int selectLoopCount,
            string selectPromiseCity)
        {
            SelectProject = selectProject;
            SelectProvisioningPackage = selectProvisioningPackage;
            SelectSerialNumber = selectSerialNumber;
            SelectSlot = selectSlot;
            SelectTaskOpList = selectTaskOpList;
            SelectArgs = selectArgs;
            UseExternalProvisioningTester = useExternalProvisioningTester;
            SelectSkuDocumentDict = selectSkuDocumentDict;
            SelectGenealogyFile = selectGenealogyFile;
            SelectLoopCount = selectLoopCount;
            SelectPromiseCity = selectPromiseCity;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace CapsuleParser
{
    class XmlParser
    {
        static List<string> infArray = new List<string>();
        static List<string> binArray = new List<string>();
        public static void ParseCapsules(Dictionary<string, string> argTable)
        {
            string configFile = "CapsuleInfoConfiguration.xml";

            if (argTable.ContainsKey("ConfigFile"))
            {
                configFile = argTable["ConfigFile"];
            }
            // string dirPath = @"C:\Documents\PrototypeTools\CapsuleParser\CapsuleParser\bin\Debug\Capsules";
            CapsuleInfoConfigurationFile capsuleInfoConfigurationFile = new CapsuleInfoConfigurationFile(configFile);

            if (argTable.ContainsKey("CapsuleFolder"))
            {
                string dirPath = argTable["CapsuleFolder"];
                ProcessCapsuleFolder(dirPath, capsuleInfoConfigurationFile);
            }
            else if (argTable.ContainsKey("InfFile") && argTable.ContainsKey("BinFile"))
            {
                UpdateCapsuleVersion(capsuleInfoConfigurationFile, argTable["InfFile"], Path.GetFileName(argTable["BinFile"]));

            }
            else
            {
                throw new Exception("Not supported/Valid argements!");
            }

            capsuleInfoConfigurationFile.Serialize();
        }

        private static void ProcessCapsuleFolder(string dirPath, CapsuleInfoConfigurationFile capsuleInfoConfigurationFile)
        {
            Utility.ProcessDirectory(dirPath, infArray, binArray);

            foreach (string infFile in infArray)
            {
                foreach (string binFile in binArray)
                {
                    if (Path.GetDirectoryName(infFile).ToUpper().CompareTo(Path.GetDirectoryName(binFile).ToUpper()) == 0)
                    {
                        string infFileName = Path.GetFileName(infFile);
                        string binFileName = Path.GetFileName(binFile);

                        UpdateCapsuleVersion(capsuleInfoConfigurationFile, infFile, binFileName);

                        /*   int index1 = infFileName.IndexOf(".");

                           int index2 = binFileName.IndexOf("_");

                           if (infFileName.Remove(index1).CompareTo(binFileName.Remove(index2)) == 0)
                           {
                               UpdateCapsuleVersion(capsuleInfoConfigurationFile, infFile, binFileName);
                           }
                       */
                    }


                }
            }
        }

        private static void UpdateCapsuleVersion(CapsuleInfoConfigurationFile capsuleInfoConfigurationFile, string pathOfInf, string pathOfBin)
        {
            /*  Capsule capsule = capsuleInfoConfigurationFile.GetCapsuleByInfFilePath(@"ME\Provisioning\SurfaceME.inf");
              AllowedVersion allowedVersion = capsule.DeviceHardwareId.AllowedVersionList.GetCurrentVersion();
              allowedVersion.SetVersionFromFilename(@"Capsules\ME\Provisioning\SurfaceME_15.0.1377.4.bin");
              capsuleInfoConfigurationFile.Serialize();*/

            Capsule capsule = capsuleInfoConfigurationFile.GetCapsuleByInfFilePath(pathOfInf);
            AllowedVersion allowedVersion = capsule.DeviceHardwareId.AllowedVersionList.GetCurrentVersion();
            allowedVersion.SetVersionFromFilename(pathOfBin);


        }
    }
}

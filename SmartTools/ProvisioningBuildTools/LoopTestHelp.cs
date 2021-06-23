using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProvisioningBuildTools
{
    public static class LoopTestHelp
    {
        public static Dictionary<string, Dictionary<string, Func<string, string, XDocument>>> GetAllSkus(string workingDir, string currentGenealogyFile, ILogNotify logNotify)
        {
            string[] imageConfigFiles = Directory.EnumerateFiles(Path.Combine(workingDir, "Config"), "ImageConfiguration*.xml", SearchOption.AllDirectories).ToArray();
            string promiseCity = null;
            string dirName = null;
            string dirFullName = null;
            DirectoryInfo dirInfo = null;
            XDocument genealogyDocument = null;
            XDocument imageConfigDocument = null;
            XDocument softwareAssemblyDocument = null;
            Dictionary<string, Func<string, string, XDocument>> skuDocumentDict = null;
            Dictionary<string, Dictionary<string, Func<string, string, XDocument>>> promiseCityDict = new Dictionary<string, Dictionary<string, Func<string, string, XDocument>>>();
            string partNumber = null;
            string softwareAssemblyFile = null;
            string imageConfigFileName = null;
            string sku = null;
            string name = null;
            try
            {
                foreach (var imageConfigFile in imageConfigFiles)
                {
                    imageConfigDocument = XDocument.Load(imageConfigFile);
                    imageConfigFileName = Path.GetFileNameWithoutExtension(imageConfigFile);

                    promiseCity = string.Empty;
                    dirInfo = new DirectoryInfo(Path.GetDirectoryName(imageConfigFile));
                    dirName = dirInfo.Name;
                    dirFullName = dirInfo.FullName;

                    if (string.Compare(dirName, "Config", StringComparison.InvariantCultureIgnoreCase) != 0)
                    {
                        promiseCity = dirName;
                    }

                    if (imageConfigFileName.Contains("_"))
                    {
                        if (string.IsNullOrEmpty(promiseCity))
                        {
                            promiseCity = imageConfigFileName.Split('_').Last();
                        }
                        else
                        {
                            promiseCity = $"{imageConfigFileName.Split('_').Last()}_{promiseCity}";
                        }
                    }

                    if (string.IsNullOrEmpty(promiseCity))
                    {
                        promiseCity = "NULL";
                    }

                    softwareAssemblyFile = Directory.EnumerateFiles(dirFullName, "SoftwareAssembly*.xml").FirstOrDefault();

                    if (string.IsNullOrEmpty(softwareAssemblyFile))
                    {
                        logNotify.WriteLog($"SoftwareAssembly*.xml not found in {dirFullName}", true);
                        continue;
                    }

                    softwareAssemblyDocument = XDocument.Load(softwareAssemblyFile);

                    skuDocumentDict = new Dictionary<string, Func<string, string, XDocument>>();

                    promiseCityDict[promiseCity] = skuDocumentDict;
                    foreach (var versionElement in imageConfigDocument.Descendants("CustomerImage").Descendants(@"ImageVersion"))
                    {
                        foreach (var skuElement in versionElement.Descendants("Image"))
                        {
                            sku = $"{versionElement.Attribute("Version").Value}_{skuElement.Attribute("ImageNumber").Value}";

                            partNumber = skuElement.Attribute("ImageRegionPartNumber").Value;

                            XElement currentSoftwareAssembly = softwareAssemblyDocument.Descendants("SoftwareAssembly").FirstOrDefault(element => element.Element("ImageConfig").Attribute("PartNumber").Value == partNumber);

                            if (currentSoftwareAssembly == null)
                            {
                                logNotify.WriteLog($"ImageRegionPartNumber {partNumber} of SKU {sku} not found in {softwareAssemblyFile}", true);
                                continue;
                            }

                            Func<string, string, XDocument> getGenealogyDocument = (sn, nodeNameForSN) =>
                            {
                                genealogyDocument = XDocument.Load(currentGenealogyFile);
                                XElement currentGenealogy = genealogyDocument.Descendants(nodeNameForSN).FirstOrDefault(element => element.Value == sn).Parent;
                                
                                if (currentGenealogy == null)
                                {
                                    logNotify.WriteLog($"sn {sn} not found in {currentGenealogyFile}", true);
                                    return null;
                                }

                                currentGenealogy.SetElementValue("SoftwareAssemblyPartNumber", currentSoftwareAssembly.Attribute("PartNumber").Value);
                                
                                foreach (var softwareAssemblyItem in currentSoftwareAssembly.Elements())
                                {
                                    name = softwareAssemblyItem.Name.ToString();
                                    switch (name)
                                    {
                                        case "WifiConfig":
                                            name = "WifiRegion";
                                            break;
                                        default:
                                            break;
                                    }

                                    currentGenealogy.SetElementValue(name, softwareAssemblyItem.Attribute("PartNumber").Value);
                                }

                                return genealogyDocument;
                            };

                            skuDocumentDict[sku] = getGenealogyDocument;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logNotify.WriteLog(ex);
            }

            return promiseCityDict;
        }
    }
}


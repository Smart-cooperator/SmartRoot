using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProvisioningBuildTools
{
    public static class LoopTestHelp
    {
        public static Dictionary<string, Dictionary<string, Func<string, string, XDocument>>> GetAllSkus(string workingDir, string currentGenealogyFile, ILogNotify logNotify)
        {
            string[] imageConfigFiles = null;
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

                imageConfigFiles = Directory.EnumerateFiles(Path.Combine(workingDir, "Config"), "ImageConfiguration*.xml", SearchOption.AllDirectories).ToArray();

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

                    //if (imageConfigFileName.Contains("_"))
                    //{
                    //    if (string.IsNullOrEmpty(promiseCity))
                    //    {
                    //        promiseCity = imageConfigFileName.Split('_').Last();
                    //    }
                    //    else
                    //    {
                    //        promiseCity = $"{imageConfigFileName.Split('_').Last()}_{promiseCity}";
                    //    }
                    //}

                    softwareAssemblyFile = Directory.EnumerateFiles(dirFullName, "SoftwareAssembly*.xml").FirstOrDefault();

                    if (string.IsNullOrEmpty(softwareAssemblyFile))
                    {
                        logNotify.WriteLog($"SoftwareAssembly*.xml not found in {dirFullName}", true);
                        continue;
                    }

                    softwareAssemblyDocument = XDocument.Load(softwareAssemblyFile);

                    //Dictionary<string, string> softwareAssemblyFilters = new Dictionary<string, string>();

                    //string stageConfigurationFile = Directory.EnumerateFiles(Path.Combine(workingDir, "Config"), "StageConfiguration*.xml").FirstOrDefault();

                    //if (!string.IsNullOrEmpty(stageConfigurationFile))
                    //{
                    //    XDocument stageConfigurationDocument = XDocument.Load(stageConfigurationFile);

                    //    string pattern = @"(?<subProject>[^</\s]+)SwAssemblyPartNumbers";

                    //    Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

                    //    Match ma = regex.Match(stageConfigurationDocument.ToString());

                    //    while (ma.Success)
                    //    {
                    //        if (!softwareAssemblyFilters.ContainsKey(ma.Groups["subProject"].Value))
                    //        {
                    //            softwareAssemblyFilters[ma.Groups["subProject"].Value] = string.Join(",", stageConfigurationDocument.Descendants($"{ma.Groups["subProject"].Value}SwAssemblyPartNumbers").FirstOrDefault()?.Descendants("SwAssemblyPartNumber")?.Select(e => e.Value)??Enumerable.Empty<string>());
                    //        }

                    //        ma = ma.NextMatch();
                    //    }
                    //}

                    //if (softwareAssemblyFilters.Count == 0)
                    //{
                    //    softwareAssemblyFilters[string.Empty] = string.Empty;
                    //}

                    //foreach (var filter in softwareAssemblyFilters)

                    //List<string> subProjectList = new List<string>();
                    List<string> skuList = new List<string>();

                    try
                    {
                        //subProjectList.AddRange(softwareAssemblyDocument.Descendants("SoftwareAssembly").Select(ele => ele.Attribute("Description").Value.Split(',')[2]).Distinct());
                        foreach (var description in softwareAssemblyDocument.Descendants("SoftwareAssembly").Select(ele => ele.Attribute("Description").Value.Split(',')))
                        {
                            if (description[3] == "WI-FI" || description[3] == "LTE")
                            {
                                skuList.Add(string.Join(",", description.Skip(2).Take(4)));
                            }
                            else
                            {
                                skuList.Add(string.Join(",", description.Skip(2).Take(3)));
                            }
                        }

                        skuList = skuList.Distinct().ToList();
                    }
                    catch (Exception)
                    {
                        ;
                    }

                    //if (subProjectList.Count == 0)
                    //{
                    //    subProjectList.Add(string.Empty);
                    //}

                    if (skuList.Count == 0)
                    {
                        skuList.Add(string.Empty);
                    }

                    foreach (var skuTemp in skuList)
                    {
                        //string promiseCityFinal = string.Join("_", filter.Key, promiseCity).Trim('_');
                        //string promiseCityFinal = string.Join("_", subProject.Replace(" ", ""), promiseCity).Trim('_');

                        string promiseCityFinal = promiseCity;
                        if (string.IsNullOrEmpty(promiseCityFinal))
                        {
                            promiseCityFinal = "NULL";
                        }

                        if (!promiseCityDict.ContainsKey(promiseCityFinal))
                        {
                            skuDocumentDict = new Dictionary<string, Func<string, string, XDocument>>();
                            promiseCityDict[promiseCityFinal] = skuDocumentDict;
                        }

                        foreach (var versionElement in imageConfigDocument.Descendants("CustomerImage").Descendants(@"ImageVersion"))
                        {
                            if (!string.IsNullOrEmpty(versionElement.Attribute("Version").Value))
                            {
                                foreach (var skuElement in versionElement.Descendants("Image"))
                                {
                                    if (!string.IsNullOrEmpty(skuElement.Attribute("ImageNumber").Value) || !string.IsNullOrEmpty(skuElement.Attribute("ImageRegionPartNumber").Value))
                                    {
                                        //if (!string.IsNullOrEmpty(skuElement.Attribute("ImageNumber").Value))
                                        //{
                                        //    sku = $"{versionElement.Attribute("Version").Value}_{skuElement.Attribute("ImageNumber").Value}";
                                        //}
                                        //else
                                        //{
                                        //    sku = $"{versionElement.Attribute("Version").Value}_{skuElement.Attribute("ImageRegionPartNumber").Value}";
                                        //}

                                        //if (!string.IsNullOrEmpty(subProject))
                                        //{
                                        //    sku = $"{subProject.Replace(" ", string.Empty)}_{sku}";
                                        //}

                                        partNumber = skuElement.Attribute("ImageRegionPartNumber").Value;

                                        //XElement currentSoftwareAssembly = softwareAssemblyDocument.Descendants("SoftwareAssembly").FirstOrDefault(element => element.Element("ImageConfig").Attribute("PartNumber").Value == partNumber && (string.IsNullOrEmpty(filter.Value) || filter.Value.ToUpper().Contains(element.Attribute("PartNumber").Value.ToUpper())));
                                        //XElement currentSoftwareAssembly = softwareAssemblyDocument.Descendants("SoftwareAssembly").FirstOrDefault(element => element.Element("ImageConfig").Attribute("PartNumber").Value == partNumber && (string.IsNullOrEmpty(subProject) || element.Attribute("Description").Value.ToUpper().Contains($",{subProject.ToUpper()},")));
                                        XElement currentSoftwareAssembly = softwareAssemblyDocument.Descendants("SoftwareAssembly").FirstOrDefault(element => element.Element("ImageConfig").Attribute("PartNumber").Value == partNumber && (string.IsNullOrEmpty(skuTemp) || element.Attribute("Description").Value.ToUpper().Contains(skuTemp.ToUpper())));

                                        if (currentSoftwareAssembly == null)
                                        {
                                            //logNotify.WriteLog($"ImageRegionPartNumber {partNumber} of SKU {sku} not found in {softwareAssemblyFile}", true);
                                            continue;
                                        }

                                        if (string.IsNullOrEmpty(skuTemp))
                                        {
                                            if (!string.IsNullOrEmpty(skuElement.Attribute("ImageNumber").Value))
                                            {
                                                sku = $"{versionElement.Attribute("Version").Value}_{skuElement.Attribute("ImageNumber").Value}";
                                            }
                                            else
                                            {
                                                sku = $"{versionElement.Attribute("Version").Value}_{skuElement.Attribute("ImageRegionPartNumber").Value}";
                                            }

                                            int i = 0;

                                            while (skuDocumentDict.ContainsKey($"{sku}_{i}"))
                                            {
                                                i++;
                                            }

                                            sku = $"{sku}_{i}";
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(skuElement.Attribute("ImageNumber").Value))
                                            {
                                                if (skuTemp.Contains("WI-FI") || skuTemp.Contains("LTE"))
                                                {
                                                    sku = $"{string.Join("_", skuTemp.Split(',').Take(2))}_{versionElement.Attribute("Version").Value}_{skuElement.Attribute("ImageNumber").Value}".Replace(" ", "");
                                                }
                                                else
                                                {
                                                    sku = $"{string.Join("_", skuTemp.Split(',').Take(1))}_{versionElement.Attribute("Version").Value}_{skuElement.Attribute("ImageNumber").Value}".Replace(" ", "");
                                                }

                                                sku = sku.Replace(" ", "");
                                            }
                                            else
                                            {
                                                sku = skuTemp.Replace(" ","").Replace(",","_");
                                            }
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


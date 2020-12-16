using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Utilities;

namespace ProvisioningBuildTools.SelectOutput
{
    public class SelectPackagesInfoOutput
    {
        public Func<string, string> GeneratePackageConfig { get; }

        public SelectPackagesInfoOutput(List<Package> packages)
        {
            GeneratePackageConfig = new Func<string, string>(
                (packageConfigFile) =>
                                    {
                                        StringBuilder resultSB = new StringBuilder();

                                        packages.Distinct(new Package());

                                        XmlDocument packageDoc = new XmlDocument();
                                        packageDoc.Load(packageConfigFile);
                                        XmlElement root = packageDoc.DocumentElement;

                                        for (int i = 0; i < packages.Count; i++)
                                        {
                                            Package package = packages[i];

                                            Version version = new Version(package.Version);

                                            if (version.Build == -1)
                                            {
                                                version = new Version(version.Major, version.Minor, 0);
                                            }

                                            if (version.Revision == -1)
                                            {
                                                version = new Version(version.Major, version.Minor, version.Build, 0);
                                            }

                                            string versionStr = version.Revision == 0 ? version.ToString(3) : version.ToString(4);

                                            string shareFolder = Path.Combine(Command.GLOBALPACKAGEFOLDER, "Packages", package.Id, versionStr);

                                            resultSB.AppendLine($@"Please share: {shareFolder}\");

                                            if (Directory.Exists(shareFolder))
                                            {
                                                Directory.Delete(shareFolder, true);
                                            }

                                            if (i >= root.ChildNodes.Count)
                                            {
                                                root.AppendChild(root.LastChild.Clone());
                                            }

                                            root.LastChild.Attributes["id"].Value = package.Id;
                                            root.LastChild.Attributes["source"].Value = package.Source;
                                            root.LastChild.Attributes["Version"].Value = package.Version;
                                            root.LastChild.Attributes["UseDangerDestination"].Value = "True";
                                            root.LastChild.Attributes["destination"].Value = Path.Combine("Packages", package.Id, versionStr); ;
                                        }

                                        packageDoc.Save(packageConfigFile);

                                        return resultSB.ToString();
                                    }
                );
        }

        public SelectPackagesInfoOutput()
        {

        }
    }

    public class Package : IEqualityComparer<Package>
    {
        public string Id { get; set; }

        public string Version { get; set; }

        public string Source { get; set; }

        public Package(string id, string source, string version)
        {
            Id = id;
            Source = source;
            Version = version;
        }

        public Package()
        {
            Id = string.Empty;
            Source = string.Empty;
            Version = string.Empty;
        }

        public bool Equals(Package x, Package y)
        {
            if (x.Id.Trim().ToUpper() == y.Id.Trim().ToUpper() && x.Version.Trim().ToUpper() == y.Version.Trim().ToUpper() && x.Source.Trim().ToUpper() == y.Source.Trim().ToUpper())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(Package obj)
        {
            return $"{obj.Id.Trim().ToUpper()}{obj.Source.Trim().ToUpper()}{obj.Version.Trim().ToUpper()}".GetHashCode();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ProvisioningBuildTools
{
    public class ProjectSection : ConfigurationSection
    {
        [ConfigurationProperty("projects", IsDefaultCollection = false)]
        public ProjectConfigurationElementCollection Settings
        {
            get
            {
                return (ProjectConfigurationElementCollection)base["projects"];
            }
        }

        public static ProjectSection Instance
        {
            get
            {
                try
                {
                    ConfigurationManager.RefreshSection("projectSettings");

                    return (ProjectSection)ConfigurationManager.GetSection("projectSettings");
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }

    public class ProjectConfigurationElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ProjectConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as ProjectConfigurationElement).Name;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            get { return "project"; }
        }
    }

    public class ProjectConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("sourceFolder", DefaultValue = "")]
        public string SourceFolder
        {
            get { return (string)base["sourceFolder"]; }
            set { base["sourceFolder"] = value; }
        }

        [ConfigurationProperty("provisioningToolsFolder", DefaultValue = "")]
        public string ProvisioningToolsFolder
        {
            get { return (string)base["provisioningToolsFolder"]; }
            set { base["provisioningToolsFolder"] = value; }
        }

        [ConfigurationProperty("buildScriptsFolder", DefaultValue = "")]
        public string BuildScriptsFolder
        {
            get { return (string)base["buildScriptsFolder"]; }
            set { base["buildScriptsFolder"] = value; }
        }

        [ConfigurationProperty("provisioningPackageFolder", DefaultValue = "")]
        public string ProvisioningPackageFolder
        {
            get { return (string)base["provisioningPackageFolder"]; }
            set { base["provisioningPackageFolder"] = value; }
        }

        [ConfigurationProperty("capsuleFolder", DefaultValue = "")]
        public string CapsuleFolder
        {
            get { return (string)base["capsuleFolder"]; }
            set { base["capsuleFolder"] = value; }
        }

        [ConfigurationProperty("capsuleInfoConfigurationPath", DefaultValue = "")]
        public string CapsuleInfoConfigurationPath
        {
            get { return (string)base["capsuleInfoConfigurationPath"]; }
            set { base["capsuleInfoConfigurationPath"] = value; }
        }

        [ConfigurationProperty("inputGenealogyPath", DefaultValue = "")]
        public string InputGenealogyPath
        {
            get { return (string)base["inputGenealogyPath"]; }
            set { base["inputGenealogyPath"] = value; }
        }

        [ConfigurationProperty("taskOpCodeList", DefaultValue = "")]
        public string TaskOpCodeList
        {
            get { return (string)base["taskOpCodeList"]; }
            set { base["taskOpCodeList"] = value; }
        }

        [ConfigurationProperty("provisioningTesterPreArgFormat", DefaultValue = "")]
        public string ProvisioningTesterPreArgFormat
        {
            get { return (string)base["provisioningTesterPreArgFormat"]; }
            set { base["provisioningTesterPreArgFormat"] = value; }
        }

        [ConfigurationProperty("testEnvironmentType", DefaultValue = "")]
        public string TestEnvironmentType
        {
            get { return (string)base["testEnvironmentType"]; }
            set { base["testEnvironmentType"] = value; }
        }

        [ConfigurationProperty("inputGenealogyFileName", DefaultValue = "InputGenealogy*.xml")]
        public string InputGenealogyFileName
        {
            get { return (string)base["inputGenealogyFileName"]; }
            set { base["inputGenealogyFileName"] = value; }
        }

        [ConfigurationProperty("nodeNameForSN", DefaultValue = "SystemDutSn")]
        public string NodeNameForSN
        {
            get { return (string)base["nodeNameForSN"]; }
            set { base["nodeNameForSN"] = value; }
        }

        [ConfigurationProperty("useExternalProvisioningTester", DefaultValue = null)]
        public string UseExternalProvisioningTester
        {
            get { return (string)base["useExternalProvisioningTester"]; }
            set { base["useExternalProvisioningTester"] = value; }
        }
    }
}

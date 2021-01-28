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
                return (ProjectSection)ConfigurationManager.GetSection("projectSettings");
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
    }
}

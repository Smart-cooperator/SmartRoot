using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalProvisioningTester
{
    public class MappingSection : ConfigurationSection
    {
        public static MappingSection Instance
        {
            get
            {
                return (MappingSection)ConfigurationManager.GetSection("mappingSettings");
            }
        }

        [ConfigurationProperty("markedMappings", IsDefaultCollection = false)]
        public MarkedMappings MarkedMappings
        {
            get
            {
                return (MarkedMappings)base["markedMappings"];
            }
        }

        [ConfigurationProperty("commentMappings", IsDefaultCollection = false)]
        public CommentMappings CommentMappings
        {
            get
            {
                return (CommentMappings)base["commentMappings"];
            }
        }

        [ConfigurationProperty("taskOpMappings", IsDefaultCollection = false)]
        public TaskOpMappings TaskOpMappings
        {
            get
            {
                return (TaskOpMappings)base["taskOpMappings"];
            }
        }
    }

    public class MarkedMappings : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new MarkedMapping();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return Guid.NewGuid().ToString();
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            get { return "markedMapping"; }
        }
    }

    public class MarkedMapping : ConfigurationElement
    {
        [ConfigurationProperty("value", DefaultValue = "")]
        public string Value
        {
            get { return (string)base["value"]; }
            set { base["value"] = value; }
        }
    }

    public class CommentMappings : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CommentMapping();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as CommentMapping).Name;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            get { return "commentMapping"; }
        }
    }

    public class CommentMapping : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("allowedComments", IsDefaultCollection = false)]
        public AllowedComments AllowedComments
        {
            get
            {
                return (AllowedComments)base["allowedComments"];
            }
        }
    }

    public class AllowedComments : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new AllowedComment();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return Guid.NewGuid().ToString();
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            get { return "allowedComment"; }
        }
    }

    public class AllowedComment : ConfigurationElement
    {
        [ConfigurationProperty("value", DefaultValue = "")]
        public string Value
        {
            get { return (string)base["value"]; }
            set { base["value"] = value; }
        }
    }

    public class TaskOpMappings : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new TaskOpMapping();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as TaskOpMapping).Name;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            get { return "taskOpMapping"; }
        }
    }

    public class TaskOpMapping : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("value", DefaultValue = "")]
        public string Value
        {
            get { return (string)base["value"]; }
            set { base["value"] = value; }
        }
    }
}

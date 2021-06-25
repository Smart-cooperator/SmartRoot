using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProvisioningBuildTools.Global
{
    public class GlobalValue
    {
        public static ProjectList Root;

        /// <summary>
        /// Read HardwareValidation.xml into class object instance below
        /// </summary>
        public static void Deserialize()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ProjectList));
            
            string globalXml = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\Global\GlobalValue.xml";

            if (File.Exists(globalXml))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(globalXml))
                    {
                        Root = (ProjectList)serializer.Deserialize(reader);
                    }                                      
                }
                catch (Exception)
                {

                }
            }


            if (Root == null)
            {
                Root = new ProjectList();
                Root.ProjectsNode = new ProjectsNode();
                Root.ProjectsNode.Projects = new Project[] { };
            }
        }

        public static void Serialize()
        {
            if (Root == null)
            {
                Root = new ProjectList();
                Root.ProjectsNode = new ProjectsNode();
                Root.ProjectsNode.Projects = new Project[] { };
            }

            XmlSerializer serializer = new XmlSerializer(typeof(ProjectList));

            string globalXml = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\Global\GlobalValue.xml";

            if (!Directory.Exists(Path.GetDirectoryName(globalXml)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(globalXml));               
            }

            //if (File.Exists(globalXml))
            //{
            //    File.Delete(globalXml);
            //}

            using (StreamWriter writer = new StreamWriter(globalXml))
            {
                serializer.Serialize(writer, Root);
            }            
        }

        static GlobalValue()
        {
            Deserialize();
        }
    }
    [Serializable]
    [XmlRoot("projectList")]
    public class ProjectList
    {
        [XmlElement("selectedProject")]
        public string SelectedProject { get; set; }

        [XmlElement("projects")]
        public ProjectsNode ProjectsNode { get; set; }

        public Project GetProject(string projectName)
        {
            ProjectsNode = ProjectsNode ?? new ProjectsNode();

            return ProjectsNode.GetProject(projectName);

        }

        public Project AddProject(string name)
        {
            ProjectsNode = ProjectsNode ?? new ProjectsNode();

            return ProjectsNode.AddProject(name);
        }
    }

    [Serializable]
    public class ProjectsNode
    {
        [XmlElement("project", typeof(Project))]
        public Project[] Projects { get; set; }

        public Project GetProject(string projectName)
        {
            return Projects?.Where(b => b.Name.ToLower() == projectName?.ToLower()).FirstOrDefault();
        }

        public Project AddProject(string name)
        {
            Project project = new Project();
            project.Name = name;
            Projects = Projects ?? (new Project[0]);
            Projects = Projects.Append(project).ToArray();
            return project;
        }
    }

    [Serializable]
    public class Project
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("provisioningPackage")]
        public string ProvisioningPackage { get; set; }

        [XmlElement("serialNumber")]
        public string SerialNumber { get; set; }

        [XmlElement("slot")]
        public string Slot { get; set; }

        [XmlElement("taskOpCodeList")]
        public string TaskOpCodeList { get; set; }

        [XmlElement("usuallyTaskOpCodeList")]
        public string UsuallyTaskOpCodeList { get; set; }
    }
}

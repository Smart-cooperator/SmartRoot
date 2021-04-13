//-------------------------------------------------------------------------------
//Copyright (c) Microsoft Corporation.  All rights reserved.
//-------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CapsuleParser
{
    /// <summary>
    /// This class represents the container for CapsuleInfoConfiguration.xml file contents
    /// </summary>
    public class CapsuleInfoConfigurationFile
    {
        private Log log = Log.GetInstance;
      //  private ProductConfiguration config = ProductConfiguration.GetInstance;

        /// <summary>
        /// CapsuleInfoConfiguration.xml file XmlRoot node deserialized content object
        /// </summary>
        private CapsuleSettingList _capsuleSettingList;

        public string capsuleConfig;

        /// <summary>
        /// Constructor opens filename and deserializes it's content
        /// </summary>
        /// <param name="filename">Full path to the CapsuleInfoConfiguration.xml file to open and deserialize</param>
        public CapsuleInfoConfigurationFile(string inputCapsuleInfoConfigurationPath)
        {
            capsuleConfig = inputCapsuleInfoConfigurationPath;
            XmlSerializer serializer = new XmlSerializer(typeof(CapsuleSettingList));

            // Commented out while this.Serializer_UnknownElement is not in use (see note on function below)
            //serializer.UnknownElement += new XmlElementEventHandler(Serializer_UnknownElement);
            
            FileStream fs = new FileStream(capsuleConfig, FileMode.Open, FileAccess.Read);
            _capsuleSettingList = serializer.Deserialize(fs) as CapsuleSettingList;
            fs.Close();

            if (_capsuleSettingList != null)
            {
                _capsuleSettingList.Initialize();
                _capsuleSettingList.PopulateChildElements(null);
                _capsuleSettingList.PopulateCapsuleVersionHelperClasses();
            }
        }

        /// <summary>
        /// Write out the XML Objects to CapsuleInfoConfiguration.xml file
        /// </summary>
        public void Serialize()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            XmlSerializer serializer = new XmlSerializer(typeof(CapsuleSettingList));
            FileStream fs = new FileStream(this.capsuleConfig, FileMode.Open, FileAccess.ReadWrite);
            fs.SetLength(0);
            XmlWriter writer = XmlWriter.Create(fs, settings);
            serializer.Serialize(writer, _capsuleSettingList);
            fs.Close();
        }

        /// <summary>
        /// Validate CapsuleInfoConfiguration.xml file recursively from the XmlRoot element
        /// </summary>
        /// <returns>true if all validations succeed</returns>
        public bool Validate()
        {
            return _capsuleSettingList.Validate();
        }

        /// <summary>
        /// Walk the element tree, calling ListCapsuleInfo on each element object
        /// </summary>
        /// <returns>string of capsule info</returns>
        public string ListCapsuleInfo()
        {
            log.Dump(); // start clean
            _capsuleSettingList.ListCapsuleInfo();

            StringBuilder sb = new StringBuilder();
            List<string> loglines = log.GetLogMessages;

            foreach(string line in loglines)
            {
                sb.Append(line);
                sb.Append("\r\n");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Retrieve the list of CapsuleInfo elements in the specified CombinationName element
        /// </summary>
        /// <param name="capsuleCombinationName">CombinationName element to query</param>
        /// <returns>List of CapsuleInfo objects from this CombinationName element</returns>
        public List<CapsuleInfo> GetCapsuleInfoList(string capsuleCombinationName)
        {
            CapsuleCombination cc = GetCapsuleCombination(capsuleCombinationName).FirstOrDefault();
            return cc == null ? new List<CapsuleInfo>() : cc.CapsuleInfos.ToList();
        }

        /// <summary>
        /// Retrieve the list of CapsuleCombination elements
        /// </summary>
        /// <returns>List of CapsuleCombinations</returns>
        public List<CapsuleCombination> GetCapsuleCombinations()
        {
            return _capsuleSettingList.CapsuleCombinationList.CapsuleCombinations.ToList();
        }

        /// <summary>
        /// Retrieve the specific CapsuleCombination element object by name
        /// </summary>
        /// <param name="capsuleCombinationName">CapsuleCombination name to retrieve</param>
        /// <returns>List of CapsuleCombination matching the specified name</returns>
        public List<CapsuleCombination> GetCapsuleCombination(string capsuleCombinationName)
        {
            return GetCapsuleCombinations().Where(c => c.Name.Equals(capsuleCombinationName, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /// <summary>
        /// Retrieve the specific Capsule element object by partial .INF filename
        /// </summary>
        /// <param name="infFilename">.INF filespec to search for</param>
        /// <returns>Capsule object if found, else null</returns>
        public Capsule GetCapsuleByInfFilePath(string infFilename)
        {
            Capsule[] capsules = _capsuleSettingList.CapsuleList.Capsules;
            Capsule retval = null;
            
            foreach(Capsule capsule in capsules)
            {
               // infFilename.ToLower().Contains(capsule.DeviceHardwareId.CapsulePath.ToLower())
              //  if (capsule.DeviceHardwareId.CapsulePath.ToLower().CompareTo(infFilename.ToLower()) == 0)
                if(infFilename.ToLower().Contains(capsule.DeviceHardwareId.CapsulePath.ToLower()))
                {
                    retval = capsule;
                    break;
                }
            }
            return retval;
        }

        /// <summary>
        /// Retrieve the Capsule object given a Hardware Id
        /// </summary>
        /// <param name="hardwareId">HardwareId of the Capsule object to retrieve</param>
        /// <returns>Capsule object matching the specified HardwareId</returns>
        public Capsule GetCapsuleByHardwareId(string hardwareId)
        {
            CapsuleList list = _capsuleSettingList.CapsuleList;
            return list.Capsules.Where(c => c.DeviceHardwareId.Id.Equals(hardwareId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault() as Capsule;
        }

        /// <summary>
        /// XmlSerialization callback for XML File elements which are unknown in the Serializable classes declared below in this file
        /// This method is primarily for development debugging support, but can be used, if necessary, during runtime processing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Serializer_UnknownElement(object sender, XmlElementEventArgs e)
        {
            //System.Diagnostics.Debugger.Break();
        }
    }

    /// <summary>
    /// Base class for all serializable CapsuleInfoConfiguration.xml element class objects.
    /// Used by reflection code to assist in Populating childElements to provide for navigation 
    /// </summary>
    public abstract class CapsuleInfoConfigurationElementBaseClass
    {
   
    //    [XmlIgnore()]
   //     protected Log log = Log.GetInstance;

        /// <summary>
        /// Every Xml Element class has an Identity by name
        /// Note that Array objects have numeric suffixes to provide Dictionary.Key uniqueness
        /// </summary>
        [XmlIgnore()]
        public string ElementName;

        /// <summary>
        /// Parent Element object used for navigation (FindRoot) and other 'look-aside' operations
        /// </summary>
        [XmlIgnore()]
        public CapsuleInfoConfigurationElementBaseClass Parent;

        /// <summary>
        /// Dictionary of ChildElements for purpose of finding child objects recursively
        /// </summary>
        [XmlIgnore()]
        protected Dictionary<string, CapsuleInfoConfigurationElementBaseClass> ChildElements;

        /// <summary>
        /// Path to root directory where capsules are kept (ExternalDrops\Bin\[PRODUCT] in most product branches)
        /// </summary>
        [XmlIgnore()]
        public string CapsuleBinariesPath { get; set; }

        /// <summary>
        /// Used to provide Comments on Xml Element. Replaces <!-- --> comments in XML
        /// </summary>
        [XmlAttribute(Constants.Comment)]
        public string Comment { get; set; }

        /// <summary>
        /// Required Validate Methods for all derived classes
        /// </summary>
        /// <param name="messages">LogMessages to collect report data</param>
        /// <returns>true if call capsules validate</returns>
        public abstract bool Validate();

        /// <summary>
        /// List any/all information regarding each element, attribute and child elements
        /// </summary>
        public abstract void ListCapsuleInfo();

        public virtual void Initialize()
        {
           // this.CapsuleBinariesPath = this.config.Get(Constants.CapsuleBinariesPath);
        }

        /// <summary>
        /// What the Serializable Object tree and pre-populate the Parent and ChildElements members to assist in subsequent navigations
        /// </summary>
        /// <param name="parent">Parent Object of this object to hydrate</param>
        public void PopulateChildElements(CapsuleInfoConfigurationElementBaseClass parent)
        {
            // Store the Parent for navigation. Root is null.
            Parent = parent;

            // Create a Dictionary of Child Objects
            ChildElements = new Dictionary<string, CapsuleInfoConfigurationElementBaseClass>(StringComparer.InvariantCultureIgnoreCase);

            // Get the Object Properties which are Public, Instance and based on CapsuleInfoConfigurationElementBaseClass
            List<PropertyInfo> propInfos = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();

            // Populate ChildElements
            foreach (PropertyInfo prop in propInfos)
            {
                if (prop.PropertyType.IsArray)
                {
                    Array propArray = (Array)prop.GetValue(this);

                    if (propArray != null)
                    {
                        for (int i = 0; i < propArray.Length; i++)
                        {
                            object arrayObject = propArray.GetValue(i);

                            if (arrayObject != null)
                            {
                                if(arrayObject.GetType().IsSubclassOf(typeof(CapsuleInfoConfigurationElementBaseClass)))
                                {
                                    CapsuleInfoConfigurationElementBaseClass obj = arrayObject as CapsuleInfoConfigurationElementBaseClass;
                                    obj.ElementName = arrayObject.GetType().Name + i.ToString();
                                    ChildElements.Add(obj.ElementName, obj);
                                }

                                // Set ComponentType enum
                                if(arrayObject.GetType() == typeof(CapsuleInfo))
                                {
                                    ((CapsuleInfo)arrayObject).SetComponentType();
                                }

                                // Set UpdateCondition enum
                                if (arrayObject.GetType() == typeof(CapsuleProperty))
                                {
                                    ((CapsuleProperty)arrayObject).SetUpdateConditionSetting();
                                }
                            }
                        }
                    }
                }
                else
                {
                    object propValue = prop.GetValue(this);

                    if (propValue != null && propValue.GetType().IsSubclassOf(typeof(CapsuleInfoConfigurationElementBaseClass)))
                    {
                        CapsuleInfoConfigurationElementBaseClass obj = propValue as CapsuleInfoConfigurationElementBaseClass;
                        obj.ElementName = prop.Name;
                        ChildElements.Add(obj.ElementName, obj);
                    }
                }
            }

            // Recurse in to ChildElements to populate their child objects
            foreach (CapsuleInfoConfigurationElementBaseClass element in ChildElements.Values)
            {
                element.Initialize();
                element.PopulateChildElements(this);
            }
        }

        /// <summary>
        /// Walk the ChildElement tree(s) and for all AllowedVersion objects, Set their CapsuleVersionHelperClass objects
        /// CapsuleVersionHelperClass objects contain the code necessary to interpret a Capsule's version schema formatting
        /// </summary>
        public void PopulateCapsuleVersionHelperClasses()
        {
            foreach (CapsuleInfoConfigurationElementBaseClass element in ChildElements.Values)
            {
                if (element.GetType() == typeof(AllowedVersion))
                {
                    AllowedVersion allowedVersion = element as AllowedVersion;

                    if (allowedVersion != null)
                    {
                        allowedVersion.SetCapsuleVersionHelperClass(); // allowedVersionList.deviceHardwareId.Capsule
                    }
                }
                element.PopulateCapsuleVersionHelperClasses();
            }
        }

        /// <summary>
        /// Walk the parent member objects until we find the root object (root object parent member == null)
        /// </summary>
        /// <returns>Root object of the XmlElements tree</returns>
        public CapsuleInfoConfigurationElementBaseClass FindRootElement()
        {
            CapsuleInfoConfigurationElementBaseClass parent = Parent;
            
            while (parent.Parent != null)
            {
                parent = parent.Parent;
            }
            return parent;
        }

        /// <summary>
        /// Walk the ChildElement tree, returning the object which matches the specified name
        /// </summary>
        /// <param name="name">name of the element to retrieve</param>
        /// <returns>XmlElement object with the specified name</returns>
        public CapsuleInfoConfigurationElementBaseClass FindChildElementByName(string name)
        {
            CapsuleInfoConfigurationElementBaseClass foundElement = null;

            // Due to Array Element Suffix (0,1,2...), need to iterate each using StartsWith compare
            foreach(string childElementName in this.ChildElements.Keys)
            {
                if(childElementName.StartsWith(name))
                {
                    foundElement = ChildElements[childElementName];
                    break;
                }
            }

            if(foundElement == null)
            {
                foreach (CapsuleInfoConfigurationElementBaseClass currentElement in ChildElements.Values)
                {
                    foundElement = currentElement.FindChildElementByName(name);

                    if (foundElement != null)
                    {
                        break;
                    }
                }
            }
            return foundElement;
        }

        /// <summary>
        /// Walk the ChildElements tree, returning an array of objects which .StartsWith the specified name
        /// .StartsWith is used so that XmlElement arrays, which has numeric suffixes, are queryable
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CapsuleInfoConfigurationElementBaseClass[] FindChildElementsByName(string name)
        {
            List<CapsuleInfoConfigurationElementBaseClass> foundElements = new List<CapsuleInfoConfigurationElementBaseClass>();

            // Iterate on the current ChildElements
            foreach (string key in ChildElements.Keys)
            {
                if (key.StartsWith(name, StringComparison.OrdinalIgnoreCase))
                {
                    foundElements.Add(ChildElements[key]);
                }
            }

            // Iterate on the ChildElement's ChildElements
            foreach (CapsuleInfoConfigurationElementBaseClass child in ChildElements.Values)
            {
                foundElements.AddRange(child.FindChildElementsByName(name).ToList());
            }
            return foundElements.ToArray();
        }

        public void Log(string msg)
        {
            // log.Append(msg);
        }
    }

    /// <summary>
    /// Root object of the CapsuleInfoConfiguration file
    /// </summary>
    [Serializable()]
    [XmlRoot(Constants.capsuleSettingList)]
    public class CapsuleSettingList : CapsuleInfoConfigurationElementBaseClass
    {
        [XmlElement(Constants.capsuleCombinationList, typeof(CapsuleCombinationList))]
        public CapsuleCombinationList CapsuleCombinationList { get; set; }

        [XmlElement(Constants.capsuleList, typeof(CapsuleList))]
        public CapsuleList CapsuleList { get; set; }

        [XmlElement("capsuleToolList", typeof(CapsuleToolList))]
        public CapsuleToolList CapsuleToolList { get; set; }

        /// <summary>
        /// Validate method, not implemented yet
        /// </summary>
        /// <param name="messages">LogMessages to collect report data</param>
        /// <returns>true if call capsules validate</returns>
        public override bool Validate()
        {
            // TODO: CapsuleCombinationList.Validate();
            return CapsuleList.Validate();
        }

        public override void ListCapsuleInfo()
        {
            CapsuleCombinationList.ListCapsuleInfo();
            CapsuleList.ListCapsuleInfo();
        }
    }

    /// <summary>
    /// CapsuleTools element section of CapsuleInfoConfiguration file
    /// </summary>
    public class CapsuleToolList
    {
        [XmlElement("tool", typeof(CapsuleTool))]
        public CapsuleTool[] CapsuleTools { get; set; }
    }

    /// <summary>
    /// CapsuleTool object of the CapsuleToolList in XML
    /// </summary>
    public class CapsuleTool
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("path")]
        public string Path { get; set; }

        [XmlElement("command")]
        public string Command { get; set; }

        [XmlElement("arguments")]
        public string Args { get; set; }
    }
    /// <summary>
    /// CapsuleCombinationList element section of CapsuleInfoConfiguration file
    /// </summary>
    [Serializable()]
    public class CapsuleCombinationList : CapsuleInfoConfigurationElementBaseClass
    {
        [XmlElement(Constants.capsuleCombination, typeof(CapsuleCombination))]
        public CapsuleCombination[] CapsuleCombinations { get; set; }

        [XmlElement("selectOneCapsuleCombination", typeof(SelectOneCapsuleCombination))]
        public SelectOneCapsuleCombination[] SelectOneCapsuleCombinations { get; set; }

        /// <summary>
        /// Validate method, not implemented yet
        /// </summary>
        /// <param name="messages">LogMessages to collect report data</param>
        /// <returns>true if call capsules validate</returns>
        public override bool Validate()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Iterate on child elements calling ListCapsuleInfo
        /// </summary>
        public override void ListCapsuleInfo()
        {
            foreach(CapsuleCombination capsuleCombo in CapsuleCombinations)
            {
                capsuleCombo.ListCapsuleInfo();
            }
        }
    }

    /// <summary>
    /// CapsuleCombination represents a set of capsules to update as a group, previous to the reboot which causes UEFI to perform capsule updates
    /// </summary>
    [Serializable()]
    public class CapsuleCombination : CapsuleInfoConfigurationElementBaseClass
    {
        [XmlAttribute(Constants.nameAttribute)]
        public string Name { get; set; }

        [XmlElement(Constants.capsuleInfo, typeof(CapsuleInfo))]
        public CapsuleInfo[] CapsuleInfos { get; set; }

        /// <summary>
        /// Validate method, not implemented yet
        /// </summary>
        /// <param name="messages">LogMessages to collect report data</param>
        /// <returns>true if call capsules validate</returns>
        public override bool Validate()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Iterate on child elements calling ListCapsuleInfo
        /// </summary>
        public override void ListCapsuleInfo()
        {
            Log($"CapsuleCombination: {Name}");

            foreach (CapsuleInfo capsuleInfo in CapsuleInfos)
            {
                capsuleInfo.ListCapsuleInfo();
            }
        }
    }

    public class SelectOneCapsuleCombination : CapsuleCombination
    {
        [XmlAttribute("component")]
        public string Component { get; set; }

        [XmlElement("selectOneCapsuleInfo")]
        public CapsuleInfo[] SelectOneCapsuleInfos { get; set; }
    }

    /// <summary>
    /// CapsuleInfo element section
    /// </summary>
    [Serializable()]
    public class CapsuleInfo : CapsuleInfoConfigurationElementBaseClass
    {
        [XmlAttribute(Constants.component)]
        public string Component { get; set; }

        [XmlAttribute(Constants.installSet)]
        public string InstallSet { get; set; }

        [XmlAttribute(Constants.skipIfNotFound)]
        public bool SkipIfNotFound { get; set; }

        [XmlElement(Constants.capsuleProperty, typeof(CapsuleProperty))]
        public CapsuleProperty[] CapsuleProperties { get; set; }

        /// <summary>
        /// An Enumerated setting for the Type of Capsule specified by string Component in the XML
        /// </summary>
        [XmlIgnore()]
        public CapsuleInfoType CapsuleInfoType { get; set; }

        /// <summary>
        /// Method to set the enumeration CapsuleInfoType from the Xml's Component specification
        /// </summary>
        /// <returns></returns>
        public bool SetComponentType()
        {
            bool retval = true; 

            switch(Component)
            {
                case Constants.UEFI:
                    CapsuleInfoType = CapsuleInfoType.Uefi;
                    break;
                case Constants.Touch:
                    CapsuleInfoType = CapsuleInfoType.Touch;
                    break;
                case Constants.SAM:
                    CapsuleInfoType = CapsuleInfoType.Sam;
                    break;
                case Constants.UEFI_Transition:
                    CapsuleInfoType = CapsuleInfoType.UEFI_Transition; // ??? Should this just be UEFI?
                    break;
                case Constants.ME:
                    CapsuleInfoType = CapsuleInfoType.ME;
                    break;
                case Constants.USBC:
                    CapsuleInfoType = CapsuleInfoType.USBC;
                    break;
                case Constants.PD:
                    CapsuleInfoType = CapsuleInfoType.PD;
                    break;
                case Constants.SMF:
                    CapsuleInfoType = CapsuleInfoType.SMF;
                    break;
                case Constants.TCON:
                    CapsuleInfoType = CapsuleInfoType.TCON;
                    break;
                default:
                    CapsuleInfoType = CapsuleInfoType.Unknown;
                    retval = false;
                    break;
            }
            return retval;
        }

        /// <summary>
        /// Try to get a CapsuleProperty instance that match with the given hardware ID.
        /// </summary>
        /// <param name="hardwareId">The hardware ID of device that wanted.</param>
        /// <returns>The CapsuleProperty instance that with matched hardware ID.</returns>
        public CapsuleProperty TryGetMatchedCapsuleProperty(string hardwareId)
        {
            CapsuleProperty capsuleProperty = null;

            foreach (CapsuleProperty property in CapsuleProperties)
            {
                if (hardwareId.IndexOf(property.GetCapsule().DeviceHardwareId.Id, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    capsuleProperty = property;
                    break;
                }
            }
            return capsuleProperty;
        }

        /// <summary>
        /// Determines if a Capsule has to be updated given the hardwareId and current hardwareVersion
        /// </summary>
        /// <param name="hardwareId">HardwareId of the Capsule to check</param>
        /// <param name="hardwareVersion">Current Capsule Version on the DUT to compare to</param>
        /// <returns>true if the capsule needs to be updated</returns>
        public bool UpdateNeeded(string hardwareId, DeviceFirmwareVersion hardwareVersion)
        {
            bool retval = false;

            CapsuleProperty capsuleProperty = TryGetMatchedCapsuleProperty(hardwareId);

            if(capsuleProperty != null)
            {
                retval = capsuleProperty.UpdateNeeded(hardwareVersion);
            }
            return retval;
        }

        /// <summary>
        /// Validate method, not implemented yet
        /// </summary>
        /// <param name="messages">LogMessages to collect report data</param>
        /// <returns>true if call capsules validate</returns>
        public override bool Validate()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Iterate on child elements calling ListCapsuleInfo
        /// </summary>
        public override void ListCapsuleInfo()
        {
            Log($"    CapsuleInfo: {Component}, installSet={InstallSet}, skipIfNotFound={SkipIfNotFound}");

            foreach (CapsuleProperty capsuleProp in CapsuleProperties)
            {
                capsuleProp.ListCapsuleInfo();
            }
        }
    }

    /// <summary>
    /// CapsuleProperty element(s) of the CapsuleInfoConfiguration file
    /// </summary>
    [Serializable()]
    public class CapsuleProperty : CapsuleInfoConfigurationElementBaseClass
    {
        [XmlAttribute(Constants.capsuleName)]
        public string CapsuleName { get; set; }

        [XmlAttribute(Constants.updateCondition)]
        public string UpdateCondition { get; set; }

        /// <summary>
        /// An Enumeration of the UpdateCondition string specified in the XML
        /// </summary>
        [XmlIgnore()]
        public CapsulePropertyUpdateCondition UpdateConditionSetting { get; set; }

        /// <summary>
        /// Method to set the UpdateConditionSetting enum based on the UpdateCondition specified in XML
        /// </summary>
        /// <returns>true if UpdateConditionSetting was effectively set</returns>
        public bool SetUpdateConditionSetting()
        {
            bool retval = true;

            switch(UpdateCondition)
            {
                case Constants.UpdateConditionAlways:
                    UpdateConditionSetting = CapsulePropertyUpdateCondition.Always;
                    break;
                case Constants.UpdateConditionNever:
                    UpdateConditionSetting = CapsulePropertyUpdateCondition.Never;
                    break;
                case Constants.UpdateConditionOnMismatch:
                    UpdateConditionSetting = CapsulePropertyUpdateCondition.OnMismatch;
                    break;
                case Constants.UpdateConditionLessThan:
                    UpdateConditionSetting = CapsulePropertyUpdateCondition.LessThan;
                    break;
                default:
                    UpdateConditionSetting = CapsulePropertyUpdateCondition.Unknown;
                    retval = false;
                    break;
            }
            return retval;
        }

        /// <summary>
        /// Return the Capsule associated with this CapsuleProperty object
        /// </summary>
        /// <returns>Capsule object matching this CapsuleProperty by CapsuleName</returns>
        public Capsule GetCapsule()
        {
            CapsuleInfoConfigurationElementBaseClass root = FindRootElement();
            CapsuleList capsuleList = root.FindChildElementByName(Constants.capsuleList) as CapsuleList;
            return capsuleList.Capsules.Where(c => c.Name.Equals(CapsuleName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault() as Capsule;
        }

        /// <summary>
        /// Determines if a Capsule has to be updated given this capsule's hardwareId and current hardwareVersion
        /// </summary>
        /// <param name="hardwareVersion">Current Capsule Version on the DUT to compare to</param>
        /// <returns>true if the capsule needs to be updated</returns>
        public bool UpdateNeeded(DeviceFirmwareVersion hardwareVersion)
        {
            bool retval = false;

            Capsule capsule = GetCapsule();
            AllowedVersion currentVersion = capsule.DeviceHardwareId.AllowedVersionList.GetCurrentVersion();
            retval = currentVersion.UpdateNeeded(hardwareVersion, this.UpdateConditionSetting);

            return retval;
        }

        /// <summary>
        /// Validate method, not implemented yet
        /// </summary>
        /// <param name="messages">LogMessages to collect report data</param>
        /// <returns>true if call capsules validate</returns>
        public override bool Validate()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Iterate on child elements calling ListCapsuleInfo
        /// </summary>
        public override void ListCapsuleInfo()
        {
            Log($"        CapsuleProperty: {CapsuleName}, updateCondition={UpdateCondition}");
        }
    }

    /// <summary>
    /// Collection of Capsule objects in XML
    /// </summary>
    [Serializable()]
    public class CapsuleList : CapsuleInfoConfigurationElementBaseClass
    {
        [XmlElement(Constants.capsule, typeof(Capsule))]
        public Capsule[] Capsules { get; set; }

        /// <summary>
        /// Dispatch to each Capsule object to validate
        /// </summary>
        /// <param name="messages">LogMessages to collect report data</param>
        /// <returns>true if call capsules validate</returns>
        public override bool Validate()
        {
            bool retval = true;
            int numCapsules = Capsules.Length;

            for (int i = 0; i < numCapsules && retval == true; i++)
            {
                retval = retval && Capsules[i].Validate();
            }
            return retval;
        }

        /// <summary>
        /// Iterate on child elements calling ListCapsuleInfo
        /// </summary>
        public override void ListCapsuleInfo()
        {
            foreach (Capsule capsule in Capsules)
            {
                capsule.ListCapsuleInfo();
            }
        }
    }

    /// <summary>
    /// Capsule Objects in XML
    /// </summary>
    [Serializable()]
    public class Capsule : CapsuleInfoConfigurationElementBaseClass
    {
        [XmlAttribute(Constants.nameAttribute)]
        public string Name { get; set; }

        [XmlElement(Constants.deviceHardwareId, typeof(DeviceHardwareId))]
        public DeviceHardwareId DeviceHardwareId { get; set; }

        /// <summary>
        /// Drill down into the DeviceHardwareId element to validate
        /// </summary>
        /// <param name="messages">LogMessages to collect report data</param>
        /// <returns>true if call capsules validate</returns>
        public override bool Validate()
        {
            return DeviceHardwareId.Validate();
        }

        /// <summary>
        /// Iterate on child elements calling ListCapsuleInfo
        /// </summary>
        public override void ListCapsuleInfo()
        {
            Log($"Capsule: {Name}");
            DeviceHardwareId.ListCapsuleInfo();
        }
    }

    /// <summary>
    /// DeviceHardwareId object of the Capsules in XML
    /// </summary>
    [Serializable()]
    public class DeviceHardwareId : CapsuleInfoConfigurationElementBaseClass
    {
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        [XmlAttribute(Constants.id)]
        public string Id { get; set; }

        [XmlElement(Constants.capsulePath)]
        public string CapsulePath { get; set; }

        [XmlElement(Constants.allowedVersionList, typeof(AllowedVersionList))]
        public AllowedVersionList AllowedVersionList { get; set; }

        [XmlIgnore()]
        public string infFile;
        [XmlIgnore()]
        public string infFilePath;

        /// <summary>
        /// Called by parent element to pass the root path to files in local product branch directory
        /// </summary>
        /// <param name="capsuleRootPath">Path to product branch capsule files</param>
        public override void Initialize()
        {
            base.Initialize();
            // If a product branch is non-conformant in path mapping to DUT, Configuration.xml can provide a "CapsulePathReplaceToken" element to special case mapping
        //    string capsulePathReplaceToken = string.IsNullOrWhiteSpace(config.Get(Constants.CapsulePathReplaceToken)) ? Constants.DutDefaultCapsulePath : config.Get(Constants.CapsulePathReplaceToken);
        //    infFile = CapsulePath.ToLower().Replace(capsulePathReplaceToken.ToLower(), this.CapsuleBinariesPath.ToLower());
        //    infFilePath = Path.GetDirectoryName(infFile);
        }

        /// <summary>
        /// Validate presence of .CAT, .INF files and .BIN file contains the AllowedVersion
        /// </summary>
        /// <param name="messages">LogMessages to collect report data</param>
        /// <returns>true if call capsules validate</returns>
        public override bool Validate()
        {
            return true;
        }

        /// <summary>
        /// Iterate on child elements calling ListCapsuleInfo
        /// </summary>
        public override void ListCapsuleInfo()
        {
            Log($"    DeviceHardwareId: {Id}");
            Log($"        CapsulePath: {CapsulePath}");
            AllowedVersionList.ListCapsuleInfo();
        }
    }

    /// <summary>
    /// List of AllowedVersions in XML
    /// </summary>
    [Serializable()]
    public class AllowedVersionList : CapsuleInfoConfigurationElementBaseClass
    {
        [XmlElement(Constants.allowedVersion, typeof(AllowedVersion))]
        public AllowedVersion[] AllowedVersions { get; set; }

        /// <summary>
        /// Name of the Capsule's .BIN file for validation of AllowedVersion(s)
        /// </summary>
        [XmlIgnore()]
        public string BinFile { get; set; }

        /// <summary>
        /// Drill down into each AllowedVersion to verify if .Bin file matches
        /// </summary>
        /// <param name="messages">LogMessages to collect report data</param>
        /// <returns>true if call capsules validate</returns>
        public override bool Validate()
        {
            bool retval = true;

            int allowedVersionCount = AllowedVersions.Length;

            for (int i = 0; i < allowedVersionCount; i++)
            {
                AllowedVersions[i].CapsuleBinariesPath = this.CapsuleBinariesPath;
                AllowedVersions[i].BinFile = this.BinFile;
                retval = AllowedVersions[i].Validate() && retval;
            }
            return retval;
        }

        /// <summary>
        /// Returns the AllowedVersion which represents the CurrentVersion to update to
        /// </summary>
        /// <returns>AllowedVersion object attributed as CurrentVersion</returns>
        public AllowedVersion GetCurrentVersion()
        {
            return AllowedVersions.Where(v => v.IsCurrentVersion() == true).FirstOrDefault();
        }

        /// <summary>
        /// Returns a string listing a Version format for each AllowedVersion in this list
        /// </summary>
        /// <returns>String of allowed versions</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach(AllowedVersion version in AllowedVersions)
            {
                sb.AppendFormat($"{version.GetDeviceFirmwareVersion().ToString()}");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Iterate on child elements calling ListCapsuleInfo
        /// </summary>
        public override void ListCapsuleInfo()
        {
            Log("        AllowedVersions:");

            foreach (AllowedVersion version in AllowedVersions)
            {
                DeviceFirmwareVersion fw = version.GetDeviceFirmwareVersion();

                if(!ReferenceEquals(fw,null))
                {
                    Log($"            {version.GetDeviceFirmwareVersion().ToString()}");
                }
                else
                {
                    foreach(XmlElement element in version.AllElements)
                    {
                        Log($"            {element.Name}: {element.InnerText}");
                    }
                }
            }
        }
    }

    /// <summary>
    /// AllowedVersion entry for a given capsule in XML
    /// </summary>
    [Serializable()]
    public class AllowedVersion : CapsuleInfoConfigurationElementBaseClass
    {
        [XmlAttribute]
        public string CapsuleVersionHelperClass { get; set; }

        [XmlAttribute]
        public string CurrentVersion { get; set; }          // If there are more than 1 AllowedVersion elements, 1 must has the attribute CurrentVersion="true"

        [XmlAttribute]
        public string CapsuleVersionFormat { get; set; }

        [XmlAttribute]
        public string ObsoleteVersion { get; set; }         // NOT USED YET: intended to be used to remove a capsule if this allowed version has the attributed ObsoleteVersion="true"

        [XmlAnyElement]
        public XmlElement[] AllElements { get; set; }

        [XmlIgnore()]
        public string BinFile { get; set; }

        /// <summary>
        /// Instance of a CapsuleVersion Helper Class which interprets the versioning information for this AllowedVersion specification
        /// </summary>
        [XmlIgnore()]
        public CapsuleVersionHelperBaseClass CapsuleVersion;

        /// <summary>
        /// CreateInstance and set CapsuleVersion for the CapsuleVersion Helper, and pass in the subelements of the AllowedVersion element in XML
        /// </summary>
        public void SetCapsuleVersionHelperClass()
        {
            CapsuleSettingList root = this.FindRootElement() as CapsuleSettingList;
            string capsuleName = string.Empty;

            // CapsuleVersionHelperClass, an attribute from AllowedVersion elements in CapsuleInfoConfiguration.xml,
            // is case sensitive and maps to a subclass of CapsuleVersionHelperBaseClass declared in CapsuleVersionHelpers.cs
            if (CapsuleVersionHelperClass == null)
            {
                // This CapsuleInfoConfiguration.xml file AllowedVersion elements do not have a CapsuleVersionHelperClass attribute
                // Try to determine the best class to use. Using the Capsule parent object name attribute, find the CapsuleProperty name attribute.
                // From there, use the CapsuleInfo parent component attribute as a suffix to "CapsuleVersion_" to select the helper class
                string component = string.Empty;
                CapsuleInfoConfigurationElementBaseClass allowedVersionList = this.Parent;
                CapsuleInfoConfigurationElementBaseClass deviceHardwareId = allowedVersionList.Parent;
                Capsule capsule = deviceHardwareId.Parent as Capsule;
                capsuleName = capsule.Name;

                List<CapsuleInfoConfigurationElementBaseClass> capsuleProptertyElements = root.FindChildElementsByName("capsuleProperty").ToList();
                foreach (CapsuleProperty capsuleProp in capsuleProptertyElements)
                {
                    CapsuleProperty capsuleProperty = capsuleProp as CapsuleProperty;
                    if (capsuleProperty.CapsuleName == capsuleName)
                    {
                        CapsuleInfo capsuleInfo = capsuleProperty.Parent as CapsuleInfo;
                        component = capsuleInfo.Component;
                        this.CapsuleVersionHelperClass = "CapsuleVersion_" + component;
                        break;
                    }
                }
            }
            if(this.CapsuleVersionHelperClass == null)
            {
                string msg = $"AllowedVersion.SetCapsuleVersionHelperClass: CapsuleHelperVersionClass not set for capsuleName = {capsuleName}. Likely no CapsuleInfo for this Capsule";
                root.Log(msg);
                return;
            }
            else
            {
                // string fullyQualifiedClassName = "Microsoft.Manufacturing.Provisioning.Product.DeviceAttributes.Capsules." + CapsuleVersionHelperClass;

                string fullyQualifiedClassName = "CapsuleParser." + CapsuleVersionHelperClass;

                Type t = Assembly.GetExecutingAssembly().GetType(fullyQualifiedClassName);
                if (t != null)
                {
                    CapsuleVersion = Activator.CreateInstance(t) as CapsuleVersionHelperBaseClass;
                    CapsuleVersion.SetDeviceFirmwareVersion(AllElements);
                }
                else
                {
                    string msg = $"AllowedVersion.SetCapsuleVersionHelperClass: Helper Class {this.CapsuleVersionHelperClass} not found in this assembly";
                    root.Log(msg);
                }
            }
        }

        public bool SetVersionFromFilename(string filename)
        {
            bool retval = false;
            XmlElement[] allElements = this.CapsuleVersion?.GetVersionFromFilename(filename);

            if (allElements != null)
            {
                this.AllElements = allElements;
                retval = true;
            }
            return retval;
        }

        /// <summary>
        /// Determine if this object is the CurrentVersion for this capsule
        /// </summary>
        /// <returns>true if this is the current version</returns>
        public bool IsCurrentVersion()
        {
            bool retval = false;

            AllowedVersionList parentList = this.Parent as AllowedVersionList;

            if(parentList.AllowedVersions.Count() > 1)
            {
                retval = string.Compare(this.CurrentVersion, "true", true) == 0;
            }
            if ((parentList.AllowedVersions.Count() == 1) && ReferenceEquals(this, parentList.AllowedVersions[0]))
            {
                retval = true;
            }
            return retval;
        }

        /// <summary>
        /// Determine if this object version is obsolete for this capsule
        /// Can allow capsules to be removed without updating (not implemented yet), which can occur when HardwareId changes for a capsule 
        /// </summary>
        /// <returns>true if this allowed version is obsolete</returns>
        public bool IsObsolete()
        {
            return string.Compare(this.ObsoleteVersion, "true", true) == 0;
        }

        //
        // CapsuleVersion Wrappers
        //

        /// <summary>
        /// Get the DeviceFirmwareVersion object for this Capsule
        /// </summary>
        /// <returns></returns>
        public DeviceFirmwareVersion GetDeviceFirmwareVersion()
        {
            return this.CapsuleVersion?.GetDeviceFirmwareVersion();
        }

        /// <summary>
        /// Call the CapsuleVersion helper class to determine if update is needed based on hardwareVersion and UpdateCondition setting
        /// </summary>
        /// <param name="hardwareVersion">Current Hardware Capsule Version on DUT</param>
        /// <param name="condition">UpdateCondition for this capsule</param>
        /// <returns>true if capsule needs updating</returns>
        public bool UpdateNeeded(DeviceFirmwareVersion hardwareVersion, CapsulePropertyUpdateCondition condition)
        {
            return (this.CapsuleVersion == null) ? false : this.CapsuleVersion.UpdateNeeded(hardwareVersion, condition);
        }

        public override bool Validate()
        {
            bool retval = false;
            XmlElement[] allElements = this.CapsuleVersion?.GetVersionFromFilename(this.BinFile);

            for(int i = 0; i < allElements.Length; i++)
            {
                retval = retval && allElements[i] == this.AllElements[i];
            }
            return retval;
        }

        /// <summary>
        /// Iterate on child elements calling ListCapsuleInfo
        /// </summary>
        public override void ListCapsuleInfo()
        {
            Log($"{GetDeviceFirmwareVersion().ToString()}");
        }       
    }
}

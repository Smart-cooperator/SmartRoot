//-------------------------------------------------------------------------------
//Copyright (c) Microsoft Corporation.  All rights reserved.
//-------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace CapsuleParser
{
    /// <summary>
    /// Abstract Base Class for all Capsule Version Helper Class schemas
    /// </summary>
    public abstract class CapsuleVersionHelperBaseClass : ICapsuleVersion
    {
        /// <summary>
        /// DeviceFirmwareVersion - included in base class as most Capsules conform to Major.Minor.Build.[Revision]
        /// Non-compliant Capsule Version classes should override GetDeviceFirmwareVersion as appropriate
        /// </summary>
        protected DeviceFirmwareVersion _deviceFirmwareVersion;

        // default values
        protected int majorLength = 8;
        protected int minorLength = 8;
        protected int buildLength = 8;
        protected int revisionLength = 8;

        /// <summary>
        /// Return a DeviceFirmwareVersion given the version passed in, typically from HardwareIds of the capsule on the DUT
        /// </summary>
        /// <param name="version">version number to decode</param>
        /// <returns>DeviceFirmwareVersion object for the version number passed in</returns>
        public abstract DeviceFirmwareVersion GetFirmwareVersionFromNumber(uint version);

        /// <summary>
        /// Return a DeviceFirmwareVersion given the version passed in, typically from HardwareIds of the capsule on the DUT
        /// </summary>
        /// <param name="version">version number to decode</param>
        /// <returns>DeviceFirmwareVersion object for the version number passed in</returns>
        public abstract XmlElement[] GetVersionFromFilename(string filename);

        /// <summary>
        /// Set and return the DeviceFirmwareVersion based on the provided XmlElements passed in (Major.Minor.Build.[Revision] assumed)
        /// </summary>
        /// <param name="versionElements">XmlElements describing the version subelements</param>
        /// <returns>DeviceFirmwareVersion representing the XmlElement specifed</returns>
        public abstract DeviceFirmwareVersion SetDeviceFirmwareVersion(XmlElement[] versionElements);

        /// <summary>
        /// Returns the DeviceFirmwareVersion for the Capsule AllowedVersion for which this CapsuleVersion helper class is associated
        /// </summary>
        /// <returns>DeviceFirmwareVersion of the associated AllowedVersion</returns>
        public DeviceFirmwareVersion GetDeviceFirmwareVersion()
        {
            return _deviceFirmwareVersion;
        }

        /// <summary>
        /// Determines if capsule needs to be updated based on HardwareVersion and UpdateCondition
        /// </summary>
        /// <param name="hardwareVersion">The current Hardware Version on the DUT</param>
        /// <param name="condition">The UpdateCondition specified for this capsule in XML</param>
        /// <returns>true if an Update is needed</returns>
        public virtual bool UpdateNeeded(DeviceFirmwareVersion hardwareVersion, CapsulePropertyUpdateCondition condition)
        {
            bool retval = false;

            switch(condition)
            {
                case CapsulePropertyUpdateCondition.Never:
                    break;

                case CapsulePropertyUpdateCondition.Always:
                    retval = true;
                    break;

                case CapsulePropertyUpdateCondition.OnMismatch:
                    retval = (hardwareVersion != this._deviceFirmwareVersion);
                    break;

                case CapsulePropertyUpdateCondition.LessThan:
                    retval = (hardwareVersion < this._deviceFirmwareVersion);
                    break;
            
                case CapsulePropertyUpdateCondition.Unknown:
                    break;

                default:
                    break;
            }
            return retval;
        }

        public XmlElement GetVersionElementByName(string name, XmlElement[] allElements)
        {
            List<XmlElement> elementList = allElements.ToList();
            return elementList.Where(v => v.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }
    }

    ///
    ///  Intermediate subclasses for common version schemas
    ///

    /// <summary>
    /// Major.Minor.Build Schema Capsule Version
    /// </summary>
    public class CapsuleVersionField_MajorMinorBuild : CapsuleVersionHelperBaseClass
    {
        /// <summary>
        /// Return a DeviceFirmwareVersion given the version passed in, typically from HardwareIds of the capsule on the DUT
        /// </summary>
        /// <param name="version">version number to decode</param>
        /// <returns>DeviceFirmwareVersion object for the version number passed in</returns>
        public override DeviceFirmwareVersion GetFirmwareVersionFromNumber(uint versionNum)
        {
            int patternlength = majorLength + minorLength + buildLength;

            uint majorMask = ((uint)1 << majorLength) - 1;
            uint minorMask = ((uint)1 << minorLength) - 1;
            uint buildMask = ((uint)1 << buildLength) - 1;
            uint revisionMask = ((uint)1 << revisionLength) - 1;

            uint major = (versionNum >> (patternlength - majorLength)) & majorMask;
            uint minor = (versionNum >> (patternlength - majorLength - minorLength)) & minorMask;
            uint build = (versionNum >> (patternlength - majorLength - minorLength - buildLength)) & buildMask;
            uint revision = versionNum & revisionMask;

            return new DeviceFirmwareVersion(major, minor, build);
        }

        public override XmlElement[] GetVersionFromFilename(string filename)
        {
            string nameonly = Path.GetFileName(filename);
            string verstr = nameonly.Substring(nameonly.IndexOf('_') + 1);
            string[] verparts = verstr.Split('.');
            XmlElement[] allElements = null;

            if (verparts.Length >= 4)   // major.minor.build.bin
            {
                XmlDocument doc = new XmlDocument();
                allElements = new XmlElement[4];
                allElements[0] = doc.CreateElement(Constants.major);
                allElements[1] = doc.CreateElement(Constants.minor);
                allElements[2] = doc.CreateElement(Constants.build);
                allElements[3] = doc.CreateElement(Constants.revision);

                GetVersionElementByName(Constants.major, allElements).InnerText = verparts[0];
                GetVersionElementByName(Constants.minor, allElements).InnerText = verparts[1];
                GetVersionElementByName(Constants.build, allElements).InnerText = verparts[2];
                GetVersionElementByName(Constants.revision, allElements).InnerText = "0";
            }
            return allElements;
        }

        /// <summary>
        /// Set and return the DeviceFirmwareVersion based on the provided XmlElements passed in (Major.Minor.Build.[Revision] assumed)
        /// </summary>
        /// <param name="versionElements">XmlElements describing the version subelements</param>
        /// <returns>DeviceFirmwareVersion representing the XmlElement specifed</returns>
        public override DeviceFirmwareVersion SetDeviceFirmwareVersion(XmlElement[] versionElements)
        {
            _deviceFirmwareVersion = new DeviceFirmwareVersion();

            uint value;

            XmlElement majorElement = GetVersionElementByName(Constants.major, versionElements);
            XmlElement minorElement = GetVersionElementByName(Constants.minor, versionElements);
            XmlElement buildElement = GetVersionElementByName(Constants.build, versionElements);
            XmlElement revisionElement = GetVersionElementByName(Constants.revision, versionElements);

            if (majorElement != null)
            {
                if(uint.TryParse(majorElement.InnerText, out value))
                {
                    _deviceFirmwareVersion.Major = value;
                }
            }

            if (minorElement != null)
            {
                if (uint.TryParse(minorElement.InnerText, out value))
                {
                    _deviceFirmwareVersion.Minor = value;
                }
            }

            if (buildElement != null)
            {
                if (uint.TryParse(buildElement.InnerText, out value))
                {
                    _deviceFirmwareVersion.Build = value;
                }
            }

            _deviceFirmwareVersion.Revision = 0;

            return _deviceFirmwareVersion;
        }
    }

    /// <summary>
    /// Major.Minor.Build.Revision Schema Capsule Version
    /// </summary>
    public class CapsuleVersionField_MajorMinorBuildRevision : CapsuleVersionHelperBaseClass
    {
        /// <summary>
        /// Return a DeviceFirmwareVersion given the version passed in, typically from HardwareIds of the capsule on the DUT
        /// </summary>
        /// <param name="version">version number to decode</param>
        /// <returns>DeviceFirmwareVersion object for the version number passed in</returns>
        public override DeviceFirmwareVersion GetFirmwareVersionFromNumber(uint versionNum)
        {
            int patternlength = majorLength + minorLength + buildLength + revisionLength;

            uint majorMask = ((uint)1 << majorLength) - 1;
            uint minorMask = ((uint)1 << minorLength) - 1;
            uint buildMask = ((uint)1 << buildLength) - 1;
            uint revisionMask = ((uint)1 << revisionLength) - 1;

            uint major = (versionNum >> (patternlength - majorLength)) & majorMask;
            uint minor = (versionNum >> (patternlength - majorLength - minorLength)) & minorMask;
            uint build = (versionNum >> (patternlength - majorLength - minorLength - buildLength)) & buildMask;
            uint revision = versionNum & revisionMask;

            return new DeviceFirmwareVersion(major, minor, build, revision);
        }

        public override XmlElement[] GetVersionFromFilename(string filename)
        {
            string nameonly = Path.GetFileName(filename);
            string verstr = nameonly.Substring(nameonly.IndexOf('_') + 1);
            string[] verparts = verstr.Split('.');
            XmlElement[] allElements = null;

            if (verparts.Length == 5)   // major.minor.build.revision.bin
            {
                XmlDocument doc = new XmlDocument();
                allElements = new XmlElement[4];
                allElements[0] = doc.CreateElement(Constants.major);
                allElements[1] = doc.CreateElement(Constants.minor);
                allElements[2] = doc.CreateElement(Constants.build);
                allElements[3] = doc.CreateElement(Constants.revision);

                GetVersionElementByName(Constants.major, allElements).InnerText = verparts[0];
                GetVersionElementByName(Constants.minor, allElements).InnerText = verparts[1];
                GetVersionElementByName(Constants.build, allElements).InnerText = verparts[2];
                GetVersionElementByName(Constants.revision, allElements).InnerText = verparts[3];
            }
            return allElements;
        }

        /// <summary>
        /// Set and return the DeviceFirmwareVersion based on the provided XmlElements passed in (Major.Minor.Build.[Revision] assumed)
        /// </summary>
        /// <param name="versionElements">XmlElements describing the version subelements</param>
        /// <returns>DeviceFirmwareVersion representing the XmlElement specifed</returns>
        public override DeviceFirmwareVersion SetDeviceFirmwareVersion(XmlElement[] versionElements)
        {
            _deviceFirmwareVersion = new DeviceFirmwareVersion();

            uint value;

            XmlElement majorElement = GetVersionElementByName(Constants.major, versionElements);
            XmlElement minorElement = GetVersionElementByName(Constants.minor, versionElements);
            XmlElement buildElement = GetVersionElementByName(Constants.build, versionElements);
            XmlElement revisionElement = GetVersionElementByName(Constants.revision, versionElements);

            if (majorElement != null)
            {
                if (uint.TryParse(majorElement.InnerText, out value))
                {
                    _deviceFirmwareVersion.Major = value;
                }
            }

            if (minorElement != null)
            {
                if (uint.TryParse(minorElement.InnerText, out value))
                {
                    _deviceFirmwareVersion.Minor = value;
                }
            }

            if (buildElement != null)
            {
                if (uint.TryParse(buildElement.InnerText, out value))
                {
                    _deviceFirmwareVersion.Build = value;
                }
            }

            if (revisionElement != null)
            {
                if (uint.TryParse(revisionElement.InnerText, out value))
                {
                    _deviceFirmwareVersion.Revision = value;
                }
            }

            return _deviceFirmwareVersion;
        }
    }

    //
    // Capsule Specific subclasses
    //

    /// <summary>
    /// UEFI Capsule version interpreter
    /// </summary>
    public class CapsuleVersion_UEFI : CapsuleVersionField_MajorMinorBuild
    {
        /// <summary>
        /// Sets the bitfield width parameters
        /// </summary>
        public CapsuleVersion_UEFI()
        {
            majorLength = 10;
            minorLength = 12;
            buildLength = 10;
            revisionLength = 0;
        }
    }

    /// <summary>
    /// USBC Capsule version interpreter
    /// </summary>
    public class CapsuleVersion_USBC : CapsuleVersionField_MajorMinorBuild
    {
        /// <summary>
        /// Sets the bitfield width parameters
        /// </summary>
        public CapsuleVersion_USBC()
        {
            majorLength = 8;
            minorLength = 16;
            buildLength = 8;
            revisionLength = 0;
        }
    }

   
    /// <summary>
    /// SAM Capsule version interpreter
    /// </summary>
    public class CapsuleVersion_SAM : CapsuleVersionField_MajorMinorBuild
    {
        /// <summary>
        /// Sets the bitfield width parameters
        /// </summary>
        public CapsuleVersion_SAM()
        {
            majorLength = 8;
            minorLength = 16;
            buildLength = 8;
            revisionLength = 0;
        }
    }

    /// <summary>
    /// PD Capsule version interpreter
    /// </summary>
    public class CapsuleVersion_PD : CapsuleVersionField_MajorMinorBuild
    {
        /// <summary>
        /// Sets the bitfield width parameters
        /// </summary>
        public CapsuleVersion_PD()
        {
            majorLength = 8;
            minorLength = 16;
            buildLength = 8;
            revisionLength = 0;
        }
    }

    /// <summary>
    /// ME Capsule version interpreter
    /// </summary>
    public class CapsuleVersion_ME : CapsuleVersionField_MajorMinorBuildRevision
    {
        /// <summary>
        /// Sets the bitfield width parameters
        /// </summary>
        public CapsuleVersion_ME()
        {
            majorLength = 4;
            minorLength = 4;
            buildLength = 8;
            revisionLength = 16;
        }

        // NOTE: Override UpdateNeeded to disallow OnMismatch UpdateCondition. Only LessThan is appropriate

        /// <summary>
        /// Return a DeviceFirmwareVersion given the version passed in, typically from HardwareIds of the capsule on the DUT
        /// Note the bitfield swapping of Build and Revision arguments to DeviceFirmwareVersion constructor
        /// </summary>
        /// <param name="version">version number to decode</param>
        /// <returns>DeviceFirmwareVersion object for the version number passed in</returns>
        public override DeviceFirmwareVersion GetFirmwareVersionFromNumber(uint versionNum)
        {
            return new DeviceFirmwareVersion((versionNum & 0xF0000000) >> 28, (versionNum & 0x0F000000) >> 24, (versionNum & 0x0000FFFF), (versionNum & 0x00FF0000) >> 16);
        }
    }

    public class CapsuleVersion_ME_NotSwapped : CapsuleVersionField_MajorMinorBuildRevision
    {
        /// <summary>
        /// Sets the bitfield width parameters
        /// </summary>
        public CapsuleVersion_ME_NotSwapped()
        {
            majorLength = 4;
            minorLength = 4;
            buildLength = 8;
            revisionLength = 16;
        }

        // NOTE: Override UpdateNeeded to disallow OnMismatch UpdateCondition. Only LessThan is appropriate

        /// <summary>
        /// Return a DeviceFirmwareVersion given the version passed in, typically from HardwareIds of the capsule on the DUT
        /// Note the bitfield swapping of Build and Revision arguments to DeviceFirmwareVersion constructor
        /// </summary>
        /// <param name="version">version number to decode</param>
        /// <returns>DeviceFirmwareVersion object for the version number passed in</returns>
        public override DeviceFirmwareVersion GetFirmwareVersionFromNumber(uint versionNum)
        {
            return new DeviceFirmwareVersion((versionNum & 0xF0000000) >> 28, (versionNum & 0x0F000000) >> 24, (versionNum & 0x0000FFFF), (versionNum & 0x00FF0000) >> 16);
        }
    }

    /// <summary>
    /// Touch Capsule version interpreter
    /// </summary>
    public class CapsuleVersion_Touch : CapsuleVersionField_MajorMinorBuildRevision
    {
        /// <summary>
        /// Sets the bitfield width parameters
        /// </summary>
        public CapsuleVersion_Touch()
        {
            majorLength = 8;
            minorLength = 8;
            buildLength = 8;
            revisionLength = 8;
        }
    }

    /// <summary>
    /// SMF Capsule version interpreter
    /// </summary>
    public class CapsuleVersion_SMF : CapsuleVersionField_MajorMinorBuildRevision
    {
        /// <summary>
        /// Sets the bitfield width parameters
        /// </summary>
        public CapsuleVersion_SMF()
        {
            majorLength = 8;
            minorLength = 8;
            buildLength = 8;
            revisionLength = 8;
        }
    }

    /// <summary>
    /// TCON Capsule version interpreter
    /// </summary>
    public class CapsuleVersion_TCON : CapsuleVersionField_MajorMinorBuildRevision
    {
        /// <summary>
        /// Sets the bitfield width parameters
        /// </summary>
        public CapsuleVersion_TCON()
        {
            majorLength = 8;
            minorLength = 8;
            buildLength = 8;
            revisionLength = 8;
        }
    }
}


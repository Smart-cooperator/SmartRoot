//-------------------------------------------------------------------------------
//Copyright (c) Microsoft Corporation.  All rights reserved.
//-------------------------------------------------------------------------------

namespace CapsuleParser
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the raw ambient light sensor samples.
    /// </summary>

    public class DeviceFirmwareVersion : IComparable
    {
        /// <summary>
        /// Initializes a new instance of the FirmwareVersion class.
        /// </summary>
        public DeviceFirmwareVersion()
        {
        }

        /// <summary>
        /// Initializes a new instance of the FirmwareVersion class.
        /// </summary>
        /// <param name="major">The major version of the firmware.</param>
        /// <param name="minor">The minor version of the firmware.</param>
        /// <param name="build">The build version of the firmware.</param>
        public DeviceFirmwareVersion(uint major, uint minor, uint build)
            : this(major, minor, build, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the FirmwareVersion class.
        /// </summary>
        /// <param name="major">The major version of the firmware.</param>
        /// <param name="minor">The minor version of the firmware.</param>
        /// <param name="build">The build version of the firmware.</param>
        /// <param name="revision">The revision version of the firmware.</param>
        public DeviceFirmwareVersion(uint major, uint minor, uint build, uint revision)
        {
            this.Major = major;
            this.Minor = minor;
            this.Build = build;
            this.Revision = revision;
        }

        /// <summary>
        /// Gets or sets the major version of the firmware.
        /// </summary>
        public uint Major
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the minor version of the firmware.
        /// </summary>
        public uint Minor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the build version of the firmware.
        /// </summary>
         public uint Build
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the revision version of the firmware.
        /// </summary>
        public uint Revision
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating that one instance of the FirmwareVersion class is greater than another instance.
        /// </summary>
        /// <param name="a">The first instance of the FirmwareVersion class.</param>
        /// <param name="b">The second instance of the FirmwareVersion class.</param>
        /// <returns>A value indicating that one instance of the FirmwareVersion class is greater than another instance.</returns>
        public static bool operator >(DeviceFirmwareVersion a, DeviceFirmwareVersion b)
        {
            // Major version matching
            if (a.Major > b.Major)
            {
                return true;
            }

            if (a.Major < b.Major)
            {
                return false;
            }

            // Minor version matching
            if (a.Minor > b.Minor)
            {
                return true;
            }

            if (a.Minor < b.Minor)
            {
                return false;
            }

            // Build version matching
            if (a.Build > b.Build)
            {
                return true;
            }

            if (a.Build < b.Build)
            {
                return false;
            }

            return a.Revision > b.Revision;
        }

        /// <summary>
        /// Gets a value indicating that one instance of the FirmwareVersion class is greater than another instance.
        /// </summary>
        /// <param name="a">The first instance of the FirmwareVersion class.</param>
        /// <param name="b">The second instance of the FirmwareVersion class.</param>
        /// <returns>A value indicating that one instance of the FirmwareVersion class is greater than another instance.</returns>
        public static bool operator <(DeviceFirmwareVersion a, DeviceFirmwareVersion b)
        {
            // Major version matching
            if (a.Major < b.Major)
            {
                return true;
            }

            if (a.Major > b.Major)
            {
                return false;
            }

            // Minor version matching
            if (a.Minor < b.Minor)
            {
                return true;
            }

            if (a.Minor > b.Minor)
            {
                return false;
            }

            // Build version matching
            if (a.Build < b.Build)
            {
                return true;
            }

            if (a.Build > b.Build)
            {
                return false;
            }

            return a.Revision < b.Revision;
        }

        /// <summary>
        /// Gets a value indicating that one instance of the FirmwareVersion class is less than or equal to another instance.
        /// </summary>
        /// <param name="a">The first instance of the FirmwareVersion class.</param>
        /// <param name="b">The second instance of the FirmwareVersion class.</param>
        /// <returns>A value indicating that one instance of the FirmwareVersion class is less than or equal to another instance.</returns>
        public static bool operator <=(DeviceFirmwareVersion a, DeviceFirmwareVersion b)
        {
            return a < b || a.Equals(b);
        }

        /// <summary>
        /// Gets a value indicating that one instance of the FirmwareVersion class is greater than or equal to another instance.
        /// </summary>
        /// <param name="a">The first instance of the FirmwareVersion class.</param>
        /// <param name="b">The second instance of the FirmwareVersion class.</param>
        /// <returns>A value indicating that one instance of the FirmwareVersion class is greater than or equal to another instance.</returns>
        public static bool operator >=(DeviceFirmwareVersion a, DeviceFirmwareVersion b)
        {
            return a > b || a.Equals(b);
        }

        /// <summary>
        /// Gets a value indicating whether the two instances of this class are equal to each other. 
        /// </summary>
        /// <param name="a">The instance to check for equality.</param>
        /// <param name="a">The instance to verify against for equality.</param>
        /// <returns>A value indicating whether the two instances of this class are equal to each other.</returns>
        public static bool operator ==(DeviceFirmwareVersion a, DeviceFirmwareVersion b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Gets a value indicating whether the two instances of this class are equal to each other. 
        /// </summary>
        /// <param name="a">The instance to check for equality.</param>
        /// <param name="a">The instance to verify against for equality.</param>
        /// <returns>A value indicating whether the two instances of this class are equal to each other.</returns>
        public static bool operator !=(DeviceFirmwareVersion a, DeviceFirmwareVersion b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// Gets a value indicating whether the two instances of this class are equal to each other.
        /// </summary>
        /// <param name="obj">The instance to check for equality.</param>
        /// <returns>A value indicating whether the two instances of this class are equal to each other.</returns>
        public override bool Equals(object obj)
        {
            DeviceFirmwareVersion that = obj as DeviceFirmwareVersion;

            return 
                ReferenceEquals(this, obj) ||
               !ReferenceEquals(obj, null) &&
               (this.Major == that.Major && 
                this.Minor == that.Minor && 
                this.Build == that.Build && 
                this.Revision == that.Revision);
        }

        /// <summary>
        /// Gets a hash code value for this instance of the class.
        /// </summary>
        /// <returns>A hashcode value.</returns>
        public override int GetHashCode()
        {
            return (int)(this.Major ^ this.Minor ^ this.Build ^ this.Revision);
        }

        /// <summary>
        /// Gets a string representation for the firmware version.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return string.Format("Major: {0}, Minor: {1}, Build: {2}, Revision: {3}.",
                this.Major, this.Minor, this.Build, this.Revision);
        }

        /// <summary>
        /// Try parse device firmware version from input string.
        /// </summary>
        /// <param name="s">The input string.</param>
        /// <param name="version">The parsed version.</param>
        /// <returns>Determine if device firmware version parse successfully or not.</returns>
        public static bool TryParse(string s, out DeviceFirmwareVersion version)
        {
            version = null;

            string[] strArray = s.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length < 3 || strArray.Length > 4)
            {
                return false;
            }

            uint major = 0;
            if (!uint.TryParse(strArray[0], out major))
            {
                return false;
            }

            uint minor = 0;
            if (!uint.TryParse(strArray[1], out minor))
            {
                return false;
            }

            uint build = 0;
            if (!uint.TryParse(strArray[2], out build))
            {
                return false;
            }

            uint revision = 0;
            if (strArray.Length >= 4 && (!uint.TryParse(strArray[3], out revision)))
            {
                return false;
            }

            version = new DeviceFirmwareVersion(major, minor, build, revision);

            return true;
        }

        /// <summary>
        /// Compares one instance of the FirmwareVersion class to another.
        /// </summary>
        /// <param name="obj">The object to compare the current instance against.</param>
        /// <returns>A value indicating the result of the comparison.</returns>
        public int CompareTo(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                throw new ArgumentException("This instance of the firmware version class can only be compared to another FirmwareVersion.");
            }

            DeviceFirmwareVersion that = obj as DeviceFirmwareVersion;

            if (ReferenceEquals(this, that))
            {
                return 0;
            }

            if (that > this)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }
}

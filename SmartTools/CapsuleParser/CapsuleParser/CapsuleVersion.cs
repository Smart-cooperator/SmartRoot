//-------------------------------------------------------------------------------
//Copyright (c) Microsoft Corporation.  All rights reserved.
//-------------------------------------------------------------------------------

namespace CapsuleParser
{
    /// <summary>
    /// Represents the hardware component.
    /// </summary>
    public enum CapsuleInfoType : uint
    {
        /// <summary>
        /// The hardware component for UEFI.
        /// </summary>
        Uefi,

        /// <summary>
        /// The touch component for Touch.
        /// </summary>
        Touch,

        /// <summary>
        /// The hardware component for the sensor aggregator module (SAM).
        /// </summary>
        Sam,

        /// <summary>
        /// The hardware component for the embedded controller (EC).  Currently, this only exists on Rey.
        /// </summary>
        EC,

        /// <summary>
        /// The hardware component for UEFI for not production signed
        /// </summary>
        UEFI_Transition,

        /// <summary>
        /// The hardware component for ME.
        /// </summary>
        ME,

        /// <summary>
        /// The hardware component for USB-C.
        /// </summary>
        USBC,

        PD,

        SMF,

        TCON,

        /// <summary>
        /// The hardware component which has a firmware but without capsule. e.g. Pen firmware of Armor.
        /// </summary>
        Unknown
    }

    /// <summary>
    /// Represents the capsule update condition.
    /// </summary>
    public enum CapsulePropertyUpdateCondition
    {
        /// <summary>
        /// Never update the capsule.  Test will fail if the capsule doesn't match the expected version.
        /// </summary>
        Never,

        /// <summary>
        /// Update the capsule if the version doesn't match an allowed version.
        /// </summary>
        OnMismatch,

        /// <summary>
        /// Update the capsule if the current version is less than the expected version.
        /// </summary>
        LessThan,

        /// <summary>
        /// Update the firmware always.
        /// </summary>
        Always,

        /// <summary>
        /// Uunable to determine UpdateCondition.
        /// </summary>
        Unknown
    }

    /// <summary>
    /// Interface into the CapsuleVersion helper classes used by Test Cases to allow capsule version to be interpreted by the CapsuleVersion helper classes
    /// </summary>
    public interface ICapsuleVersion
    {
        /// <summary>
        /// Given a versionNum, typically from hardwareIds, return a DeviceFirmwareVerson object reflecting the versionNum specified
        /// </summary>
        /// <param name="versionNum"></param>
        /// <returns>DeviceFirmwareVersion object for the versionNum</returns>
        DeviceFirmwareVersion GetFirmwareVersionFromNumber(uint versionNum);

        /// <summary>
        /// Determine if an update is required given the hardwareVersion on the DUT and the UpdateCondition from XML
        /// </summary>
        /// <param name="hardwareVersion"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        bool UpdateNeeded(DeviceFirmwareVersion hardwareVersion, CapsulePropertyUpdateCondition condition);
    }
}

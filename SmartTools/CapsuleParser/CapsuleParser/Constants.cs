//-------------------------------------------------------------------------------
//Copyright (c) Microsoft Corporation.  All rights reserved.
//-------------------------------------------------------------------------------
namespace CapsuleParser
{
    public static class Constants
    {
        
        public const string Id = "Id";
        public const string id = "id";       

        /// <summary>
        /// Capsule Info Validation & Ingestion Settings
        /// </summary>
        public const string MteCapsulePath = "MteCapsulePath";
        public const string VulcanCapsulePath = "VulcanCapsulePath";
        public const string LocalMteCapsulePath = "LocalMteCapsulePath";
        public const string LocalVulcanCapsulePath = "LocalVulcanCapsulePath";
        public const string CapsuleInfoValidation = "CapsuleInfoValidation";
        public const string CapsuleInfoConfiguration = "CapsuleInfoConfiguration";
        public const string CapsuleInfoConfigurationPath = "CapsuleInfoConfigurationPath";
        public const string CapsuleBinariesPath = "CapsuleBinariesPath";
        public const string CapsuleConfiguration = "CapsuleConfiguration";
        public const string CapsuleBomConfiguration = "CapsuleBomConfiguration";
        public const string CapsuleBomPath = "CapsuleBomPath";
        // CapsuleInfoConfiguration Elements
        public const string capsuleSettingList = "capsuleSettingList";
        public const string capsuleCombinationList = "capsuleCombinationList";
        public const string capsuleList = "capsuleList";
        public const string capsuleCombination = "capsuleCombination";
        public const string capsuleInfo = "capsuleInfo";
        public const string capsuleProperty = "capsuleProperty";
        public const string capsule = "capsule";
        public const string deviceHardwareId = "deviceHardwareId";
        public const string capsulePath = "capsulePath";
        public const string CapsulePathReplaceToken = "CapsulePathReplaceToken";
        public const string DutDefaultCapsulePath = @"c:\KoreDeviceServer";
        public const string allowedVersionList = "allowedVersionList";
        public const string allowedVersion = "allowedVersion";
        public const string major = "major";
        public const string minor = "minor";
        public const string build = "build";
        public const string revision = "revision";
        // CapsuleInfoConfiguration Attributes
        public const string component = "component";
        public const string skipIfNotFound = "skipIfNotFound";
        public const string capsuleName = "capsuleName";
        public const string updateCondition = "updateCondition";
        public const string installSet = "installSet";
        // CapsuleInfo.Component types
        public const string UEFI = "UEFI";
        public const string Touch = "Touch";
        public const string SAM = "SAM";
        public const string UEFI_Transition = "UEFI_Transition";
        public const string ME = "ME";
        public const string USBC = "USBC";
        public const string PD = "PD";
        public const string SMF = "SMF";
        public const string TCON = "TCON";
        // Update Conditions
        public const string UpdateConditionAlways = "Always";
        public const string UpdateConditionNever = "Never";
        public const string UpdateConditionOnMismatch = "OnMismatch";
        public const string UpdateConditionLessThan = "LessThan";

        /// <summary>
        /// Common reused const strings
        /// </summary>
        public const string NameAttribute = "Name";
        public const string nameAttribute = "name";
        public const string PathElement = "Path";
        public const string MtePath = "MtePath";
        public const string VulcanPath = "VulcanPath";
        public const string Comment = "Comment";
        public const string Products = "Products";
        public const string value = "value";

        /// <summary>
        /// Common integers
        /// </summary>
        public const int MAX_PATH = 260;
        public const int LIST_BUFFER_SIZE = 65535;
    } 

}

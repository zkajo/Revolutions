using ModLib;
using ModLib.Attributes;
using System;
using System.Xml.Serialization;

namespace Revolutions
{
    [Serializable]
    public class Settings : SettingsBase
    {
        public static Settings Instance
        {
            get
            {
                return (Settings)SettingsDatabase.GetSettings("RevolutionsSettings");
            }
        }

        [XmlElement]
        public override string ID { get; set; } = "RevolutionsSettings";

        public override string ModuleFolderName => "Revolutions";

        public override string ModName => "Revolutions";

        [XmlElement]
        [SettingPropertyGroup("Mechanics")]
        [SettingProperty("Empire Loyalty")]
        public bool EmpireLoyaltyMechanics { get; set; } = true;

        [XmlElement]
        [SettingPropertyGroup("Mechanics")]
        [SettingProperty("Overextension")]
        public bool OverextensionMechanics { get; set; } = true;

        [XmlElement]
        [SettingPropertyGroup("Toggles")]
        [SettingProperty("Overextension Affects Player")]
        public bool PlayerAffectedByOverextension { get; set; } = true;

        [XmlElement]
        [SettingPropertyGroup("Toggles")]
        [SettingProperty("Allow Minor Factions")]
        public bool AllowMinorFactions { get; set; } = false;

        [XmlElement]
        [SettingPropertyGroup("Values")]
        [SettingProperty("Revolt Cooldown", 0f, 250f)]
        public float RevoltCooldownTime { get; set; } = 30.0f;

        [XmlElement]
        [SettingPropertyGroup("Values")]
        [SettingProperty("Days Until Loyalty Change", 0, 365)]
        public int DaysUntilLoyaltyChange { get; set; } = 80;

        [XmlElement]
        [SettingPropertyGroup("Values")]
        [SettingProperty("Minimum Obediance Loyalty", 0, 250)]
        public int MinimumObedianceLoyalty { get; set; } = 25;

        [XmlElement]
        [SettingPropertyGroup("Values")]
        [SettingProperty("Player In Town Loyalty Increase", 0, 100)]
        public int PlayerInTownLoyaltyIncrease { get; set; } = 5;

        [XmlElement]
        [SettingPropertyGroup("Values")]
        [SettingProperty("Over Extension Multiplier", 0f, 10f)]
        public float OverExtensionMultiplier { get; set; } = 2.0f;

        [XmlElement]
        [SettingPropertyGroup("Values")]
        [SettingProperty("Base Player Loyalty", 0f, 100)]
        public float BasePlayerLoyalty { get; set; } = 5.0f;

        [XmlElement]
        [XmlElement]
        [SettingProperty("DebugMode")]
        public bool DebugMode { get; set; } = false;
    }
}
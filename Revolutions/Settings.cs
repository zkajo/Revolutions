using MBOptionScreen.Attributes;
using MBOptionScreen.Attributes.v2;
using MBOptionScreen.Settings;

namespace Revolutions
{
    public class Settings : AttributeSettings<Settings>
    {
        public override string Id { get; set; } = "RevolutionsSettings_v1";

        public override string ModuleFolderName => "Revolutions";

        public override string ModName => "Revolutions";

        [SettingPropertyGroup("{=revolutions_01}/{=revolutions_07}")]
        [SettingPropertyFloatingInteger(displayName: "{=revolutions_08}", minValue: 0f, maxValue: 250f, HintText = "{=revolutions_09}", RequireRestart = false)]
        public float RevoltCooldownTime { get; set; } = 30.0f;

        [SettingPropertyGroup("{=revolutions_02}/{=revolutions_05}")]
        [SettingPropertyBool(displayName: "{=revolutions_10}", HintText = "{=revolutions_11}", RequireRestart = false)]
        public bool EmpireLoyaltyMechanics { get; set; } = true;

        [SettingPropertyGroup("{=revolutions_02}/{=revolutions_07}")]
        [SettingPropertyFloatingInteger(displayName: "{=revolutions_12}", minValue: 0f, maxValue: 100f, HintText = "{=revolutions_13}", RequireRestart = false)]
        public float BasePlayerLoyalty { get; set; } = 5.0f;

        [SettingPropertyGroup("{=revolutions_02}/{=revolutions_07}")]
        [SettingPropertyInteger(displayName: "{=revolutions_14}", minValue: 0, maxValue: 250, HintText = "{=revolutions_15}", RequireRestart = false)]
        public int MinimumObedianceLoyalty { get; set; } = 25;

        [SettingPropertyGroup("{=revolutions_02}/{=revolutions_07}")]
        [SettingPropertyInteger(displayName: "{=revolutions_16}", minValue: 0, maxValue: 365, HintText = "{=revolutions_17}", RequireRestart = false)]
        public int DaysUntilLoyaltyChange { get; set; } = 80;

        [SettingPropertyGroup("{=revolutions_02}/{=revolutions_07}")]
        [SettingPropertyInteger(displayName: "{=revolutions_18}", minValue: 0, maxValue: 100, HintText = "{=revolutions_19}", RequireRestart = false)]
        public int PlayerInTownLoyaltyIncrease { get; set; } = 5;

        [SettingPropertyGroup("{=revolutions_03}/{=revolutions_05}")]
        [SettingPropertyBool(displayName: "{=revolutions_20}", HintText = "{=revolutions_21}", RequireRestart = false)]
        public bool OverextensionMechanics { get; set; } = true;

        [SettingPropertyGroup("{=revolutions_03}/{=revolutions_06}")]
        [SettingPropertyBool(displayName: "{=revolutions_22}", HintText = "{=revolutions_23}", RequireRestart = false)]
        public bool PlayerAffectedByOverextension { get; set; } = true;

        [SettingPropertyGroup("{=revolutions_03}/{=revolutions_07}")]
        [SettingPropertyFloatingInteger(displayName: "{=revolutions_24}", minValue: 0f, maxValue: 10f, HintText = "{=revolutions_25}", RequireRestart = false)]
        public float OverExtensionMultiplier { get; set; } = 2.0f;

        [SettingPropertyGroup("{=revolutions_04}/{=revolutions_06}")]
        [SettingPropertyBool(displayName: "{=revolutions_26}", HintText = "{=revolutions_27}", RequireRestart = false)]
        public bool AllowMinorFactions { get; set; } = false;
    }
}
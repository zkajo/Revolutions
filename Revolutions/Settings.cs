using TaleWorlds.Core;
using MBOptionScreen.Attributes;
using MBOptionScreen.Attributes.v2;
using MBOptionScreen.Settings;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;

namespace Revolutions
{
    public class Settings : AttributeSettings<Settings>
    {
        public override string Id { get; set; } = "RevolutionsSettings_v1";

        public override string ModuleFolderName => "Revolutions";

        public override string ModName => "Revolutions";

        #region General

        [SettingPropertyGroup("{=revolutions_01}General")]
        [SettingPropertyFloatingInteger(displayName: "{=revolutions_08}Revolt Cooldown", minValue: 0f, maxValue: 250f, HintText = "{=revolutions_09}Cooldown before an uprising can occur again in the same city", RequireRestart = false)]
        public float RevoltCooldownTime { get; set; } = 30.0f;

        #endregion

        #region Loyalty

        [SettingPropertyGroup("{=str_revolutions_option_header_loyalty}/{=str_revolutions_option_subheader_mechanics}")]
        [SettingPropertyBool(displayName: "{=str_revolutions_option_loyalty_mechanics_empire_loyalty}", HintText = "{=str_revolutions_option_loyalty_mechanics_empire_loyalty_description}", RequireRestart = false)]
        public bool EmpireLoyaltyMechanics { get; set; } = true;

        [SettingPropertyGroup("{=str_revolutions_option_header_loyalty}/{=str_revolutions_option_subheader_values}")]
        [SettingPropertyFloatingInteger(displayName: "{=str_revolutions_option_loyalty_values_base_player_loyalty}", minValue: 0f, maxValue: 100f, HintText = "{=str_revolutions_option_loyalty_values_base_player_loyalty_description}", RequireRestart = false)]
        public float BasePlayerLoyalty { get; set; } = 5.0f;

        [SettingPropertyGroup("{=str_revolutions_option_header_loyalty}/{=str_revolutions_option_subheader_values}")]
        [SettingPropertyInteger(displayName: "{=str_revolutions_option_loyalty_values_minimum_obediance_loyalty}", minValue: 0, maxValue: 250, HintText = "{=str_revolutions_option_loyalty_values_minimum_obediance_loyalty_description}", RequireRestart = false)]
        public int MinimumObedianceLoyalty { get; set; } = 25;

        [SettingPropertyGroup("{=str_revolutions_option_header_loyalty}/{=str_revolutions_option_subheader_values}")]
        [SettingPropertyInteger(displayName: "{=str_revolutions_option_loyalty_values_days_until_loyalty_change}", minValue: 0, maxValue: 365, HintText = "{=str_revolutions_option_loyalty_values_days_until_loyalty_change_description}", RequireRestart = false)]
        public int DaysUntilLoyaltyChange { get; set; } = 80;

        [SettingPropertyGroup("{=str_revolutions_option_header_loyalty}/{=str_revolutions_option_subheader_values}")]
        [SettingPropertyInteger(displayName: "{=str_revolutions_option_loyalty_values_player_in_town_loyalty_increase}", minValue: 0, maxValue: 100, HintText = "{=str_revolutions_option_loyalty_values_player_in_town_loyalty_increase_description}", RequireRestart = false)]
        public int PlayerInTownLoyaltyIncrease { get; set; } = 5;

        #endregion

        #region Overextension

        [SettingPropertyGroup("{=str_revolutions_option_header_overextension}/{=str_revolutions_option_subheader_mechanics}")]
        [SettingPropertyBool(displayName: "{=str_revolutions_option_overextension_mechanics_overextension}", HintText = "{=str_revolutions_option_overextension_mechanics_overextension_description}", RequireRestart = false)]
        public bool OverextensionMechanics { get; set; } = true;

        [SettingPropertyGroup("{=str_revolutions_option_header_overextension}/{=str_revolutions_option_subheader_toggles}")]
        [SettingPropertyBool(displayName: "{=str_revolutions_option_overextension_toggles_overextension_affects_player}", HintText = "{=str_revolutions_option_overextension_toggles_overextension_affects_player_description}", RequireRestart = false)]
        public bool PlayerAffectedByOverextension { get; set; } = true;

        [SettingPropertyGroup("{=str_revolutions_option_header_overextension}/{=str_revolutions_option_subheader_values}")]
        [SettingPropertyFloatingInteger(displayName: "{=str_revolutions_option_overextension_values_overextension_multiplier}", minValue: 0f, maxValue: 10f, HintText = "{=str_revolutions_option_overextension_values_overextension_multiplier_description}", RequireRestart = false)]
        public float OverExtensionMultiplier { get; set; } = 2.0f;

        #endregion

        #region Other

        [SettingPropertyGroup("{=str_revolutions_option_header_other}/{=str_revolutions_option_subheader_toggles}")]
        [SettingPropertyBool(displayName: "{=str_revolutions_option_other_toggles_allow_minor_factions}", HintText = "{=str_revolutions_option_other_toggles_allow_minor_factions_description}", RequireRestart = false)]
        public bool AllowMinorFactions { get; set; } = false;

        #endregion
    }
}
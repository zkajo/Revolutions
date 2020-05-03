using MBOptionScreen.Attributes;
using MBOptionScreen.Attributes.v2;
using MBOptionScreen.Settings;

namespace Revolutions
{
    public class Settings : AttributeSettings<Settings>
    {
        public override string Id { get; set; } = "RevolutionsSettings_v042";

        public override string ModuleFolderName => "Revolutions";

        public override string ModName => "Revolutions";

        #region General

        [SettingPropertyGroup("{=MYu8nKqq}General")]
        [SettingPropertyFloatingInteger(displayName: "{=Pqk4uuGz}Revolt Cooldown", minValue: 0f, maxValue: 250f, HintText = "{=zx12ijyW}The time before another revolt can arise in the same city.", RequireRestart = false)]
        public float RevoltCooldownTime { get; set; } = 30.0f;

        #endregion

        #region Loyalty

        [SettingPropertyGroup("{=hPERH4u4}Loyalty/{=ONkcmltF}Mechanics")]
        [SettingPropertyBool(displayName: "{=uy96VZX2}Empire", HintText = "{=YnRmSelp}Activates/Deactivates the mechanic for the empire loyalty.", RequireRestart = false)]
        public bool EmpireLoyaltyMechanics { get; set; } = true;

        [SettingPropertyGroup("{=hPERH4u4}Loyalty/{=v0coMIcl}Values")]
        [SettingPropertyFloatingInteger(displayName: "{=qNWmNP0z}Base Loyalty", minValue: 0f, maxValue: 100f, HintText = "{=tOZS0maD}The base loyalty of cities to the player.", RequireRestart = false)]
        public float BasePlayerLoyalty { get; set; } = 5.0f;

        [SettingPropertyGroup("{=hPERH4u4}Loyalty/{=v0coMIcl}Values")]
        [SettingPropertyInteger(displayName: "{=ZuwszZww}Minimum Obediance", minValue: 0, maxValue: 250, HintText = "{=BkGsVccZ}Minimal loyalty is required for the city's obedience to the owner.", RequireRestart = false)]
        public int MinimumObedianceLoyalty { get; set; } = 25;

        [SettingPropertyGroup("{=hPERH4u4}Loyalty/{=v0coMIcl}Values")]
        [SettingPropertyInteger(displayName: "{=3fQwLwkC}Loyalty change", minValue: 0, maxValue: 365, HintText = "{=dRoMEfb0}Days until the city's loyalty to the new owner changes.", RequireRestart = false)]
        public int DaysUntilLoyaltyChange { get; set; } = 80;

        [SettingPropertyGroup("{=hPERH4u4}Loyalty/{=v0coMIcl}Values")]
        [SettingPropertyInteger(displayName: "{=cXCfwzPp}Loyalty Increase", minValue: 0, maxValue: 100, HintText = "{=HWiD4aR1}The amount by which loyalty increases when the owner is in town.", RequireRestart = false)]
        public int PlayerInTownLoyaltyIncrease { get; set; } = 5;

        #endregion

        #region Overextension

        [SettingPropertyGroup("{=Ts1iV2pO}Overextension/{=ONkcmltF}Mechanics")]
        [SettingPropertyBool(displayName: "{=sBw5Qz3q}Overextension", HintText = "{=ZRlNsvev}Activates/Deactivates the mechanics for overextension.", RequireRestart = false)]
        public bool OverextensionMechanics { get; set; } = true;

        [SettingPropertyGroup("{=Ts1iV2pO}Overextension/{=jPHYDjzQ}Toggles")]
        [SettingPropertyBool(displayName: "{=vuDS5ns8}Affects Player", HintText = "{=7LzQHMVj}Does the overextension mechanism affect the player.", RequireRestart = false)]
        public bool PlayerAffectedByOverextension { get; set; } = true;

        [SettingPropertyGroup("{=Ts1iV2pO}Overextension/{=v0coMIcl}Values")]
        [SettingPropertyFloatingInteger(displayName: "{=q2tbqN8d}Multiplier", minValue: 0f, maxValue: 10f, HintText = "{=6m1Ss8fW}A multiplier to calculate overextension.", RequireRestart = false)]
        public float OverExtensionMultiplier { get; set; } = 2.0f;

        #endregion

        #region Other

        [SettingPropertyGroup("{=XQGxDr11}Other/{=jPHYDjzQ}Toggles")]
        [SettingPropertyBool(displayName: "{=WbOKuWbq}Minor Factions", HintText = "{=P1g6H41e}If necessary, new, smaller fractions are formed.", RequireRestart = false)]
        public bool AllowMinorFactions { get; set; } = false;

        #endregion
    }
}
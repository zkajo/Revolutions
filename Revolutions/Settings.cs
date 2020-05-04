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

        [SettingPropertyGroup(groupName: "{=MYu8nKqq}General", order: 0)]
        [SettingPropertyFloatingInteger(displayName: "{=Pqk4uuGz}Revolt Cooldown", minValue: 0f, maxValue: 250f, Order = 0, HintText = "{=zx12ijyW}The time before another revolt can arise in the same faction.", RequireRestart = false)]
        public float RevoltCooldownTime { get; set; } = 30.0f;

        [SettingPropertyGroup("{=MYu8nKqq}General", order: 0)]
        [SettingPropertyFloatingInteger(displayName: "{=qNWmNP0z}Base Loyalty", minValue: 0f, maxValue: 100f, Order = 1, HintText = "{=tOZS0maD}The base loyalty of cities to the player.", RequireRestart = false)]
        public float BasePlayerLoyalty { get; set; } = 5.0f;

        [SettingPropertyGroup("{=MYu8nKqq}General", order: 0)]
        [SettingPropertyInteger(displayName: "{=3fQwLwkC}Loyalty Change", minValue: 0, maxValue: 365, Order = 2, HintText = "{=dRoMEfb0}Days until the city's loyalty to the new owner changes.", RequireRestart = false)]
        public int DaysUntilLoyaltyChange { get; set; } = 60;

        [SettingPropertyGroup("{=MYu8nKqq}General", order: 0)]
        [SettingPropertyInteger(displayName: "{=cXCfwzPp}Loyalty Increase", minValue: 0, maxValue: 100, Order = 3, HintText = "{=HWiD4aR1}The amount by which loyalty increases when the owner is in town.", RequireRestart = false)]
        public int PlayerInTownLoyaltyIncrease { get; set; } = 5;

        [SettingPropertyGroup("{=MYu8nKqq}General", order: 0)]
        [SettingPropertyInteger(displayName: "{=ZuwszZww}Minimum Obedience", minValue: 0, maxValue: 250, Order = 4, HintText = "{=BkGsVccZ}Minimal loyalty is required for the city's obedience to the owner.", RequireRestart = false)]
        public int MinimumObedienceLoyalty { get; set; } = 25;

        #endregion

        #region Loyalty

        [SettingPropertyGroup(groupName: "{=hPERH4u4}Imperial Loyalty", order : 1)]
        [SettingPropertyBool(displayName: "{=hPERH4u4}Imperial Loyalty", Order = 0, HintText = "{=YnRmSelp}Activates/Deactivates the mechanic for the empire loyalty.", RequireRestart = false)]
        public bool EmpireLoyaltyMechanics { get; set; } = true;

        #endregion

        #region Overextension

        [SettingPropertyGroup(groupName: "{=Ts1iV2pO}Overextension", order: 2)]
        [SettingPropertyBool(displayName: "{=Ts1iV2pO}Overextension", Order = 0, HintText = "{=ZRlNsvev}Activates/Deactivates the mechanics for overextension.", RequireRestart = false)]
        public bool OverextensionMechanics { get; set; } = true;

        [SettingPropertyGroup(groupName: "{=Ts1iV2pO}Overextension", order: 2)]
        [SettingPropertyBool(displayName: "{=vuDS5ns8}Affects Player", Order = 1, HintText = "{=7LzQHMVj}Does the overextension mechanism affect the player.", RequireRestart = false)]
        public bool PlayerAffectedByOverextension { get; set; } = true;

        [SettingPropertyGroup(groupName: "{=Ts1iV2pO}Overextension", order: 2)]
        [SettingPropertyFloatingInteger(displayName: "{=q2tbqN8d}Multiplier", minValue: 0f, maxValue: 10f, Order = 2, HintText = "{=6m1Ss8fW}A multiplier to calculate overextension.", RequireRestart = false)]
        public float OverExtensionMultiplier { get; set; } = 2.0f;

        #endregion

        #region Minor Factions

        [SettingPropertyGroup(groupName: "{=WbOKuWbq}Minor Factions", order: 3)]
        [SettingPropertyBool(displayName: "{=WbOKuWbq}Minor Factions", Order = 0, HintText = "{=P1g6H41e}Activates/Deactivates the mechanics for minor factions.", RequireRestart = false)]
        public bool AllowMinorFactions { get; set; } = true;

        #endregion
    }
}
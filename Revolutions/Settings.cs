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

        [SettingPropertyGroup("{=MYu8nKqq}")]
        [SettingPropertyFloatingInteger(displayName: "{=Pqk4uuGz}", minValue: 0f, maxValue: 250f, HintText = "{=zx12ijyW}", RequireRestart = false)]
        public float RevoltCooldownTime { get; set; } = 30.0f;

        #endregion

        #region Loyalty

        [SettingPropertyGroup("{=hPERH4u4}/{=ONkcmltF}")]
        [SettingPropertyBool(displayName: "{=uy96VZX2}", HintText = "{=YnRmSelp}", RequireRestart = false)]
        public bool EmpireLoyaltyMechanics { get; set; } = true;

        [SettingPropertyGroup("{=hPERH4u4}/{=v0coMIcl}")]
        [SettingPropertyFloatingInteger(displayName: "{=qNWmNP0z}", minValue: 0f, maxValue: 100f, HintText = "{=tOZS0maD}", RequireRestart = false)]
        public float BasePlayerLoyalty { get; set; } = 5.0f;

        [SettingPropertyGroup("{=hPERH4u4}/{=v0coMIcl}")]
        [SettingPropertyInteger(displayName: "{=ZuwszZww}", minValue: 0, maxValue: 250, HintText = "{=BkGsVccZ}", RequireRestart = false)]
        public int MinimumObedianceLoyalty { get; set; } = 25;

        [SettingPropertyGroup("{=hPERH4u4}/{=v0coMIcl}")]
        [SettingPropertyInteger(displayName: "{=3fQwLwkC}", minValue: 0, maxValue: 365, HintText = "{=dRoMEfb0}", RequireRestart = false)]
        public int DaysUntilLoyaltyChange { get; set; } = 80;

        [SettingPropertyGroup("{=hPERH4u4}/{=v0coMIcl}")]
        [SettingPropertyInteger(displayName: "{=cXCfwzPp}", minValue: 0, maxValue: 100, HintText = "{=HWiD4aR1}", RequireRestart = false)]
        public int PlayerInTownLoyaltyIncrease { get; set; } = 5;

        #endregion

        #region Overextension

        [SettingPropertyGroup("{=Ts1iV2pO}/{=ONkcmltF}")]
        [SettingPropertyBool(displayName: "{=sBw5Qz3q}", HintText = "{=ZRlNsvev}", RequireRestart = false)]
        public bool OverextensionMechanics { get; set; } = true;

        [SettingPropertyGroup("{=Ts1iV2pO}/{=jPHYDjzQ}")]
        [SettingPropertyBool(displayName: "{=vuDS5ns8}", HintText = "{=7LzQHMVj}", RequireRestart = false)]
        public bool PlayerAffectedByOverextension { get; set; } = true;

        [SettingPropertyGroup("{=Ts1iV2pO}/{=v0coMIcl}")]
        [SettingPropertyFloatingInteger(displayName: "{=q2tbqN8d}", minValue: 0f, maxValue: 10f, HintText = "{=6m1Ss8fW}", RequireRestart = false)]
        public float OverExtensionMultiplier { get; set; } = 2.0f;

        #endregion

        #region Other

        [SettingPropertyGroup("{=XQGxDr11}/{=jPHYDjzQ}")]
        [SettingPropertyBool(displayName: "{=WbOKuWbq}", HintText = "{=P1g6H41e}", RequireRestart = false)]
        public bool AllowMinorFactions { get; set; } = false;

        #endregion
    }
}
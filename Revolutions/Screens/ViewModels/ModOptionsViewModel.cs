using System;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Revolutions.Screens.ViewModels
{
    public class ModOptionsViewModel : ViewModel
    {
        private readonly Action m_onClose;

        #region revolt cooldown

        [DataSourceProperty] public float SliderRevoltCooldownMinValue => 0f;
        [DataSourceProperty] public float SliderRevoltCooldownMaxValue => 250f;

        private string GetText(string id)
        {
            TextObject textObject = GameTexts.FindText(id);
            return textObject.ToString();
        }

        private float m_revolt_cooldown;

        [DataSourceProperty] public string RevoltCooldownString { get; private set; }

        [DataSourceProperty]
        public float RevoltCooldown
        {
            get => m_revolt_cooldown;
            set
            {
                SetField(ref m_revolt_cooldown, value, nameof(RevoltCooldown));
                TextObject textObject = GameTexts.FindText("str_opt_RevoltCooldown");
                textObject.SetTextVariable("COOLDOWN", (int)m_revolt_cooldown);

                RevoltCooldownString = textObject.ToString();
                OnPropertyChanged(nameof(RevoltCooldownString));
                SubModule.Configuration.RevoltCooldownTime = m_revolt_cooldown;
            }
        }

        private float m_daysUntilLoyaltyChange;

        [DataSourceProperty]
        public string DoneDesc
        {
            get { return GetText("str_rev_Done"); }
        }

        [DataSourceProperty] public string OverextensionAffectsPlayerDesc { get { return GetText("str_opt_OverextPlayerDesc"); } }
        [DataSourceProperty] public string OverextensionDesc { get { return GetText("str_opt_OverextDesc"); } }
        [DataSourceProperty] public string ImperialLoyaltyMechanicDesc { get { return GetText("str_opt_ImpLoyaltyMechDesc"); } }
        [DataSourceProperty] public string DaysUntilLoyaltyChangeString { get; private set; }
        [DataSourceProperty] public float SliderDaysUntilLoyaltyChangeMinValue => 0f;
        [DataSourceProperty] public float SliderDaysUntilLoyaltyChangeMaxValue => 250f;
        [DataSourceProperty] public string MinorFactionsEnabledDesc { get { return GetText("str_opt_MinFactEnabledDesc"); } }
        [DataSourceProperty] public string DebugModeDesc { get { return GetText("str_opt_DebugDesc"); } }

        [DataSourceProperty]
        public float DaysUntilLoyaltyChange
        {
            get => m_daysUntilLoyaltyChange;
            set
            {
                SetField(ref m_daysUntilLoyaltyChange, value, nameof(DaysUntilLoyaltyChange));
                TextObject textObject = GameTexts.FindText("str_opt_LoyaltyChangeDays");
                textObject.SetTextVariable("DAYS", (int)m_daysUntilLoyaltyChange);

                DaysUntilLoyaltyChangeString = textObject.ToString();
                OnPropertyChanged(nameof(DaysUntilLoyaltyChangeString));
                SubModule.Configuration.DaysUntilLoyaltyChange = (int)m_daysUntilLoyaltyChange;
            }
        }

        #endregion

        #region Mechanics

        private bool m_EmpireLoyaltyMechanicsEnabled;
        private bool m_OverextensionMechanicsEnabled;
        private bool m_PlayerAffectedByOverextension;
        private bool m_MinorFactionsEnabled;
        private bool m_DebugModeEnabled;

        [DataSourceProperty]
        public bool EmpireLoyaltyMechanicsEnabled
        {
            get => m_EmpireLoyaltyMechanicsEnabled;
            set
            {
                SetField(ref m_EmpireLoyaltyMechanicsEnabled, value, nameof(EmpireLoyaltyMechanicsEnabled));
                SubModule.Configuration.EmpireLoyaltyMechanics = m_EmpireLoyaltyMechanicsEnabled;
            }
        }

        [DataSourceProperty]
        public bool OverextensionMechanicsEnabled
        {
            get => m_OverextensionMechanicsEnabled;
            set
            {
                SetField(ref m_OverextensionMechanicsEnabled, value, nameof(OverextensionMechanicsEnabled));
                SubModule.Configuration.OverextensionMechanics = m_OverextensionMechanicsEnabled;
            }
        }

        [DataSourceProperty]
        public bool MinorFactionsEnabled
        {
            get => m_MinorFactionsEnabled;
            set
            {
                SetField(ref m_MinorFactionsEnabled, value, nameof(MinorFactionsEnabled));
                SubModule.Configuration.AllowMinorFactions = m_MinorFactionsEnabled;
            }
        }

        [DataSourceProperty]
        public bool DebugModeEnabled
        {
            get => m_DebugModeEnabled;
            set
            {
                SetField(ref m_DebugModeEnabled, value, nameof(DebugModeEnabled));
                SubModule.Configuration.DebugMode = m_DebugModeEnabled;
            }
        }

        [DataSourceProperty]
        public bool PlayerAffectedByOverextension
        {
            get => m_PlayerAffectedByOverextension;
            set
            {
                SetField(ref m_PlayerAffectedByOverextension, value, nameof(PlayerAffectedByOverextension));
                SubModule.Configuration.PlayerAffectedByOverextension = m_PlayerAffectedByOverextension;
            }
        }

        #endregion

        public sealed override void RefreshValues()
        {
            base.RefreshValues();
            RevoltCooldown = SubModule.Configuration.RevoltCooldownTime;
            EmpireLoyaltyMechanicsEnabled = SubModule.Configuration.EmpireLoyaltyMechanics;
            OverextensionMechanicsEnabled = SubModule.Configuration.OverextensionMechanics;
            PlayerAffectedByOverextension = SubModule.Configuration.PlayerAffectedByOverextension;
            DaysUntilLoyaltyChange = SubModule.Configuration.DaysUntilLoyaltyChange;
            MinorFactionsEnabled = SubModule.Configuration.AllowMinorFactions;
            DebugModeEnabled = SubModule.Configuration.DebugMode;
        }

        public ModOptionsViewModel(Action onClose)
        {
            m_onClose = onClose;
            RefreshValues();
        }

        private void ExitOptionsMenu()
        {
            m_onClose?.Invoke();
            ScreenManager.PopScreen();
        }

        private void ExecuteDone() => m_onClose?.Invoke();
    }
}

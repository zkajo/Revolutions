using System;
using Revolutions.CampaignBehaviours;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Options.ManagedOptions;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace Revolutions.Screens.ViewModels
{
    public class ModOptionsViewModel : ViewModel
    {
        private ModOptionsData _data => ModOptions.OptionsData;
        private readonly Action m_onClose;

        #region revolt cooldown
        
        [DataSourceProperty] public float SliderRevoltCooldownMinValue => 0f;
        [DataSourceProperty] public float SliderRevoltCooldownMaxValue => 250f;

        private float m_revolt_cooldown;

        [DataSourceProperty] public string RevoltCooldownString { get; private set; }
        
        [DataSourceProperty]
        public float RevoltCooldown
        {
            get => m_revolt_cooldown;
            set
            {
                SetField(ref m_revolt_cooldown, value, nameof(RevoltCooldown));
                RevoltCooldownString = $"Revolt cooldown per faction: {m_revolt_cooldown:0} days";
                OnPropertyChanged(nameof(RevoltCooldownString));
                _data.RevoltCooldownTime = m_revolt_cooldown;
            }
        }

        private float m_daysUntilLoyaltyChange;
        [DataSourceProperty] public string DaysUntilLoyaltyChangeString { get; private set; }
        [DataSourceProperty] public float SliderDaysUntilLoyaltyChangeMinValue => 0f;
        [DataSourceProperty] public float SliderDaysUntilLoyaltyChangeMaxValue => 250f;

        [DataSourceProperty]
        public float DaysUntilLoyaltyChange
        {
            get => m_daysUntilLoyaltyChange;
            set
            {
                SetField(ref m_daysUntilLoyaltyChange, value, nameof(DaysUntilLoyaltyChange));
                DaysUntilLoyaltyChangeString = $"City changes loyalty to current owner in: {(int)m_daysUntilLoyaltyChange} days";
                OnPropertyChanged(nameof(DaysUntilLoyaltyChangeString));
                _data.DaysUntilLoyaltyChange = (int)m_daysUntilLoyaltyChange;
            }
        }
        
        #endregion

        #region Mechanics

        private bool m_EmpireLoyaltyMechanicsEnabled;
        private bool m_OverextensionMechanicsEnabled;
        private bool m_PlayerAffectedByOverextension;
        
        [DataSourceProperty]
        public bool EmpireLoyaltyMechanicsEnabled
        {
            get => m_EmpireLoyaltyMechanicsEnabled;
            set
            {
                SetField(ref m_EmpireLoyaltyMechanicsEnabled, value, nameof(EmpireLoyaltyMechanicsEnabled));
                ModOptions.OptionsData.EmpireLoyaltyMechanics = m_EmpireLoyaltyMechanicsEnabled;
            } 
        }
        
        [DataSourceProperty]
        public bool OverextensionMechanicsEnabled
        {
            get => m_OverextensionMechanicsEnabled;
            set
            {
                SetField(ref m_OverextensionMechanicsEnabled, value, nameof(OverextensionMechanicsEnabled));
                ModOptions.OptionsData.OverextensionMechanics = m_OverextensionMechanicsEnabled;
            } 
        }
        
        [DataSourceProperty]
        public bool PlayerAffectedByOverextension
        {
            get => m_PlayerAffectedByOverextension;
            set
            {
                SetField(ref m_PlayerAffectedByOverextension, value, nameof(PlayerAffectedByOverextension));
                ModOptions.OptionsData.PlayerAffectedByOverextension = m_PlayerAffectedByOverextension;
            } 
        }

        #endregion
        
        public sealed override void RefreshValues()
        {
            base.RefreshValues();
            RevoltCooldown = _data.RevoltCooldownTime;
            EmpireLoyaltyMechanicsEnabled = _data.EmpireLoyaltyMechanics;
            OverextensionMechanicsEnabled = _data.OverextensionMechanics;
            PlayerAffectedByOverextension = _data.PlayerAffectedByOverextension;
            DaysUntilLoyaltyChange = _data.DaysUntilLoyaltyChange;
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
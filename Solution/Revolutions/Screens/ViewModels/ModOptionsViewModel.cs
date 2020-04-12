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
        [DataSourceProperty] public float SliderRevoltCooldownMaxValue => 100f;

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
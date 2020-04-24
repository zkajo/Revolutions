using System;
using Revolutions.CampaignBehaviours;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.Localization;
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
                _data.RevoltCooldownTime = m_revolt_cooldown;
            }
        }

        private float m_daysUntilLoyaltyChange;
        
        [DataSourceProperty] public  string DoneDesc
        {
            get { return GetText("str_rev_Done"); }
        }
        
        [DataSourceProperty] public string OverextensionAffectsPlayerDesc {  get { return GetText("str_opt_OverextPlayerDesc"); }  }
        [DataSourceProperty] public string OverextensionDesc {  get { return GetText("str_opt_OverextDesc"); }  }
        [DataSourceProperty] public string ImperialLoyaltyMechanicDesc {  get { return GetText("str_opt_ImpLoyaltyMechDesc"); }  }
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
                TextObject textObject = GameTexts.FindText("str_opt_LoyaltyChangeDays");
                textObject.SetTextVariable("DAYS", (int)m_daysUntilLoyaltyChange);
                
                DaysUntilLoyaltyChangeString = textObject.ToString();
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
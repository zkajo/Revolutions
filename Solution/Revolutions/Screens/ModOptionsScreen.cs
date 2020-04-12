using Revolutions.CampaignBehaviours;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using Revolutions.Screens.ViewModels;

namespace Revolutions.Screens
{
    public class ModOptionsScreen : ScreenBase
    {
        private ModOptionsViewModel _dataSource;
        private GauntletLayer _gauntletLayer;
        private GauntletMovie _movie;

        private bool _firstRender;

        public ModOptionsScreen(ModOptionsData optionsData)
        {
            
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _dataSource = new ModOptionsViewModel(OnClose);
            _gauntletLayer = new GauntletLayer(100);
            _gauntletLayer.IsFocusLayer = true;
            AddLayer(_gauntletLayer);
            _gauntletLayer.InputRestrictions.SetInputRestrictions();
            ScreenManager.TrySetFocus(_gauntletLayer);
            _movie = _gauntletLayer.LoadMovie("RevolutionModOptions", _dataSource);
            _firstRender = true;
        }

        private void OnClose()
        {
            
        }

        protected override void OnFinalize()
        {
            base.OnFinalize();
            _dataSource = null;
            _gauntletLayer = null;
            _movie = null;
        }

        protected override void OnFrameTick(float dt)
        {
            base.OnFrameTick(dt);
            InputContext input = _gauntletLayer.Input;

            if (_firstRender)
            {
                //TODO  something flashy
            }
        }
    }
}
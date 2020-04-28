using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
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
            this._dataSource = new ModOptionsViewModel(this.OnClose);
            this._gauntletLayer = new GauntletLayer(100)
            {
                IsFocusLayer = true
            };
            this.AddLayer(this._gauntletLayer);
            this._gauntletLayer.InputRestrictions.SetInputRestrictions();
            ScreenManager.TrySetFocus(this._gauntletLayer);
            this._movie = this._gauntletLayer.LoadMovie("RevolutionModOptions", this._dataSource);
            this._firstRender = true;
        }

        private void OnClose()
        {

        }

        protected override void OnFinalize()
        {
            base.OnFinalize();
            this._dataSource = null;
            this._gauntletLayer = null;
            this._movie = null;
        }

        protected override void OnFrameTick(float dt)
        {
            base.OnFrameTick(dt);

            _ = this._gauntletLayer.Input;
        }
    }
}
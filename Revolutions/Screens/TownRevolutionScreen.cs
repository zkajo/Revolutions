using Revolutions.CampaignBehaviours;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using Revolutions.Screens.ViewModels;

namespace Revolutions.Screens
{
    class TownRevolutionScreen : ScreenBase
    {
        private readonly SettlementInfo _settlementInfo;
        private readonly FactionInfo _factionInfo;

        private TownRevolutionViewModel _dataSource;
        private GauntletLayer _gauntletLayer;
        private GauntletMovie _movie;

        private bool _firstRender;

        protected override void OnFinalize()
        {
            base.OnFinalize();
            this._gauntletLayer = null;
            this._dataSource = null;
            this._movie = null;
        }

        public TownRevolutionScreen(SettlementInfo settinfo, FactionInfo factInfo)
        {
            this._settlementInfo = settinfo;
            this._factionInfo = factInfo;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            this._dataSource = new TownRevolutionViewModel(this._settlementInfo, this._factionInfo);
            this._gauntletLayer = new GauntletLayer(100)
            {
                IsFocusLayer = true
            };
            this.AddLayer(this._gauntletLayer);
            this._gauntletLayer.InputRestrictions.SetInputRestrictions();
            ScreenManager.TrySetFocus(this._gauntletLayer);
            this._movie = this._gauntletLayer.LoadMovie("TownRevolutionScreen", this._dataSource);
            this._firstRender = true;
        }

        protected override void OnFrameTick(float dt)
        {
            base.OnFrameTick(dt);

            _ = this._gauntletLayer.Input;
        }
    }
}

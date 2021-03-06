﻿using Revolutions.CampaignBehaviours;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using Revolutions.Screens.ViewModels;

namespace Revolutions.Screens
{
    class TownRevolutionScreen : ScreenBase
    {
        private SettlementInfo _settlementInfo;
        private FactionInfo _factionInfo;
        private TownRevolutionViewModel _dataSource;
        private GauntletLayer _gauntletLayer;
        private GauntletMovie _movie;

        private bool _firstRender;

        protected override void OnFinalize()
        {
            base.OnFinalize();
            _gauntletLayer = null;
            _dataSource = null;
            _movie = null;
        }

        public TownRevolutionScreen(SettlementInfo settinfo, FactionInfo factInfo)
        {
            this._settlementInfo = settinfo;
            this._factionInfo = factInfo;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _dataSource = new TownRevolutionViewModel(this._settlementInfo, this._factionInfo);
            _gauntletLayer = new GauntletLayer(100);
            _gauntletLayer.IsFocusLayer = true;
            AddLayer(_gauntletLayer);
            _gauntletLayer.InputRestrictions.SetInputRestrictions();
            ScreenManager.TrySetFocus(_gauntletLayer);
            _movie = _gauntletLayer.LoadMovie("TownRevolutionScreen", _dataSource);
            _firstRender = true;
        }

        protected override void OnFrameTick(float dt)
        {
            base.OnFrameTick(dt);
            InputContext input = _gauntletLayer.Input;
        }
    }
}

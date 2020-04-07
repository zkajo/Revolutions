using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace Revolutions.Screens.ViewModels
{
    class TownRevolutionViewModel : ViewModel
    {
        private SettlementInfo settlementInfo;

        public TownRevolutionViewModel(SettlementInfo info)
        {
            settlementInfo = info;
            _factionVisual = new ImageIdentifierVM(BannerCode.CreateFrom(settlementInfo.GetOriginalFaction().Banner), true);
        }

        private ImageIdentifierVM _factionVisual;

        [DataSourceProperty]
        public ImageIdentifierVM FactionVisual
        {
            get
            {
                return this._factionVisual;
            }
            set
            {
                if (value != this._factionVisual)
                {
                    this.FactionVisual = value;
                    base.OnPropertyChanged("FactionVisual");
                }
            }
        }

        [DataSourceProperty]
        public string TownDescription
        {
            get
            {
                if (settlementInfo.RevoltProgress < 10)
                {
                    return "The people of " + settlementInfo.GetSettlement().Name + " seem to be content";
                }
                else
                {
                    return "Flames of revolution are slowly stirring in " + settlementInfo.GetSettlement().Name;
                }
            }
        }

        [DataSourceProperty]
        public string TownOwnership
        {
            get
            {
                return "Population is loyal to the " + settlementInfo.GetOriginalFaction().Name;
            }
        }

        [DataSourceProperty]
        public string RevolutionProgress
        {
            get
            {
                return "Current revolution progress is " + settlementInfo.RevoltProgress + "%";
            }
        }

        private void ExitTownPropertyMenu()
        {
            ScreenManager.PopScreen();
        }

        private void RefreshProperties()
        {
            OnPropertyChanged("TownDescription");
            OnPropertyChanged("RevolutionProgress");
            OnPropertyChanged("TownOwnership");
            OnPropertyChanged("FactionVisual");
        }
    }
}

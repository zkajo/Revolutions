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
        private FactionInfo factionInfo;

        public TownRevolutionViewModel(SettlementInfo settInfo, FactionInfo factInfo)
        {
            settlementInfo = settInfo;
            factionInfo = factInfo;
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

        [DataSourceProperty]
        public string RevoltMood
        {
            get
            {
                if (factionInfo.GetFaction().StringId == settlementInfo.GetOriginalFaction().StringId)
                {
                    return "People are content with the current rule";
                }

                if (factionInfo.RevoltCanHappen())
                {
                    return "Some talk of raising banners of their homeland.";
                    
                }
                else
                {
                    if (factionInfo.RevoltedSettlement() == null)
                    {
                        return " ";
                    }

                    if (factionInfo.RevoltedSettlement().StringId == settlementInfo.GetSettlement().StringId)
                    {
                        return "The people of this town had revolted recently, and don't wish to spill blood again.";
                    }

                    return "After hearing of blood spilled in " + factionInfo.RevoltedSettlement().Name + " citizens are afraid of revolting.";
                }
            }
            set
            {
                if (value != this.RevoltMood)
                {
                    this.RevoltMood = value;
                    base.OnPropertyChanged("RevoltMood");
                }
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
            OnPropertyChanged("FactionVisual");
        }
    }
}

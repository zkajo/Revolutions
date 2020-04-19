using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revolutions.CampaignBehaviours;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace Revolutions.Screens.ViewModels
{
    class TownRevolutionViewModel : ViewModel
    {
        private SettlementInfo _settlementInfo;
        private FactionInfo _factionInfo;

        public TownRevolutionViewModel(SettlementInfo settInfo, FactionInfo factInfo)
        {
            _settlementInfo = settInfo;
            _factionInfo = factInfo;
            _factionVisual = new ImageIdentifierVM(BannerCode.CreateFrom(_settlementInfo.OriginalFaction.Banner), true);
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
                if (_settlementInfo.RevoltProgress < 10)
                {
                    return "The people of " + _settlementInfo.Settlement.Name + " seem to be content";
                }
                else
                {
                    return "Flames of revolution are slowly stirring in " + _settlementInfo.Settlement.Name;
                }
            }
        }

        [DataSourceProperty]
        public string TownOwnership
        {
            get
            {
                return "Population is loyal to the " + _settlementInfo.OriginalFaction.Name;
            }
        }

        [DataSourceProperty]
        public string RevolutionProgress
        {
            get
            {
                return "Current revolution progress is " + _settlementInfo.RevoltProgress + "%";
            }
        }

        [DataSourceProperty]
        public string RevoltMood
        {
            get
            {
                if (_factionInfo.GetFaction().StringId == _settlementInfo.OriginalFaction.StringId)
                {
                    return "People are content with the current rule";
                }

                if (_factionInfo.RevoltCanHappen())
                {
                    string inspiration = "";
                    if (_factionInfo.SuccessfulRevolt())
                    {
                        //no idea why that's the case, but it is O_O
                        //TODO find a cause of this?
                        if (_factionInfo.RevoltedSettlement() == null)
                        {
                            inspiration = " Many are inspired by tales of revolts in the kingdom.";
                        }
                        else
                        {
                            inspiration = " Many are inspired by events at " + _factionInfo.RevoltedSettlement().Name;
                        }
                    }
                    
                    return "Some talk of raising banners of their homeland." + inspiration;
                    
                }
                else
                {
                    if (_factionInfo.RevoltedSettlement() == null)
                    {
                        return " ";
                    }

                    if (_factionInfo.RevoltedSettlement().StringId == _settlementInfo.Settlement.StringId)
                    {
                        return "The people of this town had revolted recently, and don't wish to spill blood again.";
                    }

                    return "After hearing of blood spilled in " + _factionInfo.RevoltedSettlement().Name + " citizens are afraid of revolting.";
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

        private void OpenOptionsMenu()
        {
            ModOptions.CreateModOptionsMenu();
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

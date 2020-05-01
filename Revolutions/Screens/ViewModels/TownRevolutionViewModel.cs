using ModLibrary.Factions;
using ModLibrary.Settlements;
using Revolutions.CampaignBehaviors;
using Revolutions.Factions;
using Revolutions.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Revolutions.Screens.ViewModels
{
    public class TownRevolutionViewModel : ViewModel
    {
        private SettlementInfoRevolutions _settlementInfo;
        private FactionInfoRevolutions _factionInfo;

        public TownRevolutionViewModel(SettlementInfoRevolutions settInfo, FactionInfoRevolutions factInfo)
        {
            _settlementInfo = settInfo;
            _factionInfo = factInfo;
            _factionVisual = new ImageIdentifierVM(BannerCode.CreateFrom(_settlementInfo.LoyalFaction.Banner), true);
        }

        private ImageIdentifierVM _factionVisual;

        [DataSourceProperty] public  string DoneDesc
        {
            get { return GetText("str_rev_Done"); }
        }

        [DataSourceProperty] public string InformationDesc
        {
            get { return GetText("str_TD_Information"); }
        }
        
        [DataSourceProperty] public  string OptionsDesc
        {
            get { return GetText("str_rev_Options"); }
        }
        
        private string GetText(string id)
        {
            TextObject textObject = GameTexts.FindText(id);
            return textObject.ToString();
        }
        
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
                if (_settlementInfo.RevolutionProgress < 10)
                {
                    TextObject textObject = GameTexts.FindText("str_TD_Content");
                    textObject.SetTextVariable("SETTLEMENT", this._settlementInfo.Settlement.Name);

                    return textObject.ToString();
                }
                else
                {
                    TextObject textObject = GameTexts.FindText("str_TD_FlamesOfRevolution");
                    textObject.SetTextVariable("SETTLEMENT", this._settlementInfo.Settlement.Name);
                    
                    return textObject.ToString();
                }
            }
        }

        [DataSourceProperty]
        public string TownOwnership
        {
            get
            {
                TextObject textObject = GameTexts.FindText("str_TD_LoyalToFaction");
                textObject.SetTextVariable("FACTION", this._settlementInfo.LoyalFaction.Name);

                return textObject.ToString();
            }
        }

        [DataSourceProperty]
        public string RevolutionProgress
        {
            get
            {                
                TextObject textObject = GameTexts.FindText("str_TD_RevoltProgress");
                textObject.SetTextVariable("PROGRESS", _settlementInfo.RevolutionProgress);

                return textObject.ToString();
            }
        }

        [DataSourceProperty]
        public string RevoltMood
        {
            get
            {
                if (_factionInfo.FactionId == _settlementInfo.LoyalFaction.StringId)
                {
                    TextObject textObject = GameTexts.FindText("str_TD_Mood_Content");
                    return textObject.ToString();
                }

                if (_factionInfo.CanRevolt)
                {
                    TextObject inspiration = new TextObject("");
                    if (_factionInfo.SuccessfullyRevolted)
                    {
                        //no idea why that's the case, but it is O_O
                        //TODO find a cause of this?
                        if (_factionInfo.RevoltedSettlement == null)
                        {
                            inspiration = GameTexts.FindText("str_TD_Mood_inspiration_01");
                        }
                        else
                        {
                            inspiration = GameTexts.FindText("str_TD_Mood_inspiration_02");
                            inspiration.SetTextVariable("SETTLEMENT", _factionInfo.RevoltedSettlement.Name);
                        }
                    }
                    
                    TextObject baseText = GameTexts.FindText("str_TD_Mood_RaiseBanners");
                    return baseText.ToString() + " " + inspiration.ToString();
                    
                }
                else
                {
                    if (_factionInfo.RevoltedSettlement == null)
                    {
                        return " ";
                    }

                    if (_factionInfo.RevoltedSettlementId == _settlementInfo.Settlement.StringId)
                    {
                        TextObject option = GameTexts.FindText("str_TD_Mood_RecentRevolt");
                        return option.ToString();
                    }
                    
                    TextObject textObject = GameTexts.FindText("str_TD_Mood_RecentRevolt2");
                    textObject.SetTextVariable("SETTLEMENT", _factionInfo.RevoltedSettlement.Name);
                    
                    return textObject.ToString();
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
            GuiHandlersBehaviour.CreateModOptionsMenu();
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
using Revolutions.CampaignBehaviours;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Revolutions.Screens.ViewModels
{
    class TownRevolutionViewModel : ViewModel
    {
        private readonly SettlementInfo _settlementInfo;
        private readonly FactionInfo _factionInfo;

        public TownRevolutionViewModel(SettlementInfo settInfo, FactionInfo factInfo)
        {
            this._settlementInfo = settInfo;
            this._factionInfo = factInfo;
            this._factionVisual = new ImageIdentifierVM(BannerCode.CreateFrom(this._settlementInfo.OriginalFaction.Banner), true);
        }

        private readonly ImageIdentifierVM _factionVisual;

        [DataSourceProperty]
        public string DoneDesc
        {
            get { return this.GetText("str_rev_Done"); }
        }

        [DataSourceProperty]
        public string InformationDesc
        {
            get { return this.GetText("str_TD_Information"); }
        }

        [DataSourceProperty]
        public string OptionsDesc
        {
            get { return this.GetText("str_rev_Options"); }
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
                if (this._settlementInfo.RevoltProgress < 10)
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
                textObject.SetTextVariable("FACTION", this._settlementInfo.OriginalFaction.Name);

                return textObject.ToString();
            }
        }

        [DataSourceProperty]
        public string RevolutionProgress
        {
            get
            {
                TextObject textObject = GameTexts.FindText("str_TD_RevoltProgress");
                textObject.SetTextVariable("PROGRESS", this._settlementInfo.RevoltProgress);

                return textObject.ToString();
            }
        }

        [DataSourceProperty]
        public string RevoltMood
        {
            get
            {
                if (this._factionInfo.Faction.StringId == this._settlementInfo.OriginalFaction.StringId)
                {
                    TextObject textObject = GameTexts.FindText("str_TD_Mood_Content");
                    return textObject.ToString();
                }

                if (this._factionInfo.RevoltCanHappen())
                {
                    TextObject inspiration = new TextObject("");
                    if (this._factionInfo.SuccessfulRevolt())
                    {
                        //no idea why that's the case, but it is O_O
                        //TODO find a cause of this?
                        if (this._factionInfo.RevoltedSettlement() == null)
                        {
                            inspiration = GameTexts.FindText("str_TD_Mood_inspiration_01");
                        }
                        else
                        {
                            inspiration = GameTexts.FindText("str_TD_Mood_inspiration_02");
                            inspiration.SetTextVariable("SETTLEMENT", this._factionInfo.RevoltedSettlement().Name);
                        }
                    }

                    TextObject baseText = GameTexts.FindText("str_TD_Mood_RaiseBanners");
                    return baseText.ToString() + " " + inspiration.ToString();

                }
                else
                {
                    if (this._factionInfo.RevoltedSettlement() == null)
                    {
                        return " ";
                    }

                    if (this._factionInfo.RevoltedSettlement().StringId == this._settlementInfo.Settlement.StringId)
                    {
                        TextObject option = GameTexts.FindText("str_TD_Mood_RecentRevolt");
                        return option.ToString();
                    }

                    TextObject textObject = GameTexts.FindText("str_TD_Mood_RecentRevolt2");
                    textObject.SetTextVariable("SETTLEMENT", this._factionInfo.RevoltedSettlement().Name);

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
            ModOptions.CreateModOptionsMenu();
        }

        private void RefreshProperties()
        {
            this.OnPropertyChanged("TownDescription");
            this.OnPropertyChanged("RevolutionProgress");
            this.OnPropertyChanged("TownOwnership");
            this.OnPropertyChanged("FactionVisual");
            //TODO: Trigger 2-times?
            this.OnPropertyChanged("FactionVisual");
        }
    }
}

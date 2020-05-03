using Revolutions.Components.Factions;
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
            this._settlementInfo = settInfo;
            this._factionInfo = factInfo;
            this._factionVisual = new ImageIdentifierVM(BannerCode.CreateFrom(this._settlementInfo.LoyalFaction.Banner), true);
        }

        private ImageIdentifierVM _factionVisual;

        [DataSourceProperty]
        public string DoneDesc
        {
            get { return new TextObject("{=3fQwWiDZ}Done").ToString(); }
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
                if (this._settlementInfo.RevolutionProgress < 10)
                {
                    TextObject textObject = new TextObject("{=3fBkqk4u}The people of {SETTLEMENT} seem to be content.");
                    textObject.SetTextVariable("SETTLEMENT", this._settlementInfo.Settlement.Name);

                    return textObject.ToString();
                }
                else
                {
                    TextObject textObject = new TextObject("{=dRoS0maD}Flames of revolution are slowly stirring in {SETTLEMENT}.");
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
                TextObject textObject = new TextObject("{=MYu8szGz}Population is loyal to the {FACTION}.");
                textObject.SetTextVariable("FACTION", this._settlementInfo.LoyalFaction.Name);

                return textObject.ToString();
            }
        }

        [DataSourceProperty]
        public string RevolutionProgress
        {
            get
            {
                TextObject textObject = new TextObject("{=q2tbSs8d}Current revolt progress is {PROGRESS}%.");
                textObject.SetTextVariable("PROGRESS", this._settlementInfo.RevolutionProgress);

                return textObject.ToString();
            }
        }

        [DataSourceProperty]
        public string RevoltMood
        {
            get
            {
                if (this._factionInfo.FactionId == this._settlementInfo.LoyalFaction.StringId)
                {
                    TextObject textObject = new TextObject("{=zQNPQz3q}People are content with the current rule.");
                    return textObject.ToString();
                }

                if (this._factionInfo.CanRevolt)
                {
                    TextObject inspiration = new TextObject("");
                    if (this._factionInfo.SuccessfullyRevolted)
                    {
                        //no idea why that's the case, but it is O_O
                        //TODO find a cause of this?
                        if (this._factionInfo.RevoltedSettlement == null)
                        {
                            inspiration = new TextObject("{=qZS0ma0z}Many are inspired by tales of revolts in the kingdom.");
                        }
                        else
                        {
                            inspiration = new TextObject("{=7LzQWiDZ}Many are inspired by events at {SETTLEMENT}.");
                            inspiration.SetTextVariable("SETTLEMENT", this._factionInfo.RevoltedSettlement.Name);
                        }
                    }

                    TextObject baseText = new TextObject("{=HWiDqN8d}Some talk of raising banners of their homeland.");
                    return baseText.ToString() + " " + inspiration.ToString();

                }
                else
                {
                    if (this._factionInfo.RevoltedSettlement == null)
                    {
                        return " ";
                    }

                    if (this._factionInfo.RevoltedSettlementId == this._settlementInfo.Settlement.StringId)
                    {
                        TextObject option = new TextObject("{=q2tbH41e}The people of this town had revolted recently, and don't wish to spill blood again.");
                        return option.ToString();
                    }

                    TextObject textObject = new TextObject("{=6m7Ss8fW}After hearing of blood spilled in {SETTLEMENT} citizens are afraid of revolting.");
                    textObject.SetTextVariable("SETTLEMENT", this._factionInfo.RevoltedSettlement.Name);

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

        private void RefreshProperties()
        {
            this.OnPropertyChanged("TownDescription");
            this.OnPropertyChanged("RevolutionProgress");
            this.OnPropertyChanged("TownOwnership");
            this.OnPropertyChanged("FactionVisual");
            this.OnPropertyChanged("FactionVisual");
        }
    }
}
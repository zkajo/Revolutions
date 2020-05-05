using Revolutions.Components.Factions;
using Revolutions.Components.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Revolutions.Screens.ViewModels
{
    public class TownRevolutionViewModel : ViewModel
    {
        private readonly SettlementInfoRevolutions SettlementInfo;
        private readonly FactionInfoRevolutions FactionInfo;

        public TownRevolutionViewModel(SettlementInfoRevolutions settlementInfo, FactionInfoRevolutions factionInfo)
        {
            this.SettlementInfo = settlementInfo;
            this.FactionInfo = factionInfo;
            this.FactionVisual = new ImageIdentifierVM(BannerCode.CreateFrom(this.SettlementInfo.LoyalFaction.Banner), true);
        }

        [DataSourceProperty]
        public string DoneDesc
        {
            get { return new TextObject("{=3fQwWiDZ}Done").ToString(); }
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
                    this._factionVisual = value;
                    base.OnPropertyChanged("FactionVisual");
                }
            }
        }

        [DataSourceProperty]
        public string TownDescription
        {
            get
            {
                if (this.SettlementInfo.RevolutionProgress < 10)
                {
                    var textObject = new TextObject("{=3fBkqk4u}The people of {SETTLEMENT} seem to be content.");
                    textObject.SetTextVariable("SETTLEMENT", this.SettlementInfo.Settlement.Name);

                    return textObject.ToString();
                }
                else
                {
                    var textObject = new TextObject("{=dRoS0zTD}Flames of revolution are slowly stirring in {SETTLEMENT}.");
                    textObject.SetTextVariable("SETTLEMENT", this.SettlementInfo.Settlement.Name);

                    return textObject.ToString();
                }
            }
        }

        [DataSourceProperty]
        public string TownOwnership
        {
            get
            {
                var textObject = new TextObject("{=MYu8szGz}Population is loyal to {FACTION}.");
                textObject.SetTextVariable("FACTION", this.SettlementInfo.LoyalFaction.Name);

                return textObject.ToString();
            }
        }

        [DataSourceProperty]
        public string RevolutionProgress
        {
            get
            {
                var textObject = new TextObject("{=q2tbSs8d}Current revolt progress is {PROGRESS}%.");
                textObject.SetTextVariable("PROGRESS", this.SettlementInfo.RevolutionProgress);

                return textObject.ToString();
            }
        }

        [DataSourceProperty]
        public string RevoltMood
        {
            get
            {
                if (this.FactionInfo.FactionId == this.SettlementInfo.LoyalFaction.StringId)
                {
                    var textObject = new TextObject("{=zQNPQz3q}People are content with the current rule.");
                    return textObject.ToString();
                }

                if (this.FactionInfo.CanRevolt)
                {
                    var inspiration = new TextObject("");
                    if (this.FactionInfo.SuccessfullyRevolted)
                    {
                        if (this.FactionInfo.RevoltedSettlement == null)
                        {
                            inspiration = new TextObject("{=qZS0ma0z}Many are inspired by tales of revolts in the kingdom.");
                        }
                        else
                        {
                            inspiration = new TextObject("{=7LzQWiDZ}Many are inspired by events at {SETTLEMENT}.");
                            inspiration.SetTextVariable("SETTLEMENT", this.FactionInfo.RevoltedSettlement.Name);
                        }
                    }

                    var baseText = new TextObject("{=HWiDqN8d}Some talk of raising banners of their homeland.");
                    return baseText.ToString() + " " + inspiration.ToString();

                }
                else
                {
                    if (this.FactionInfo.RevoltedSettlement == null)
                    {
                        return string.Empty;
                    }

                    if (this.FactionInfo.RevoltedSettlementId == this.SettlementInfo.Settlement.StringId)
                    {
                        var option = new TextObject("{=q2tbH41e}The people of this town had revolted recently, and don't wish to spill blood again.");
                        return option.ToString();
                    }

                    var textObject = new TextObject("{=6m7Ss8fW}After hearing of blood spilled in {SETTLEMENT} citizens are afraid of revolting.");
                    textObject.SetTextVariable("SETTLEMENT", this.FactionInfo.RevoltedSettlement.Name);

                    return textObject.ToString();
                }
            }
        }

        private void ExitTownPropertyMenu()
        {
            ScreenManager.PopScreen();
        }

        private void RefreshProperties()
        {
            this.OnPropertyChanged("FactionVisual");
            this.OnPropertyChanged("TownDescription");
            this.OnPropertyChanged("TownOwnership");
            this.OnPropertyChanged("RevolutionProgress");
            this.OnPropertyChanged("RevoltMood");
        }
    }
}
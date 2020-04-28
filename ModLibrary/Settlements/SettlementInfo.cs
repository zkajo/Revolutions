using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace ModLibrary.Settlements
{
    [Serializable]
    public class SettlementInfo
    {
        public SettlementInfo(Settlement settlement)
        {
            this.Settlement = settlement;
            this.OriginalCulture = settlement.Culture;
            this.OriginalFaction = settlement.MapFaction;
            this.CurrentFaction = settlement.MapFaction;
            this.PreviousFaction = settlement.MapFaction;
        }

        public string SettlementId => this.Settlement.StringId;

        public string OriginalCultureId => this.OriginalCulture.StringId;

        public string OriginalFactionId => this.OriginalFaction.StringId;

        public string CurrentFactionId => this.CurrentFaction.StringId;

        public string PreviousFactionId => this.PreviousFaction.StringId;

        public Settlement Settlement { get; private set; }

        public CultureObject OriginalCulture
        {
            get
            {
                return Game.Current.ObjectManager.GetObject<CultureObject>(this.OriginalCultureId);
            }
            private set
            {
                this.OriginalCulture = value;
            }
        }

        public IFaction OriginalFaction
        {
            get
            {
                if (this.OriginalFactionId.ToLower().Contains("player"))
                {
                    return Hero.MainHero.MapFaction;
                }

                foreach (IFaction faction in Campaign.Current.Factions)
                {
                    if (faction.StringId == this.OriginalFactionId)
                    {
                        return faction;
                    }
                }

                foreach (IFaction faction in Campaign.Current.Factions)
                {
                    if (faction.Culture.StringId == this.OriginalCultureId)
                    {
                        return faction;
                    }
                }

                return null;
            }

            private set
            {
                this.OriginalFaction = value;
            }
        }

        public IFaction CurrentFaction
        {
            get
            {
                return this.Settlement.MapFaction;
            }
            set
            {
                this.CurrentFaction = value;
            }
        }

        public IFaction PreviousFaction { get; set; }
    }
}
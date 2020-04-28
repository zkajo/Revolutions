using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace Revolutions.CampaignBehaviours
{
    public class SettlementInfo
    {
        public SettlementInfo(Settlement settlement)
        {
            this._settlementId = settlement.StringId;
            this._originalFactionId = settlement.MapFaction.StringId;
            this._originalCultureId = settlement.Culture.StringId;
        }

        public IFaction OriginalFaction => this.GetOriginalFaction();

        public CultureObject OriginalCulture => this.GetOriginalCulture();

        public Settlement Settlement => this.GetSettlement();

        public IFaction CurrentFaction => this.Settlement.MapFaction;

        public string GetId()
        {
            return this._settlementId;
        }

        public bool IsOfCulture(string cultureStringId)
        {
            if (this._originalCultureId == cultureStringId)
            {
                return true;
            }

            return false;
        }

        private Settlement GetSettlement()
        {
            return Settlement.Find(this._settlementId);
        }

        public bool OriginalOwnerIsOfImperialCulture()
        {
            if (this.GetOriginalFaction().Culture.Name.Contains("Empire"))
            {
                return true;
            }

            return false;
        }

        public bool OwnerIsOfImperialCulture()
        {
            if (this.GetSettlement().MapFaction.Culture.Name.Contains("Empire"))
            {
                return true;
            }

            return false;
        }

        private IFaction GetOriginalFaction()
        {
            if (this._originalFactionId.Contains("Player") || this._originalFactionId.Contains("player"))
            {
                return Hero.MainHero.MapFaction;
            }

            foreach (var faction in Campaign.Current.Factions)
            {
                if (faction.StringId == this._originalFactionId)
                {
                    return faction;
                }
            }

            foreach (var faction in Campaign.Current.Factions)
            {
                if (faction.Culture.StringId == this._originalCultureId)
                {
                    return faction;
                }
            }

            return null;
        }

        private CultureObject GetOriginalCulture()
        {
            return Game.Current.ObjectManager.GetObject<CultureObject>(this._originalCultureId);
        }

        public void UpdateOwnership()
        {
            if (this.OriginalFaction.StringId == this.CurrentFaction.StringId)
            {
                return;
            }

            if (this.daysOwnedByOwner >= ModOptions.OptionsData.DaysUntilLoyaltyChange)
            {
                this._originalFactionId = this.CurrentFaction.StringId;
            }

            this.daysOwnedByOwner++;
        }

        public void ResetOwnership()
        {
            this.RevoltProgress = 0;
            this.daysOwnedByOwner = 0;
        }

        [SaveableField(1)]
        private readonly string _settlementId;

        [SaveableField(2)]
        private string _originalFactionId;

        [SaveableField(3)]
        private readonly string _originalCultureId;

        [SaveableField(4)]
        public float RevoltProgress = 0;

        [SaveableField(5)]
        private int daysOwnedByOwner = 0;
    }
}
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace Revolutions.CampaignBehaviours
{
    public class SettlementInfo
    {
        public SettlementInfo(Settlement settlement)
        {
            _settlementId = settlement.StringId;
            _originalFactionId = settlement.MapFaction.StringId;
            _originalCultureId = settlement.Culture.StringId;
        }
        
        public IFaction OriginalFaction => GetOriginalFaction();
        public CultureObject OriginalCulture => GetOriginalCulture();
        public Settlement Settlement => GetSettlement();
        public IFaction CurrentFaction => Settlement.MapFaction;

        public string GetId()
        {
            return _settlementId;
        }

        public bool IsOfCulture(string cultureStringId)
        {
            if (_originalCultureId == cultureStringId)
            {
                return true;
            }

            return false;
        }

        private Settlement GetSettlement()
        {
            return Settlement.Find(_settlementId);
        }

        public bool OriginalOwnerIsOfImperialCulture()
        {
            if (GetOriginalFaction().Culture.Name.Contains("Empire"))
            {
                return true;
            }

            return false;
        }

        public bool OwnerIsOfImperialCulture()
        {
            if (GetSettlement().MapFaction.Culture.Name.Contains("Empire"))
            {
                return true;
            }

            return false;
        }

        private IFaction GetOriginalFaction()
        {
            if (_originalFactionId.Contains("Player") || _originalFactionId.Contains("player"))
            {
                return Hero.MainHero.MapFaction;
            }
            
            foreach (var faction in Campaign.Current.Factions)
            {
                if (faction.StringId == _originalFactionId)
                {
                    return faction;
                }
            }

            foreach (var faction in Campaign.Current.Factions)
            {
                if (faction.Culture.StringId == _originalCultureId)
                {
                    return faction;
                }
            }

            return null;
        }

        private CultureObject GetOriginalCulture()
        {
            return Game.Current.ObjectManager.GetObject<CultureObject>(_originalCultureId);
        }

        public void UpdateOwnership()
        {
            if (OriginalFaction.StringId == CurrentFaction.StringId)
            {
                return;
            }

            if (daysOwnedByOwner >= ModOptions.OptionsData.DaysUntilLoyaltyChange)
            {
                _originalFactionId = CurrentFaction.StringId;
            }

            daysOwnedByOwner++;
        }

        public void ResetOwnership()
        {
            RevoltProgress = 0;
            daysOwnedByOwner = 0;
        }
        
        [SaveableField(1)] private string _settlementId;
        [SaveableField(2)] private string _originalFactionId;
        [SaveableField(3)] private string _originalCultureId;
        [SaveableField(4)] public float RevoltProgress = 0;
        [SaveableField(5)] private int daysOwnedByOwner = 0;
    }
}
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

        public Settlement GetSettlement()
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

        public IFaction GetOriginalFaction()
        {
            foreach (var faction in Campaign.Current.Factions)
            {
                if (faction.StringId == _originalFactionId)
                {
                    return faction;
                }
            }

            return null;
        }

        public CultureObject GetOriginalCulture()
        {
            return Game.Current.ObjectManager.GetObject<CultureObject>(_originalCultureId);
        }
        
        [SaveableField(1)] private string _settlementId;
        [SaveableField(2)] private string _originalFactionId;
        [SaveableField(3)] private string _originalCultureId;
        [SaveableField(4)] public float RevoltProgress = 0;
    }
}
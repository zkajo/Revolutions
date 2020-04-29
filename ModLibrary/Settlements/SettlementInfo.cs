using System;
using TaleWorlds.CampaignSystem;

namespace ModLibrary.Settlements
{
    [Serializable]
    public class SettlementInfo
    {
        public SettlementInfo()
        {

        }

        public SettlementInfo(Settlement settlement)
        {
            this.SettlementId = settlement.StringId;
            this.InitialCultureId = settlement.Culture.StringId;
            this.InitialFactionId = settlement.MapFaction.StringId;
            this.OriginalFactionId = settlement.MapFaction.StringId;
            this.CurrentFactionId = settlement.MapFaction.StringId;
            this.PreviousFactionId = settlement.MapFaction.StringId;
        }

        public string SettlementId { get; set; }

        public string InitialCultureId { get; set; }

        public string InitialFactionId { get; set; }

        public string OriginalFactionId { get; set; }

        public string CurrentFactionId { get; set; }

        public string PreviousFactionId { get; set; }

        public void ChangeOwner(Hero oldOwner, Hero newOwner)
        {
            CurrentFactionId = newOwner.Clan.MapFaction.StringId;
            PreviousFactionId = oldOwner.Clan.MapFaction.StringId;
        }
    }
}
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

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
            this.CurrentFactionId = settlement.MapFaction.StringId;
            this.PreviousFactionId = settlement.MapFaction.StringId;
        }

        public string SettlementId { get; set; }

        public string InitialCultureId { get; set; }

        public string InitialFactionId { get; set; }

        public string CurrentFactionId { get; set; }

        public string PreviousFactionId { get; set; }

        public Settlement Settlement => ModLibraryManagers.SettlementManager.GetSettlement(this.SettlementId);

        public CultureObject InitialCulture => Game.Current.ObjectManager.GetObject<CultureObject>(this.InitialCultureId);

        public IFaction InitialFaction => ModLibraryManagers.FactionManager.GetFaction(this.InitialFactionId);

        public IFaction CurrentFaction => ModLibraryManagers.FactionManager.GetFaction(this.CurrentFactionId);

        public IFaction PreviousFaction => ModLibraryManagers.FactionManager.GetFaction(this.PreviousFactionId);

        public bool IsInitialFactionOfImperialCulture => ModLibraryManagers.FactionManager.GetFaction(this.InitialFactionId).Name.ToLower().Contains("empire");

        public bool IsCurrentFactionOfImperialCulture => ModLibraryManagers.FactionManager.GetFaction(this.CurrentFactionId).Name.ToLower().Contains("empire");

        public bool IsPreviousFactionOfImperialCulture => ModLibraryManagers.FactionManager.GetFaction(this.PreviousFactionId).Name.ToLower().Contains("empire");
    }
}
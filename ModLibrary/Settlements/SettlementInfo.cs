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
    }
}
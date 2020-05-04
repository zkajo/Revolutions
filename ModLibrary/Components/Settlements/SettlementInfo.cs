using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using ModLibrary.Components.Factions;

namespace ModLibrary.Components.Settlements
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

        #region Reference Properties

        public string SettlementId { get; set; }

        public string InitialCultureId { get; set; }

        public string InitialFactionId { get; set; }

        public string CurrentFactionId { get; set; }

        public string PreviousFactionId { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties

        public Settlement Settlement => ModLibraryManagers.SettlementManager.GetObjectById(this.SettlementId);

        public CultureObject InitialCulture => Game.Current.ObjectManager.GetObject<CultureObject>(this.InitialCultureId);

        public IFaction InitialFaction => ModLibraryManagers.FactionManager.GetObjectById(this.InitialFactionId);

        public FactionInfo InitialFactionInfo => ModLibraryManagers.FactionManager.GetInfoById(this.InitialFactionId);

        public IFaction CurrentFaction => ModLibraryManagers.FactionManager.GetObjectById(this.CurrentFactionId);

        public FactionInfo CurrentFactionInfo => ModLibraryManagers.FactionManager.GetInfoById(this.CurrentFactionId);

        public IFaction PreviousFaction => ModLibraryManagers.FactionManager.GetObjectById(this.PreviousFactionId);

        public FactionInfo PreviousFactionInfo => ModLibraryManagers.FactionManager.GetInfoById(this.PreviousFactionId);

        #endregion

        public PartyBase Garrision => this.Settlement.Parties?.FirstOrDefault(party => party.IsGarrison)?.Party;

        public PartyBase Militia => this.Settlement.Parties?.FirstOrDefault(party => party.IsMilitia)?.Party;

        public bool IsOfImperialCulture => this.Settlement.Culture.Name.ToLower().Contains("empire");

        public bool IsInitialFactionOfImperialCulture => ModLibraryManagers.FactionManager.GetObjectById(this.InitialFactionId).Name.ToLower().Contains("empire");

        public bool IsCurrentFactionOfImperialCulture => ModLibraryManagers.FactionManager.GetObjectById(this.CurrentFactionId).Name.ToLower().Contains("empire");

        public bool IsPreviousFactionOfImperialCulture => ModLibraryManagers.FactionManager.GetObjectById(this.PreviousFactionId).Name.ToLower().Contains("empire");

        #endregion

        #region Normal Properties



        #endregion
    }
}
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using ModLibrary.Components.Factions;

namespace ModLibrary.Components.Settlements
{
    [Serializable]
    public class SettlementInfo : IGameComponent<SettlementInfo>
    {
        #region IGameComponent<InfoType>

        public bool Equals(SettlementInfo other)
        {
            return this.SettlementId == other.SettlementId;
        }

        public override bool Equals(object other)
        {
            if (other is SettlementInfo settlementInfo)
            {
                return this.SettlementId == settlementInfo.SettlementId;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.SettlementId.GetHashCode();
        }

        #endregion

        public SettlementInfo()
        {

        }

        public SettlementInfo(Settlement settlement)
        {
            this.SettlementId = settlement.Id.InternalValue;
            this.InitialCultureId = settlement.Culture.StringId;
            this.InitialFactionId = settlement.MapFaction.StringId;
            this.CurrentFactionId = settlement.MapFaction.StringId;
            this.PreviousFactionId = settlement.MapFaction.StringId;
        }

        #region Reference Properties

        public uint SettlementId { get; set; }

        public string InitialCultureId { get; set; }

        public string InitialFactionId { get; set; }

        public string CurrentFactionId { get; set; }

        public string PreviousFactionId { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties

        public Settlement Settlement => ModLibraryManagers.SettlementManager.GetGameObject(this.SettlementId);

        public CultureObject InitialCulture => Game.Current.ObjectManager.GetObject<CultureObject>(this.InitialCultureId);

        public IFaction InitialFaction => ModLibraryManagers.FactionManager.GetGameObject(this.InitialFactionId);

        public FactionInfo InitialFactionInfo => ModLibraryManagers.FactionManager.GetInfo(this.InitialFactionId);

        public IFaction CurrentFaction => ModLibraryManagers.FactionManager.GetGameObject(this.CurrentFactionId);

        public FactionInfo CurrentFactionInfo => ModLibraryManagers.FactionManager.GetInfo(this.CurrentFactionId);

        public IFaction PreviousFaction => ModLibraryManagers.FactionManager.GetGameObject(this.PreviousFactionId);

        public FactionInfo PreviousFactionInfo => ModLibraryManagers.FactionManager.GetInfo(this.PreviousFactionId);

        #endregion

        public PartyBase Garrision => this.Settlement.Parties?.FirstOrDefault(party => party.IsGarrison)?.Party;

        public PartyBase Militia => this.Settlement.Parties?.FirstOrDefault(party => party.IsMilitia)?.Party;

        public bool IsOfImperialCulture => this.Settlement.Culture.Name.ToLower().Contains("empire");

        public bool IsInitialFactionOfImperialCulture => ModLibraryManagers.FactionManager.GetGameObject(this.InitialFactionId).Name.ToLower().Contains("empire");

        public bool IsCurrentFactionOfImperialCulture => ModLibraryManagers.FactionManager.GetGameObject(this.CurrentFactionId).Name.ToLower().Contains("empire");

        public bool IsPreviousFactionOfImperialCulture => ModLibraryManagers.FactionManager.GetGameObject(this.PreviousFactionId).Name.ToLower().Contains("empire");

        #endregion

        #region Normal Properties



        #endregion
    }
}
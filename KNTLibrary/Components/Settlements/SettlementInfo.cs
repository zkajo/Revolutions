using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using KNTLibrary.Components.Factions;

namespace KNTLibrary.Components.Settlements
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

        public Settlement Settlement => LibraryManagers.SettlementManager.GetGameObject(this.SettlementId);

        public CultureObject InitialCulture => Game.Current.ObjectManager.GetObject<CultureObject>(this.InitialCultureId);

        public IFaction InitialFaction => LibraryManagers.FactionManager.GetGameObject(this.InitialFactionId);

        public FactionInfo InitialFactionInfo => LibraryManagers.FactionManager.GetInfo(this.InitialFactionId);

        public IFaction CurrentFaction => LibraryManagers.FactionManager.GetGameObject(this.CurrentFactionId);

        public FactionInfo CurrentFactionInfo => LibraryManagers.FactionManager.GetInfo(this.CurrentFactionId);

        public IFaction PreviousFaction => LibraryManagers.FactionManager.GetGameObject(this.PreviousFactionId);

        public FactionInfo PreviousFactionInfo => LibraryManagers.FactionManager.GetInfo(this.PreviousFactionId);

        #endregion

        public PartyBase Garrision => this.Settlement.Parties?.FirstOrDefault(party => party.IsGarrison)?.Party;

        public PartyBase Militia => this.Settlement.Parties?.FirstOrDefault(party => party.IsMilitia)?.Party;

        public bool IsOfImperialCulture => this.Settlement.Culture.Name.ToString().ToLower().Contains("empire");

        public bool IsInitialFactionOfImperialCulture => LibraryManagers.FactionManager.GetGameObject(this.InitialFactionId).Name.ToString().ToLower().Contains("empire");

        public bool IsCurrentFactionOfImperialCulture => LibraryManagers.FactionManager.GetGameObject(this.CurrentFactionId).Name.ToString().ToLower().Contains("empire");

        public bool IsPreviousFactionOfImperialCulture => LibraryManagers.FactionManager.GetGameObject(this.PreviousFactionId).Name.ToString().ToLower().Contains("empire");

        #endregion

        #region Normal Properties



        #endregion
    }
}
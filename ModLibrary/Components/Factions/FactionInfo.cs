using System;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace ModLibrary.Components.Factions
{
    [Serializable]
    public class FactionInfo : IGameComponent<FactionInfo>
    {
        #region IGameComponent<InfoType>

        public bool Equals(FactionInfo other)
        {
            return this.FactionId == other.FactionId;
        }

        public override bool Equals(object other)
        {
            if (other is FactionInfo factionInfo)
            {
                return this.FactionId == factionInfo.FactionId;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.FactionId.GetHashCode();
        }

        #endregion

        public FactionInfo()
        {

        }

        public FactionInfo(IFaction faction)
        {
            this.FactionId = faction.StringId;
            this.InitialTownsCount = this.CurrentTownsCount;
        }


        #region Reference Properties

        public string FactionId { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties

        public IFaction Faction => ModLibraryManagers.FactionManager.GetGameObject(this.FactionId);

        #endregion

        public int CurrentTownsCount => this.Faction.Settlements.Where(settlement => settlement.IsTown).Count();

        #endregion

        #region Normal Properties

        public int InitialTownsCount { get; set; }

        #endregion
    }
}
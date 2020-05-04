using System;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace ModLibrary.Components.Factions
{
    [Serializable]
    public class FactionInfo
    {
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

        public IFaction Faction => ModLibraryManagers.FactionManager.GetObjectById(this.FactionId);

        #endregion

        public int CurrentTownsCount => this.Faction.Settlements.Where(settlement => settlement.IsTown).Count();

        #endregion

        #region Normal Properties

        public int InitialTownsCount { get; set; }

        #endregion
    }
}
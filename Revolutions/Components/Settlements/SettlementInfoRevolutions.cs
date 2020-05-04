using System;
using TaleWorlds.CampaignSystem;
using ModLibrary;
using ModLibrary.Components.Factions;
using ModLibrary.Components.Settlements;
using Revolutions.Components.Factions;

namespace Revolutions.Components.Settlements
{
    [Serializable]
    public class SettlementInfoRevolutions : SettlementInfo
    {
        public SettlementInfoRevolutions() : base()
        {
            this.LoyalFactionId = InitialFactionId;
        }

        public SettlementInfoRevolutions(Settlement settlement) : base(settlement)
        {
            this.LoyalFactionId = InitialFactionId;
        }

        #region Reference Properties

        public string LoyalFactionId { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties

        public IFaction LoyalFaction => ModLibraryManagers.FactionManager.GetObjectById(this.LoyalFactionId);

        public FactionInfo LoyalFactionInfo => ModLibraryManagers.FactionManager.GetInfoById(this.LoyalFactionId);

        public FactionInfoRevolutions LoyalFactionInfoRevolutions => RevolutionsManagers.FactionManager.GetInfoById(this.LoyalFactionId);

        #endregion

        #region Reference Properties Inherited

        public FactionInfoRevolutions InitialFactionInfoRevolutions => RevolutionsManagers.FactionManager.GetInfoById(this.InitialFactionId);

        public FactionInfoRevolutions CurrentFactionInfoRevolutions => RevolutionsManagers.FactionManager.GetInfoById(this.CurrentFactionId);

        public FactionInfoRevolutions PreviousFactionInfoRevolutions => RevolutionsManagers.FactionManager.GetInfoById(this.PreviousFactionId);

        #endregion

        public bool IsLoyalFactionOfImperialCulture => RevolutionsManagers.FactionManager.GetObjectById(this.LoyalFactionId).Name.ToLower().Contains("empire");

        #endregion

        #region Normal Properties

        public bool IsOwnerInSettlement { get; set; } = false;

        public float RevolutionProgress { get; set; } = 0;

        public bool HasRebellionEvent { get; set; } = false;

        public int DaysOwnedByOwner { get; set; } = 0;

        #endregion
    }
}
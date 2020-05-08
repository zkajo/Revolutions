using System;
using TaleWorlds.CampaignSystem;
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
            this.LoyalFactionId = this.InitialFactionId;
        }

        public SettlementInfoRevolutions(Settlement settlement) : base(settlement)
        {
            this.LoyalFactionId = this.InitialFactionId;
        }

        #region Reference Properties

        public string LoyalFactionId { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties

        public IFaction LoyalFaction => RevolutionsManagers.FactionManager.GetGameObject(this.LoyalFactionId);

        public FactionInfo LoyalFactionInfo => RevolutionsManagers.FactionManager.GetInfo(this.LoyalFactionId);

        public FactionInfoRevolutions LoyalFactionInfoRevolutions => RevolutionsManagers.FactionManager.GetInfo(this.LoyalFactionId);

        #endregion

        #region Reference Properties Inherited

        public FactionInfoRevolutions InitialFactionInfoRevolutions => RevolutionsManagers.FactionManager.GetInfo(this.InitialFactionId);

        public FactionInfoRevolutions CurrentFactionInfoRevolutions => RevolutionsManagers.FactionManager.GetInfo(this.CurrentFactionId);

        public FactionInfoRevolutions PreviousFactionInfoRevolutions => RevolutionsManagers.FactionManager.GetInfo(this.PreviousFactionId);

        #endregion

        public bool IsLoyalFactionOfImperialCulture => RevolutionsManagers.FactionManager.GetGameObject(this.LoyalFactionId).Name.ToString().ToLower().Contains("empire");

        #endregion

        #region Normal Properties

        public bool IsOwnerInSettlement { get; set; } = false;

        public float RevolutionProgress { get; set; } = 0;

        public bool HasRebellionEvent { get; set; } = false;

        public int DaysOwnedByOwner { get; set; } = 0;

        #endregion
    }
}
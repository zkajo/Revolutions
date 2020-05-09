using System;
using TaleWorlds.CampaignSystem;
using KNTLibrary.Components.Factions;
using KNTLibrary.Components.Settlements;
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

        public FactionInfo LoyalFactionInfo => RevolutionsManagers.FactionManager.GetInfo(this.LoyalFaction);

        public FactionInfoRevolutions LoyalFactionInfoRevolutions => RevolutionsManagers.FactionManager.GetInfo(this.LoyalFaction);

        #endregion

        #region Reference Properties Inherited

        public FactionInfoRevolutions InitialFactionInfoRevolutions => RevolutionsManagers.FactionManager.GetInfo(this.InitialFaction);

        public FactionInfoRevolutions CurrentFactionInfoRevolutions => RevolutionsManagers.FactionManager.GetInfo(this.CurrentFaction);

        public FactionInfoRevolutions PreviousFactionInfoRevolutions => RevolutionsManagers.FactionManager.GetInfo(this.PreviousFaction);

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
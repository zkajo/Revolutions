using System;
using ModLibrary.Settlements;

namespace Revolutions.Settlements
{
    [Serializable]
    public class SettlementInfoRevolutions : SettlementInfo
    {
        public SettlementInfoRevolutions(TaleWorlds.CampaignSystem.Settlement settlement) : base(settlement)
        {
        }

        public float RevolutionProgress { get; set; } = 0;

        public int DaysOwnedByOwner { get; set; } = 0;
    }
}
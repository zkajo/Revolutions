using System;
using ModLibrary.Settlements;

namespace Revolutions.Settlements
{
    [Serializable]
    public class SettlementInfoRevolutions : SettlementInfo
    {
        public SettlementInfoRevolutions()
        {

        }

        public SettlementInfoRevolutions(SettlementInfo settlementInfo)
        {
            base.SettlementId = settlementInfo.SettlementId;
            base.OriginalCultureId = settlementInfo.OriginalCultureId;
            base.OriginalFactionId = settlementInfo.OriginalFactionId;
            base.CurrentFactionId = settlementInfo.CurrentFactionId;
            base.PreviousFactionId = settlementInfo.PreviousFactionId;
        }

        public float RevolutionProgress { get; set; } = 0;

        public int DaysOwnedByOwner { get; set; } = 0;
    }
}
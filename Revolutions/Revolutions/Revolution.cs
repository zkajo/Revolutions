using System;

namespace Revolutions.Revolutions
{
    [Serializable]
    public class Revolution
    {
        public Revolution()
        {

        }

        public string PartyId { get; set; }

        public string SettlementId { get; set; }
    }
}
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace ModLibrary.Parties
{
    [Serializable]
    public class PartyInfo
    {
        public PartyInfo()
        {
            
        }

        public PartyInfo(PartyBase party)
        {
            partyId = party.Id;
        }
        
        /// <summary>
        /// Id belonging to PartyBase
        /// </summary>
        public string partyId { get; set; }

        public bool Remove { get; set; } = false;
        
        public PartyBase PartyBase => Campaign.Current.Parties.ToList().FirstOrDefault(party => party.Id == partyId);
    }
}
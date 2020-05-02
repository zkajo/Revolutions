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
            this.PartyId = party.Id;
        }

        public string PartyId { get; set; }

        public PartyBase Party => Campaign.Current.Parties.ToList().FirstOrDefault(party => party.Id == this.PartyId);

    }
}
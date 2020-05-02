using System;
using ModLibrary.Parties;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Components.Parties
{
    [Serializable]
    public class PartyInfoRevolutions : PartyInfo
    {
        public PartyInfoRevolutions() : base()
        {

        }

        public PartyInfoRevolutions(PartyBase party) : base(party)
        {

        }
    }
}
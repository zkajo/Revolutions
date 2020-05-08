using System;
using TaleWorlds.CampaignSystem;
using KNTLibrary.Components.Parties;

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

        #region Reference Properties



        #endregion

        #region Virtual Objects

        #region Reference Properties



        #endregion

        #region Reference Properties Inherited



        #endregion



        #endregion

        #region Normal Properties

        public bool CantStarve { get; set; } = false;

        #endregion
    }
}
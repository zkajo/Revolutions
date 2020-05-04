using System;
using TaleWorlds.CampaignSystem;

namespace ModLibrary.Components.Parties
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

        #region Reference Properties

        public string PartyId { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties

        public PartyBase Party => ModLibraryManagers.PartyManager.GetObjectById(this.PartyId);

        #endregion



        #endregion

        #region Normal Properties



        #endregion
    }
}
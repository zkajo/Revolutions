using TaleWorlds.CampaignSystem;

namespace ModLibrary.Components.Clans
{
    public class ClanInfo
    {
        public ClanInfo()
        {

        }

        public ClanInfo(Clan clan)
        {
            this.ClanId = clan.StringId;
        }

        #region Reference Properties

        public string ClanId { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties Objects

        public Clan Clan => ModLibraryManagers.ClanManager.GetObjectById(this.ClanId);

        #endregion



        #endregion

        #region Normal Properties

        public bool CanJoinOtherKingdoms { get; set; } = true;

        #endregion
    }
}
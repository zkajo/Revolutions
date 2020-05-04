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

        #region Saved Properties

        public string ClanId { get; set; }

        #endregion

        #region Saved Properties Objects

        public Clan Clan => ModLibraryManagers.ClanManager.GetObjectById(this.ClanId);

        #endregion

        public bool CanJoinOtherKingdoms { get; set; } = true;
    }
}
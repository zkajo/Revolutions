using TaleWorlds.CampaignSystem;

namespace ModLibrary.Clans
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

        public string ClanId { get; set; }

        public Clan Clan => ModLibraryManagers.ClanManager.GetClan(this.ClanId);

        public bool CanJoinOtherKingdoms { get; set; } = true;

        public bool Remove { get; set; }
    }
}
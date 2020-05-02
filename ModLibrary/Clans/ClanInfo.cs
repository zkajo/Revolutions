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
            StringId = clan.StringId;
        }

        public string StringId { get; set; }

        public bool CanJoinOtherKingdoms { get; set; } = true;

        public bool Remove { get; set; }
    }
}
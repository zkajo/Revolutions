using System;
using TaleWorlds.CampaignSystem;

namespace KNTLibrary.Components.Clans
{
     [Serializable]
    public class ClanInfo : IGameComponent<ClanInfo>
    {
        #region IGameComponent<InfoType>

        public bool Equals(ClanInfo other)
        {
            return this.ClanId == other.ClanId;
        }

        public override bool Equals(object other)
        {
            if (other is ClanInfo clanInfo)
            {
                return this.ClanId == clanInfo.ClanId;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.ClanId.GetHashCode();
        }

        #endregion

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

        public Clan Clan => LibraryManagers.ClanManager.GetGameObject(this.ClanId);

        #endregion



        #endregion

        #region Normal Properties

        public bool CanJoinOtherKingdoms { get; set; } = true;

        #endregion
    }
}
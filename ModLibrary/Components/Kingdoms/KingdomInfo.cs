using System;
using TaleWorlds.CampaignSystem;

namespace ModLibrary.Components.Kingdoms
{
    [Serializable]
    public class KingdomInfo : IGameComponent<KingdomInfo>
    {
        #region IGameComponent<InfoType>

        public bool Equals(KingdomInfo other)
        {
            return this.KingdomId == other.KingdomId;
        }

        public override bool Equals(object other)
        {
            if (other is KingdomInfo kingdomInfo)
            {
                return this.KingdomId == kingdomInfo.KingdomId;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.KingdomId.GetHashCode();
        }

        #endregion

        public KingdomInfo()
        {

        }

        public KingdomInfo(Kingdom kingdom)
        {
            this.KingdomId = kingdom.StringId;
        }

        #region Reference Properties

        public string KingdomId { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties

        public Kingdom Kingdom => ModLibraryManagers.KingdomManager.GetGameObject(this.KingdomId);

        #endregion



        #endregion

        #region Normal Properties

        public bool UserMadeKingdom { get; set; } = false;

        #endregion
    }
}
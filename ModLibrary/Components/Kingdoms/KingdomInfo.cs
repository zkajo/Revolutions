using TaleWorlds.CampaignSystem;

namespace ModLibrary.Components.Kingdoms
{
    public class KingdomInfo
    {
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

        public Kingdom Kingdom => ModLibraryManagers.KingdomManager.GetObjectById(this.KingdomId);

        #endregion



        #endregion

        #region Normal Properties



        #endregion
    }
}
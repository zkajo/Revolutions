using KNTLibrary.Components.Kingdoms;
using System;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Components.Kingdoms
{
    [Serializable]
    public class KingdomInfoRevolutions : KingdomInfo
    {
        public KingdomInfoRevolutions() : base()
        {

        }

        public KingdomInfoRevolutions(Kingdom kingdom) : base(kingdom)
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

        public bool LuckyNation { get; set; } = false;

        #endregion
    }
}
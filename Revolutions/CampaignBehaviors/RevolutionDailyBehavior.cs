using System;
using TaleWorlds.CampaignSystem;

namespace Revolutions.CampaignBehaviors
{
    public class RevolutionDailyBehavior : BaseRevolutionBeavior
    {
        public RevolutionDailyBehavior()
        {
            if (this.SaveDataLoaded)
            {
                return;
            }

            this.LoadSaveData();
        }

        public override void RegisterEvents()
        {
        }

        public override void SyncData(IDataStore dataStore)
        {
        }
    }
}
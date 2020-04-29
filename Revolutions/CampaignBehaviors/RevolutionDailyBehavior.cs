using TaleWorlds.CampaignSystem;

namespace Revolutions.CampaignBehaviors
{
    public class RevolutionDailyBehavior : CampaignBehaviorBase
    {
        private readonly DataStorage DataStorage;

        public RevolutionDailyBehavior(ref DataStorage dataStorage)
        {
            this.DataStorage = dataStorage;
        }

        public override void RegisterEvents()
        {
        }

        public override void SyncData(IDataStore dataStore)
        {
            //REMARK: We won't sync any data here, because the data will be synced inside RevolutionBehavior
        }
    }
}
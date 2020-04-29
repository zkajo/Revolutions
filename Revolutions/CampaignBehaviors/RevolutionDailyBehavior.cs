using TaleWorlds.CampaignSystem;

namespace Revolutions.CampaignBehaviors
{
    public class RevolutionDailyBehavior : CampaignBehaviorBase
    {
        private readonly RevolutionDataStorage RevolutionDataStorage;

        public RevolutionDailyBehavior(ref RevolutionDataStorage revolutionDataStorage)
        {
            this.RevolutionDataStorage = revolutionDataStorage;
        }

        public override void RegisterEvents()
        {
        }

        public override void SyncData(IDataStore dataStore)
        {
        }
    }
}
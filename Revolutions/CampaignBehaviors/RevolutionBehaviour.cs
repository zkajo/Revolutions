using TaleWorlds.CampaignSystem;
using Revolutions.CampaignBehaviors;

namespace Revolutions.CampaignBehaviours
{
    public class RevolutionBehavior : BaseRevolutionBeavior
    {
        public RevolutionBehavior()
        {
            if(this.SaveDataLoaded)
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
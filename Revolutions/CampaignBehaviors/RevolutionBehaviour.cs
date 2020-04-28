using System;
using TaleWorlds.CampaignSystem;
using Revolutions.CampaignBehaviors;

namespace Revolutions.CampaignBehaviours
{
    public class RevolutionBehavior : BaseRevolutionBeavior
    {
        public RevolutionBehavior()
        {
            this.LoadData();
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnBeforeSaveEvent.AddNonSerializedListener(this, new Action(this.OnBeforeSaveEvent));
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        public void OnBeforeSaveEvent()
        {
            this.SaveData();
        }
    }
}
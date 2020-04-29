using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using Revolutions.CampaignBehaviors;

namespace Revolutions.CampaignBehaviours
{
    public class RevolutionBehavior : BaseRevolutionBeavior
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunchedEvent));
        }

        public override void SyncData(IDataStore dataStore)
        {
            if (dataStore.IsLoading)
            {
                dataStore.SyncData("Revolutions.SaveId", ref base.SaveId);
                base.LoadData(base.SaveId);
            }

            if (dataStore.IsSaving)
            {
                if (base.SaveId.IsEmpty())
                {
                    base.SaveId = Guid.NewGuid().ToString();
                }

                dataStore.SyncData("Revolutions.SaveId", ref base.SaveId);
                base.SaveData(base.SaveId);
            }
        }

        private void OnSessionLaunchedEvent(CampaignGameStarter obj)
        {
            if (base.SaveId.IsEmpty())
            {
                base.InitializeData();
            }
        }
    }
}
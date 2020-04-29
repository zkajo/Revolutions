using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Revolutions.CampaignBehaviors
{
    public class RevolutionBehavior : CampaignBehaviorBase
    {
        private readonly RevolutionDataStorage RevolutionDataStorage;

        public RevolutionBehavior(ref RevolutionDataStorage revolutionDataStorage)
        {
            this.RevolutionDataStorage = revolutionDataStorage;
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunchedEvent));
        }

        public override void SyncData(IDataStore dataStore)
        {
            if (dataStore.IsLoading)
            {
                dataStore.SyncData("Revolutions.SaveId", ref this.RevolutionDataStorage.SaveId);
                this.RevolutionDataStorage.LoadData(this.RevolutionDataStorage.SaveId);
            }

            if (dataStore.IsSaving)
            {
                if (this.RevolutionDataStorage.SaveId.IsEmpty())
                {
                    this.RevolutionDataStorage.SaveId = Guid.NewGuid().ToString();
                }

                dataStore.SyncData("Revolutions.SaveId", ref this.RevolutionDataStorage.SaveId);
                this.RevolutionDataStorage.SaveData(this.RevolutionDataStorage.SaveId);
            }
        }

        private void OnSessionLaunchedEvent(CampaignGameStarter obj)
        {
            if (this.RevolutionDataStorage.SaveId.IsEmpty())
            {
                this.RevolutionDataStorage.InitializeData();
            }
        }
    }
}
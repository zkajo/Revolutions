using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Revolutions.CampaignBehaviors
{
    public class RevolutionBehavior : CampaignBehaviorBase
    {
        private readonly DataStorage DataStorage;

        public RevolutionBehavior(ref DataStorage dataStorage)
        {
            this.DataStorage = dataStorage;
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunchedEvent));
        }

        public override void SyncData(IDataStore dataStore)
        {
            if (dataStore.IsLoading)
            {
                dataStore.SyncData("Revolutions.SaveId", ref this.DataStorage.SaveId);
                this.DataStorage.LoadData();
            }

            if (dataStore.IsSaving)
            {
                if (this.DataStorage.SaveId.IsEmpty())
                {
                    this.DataStorage.SaveId = Guid.NewGuid().ToString();
                }

                dataStore.SyncData("Revolutions.SaveId", ref this.DataStorage.SaveId);
                this.DataStorage.SaveData();
            }
        }

        private void OnSessionLaunchedEvent(CampaignGameStarter obj)
        {
            if (this.DataStorage.SaveId.IsEmpty())
            {
                this.DataStorage.InitializeData();
            }
        }
    }
}
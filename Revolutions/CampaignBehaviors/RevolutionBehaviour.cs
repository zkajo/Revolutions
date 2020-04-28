using System;
using System.Collections.Generic;
using ModLibrary.Files;
using TaleWorlds.CampaignSystem;
using Revolutions.CampaignBehaviors;
using Revolutions.Settlements;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace Revolutions.CampaignBehaviours
{
    public class RevolutionBehavior : BaseRevolutionBeavior
    {
        [SaveableField(1)] private string _saveID = string.Empty;
        private int amountOfVisits = 0;
        private int negativeAmountOfVisits = 0;
        
        public override void RegisterEvents()
        {
            CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new  Action<MobileParty, Settlement>(this.Test));
        }

        private void Test(MobileParty mob, Settlement set)
        {
            amountOfVisits++;
            negativeAmountOfVisits--;
        }
        
        public override void SyncData(IDataStore dataStore)
        {
            if (dataStore.IsLoading)
            {
                dataStore.SyncData("_saveID", ref _saveID);
                amountOfVisits = FileManager.Instance.Deserialize<int>(SubModule.ModuleDataPath, _saveID, "amountOfVisits.bin");
                negativeAmountOfVisits = FileManager.Instance.Deserialize<int>(SubModule.ModuleDataPath, _saveID, "negativeAmountOfVisits.bin");
                this.LoadData(_saveID);
            }
            else if (dataStore.IsSaving)
            {
                if (_saveID.IsEmpty())
                {
                    _saveID = DateTime.Now.ToString("dddd dd MMMM yyyy HH mm ss");
                }
                dataStore.SyncData("_saveID", ref _saveID);
                FileManager.Instance.Serialize<int>(amountOfVisits, SubModule.ModuleDataPath, _saveID, "amountOfVisits.bin");
                FileManager.Instance.Serialize<int>(negativeAmountOfVisits, SubModule.ModuleDataPath, _saveID, "negativeAmountOfVisits.bin");
                this.SaveData(_saveID);
            }
        }
    }
}
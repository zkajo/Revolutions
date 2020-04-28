using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using Revolutions.Settlements;
using ModLibrary.Files;

namespace Revolutions.CampaignBehaviors
{
    public  abstract class BaseRevolutionBeavior : CampaignBehaviorBase
    {
        internal List<SettlementInfoRevolutions> SettlementRevolutionInfos = new List<SettlementInfoRevolutions>();
        internal List<Tuple<PartyBase, SettlementInfoRevolutions>> Revolutions = new List<Tuple<PartyBase, SettlementInfoRevolutions>>();

        internal void LoadData(string saveID)
        {
            this.SettlementRevolutionInfos = FileManager.Instance.Deserialize<List<SettlementInfoRevolutions>>(SubModule.ModuleDataPath, saveID, "SettlementRevolutionInfos.bin") ?? this.SettlementRevolutionInfos;
            this.Revolutions = FileManager.Instance.Deserialize<List<Tuple<PartyBase, SettlementInfoRevolutions>>>(SubModule.ModuleDataPath, saveID, "Revolutions.bin") ?? this.Revolutions;
        }

        internal void SaveData(string saveID)
        {
            FileManager.Instance.Serialize<List<SettlementInfoRevolutions>>(this.SettlementRevolutionInfos, SubModule.ModuleDataPath, saveID, "SettlementRevolutionInfos.bin");
            FileManager.Instance.Serialize<List<Tuple<PartyBase, SettlementInfoRevolutions>>>(this.Revolutions, SubModule.ModuleDataPath, saveID, "Revolutions.bin");
        }
    }
}
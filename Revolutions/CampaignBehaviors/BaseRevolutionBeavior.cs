using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using ModLibrary.Files;
using Revolutions.Settlements;

namespace Revolutions.CampaignBehaviors
{
    public  abstract class BaseRevolutionBeavior : CampaignBehaviorBase
    {
        internal List<SettlementInfoRevolutions> SettlementRevolutionInfos = new List<SettlementInfoRevolutions>();
        internal List<Tuple<PartyBase, SettlementInfoRevolutions>> Revolutions = new List<Tuple<PartyBase, SettlementInfoRevolutions>>();

        internal void LoadData()
        {
            this.SettlementRevolutionInfos = FileManager.Instance.Deserialize<List<SettlementInfoRevolutions>>(SubModule.ModuleDataPath, "SettlementRevolutionInfos.bin") ?? this.SettlementRevolutionInfos;
            this.Revolutions = FileManager.Instance.Deserialize<List<Tuple<PartyBase, SettlementInfoRevolutions>>>(SubModule.ModuleDataPath, "Revolutions.bin") ?? this.Revolutions;
        }

        internal void SaveData()
        {
            FileManager.Instance.Serialize(this.SettlementRevolutionInfos, SubModule.ModuleDataPath, "SettlementRevolutionInfos.bin");
            FileManager.Instance.Serialize(this.Revolutions, SubModule.ModuleDataPath, "Revolutions.bin");
        }
    }
}
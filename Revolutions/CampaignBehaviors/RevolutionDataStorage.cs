using System.Linq;
using System.IO;
using System.Collections.Generic;
using TaleWorlds.SaveSystem;
using ModLibrary.Files;
using ModLibrary.Settlements;
using Revolutions.Settlements;

namespace Revolutions.CampaignBehaviors
{
    public class RevolutionDataStorage
    {
        [SaveableField(0)]
        internal string SaveId = string.Empty;

        internal List<SettlementInfoRevolutions> SettlementRevolutionInfos = new List<SettlementInfoRevolutions>();

        internal void InitializeData()
        {
            this.SettlementRevolutionInfos = new List<SettlementInfoRevolutions>(SettlementManager.Instance.InitializeSettlementInfos()
                .Select(settlementInfo => new SettlementInfoRevolutions(settlementInfo)));
        }

        internal void LoadData(string saveId)
        {
            string directoryPath = Path.Combine(SubModule.ModuleDataPath, "Saves", saveId);

            this.SettlementRevolutionInfos = FileManager.Instance.Load<List<SettlementInfoRevolutions>>(directoryPath, "Settlements")
                ?? this.SettlementRevolutionInfos;
        }

        internal void SaveData(string saveId)
        {
            string directoryPath = Path.Combine(SubModule.ModuleDataPath, "Saves", saveId);

            FileManager.Instance.Save(this.SettlementRevolutionInfos, directoryPath, "Settlements");
        }
    }
}
using System.IO;
using System.Collections.Generic;
using TaleWorlds.SaveSystem;
using ModLibrary.Files;
using ModLibrary.Settlements;
using ModLibrary.Factions;
using Revolutions.Settlements;
using Revolutions.Factions;
using Revolutions.Revolutions;

namespace Revolutions
{
    public class DataStorage
    {
        [SaveableField(0)]
        internal string SaveId = string.Empty;

        internal void InitializeData()
        {
            SettlementManager<SettlementInfoRevolutions>.Instance.InitializeSettlementInfos();
            FactionManager<FactionInfoRevolutions>.Instance.InitializeFactionInfos();
        }

        internal void LoadData()
        {
            string directoryPath = Path.Combine(SubModule.ModuleDataPath, "Saves", this.SaveId);

            SettlementManager<SettlementInfoRevolutions>.Instance.SettlementInfos = FileManager.Instance.Load<List<SettlementInfoRevolutions>>(directoryPath, "SettlementInfos");
            FactionManager<FactionInfoRevolutions>.Instance.FactionInfos = FileManager.Instance.Load<List<FactionInfoRevolutions>>(directoryPath, "FactionInfos");
            RevolutionManager.Instance.Revolutions = FileManager.Instance.Load<List<Revolution>>(directoryPath, "Revolutions");
        }

        internal void SaveData()
        {
            string directoryPath = Path.Combine(SubModule.ModuleDataPath, "Saves", this.SaveId);

            FileManager.Instance.Save(SettlementManager<SettlementInfoRevolutions>.Instance.SettlementInfos, directoryPath, "SettlementInfos");
            FileManager.Instance.Save(FactionManager<FactionInfoRevolutions>.Instance.FactionInfos, directoryPath, "FactionInfos");
            FileManager.Instance.Save(RevolutionManager.Instance.Revolutions, directoryPath, "Revolutions");
        }
    }
}
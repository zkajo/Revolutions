using System.IO;
using System.Collections.Generic;
using TaleWorlds.SaveSystem;
using ModLibrary.Files;
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
            RevolutionsManagers.SettlementManager.InitializeSettlementInfos();
            RevolutionsManagers.FactionManager.InitializeFactionInfos();
        }

        internal void LoadData()
        {
            string directoryPath = Path.Combine(SubModule.ModuleDataPath, "Saves", this.SaveId);

            RevolutionsManagers.SettlementManager.SettlementInfos = FileManager.Instance.Load<List<SettlementInfoRevolutions>>(directoryPath, "SettlementInfos");
            RevolutionsManagers.FactionManager.FactionInfos = FileManager.Instance.Load<List<FactionInfoRevolutions>>(directoryPath, "FactionInfos");
            RevolutionManager.Instance.Revolutions = FileManager.Instance.Load<List<Revolution>>(directoryPath, "Revolutions");
        }

        internal void SaveData()
        {
            string directoryPath = Path.Combine(SubModule.ModuleDataPath, "Saves", this.SaveId);

            FileManager.Instance.Save(RevolutionsManagers.SettlementManager.SettlementInfos, directoryPath, "SettlementInfos");
            FileManager.Instance.Save(RevolutionsManagers.FactionManager.FactionInfos, directoryPath, "FactionInfos");
            FileManager.Instance.Save(RevolutionManager.Instance.Revolutions, directoryPath, "Revolutions");
        }
    }
}
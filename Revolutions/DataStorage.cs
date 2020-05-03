using System.IO;
using System.Collections.Generic;
using TaleWorlds.SaveSystem;
using ModLibrary;
using Revolutions.Settlements;
using Revolutions.Revolutions;
using Revolutions.Components.Clans;
using Revolutions.Components.Factions;
using Revolutions.Components.Parties;

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
            RevolutionsManagers.PartyManager.InitializePartyInfos();
            RevolutionsManagers.ClanManager.InitalizeClanInfos();
        }

        internal void LoadData()
        {
            var directoryPath = Path.Combine(SubModule.ModuleDataPath, "Saves", this.SaveId);

            RevolutionsManagers.SettlementManager.SettlementInfos = ModLibraryManagers.FileManager.Load<List<SettlementInfoRevolutions>>(directoryPath, "SettlementInfos");
            RevolutionsManagers.FactionManager.FactionInfos = ModLibraryManagers.FileManager.Load<List<FactionInfoRevolutions>>(directoryPath, "FactionInfos");
            RevolutionsManagers.RevolutionManager.Revolutions = ModLibraryManagers.FileManager.Load<List<Revolution>>(directoryPath, "Revolutions");
            RevolutionsManagers.PartyManager.PartyInfos = ModLibraryManagers.FileManager.Load<List<PartyInfoRevolutions>>(directoryPath, "PartyInfos");
            RevolutionsManagers.ClanManager.ClanInfos = ModLibraryManagers.FileManager.Load<List<ClanInfoRevolutions>>(directoryPath, "ClanInfos");
        }

        internal void SaveData()
        {
            var directoryPath = Path.Combine(SubModule.ModuleDataPath, "Saves", this.SaveId);

            ModLibraryManagers.FileManager.Save(RevolutionsManagers.SettlementManager.SettlementInfos, directoryPath, "SettlementInfos");
            ModLibraryManagers.FileManager.Save(RevolutionsManagers.FactionManager.FactionInfos, directoryPath, "FactionInfos");
            ModLibraryManagers.FileManager.Save(RevolutionsManagers.RevolutionManager.Revolutions, directoryPath, "Revolutions");
            ModLibraryManagers.FileManager.Save(RevolutionsManagers.PartyManager.PartyInfos, directoryPath, "PartyInfos");
            ModLibraryManagers.FileManager.Save(RevolutionsManagers.ClanManager.ClanInfos, directoryPath, "ClanInfos");
        }
    }
}
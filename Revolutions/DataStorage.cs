using System.IO;
using System.Collections.Generic;
using TaleWorlds.SaveSystem;
using Revolutions.Components.Kingdoms;
using Revolutions.Components.Factions;
using Revolutions.Components.Clans;
using Revolutions.Components.Parties;
using Revolutions.Components.Characters;
using Revolutions.Components.Settlements;
using Revolutions.Components.Revolutions;
using KNTLibrary;
using System.Linq;

namespace Revolutions
{
    public class DataStorage
    {
        [SaveableField(0)]
        internal string SaveId = string.Empty;

        internal void InitializeData()
        {
            RevolutionsManagers.KingdomManager.InitializeInfos();
            RevolutionsManagers.FactionManager.InitializeInfos();
            RevolutionsManagers.ClanManager.InitializeInfos();
            RevolutionsManagers.PartyManager.InitializeInfos();
            RevolutionsManagers.CharacterManager.InitializeInfos();
            RevolutionsManagers.SettlementManager.InitializeInfos();
        }

        internal void LoadData()
        {
            var directoryPath = Path.Combine(SubModule.ModuleDataPath, "Saves", this.SaveId);

            RevolutionsManagers.FactionManager.Infos = LibraryManagers.FileManager.Load<List<FactionInfoRevolutions>>(directoryPath, "FactionInfos").ToHashSet();
            RevolutionsManagers.FactionManager.CleanupDuplicatedInfos();

            RevolutionsManagers.KingdomManager.Infos = LibraryManagers.FileManager.Load<List<KingdomInfoRevolutions>>(directoryPath, "KingdomInfos").ToHashSet();
            RevolutionsManagers.KingdomManager.CleanupDuplicatedInfos();

            RevolutionsManagers.ClanManager.Infos = LibraryManagers.FileManager.Load<List<ClanInfoRevolutions>>(directoryPath, "ClanInfos").ToHashSet();
            RevolutionsManagers.ClanManager.CleanupDuplicatedInfos();

            RevolutionsManagers.PartyManager.Infos = LibraryManagers.FileManager.Load<List<PartyInfoRevolutions>>(directoryPath, "PartyInfos").ToHashSet();
            RevolutionsManagers.PartyManager.CleanupDuplicatedInfos();

            RevolutionsManagers.CharacterManager.Infos = LibraryManagers.FileManager.Load<List<CharacterInfoRevolutions>>(directoryPath, "CharacterInfos").ToHashSet();
            RevolutionsManagers.CharacterManager.CleanupDuplicatedInfos();

            RevolutionsManagers.SettlementManager.Infos = LibraryManagers.FileManager.Load<List<SettlementInfoRevolutions>>(directoryPath, "SettlementInfos").ToHashSet();
            RevolutionsManagers.SettlementManager.CleanupDuplicatedInfos();

            RevolutionsManagers.RevolutionManager.Revolutions = LibraryManagers.FileManager.Load<List<Revolution>>(directoryPath, "Revolutions").ToHashSet();
        }

        internal void SaveData()
        {
            var directoryPath = Path.Combine(SubModule.ModuleDataPath, "Saves", this.SaveId);

            LibraryManagers.FileManager.Save(RevolutionsManagers.KingdomManager.Infos, directoryPath, "KingdomInfos");
            LibraryManagers.FileManager.Save(RevolutionsManagers.FactionManager.Infos, directoryPath, "FactionInfos");
            LibraryManagers.FileManager.Save(RevolutionsManagers.ClanManager.Infos, directoryPath, "ClanInfos");
            LibraryManagers.FileManager.Save(RevolutionsManagers.PartyManager.Infos, directoryPath, "PartyInfos");
            LibraryManagers.FileManager.Save(RevolutionsManagers.CharacterManager.Infos, directoryPath, "CharacterInfos");
            LibraryManagers.FileManager.Save(RevolutionsManagers.SettlementManager.Infos, directoryPath, "SettlementInfos");
            LibraryManagers.FileManager.Save(RevolutionsManagers.RevolutionManager.Revolutions, directoryPath, "Revolutions");
        }
    }
}
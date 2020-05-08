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

            RevolutionsManagers.FactionManager.Infos = LibraryManagers.FileManager.Load<HashSet<FactionInfoRevolutions>>(directoryPath, "FactionInfos");
            RevolutionsManagers.KingdomManager.Infos = LibraryManagers.FileManager.Load<HashSet<KingdomInfoRevolutions>>(directoryPath, "KingdomInfos");
            RevolutionsManagers.ClanManager.Infos = LibraryManagers.FileManager.Load<HashSet<ClanInfoRevolutions>>(directoryPath, "ClanInfos");
            RevolutionsManagers.PartyManager.Infos = LibraryManagers.FileManager.Load<HashSet<PartyInfoRevolutions>>(directoryPath, "PartyInfos");
            RevolutionsManagers.CharacterManager.Infos = LibraryManagers.FileManager.Load<HashSet<CharacterInfoRevolutions>>(directoryPath, "CharacterInfos");
            RevolutionsManagers.SettlementManager.Infos = LibraryManagers.FileManager.Load<HashSet<SettlementInfoRevolutions>>(directoryPath, "SettlementInfos");
            RevolutionsManagers.RevolutionManager.Revolutions = LibraryManagers.FileManager.Load<HashSet<Revolution>>(directoryPath, "Revolutions");
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
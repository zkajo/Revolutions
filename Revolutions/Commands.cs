using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Missions;
using TaleWorlds.MountAndBlade.View.Screen;

namespace Revolutions
{
    public static class Commands
    {
        [CommandLineFunctionality.CommandLineArgumentFunction("start_revolution", "revolutions")]
        public static string StartRevolution(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() != 1 || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"revolutions.start_revolution [SettlementName]\".";
            }

            var settlementName = strings[0];

            var settlement = Campaign.Current.Settlements.FirstOrDefault(s => s.Name.ToString().ToLower() == settlementName.ToLower());
            if (settlement == null)
            {
                return $"There is no Settlement \"{settlementName}\".";
            }

            if (settlement.IsUnderSiege)
            {
                return $"{settlement.Name} is under siege.";
            }

            var settlementInfo = RevolutionsManagers.SettlementManager.GetInfoById(settlement.StringId);
            settlementInfo.RevolutionProgress = 100;

            RevolutionsManagers.RevolutionManager.StartRebellionEvent(settlement);

            return $"Started a revolution in {settlement.Name}.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("end_revolution", "revolutions")]
        public static string EndRevolution(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() != 2 || bool.TryParse(strings[1], out var successful) || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"revolutions.end_revolution [SettlementName] [Successful (true|false)]\".";
            }

            var settlementName = strings[0];

            var settlement = Campaign.Current.Settlements.FirstOrDefault(s => s.Name.ToString().ToLower() == settlementName.ToLower());
            if (settlement == null)
            {
                return $"There is no Settlement \"{settlementName}\".";
            }

            var revolution = RevolutionsManagers.RevolutionManager.GetRevolutionBySettlementId(settlement.StringId);
            if (revolution == null)
            {
                return $"{settlementName} is not conflicted in a revolt.";
            }

            if (successful)
            {
                RevolutionsManagers.RevolutionManager.EndSucceededRevoluton(revolution);
            }
            else
            {
                RevolutionsManagers.RevolutionManager.EndFailedRevolution(revolution);
            }

            return $"Ended a {(successful ? "successful" : "failed")} revolution in {settlement.Name}.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("set_loyal_to_player", "revolutions")]
        public static string SetLoyalToPlayer(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() != 1 || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"revolutions.set_loyal_to_player [SettlementName]\".";
            }

            var settlementName = strings[0];

            var settlement = Campaign.Current.Settlements.FirstOrDefault(s => s.Name.ToString().ToLower() == settlementName.ToLower());
            if (settlement == null)
            {
                return $"There is no Settlement \"{settlementName}\".";
            }

            var settlementInfo = RevolutionsManagers.SettlementManager.GetInfoById(settlement.StringId);
            settlementInfo.LoyalFactionId = Hero.MainHero.MapFaction.StringId;

            return $"{settlement.Name} is now loyal to {settlementInfo.LoyalFaction.Name}.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("set_loyal_to_current_owner", "revolutions")]
        public static string SetLoyalToCurrentOwner(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() != 1 || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"revolutions.set_loyal_to_current_owner [SettlementName]\".";
            }

            var settlementName = strings[0];

            var settlement = Campaign.Current.Settlements.FirstOrDefault(s => s.Name.ToString().ToLower() == settlementName.ToLower());
            if (settlement == null)
            {
                return $"There is no Settlement \"{settlementName}\".";
            }

            var settlementInfo = RevolutionsManagers.SettlementManager.GetInfoById(settlement.StringId);
            settlementInfo.LoyalFactionId = settlementInfo.CurrentFactionId;

            return $"{settlement.Name} is now loyal to {settlementInfo.LoyalFaction.Name}.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("set_loyal_to", "revolutions")]
        public static string SetLoyalTo(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() != 2 || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"revolutions.set_loyal_to [SettlementName] [FactionName]\".";
            }

            var settlementName = strings[0];

            var settlement = Campaign.Current.Settlements.FirstOrDefault(s => s.Name.ToString().ToLower() == settlementName.ToLower());
            if (settlement == null)
            {
                return $"There is no Settlement \"{settlementName}\".";
            }

            var factionName = strings[1];

            var faction = Campaign.Current.Factions.FirstOrDefault(s => s.Name.ToString().ToLower() == factionName.ToLower());
            if (settlement == null)
            {
                return $"There is no Faction \"{factionName}\".";
            }

            var settlementInfo = RevolutionsManagers.SettlementManager.GetInfoById(settlement.StringId);
            settlementInfo.LoyalFactionId = faction.StringId;

            return $"{settlement.Name} is now loyal to {settlementInfo.LoyalFaction.Name}.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("set_loyal_owner_to_player", "revolutions")]
        public static string SetLoyalOwnerToPlayer(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() != 1 || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"revolutions.set_loyal_owner_to_player [SettlementName]\".";
            }

            var settlementName = strings[0];

            var settlement = Campaign.Current.Settlements.FirstOrDefault(s => s.Name.ToString().ToLower() == settlementName.ToLower());
            if (settlement == null)
            {
                return $"There is no Settlement \"{settlementName}\".";
            }

            var settlementInfo = RevolutionsManagers.SettlementManager.GetInfoById(settlement.StringId);
            settlementInfo.RevolutionProgress = 0;
            settlementInfo.CurrentFactionId = Hero.MainHero.MapFaction.StringId;
            settlementInfo.LoyalFactionId = Hero.MainHero.MapFaction.StringId;
            settlement.OwnerClan = Hero.MainHero.Clan;

            return $"{settlement.Name} is now owned by and loyal to {Hero.MainHero.Clan.Name} ({settlementInfo.LoyalFaction.Name}).";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("set_loyal_owner_to", "revolutions")]
        public static string SetLoyalOwnerTo(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() != 2 || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"revolutions.set_loyal_owner_to [SettlementName] [ClanName]\".";
            }

            var settlementName = strings[0];

            var settlement = Campaign.Current.Settlements.FirstOrDefault(s => s.Name.ToString().ToLower() == settlementName.ToLower());
            if (settlement == null)
            {
                return $"There is no Settlement \"{settlementName}\".";
            }

            var clanName = strings[1];

            var clan = Campaign.Current.Clans.FirstOrDefault(s => s.Name.ToString().ToLower() == clanName.ToLower());
            if (clan == null)
            {
                return $"There is no Clan \"{clanName}\".";
            }

            var settlementInfo = RevolutionsManagers.SettlementManager.GetInfoById(settlement.StringId);
            settlementInfo.RevolutionProgress = 0;
            settlementInfo.CurrentFactionId = clan.MapFaction.StringId;
            settlementInfo.LoyalFactionId = clan.MapFaction.StringId;
            settlement.OwnerClan = clan;

            return $"{settlement.Name} is now owned by and loyal to {clan.Name} ({settlementInfo.LoyalFaction.Name}).";
        }
	}
}
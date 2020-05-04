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
        [CommandLineFunctionality.CommandLineArgumentFunction("set_loyal", "revolutions")]
        public static string SetLoyal(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() != 1 || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"revolutions.set_loyal [SettlementName]\".";
            }

            var settlementName = strings[0];

            var settlement = Campaign.Current.Settlements.FirstOrDefault(s => s.Name.ToString().ToLower() == settlementName.ToLower());
            if (settlement == null)
            {
                return $"There is no Settlement \"{settlementName}\".";
            }

            var settlementInfo = RevolutionsManagers.SettlementManager.GetInfoById(settlement.StringId);
            settlementInfo.RevolutionProgress = 0;
            settlementInfo.LoyalFactionId = settlementInfo.CurrentFactionId;

            return $"{settlement.Name} is now loyal to {settlementInfo.CurrentFaction.Name}.";
        }

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
            if(settlement == null)
            {
                return $"There is no Settlement \"{settlementName}\".";
            }

            if(settlement.IsUnderSiege)
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

            if(successful)
            {
                RevolutionsManagers.RevolutionManager.EndSucceededRevoluton(revolution);
            }
            else
            {
                RevolutionsManagers.RevolutionManager.EndFailedRevolution(revolution);
            }

            return $"Ended a {(successful ? "successful" : "failed")} revolution in {settlement.Name}.";
        }
	}
}
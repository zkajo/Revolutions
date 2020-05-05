using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Localization;
using Revolutions.Components.Settlements;
using System;
using TaleWorlds.Core;
using ModLibrary;

namespace Revolutions.Models
{
    public class LoyaltyModel : DefaultSettlementLoyaltyModel
    {
        private readonly DataStorage DataStorage;

        public LoyaltyModel(ref DataStorage dataStorage)
        {
            this.DataStorage = dataStorage;
        }

        public override float CalculateLoyaltyChange(Town town, StatExplainer statExplainer = null)
        {
            if (!town.IsTown)
            {
                return base.CalculateLoyaltyChange(town, statExplainer);
            }

            try
            {
                var explainedNumber = new ExplainedNumber(0.0f, statExplainer, null);
                var settlementInfo = RevolutionsManagers.SettlementManager.GetInfoById(town.Settlement?.StringId);

                if (settlementInfo.CurrentFaction.Leader == Hero.MainHero)
                {
                    explainedNumber.Add(Settings.Instance.BasePlayerLoyalty, new TextObject("{=q2tbqP0z}Bannerlord Settlement"));

                    if (Settings.Instance.PlayerAffectedByOverextension && Settings.Instance.OverextensionMechanics)
                    {
                        this.Overextension(settlementInfo, ref explainedNumber);
                    }
                }
                else
                {
                    this.BaseLoyalty(settlementInfo, ref explainedNumber);

                    if (Settings.Instance.OverextensionMechanics)
                    {
                        this.Overextension(settlementInfo, ref explainedNumber);
                    }
                }

                return explainedNumber.ResultNumber + base.CalculateLoyaltyChange(town, statExplainer);
            }
            catch (Exception exception)
            {
                InformationManager.DisplayMessage(new InformationMessage("Revolutions: Failed to calculate loyalty change! Using TaleWorld logic now.", ColorManager.Red));
                InformationManager.DisplayMessage(new InformationMessage($"Exception Details: Town: {town?.Name} | Town.StringId: {town?.StringId} | Town.Settlement: {town?.Settlement?.Name} | Town.Settlement.StringId: {town?.Settlement?.StringId}", ColorManager.Red));
                InformationManager.DisplayMessage(new InformationMessage(exception.ToString(), ColorManager.Red));
                InformationManager.DisplayMessage(new InformationMessage(exception.StackTrace, ColorManager.Red));

                return base.CalculateLoyaltyChange(town, statExplainer);
            }
        }

        private void Overextension(SettlementInfoRevolutions settlementInfo, ref ExplainedNumber explainedNumber)
        {
            if (settlementInfo.CurrentFaction.StringId == settlementInfo.LoyalFaction.StringId)
            {
                return;
            }

            if (Settings.Instance.EmpireLoyaltyMechanics)
            {
                if (settlementInfo.IsOfImperialCulture && settlementInfo.IsCurrentFactionOfImperialCulture)
                {
                    return;
                }
            }

            var factionInfo = settlementInfo.CurrentFactionInfo;
            var overExtension = factionInfo.InitialTownsCount - factionInfo.CurrentTownsCount;

            explainedNumber.Add(overExtension * Settings.Instance.OverExtensionMultiplier, new TextObject("{=YnRmNltF}Overextension"));
        }

        private void BaseLoyalty(SettlementInfoRevolutions settlementInfo, ref ExplainedNumber explainedNumber)
        {
            if (Settings.Instance.EmpireLoyaltyMechanics)
            {
                if (settlementInfo.IsOfImperialCulture)
                {
                    if (settlementInfo.IsCurrentFactionOfImperialCulture)
                    {
                        explainedNumber.Add(10, new TextObject("{=3fQwNP5z}Imperial Loyalty"));
                    }
                    else
                    {
                        explainedNumber.Add(-5, new TextObject("{=7LzQNP0z}Foreign Rule"));
                    }
                }
                else
                {
                    if (settlementInfo.IsCurrentFactionOfImperialCulture)
                    {
                        explainedNumber.Add(-5, new TextObject("{=qNWmNN8d}Imperial Aversion"));
                    }

                    if (settlementInfo.LoyalFactionId != settlementInfo.CurrentFactionId)
                    {
                        explainedNumber.Add(-5, new TextObject("{=7LzQNP0z}Foreign Rule"));
                    }
                }
            }
            else
            {
                if (settlementInfo.LoyalFactionId != settlementInfo.CurrentFactionId)
                {
                    explainedNumber.Add(-5, new TextObject("{=7LzQNP0z}Foreign Rule"));
                }
            }
        }
    }
}
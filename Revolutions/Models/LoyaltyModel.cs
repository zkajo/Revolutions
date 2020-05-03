using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using Revolutions.Settlements;
using TaleWorlds.Localization;

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
            var explainedNumber = new ExplainedNumber(0.0f, statExplainer, null);
            var settlementInfoRevolutions = RevolutionsManagers.SettlementManager.GetSettlementInfo(town.Settlement.StringId);

            if (!town.IsTown)
            {
                return explainedNumber.ResultNumber + base.CalculateLoyaltyChange(town, statExplainer);
            }

            if (settlementInfoRevolutions.CurrentFaction.Leader == Hero.MainHero)
            {
                explainedNumber.Add(Settings.Instance.BasePlayerLoyalty, new TextObject("{=q2tbqP0z}Bannerlord Settlement"));

                if (Settings.Instance.PlayerAffectedByOverextension && Settings.Instance.OverextensionMechanics)
                {
                    this.Overextension(settlementInfoRevolutions, ref explainedNumber);
                }
            }
            else
            {
                this.BaseLoyalty(settlementInfoRevolutions, ref explainedNumber);

                if (Settings.Instance.OverextensionMechanics)
                {
                    this.Overextension(settlementInfoRevolutions, ref explainedNumber);
                }
            }

            return explainedNumber.ResultNumber + base.CalculateLoyaltyChange(town, statExplainer);
        }

        private void Overextension(SettlementInfoRevolutions settlementInfoRevolutions, ref ExplainedNumber explainedNumber)
        {
            if (settlementInfoRevolutions.CurrentFaction.StringId == settlementInfoRevolutions.LoyalFaction.StringId)
            {
                return;
            }

            if (Settings.Instance.EmpireLoyaltyMechanics)
            {
                if (settlementInfoRevolutions.IsOfImperialCulture && settlementInfoRevolutions.IsCurrentFactionOfImperialCulture)
                {
                    return;
                }
            }

            var factionInfo = settlementInfoRevolutions.CurrentFactionInfo;
            var overExtension = factionInfo.InitialTownsCount - factionInfo.CurrentTownsCount;

            explainedNumber.Add(overExtension * Settings.Instance.OverExtensionMultiplier, new TextObject("{=YnRmNltF}Overextension"));
        }

        private void BaseLoyalty(SettlementInfoRevolutions settlementInfoRevolutions, ref ExplainedNumber explainedNumber)
        {
            if (Settings.Instance.EmpireLoyaltyMechanics)
            {
                if (settlementInfoRevolutions.IsOfImperialCulture)
                {
                    if (settlementInfoRevolutions.IsCurrentFactionOfImperialCulture)
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
                    if (settlementInfoRevolutions.IsCurrentFactionOfImperialCulture)
                    {
                        explainedNumber.Add(-5, new TextObject("{=qNWmNN8d}Imperial Aversion"));
                    }

                    if (settlementInfoRevolutions.LoyalFactionId != settlementInfoRevolutions.CurrentFactionId)
                    {
                        explainedNumber.Add(-5, new TextObject("{=7LzQNP0z}Foreign Rule"));
                    }
                }
            }
            else
            {
                if (settlementInfoRevolutions.LoyalFactionId != settlementInfoRevolutions.CurrentFactionId)
                {
                    explainedNumber.Add(-5, new TextObject("{=7LzQNP0z}Foreign Rule"));
                }
            }
        }
    }
}
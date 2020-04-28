using Revolutions.CampaignBehaviours;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Revolutions.Models
{
    public class LoyaltyModel : DefaultSettlementLoyaltyModel
    {
        public Revolution RevolutionBehaviour;
        private readonly float OverExtensionMultiplier = 2.0f;

        private readonly float _basePlayerLoyalty = 5.0f;

        public override float CalculateLoyaltyChange(Town town, StatExplainer explanation = null)
        {
            ExplainedNumber explainedNumber = new ExplainedNumber(0.0f, explanation, (TextObject)null);
            SettlementInfo info = this.RevolutionBehaviour.GetSettlementInformation(town.Settlement);

            if (!town.IsTown)
            {
                return explainedNumber.ResultNumber + base.CalculateLoyaltyChange(town, explanation);
            }

            if (info.Settlement.MapFaction.Leader == Hero.MainHero)
            {
                explainedNumber.Add(this._basePlayerLoyalty, GameTexts.FindText("str_loyalty_bannerlord"));

                if (ModOptions.OptionsData.PlayerAffectedByOverextension && ModOptions.OptionsData.OverextensionMechanics)
                {
                    this.Overextension(info, ref explainedNumber);
                }
            }
            else
            {
                this.BaseLoyalty(info, ref explainedNumber);

                if (ModOptions.OptionsData.OverextensionMechanics)
                {
                    this.Overextension(info, ref explainedNumber);
                }
            }

            return explainedNumber.ResultNumber + base.CalculateLoyaltyChange(town, explanation);
        }

        private void Overextension(SettlementInfo settlement, ref ExplainedNumber explainedNumber)
        {
            if (settlement.CurrentFaction.StringId == settlement.OriginalFaction.StringId)
            {
                return;
            }

            if (ModOptions.OptionsData.EmpireLoyaltyMechanics)
            {
                if (settlement.OriginalOwnerIsOfImperialCulture() && settlement.OwnerIsOfImperialCulture())
                {
                    return;
                }
            }

            int townsAboveInitialStart = this.RevolutionBehaviour.GetFactionInformation(settlement.CurrentFaction).TownsAboveInitial();

            explainedNumber.Add(-townsAboveInitialStart * this.OverExtensionMultiplier, GameTexts.FindText("str_loyalty_overextension"));
        }

        private void BaseLoyalty(SettlementInfo info, ref ExplainedNumber explainedNumber)
        {
            if (ModOptions.OptionsData.EmpireLoyaltyMechanics)
            {
                if (info.OriginalOwnerIsOfImperialCulture())
                {
                    if (info.OwnerIsOfImperialCulture())
                    {
                        explainedNumber.Add(10, GameTexts.FindText("str_loyalty_imperial"));
                    }
                    else
                    {
                        explainedNumber.Add(-5, GameTexts.FindText("str_loyalty_foreignRule"));
                    }
                }
                else
                {
                    if (info.OwnerIsOfImperialCulture())
                    {
                        explainedNumber.Add(-5, GameTexts.FindText("str_loyalty_imperialAvers"));
                    }

                    if (info.OriginalFaction.StringId != info.CurrentFaction.StringId)
                    {
                        explainedNumber.Add(-5, GameTexts.FindText("str_loyalty_foreignRule"));
                    }
                }
            }
            else
            {
                if (info.OriginalFaction.StringId != info.CurrentFaction.StringId)
                {
                    explainedNumber.Add(-5, GameTexts.FindText("str_loyalty_foreignRule"));
                }
            }
        }
    }
}
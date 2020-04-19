using Revolutions.CampaignBehaviours;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Localization;

namespace Revolutions.Models
{
    public class LoyaltyModel : DefaultSettlementLoyaltyModel
    {
        public Revolution RevolutionBehaviour;
        private float OverExtensionMultiplier = 2.0f;

        private float _basePlayerLoyalty = 5.0f;

        public override float CalculateLoyaltyChange(Town town, StatExplainer explanation = null)
        {
            ExplainedNumber explainedNumber = new ExplainedNumber(0.0f, explanation, (TextObject) null);
            SettlementInfo info = RevolutionBehaviour.GetSettlementInformation(town.Settlement);
            
            if (!town.IsTown)
            {
                return explainedNumber.ResultNumber + base.CalculateLoyaltyChange(town, explanation);
            }

            if (info.Settlement.MapFaction.Leader == Hero.MainHero)
            {    
                explainedNumber.Add(_basePlayerLoyalty, new TextObject("Bannerlord Settlement"));

                if (ModOptions.OptionsData.PlayerAffectedByOverextension && ModOptions.OptionsData.OverextensionMechanics)
                {
                    Overextension(info, ref explainedNumber);
                }
            }
            else
            {
                BaseLoyalty(info, ref explainedNumber);

                if (ModOptions.OptionsData.OverextensionMechanics)
                {
                    Overextension(info, ref explainedNumber);
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

            int townsAboveInitialStart = RevolutionBehaviour.GetFactionInformation(settlement.CurrentFaction).TownsAboveInitial();
            
            explainedNumber.Add(-townsAboveInitialStart * OverExtensionMultiplier, new TextObject("Overextension"));
        }
        
        private void BaseLoyalty(SettlementInfo info, ref ExplainedNumber explainedNumber)
        {
            if (ModOptions.OptionsData.EmpireLoyaltyMechanics)
            {
                if (info.OriginalOwnerIsOfImperialCulture())
                {
                    if (info.OwnerIsOfImperialCulture())
                    {
                        explainedNumber.Add(10, new TextObject("Imperial Loyalty"));
                    }
                    else
                    {
                        explainedNumber.Add(-5, new TextObject("Foreign rule"));
                    }
                }
                else
                {
                    if (info.OwnerIsOfImperialCulture())
                    {
                        explainedNumber.Add(-5, new TextObject("Imperial aversion"));
                    }
                    
                    if (info.OriginalFaction.StringId != info.CurrentFaction.StringId)
                    {
                        explainedNumber.Add(-5, new TextObject("Foreign rule"));
                    }
                }
            }
            else
            {
                if (info.OriginalFaction.StringId != info.CurrentFaction.StringId)
                {
                    explainedNumber.Add(-5, new TextObject("Foreign rule"));
                }
            }
        }
    }
}
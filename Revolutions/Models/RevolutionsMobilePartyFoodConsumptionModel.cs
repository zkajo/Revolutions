using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;

namespace Revolutions.Models
{
    public class RevolutionsMobilePartyFoodConsumptionModel : DefaultMobilePartyFoodConsumptionModel
    {
        private readonly DataStorage DataStorage;

        public RevolutionsMobilePartyFoodConsumptionModel(ref DataStorage dataStorage)
        {
            this.DataStorage = dataStorage;
        }

        public override bool DoesPartyConsumeFood(MobileParty mobileParty)
        {
            var partyInfo = RevolutionsManagers.PartyManager.GetInfo(mobileParty.Party);
            if (partyInfo != null && partyInfo.CantStarve)
            {
                return false;
            }

            return base.DoesPartyConsumeFood(mobileParty);
        }
    }
}
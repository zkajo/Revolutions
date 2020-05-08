using System;
using System.Linq;
using ModLibrary.Components.Kingdoms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Revolutions.CampaignBehaviors
{
    public class LuckyNationBehaviour : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameLoadedEvent));
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunchedEvent));
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void OnSessionLaunchedEvent(CampaignGameStarter starter)
        {
            this.SetLuckyNations();
        }

        private void OnGameLoadedEvent(CampaignGameStarter starter)
        {
            this.SetLuckyNations();
        }

        private void SetLuckyNations()
        {
            if (!Settings.Instance.EnableLuckyNations)
            {
                foreach (var info in RevolutionsManagers.KingdomManager.Infos.Where(kingdomInfo => kingdomInfo.LuckyNation))
                {
                    info.LuckyNation = false;
                }

                return;
            }

            if (Settings.Instance.RandomLuckyNation)
            {
                bool luckyRandomNation = RevolutionsManagers.KingdomManager.Infos.Any(kingdomInfo => kingdomInfo.LuckyNation);

                if (!luckyRandomNation)
                {
                    RevolutionsManagers.KingdomManager.Infos.GetRandomElement().LuckyNation = true;
                }
            }

            if (Settings.Instance.ImperialLuckyNation)
            {
                bool imperialLuckyNation = RevolutionsManagers.KingdomManager.Infos.Where(kingdomInfo => kingdomInfo.Kingdom.Culture.Name.ToString().ToLower().Contains("empire")).Any(kingdomInfo => kingdomInfo.LuckyNation);

                if (!imperialLuckyNation)
                {
                    RevolutionsManagers.KingdomManager.Infos.Where(kingdomInfo => kingdomInfo.Kingdom.Culture.Name.ToString().ToLower().Contains("empire")).GetRandomElement().LuckyNation = true;
                }
            }

            if (Settings.Instance.NonImperialLuckyNation)
            {
                bool nonImperialLuckyNation = RevolutionsManagers.KingdomManager.Infos.Where(kingdomInfo => !kingdomInfo.Kingdom.Culture.Name.ToString().ToLower().Contains("empire")).Any(kingdomInfo => kingdomInfo.LuckyNation);

                if (!nonImperialLuckyNation)
                {
                    RevolutionsManagers.KingdomManager.Infos.Where(kingdomInfo => !kingdomInfo.Kingdom.Culture.Name.ToString().ToLower().Contains("empire")).GetRandomElement().LuckyNation = true;
                }
            }
        }
    }
}
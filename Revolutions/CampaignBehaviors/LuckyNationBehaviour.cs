using System;
using System.Collections.Generic;
using System.Linq;
using Revolutions.Components.Kingdoms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Revolutions.CampaignBehaviors
{
    public class LuckyNationBehaviour : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunchedEvent));
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameLoadedEvent));
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
                if (!RevolutionsManagers.KingdomManager.Infos.Any(i => i.LuckyNation))
                {
                    RevolutionsManagers.KingdomManager.Infos.GetRandomElement().LuckyNation = true;
                }
            }

            if (Settings.Instance.ImperialLuckyNation)
            {
                IEnumerable<KingdomInfoRevolutions> imperialNations = RevolutionsManagers.KingdomManager.Infos.Where(i => i.Kingdom.Culture.Name.ToString().ToLower().Contains("empire"));
                if (!imperialNations.Any(i => i.LuckyNation))
                {
                    imperialNations.GetRandomElement().LuckyNation = true;
                }
            }

            if (Settings.Instance.NonImperialLuckyNation)
            {
                IEnumerable<KingdomInfoRevolutions> nonImperialLuckyNations = RevolutionsManagers.KingdomManager.Infos.Where(i => !i.Kingdom.Culture.Name.ToString().ToLower().Contains("empire"));
                if (!nonImperialLuckyNations.Any(i => i.LuckyNation))
                {
                    nonImperialLuckyNations.GetRandomElement().LuckyNation = true;
                }
            }
        }
    }
}
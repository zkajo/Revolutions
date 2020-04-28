using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;

namespace Revolutions.CampaignBehaviours
{
    public class DebugCampaignBehaviour : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {

        }

        public override void SyncData(IDataStore dataStore)
        {
            CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(this.TickEvent));
        }

        private void TickEvent(float dt)
        {
            if (ModOptions.OptionsData.DebugMode)
            {
                if (Input.IsKeyReleased(InputKey.Numpad7))
                {
                    this.DestroyCustomKingdoms();
                }

                if (Input.IsKeyReleased(InputKey.Numpad8))
                {
                    this.DestroyRevoltParties();
                }
            }
        }

        private void DestroyCustomKingdoms()
        {
            InformationManager.DisplayMessage(new InformationMessage("DestroyCustomKingdoms"));
            List<string> townNames = new List<string>();

            foreach (var settlement in Settlement.All)
            {
                if (settlement.IsTown)
                {
                    townNames.Add(settlement.Name.ToString());
                }
            }

            List<Kingdom> kingdomsToRemove = new List<Kingdom>();
            foreach (var kingdom in Kingdom.All)
            {
                foreach (string name in townNames)
                {
                    if (kingdom.StringId.Contains(name.ToLower()))
                    {
                        kingdomsToRemove.Add(kingdom);
                        break;
                    }
                }
            }

            foreach (var kingdom in kingdomsToRemove)
            {
                //mercenary clans leave kingdom
                foreach (var clan in kingdom.Clans)
                {
                    if (clan.IsClanTypeMercenary)
                    {
                        clan.ClanLeaveKingdom(false);
                        InformationManager.DisplayMessage(new InformationMessage(clan.Name + " leaves " + kingdom.Name));
                    }
                }

                int partyCount = 0;
                int length = kingdom.Parties.Count();
                //remove parties
                for (int i = 0; i < length; i++)
                {
                    if (kingdom.Parties.ToList()[i].IsLordParty)
                    {
                        kingdom.Parties.ToList()[i].RemoveParty();
                        length = kingdom.Parties.Count();
                        i--;
                        partyCount++;
                        continue;
                    }
                }
                InformationManager.DisplayMessage(new InformationMessage("Destroyed: " + partyCount.ToString() + " parties"));

                //Destroy the kingdom and modify the list
                DestroyKingdomAction.Apply(kingdom);
                Common.Instance.ModifyKingdomList(kingdoms => kingdoms.Remove(kingdom));
            }
        }

        private void DestroyRevoltParties()
        {
            InformationManager.DisplayMessage(new InformationMessage("DestroyRevoltParties"));
            int numDestroyed = 0;
            TextObject revolutionaryMob = GameTexts.FindText("str_GM_RevolutionaryMob");
            int length = Campaign.Current.MobileParties.Count;
            for (int i = 0; i < length; i++)
            {
                if (Campaign.Current.MobileParties[i].StringId == revolutionaryMob.ToString())
                {
                    Campaign.Current.MobileParties[i].RemoveParty();
                    length = Campaign.Current.MobileParties.Count;
                    numDestroyed++;
                    i--;
                    continue;
                }
            }
            InformationManager.DisplayMessage(new InformationMessage("Destroyed: " + numDestroyed.ToString() + " parties"));
        }
    }
}
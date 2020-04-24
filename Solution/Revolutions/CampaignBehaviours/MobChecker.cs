using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Revolutions.CampaignBehaviours
{
    public class MobChecker : CampaignBehaviorBase
    {
        public List<Tuple<PartyBase, int>> PartyCheckerListOne = new List<Tuple<PartyBase, int>>();
        public List<Tuple<PartyBase, int>> PartyCheckerListTwo = new List<Tuple<PartyBase, int>>();
        private int Day = 1;
        
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTickEvent));
        }

        public override void SyncData(IDataStore dataStore)
        {
            //
        }

        private void DailyTickEvent()
        {
            if (Day == 1)
            {
                PopulateList(PartyCheckerListOne);
                Day++;
            }
            else if (Day == 2)
            {
                PopulateList(PartyCheckerListTwo);
                CompareLists();

                PartyCheckerListOne.Clear();
                PartyCheckerListTwo.Clear();
                Day = 1;
            }
        }

        private void CompareLists()
        {
            if (PartyCheckerListTwo.Count == 0 || PartyCheckerListOne.Count == 0)
            {
                return;
            }

            List<Tuple<PartyBase, int>> test = PartyCheckerListOne.Intersect((PartyCheckerListTwo)).ToList();

            if (test.Count > 0)
            {
                foreach (var party in test)
                {
                    if (party.Item1.IsActive)
                    {
                        party.Item1.MobileParty.RemoveParty();
                    }

                }
            }
        }

        private void PopulateList(List<Tuple<PartyBase, int>> list)
        {
            foreach (var party in Campaign.Current.Parties)
            {
                if (party.Name.Contains(GameTexts.FindText("str_GM_RevolutionaryMob").ToString()))
                {
                    list.Add(new Tuple<PartyBase, int>(party, party.NumberOfHealthyMembers));
                }
            }
        }
    }
}
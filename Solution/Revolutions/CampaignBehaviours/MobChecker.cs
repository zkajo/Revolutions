using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;

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
            //Check for removed parties in list1
            CheckForInvisibleParties();

            if (PartyCheckerListTwo.Count == 0 || PartyCheckerListOne.Count == 0)
            {
                return;
            }
            
            int dayOneCount = PartyCheckerListOne.Count;
            for (int i = 0; i < dayOneCount; i++)
            {
                int dayOneTroops = PartyCheckerListOne[i].Item2;
                int dayTwoTroops = PartyCheckerListTwo[i].Item2;

                if (dayOneTroops == dayTwoTroops)
                {
                    //stuff is the same, so remove both
                    
                    PartyCheckerListOne[i].Item1.MobileParty.RemoveParty();
                    PartyCheckerListOne.RemoveAt(i);
                    PartyCheckerListTwo.RemoveAt(i);
                    i--;
                }

                dayOneCount = PartyCheckerListOne.Count;
            }
        }

        private void CheckForInvisibleParties()
        {
            int length = PartyCheckerListOne.Count;
            
            for (var index = 0; index < length; index++)
            {
                var party = PartyCheckerListOne[index];
                if (!party.Item1.IsVisible)
                {
                    if (PartyCheckerListTwo.Contains(party))
                    {
                        int dayTwoLength = PartyCheckerListTwo.Count;
                        
                        for (int i = 0; i < dayTwoLength; i++)
                        {
                            if (PartyCheckerListTwo[i] == party)
                            {
                                PartyCheckerListOne.RemoveAt(index);
                                index--;
                                
                                PartyCheckerListTwo.RemoveAt(i);
                                dayTwoLength = PartyCheckerListTwo.Count;
                                i--;
                            }
                        }
                    }

                    length = PartyCheckerListOne.Count;
                }
            }
        }
        
        private void PopulateList(List<Tuple<PartyBase, int>> list)
        {
            foreach (var party in Campaign.Current.Parties)
            {
                if (party.Name.Contains("Revolution"))
                {
                    list.Add(new Tuple<PartyBase, int>(party, party.NumberOfHealthyMembers));
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Revolutions.CampaignBehaviours
{
    public class MobChecker : CampaignBehaviorBase
    {
        private int _day = 1;

        public List<Tuple<PartyBase, int>> PartyCheckerListOne = new List<Tuple<PartyBase, int>>();
        public List<Tuple<PartyBase, int>> PartyCheckerListTwo = new List<Tuple<PartyBase, int>>();

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
            if (this._day == 1)
            {
                this.PopulateList(this.PartyCheckerListOne);
                this._day++;
            }
            else if (this._day == 2)
            {
                this.PopulateList(this.PartyCheckerListTwo);
                this.CompareLists();

                this.PartyCheckerListOne.Clear();
                this.PartyCheckerListTwo.Clear();
                this._day = 1;
            }
        }

        private void CompareLists()
        {
            if (this.PartyCheckerListTwo.Count == 0 || this.PartyCheckerListOne.Count == 0)
            {
                return;
            }

            List<Tuple<PartyBase, int>> test = this.PartyCheckerListOne.Intersect((this.PartyCheckerListTwo)).ToList();

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
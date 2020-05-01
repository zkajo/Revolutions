using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace ModLibrary.Parties
{
    public class PartyManager<T> where T : PartyInfo, new()
    {
        #region Singleton

        private PartyManager() { }

        static PartyManager()
        {
            PartyManager<T>.Instance = new PartyManager<T>();
        }

        public static PartyManager<T> Instance { get; private set; }

        #endregion

        public List<T> PartyInfos = new List<T>();
        
        public void InitializePartyInfos()
        {
            foreach (var party in Campaign.Current.Parties)
            {
                PartyInfo newParty = GetPartyInfo(party.Id);
                if (newParty == null)
                {
                    AddPartyInfo(party);
                }
            }
        }
        
        public void WatchParties()
        {
            if (PartyInfos.Count() == Campaign.Current.Parties.Count())
            {
                return;
            }

            foreach (var info in PartyInfos)
            {
                info.Remove = true;
            }
            
            foreach (var party in Campaign.Current.Parties)
            {
                var partyInfo = PartyInfos.FirstOrDefault(n => n.partyId == party.Id);

                if (partyInfo == null)
                {
                    AddPartyInfo(party);
                }
                else
                {
                    partyInfo.Remove = false;
                }
            }

            int length = PartyInfos.Count();
            
            for (int i = 0; i < length; i++)
            {
                if (PartyInfos[i].Remove)
                {
                    RemovePartyInfo(PartyInfos[i].partyId);
                    i--;
                }

                length = PartyInfos.Count();
            }
        }

        public void AddPartyInfo(PartyBase party)
        {
            this.PartyInfos.Add((T)Activator.CreateInstance(typeof(T), party ));
        }

        public void RemovePartyInfo(string partyID)
        {
            var toRemove = PartyInfos.FirstOrDefault(n => n.partyId == partyID);
            PartyInfos.Remove(toRemove);
        }

        public MobileParty CreateMobileParty(TextObject name, Hero leader, bool addLeaderToRoster, List<TroopRoster> troops, TroopRoster prisonRoster, Vec2 position)
        {
            MobileParty mobileParty = MobileParty.Create(name.ToString());
            TroopRoster roster = new TroopRoster();
            prisonRoster.IsPrisonRoster = true;
            
            foreach (var troop in troops)
            {
                roster.Add(troop);
            }

            mobileParty.Party.Owner = leader;

            if (addLeaderToRoster)
            {
                mobileParty.ChangePartyLeader(leader.CharacterObject);
            }
            
            mobileParty.InitializeMobileParty(new TextObject(name.ToString(), null), roster, prisonRoster, position, 2.0f, 2.0f);
            return mobileParty;
        }

        public TroopRoster CreateTroopRoster(int numberNeeded, CharacterObject troopCharacter)
        {
            TroopRoster newRoster = new TroopRoster();
            newRoster.FillMembersOfRoster(numberNeeded, troopCharacter);
            return newRoster;
        }
        
        public TroopRoster CreatePrisonRoster(int numberNeeded, CharacterObject troopCharacter)
        {
            TroopRoster newRoster = new TroopRoster();
            newRoster.FillMembersOfRoster(numberNeeded, troopCharacter);
            newRoster.IsPrisonRoster = true;
            return newRoster;
        }
        
        public PartyBase GetParty(string partyId)
        {
            return Campaign.Current.Parties.FirstOrDefault(party => party.Id == partyId);
        }

        public T GetPartyInfo(string partyId)
        {
            T info = this.PartyInfos.FirstOrDefault(partyinfo => partyinfo.partyId == partyId);

            if (info != null)
            {
                return info;
            }
            
            PartyBase missingParty = Campaign.Current.Parties.FirstOrDefault(n => n.Id == partyId);
            AddPartyInfo(missingParty);

            return PartyInfos.FirstOrDefault(partyinfo => partyinfo.partyId == partyId);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

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

        /// <summary>
        /// Creates and initialises a mobile party. Sets owner to hero.
        /// </summary>
        public MobileParty CreateMobileParty(string id, TextObject name, Vec2 position, PartyTemplateObject partyTemplate, Hero owner, bool addOwnerToRoster, bool generateName)
        {
            MobileParty mobileParty = MBObjectManager.Instance.CreateObject<MobileParty>(string.Concat(new object[]
            {
                id
            }));
            
            mobileParty.InitializeMobileParty(name, partyTemplate, position, 0f, 0f, MobileParty.PartyTypeEnum.Default, -1);
            mobileParty.Party.Owner = owner;

            if (addOwnerToRoster)
            {
                mobileParty.MemberRoster.AddToCounts(mobileParty.Party.Owner.CharacterObject, 1, false, 0, 0, true, -1);
            }

            if (generateName)
            {
                mobileParty.Name = MobilePartyHelper.GeneratePartyName(mobileParty.Party.Owner.CharacterObject);
            }
            
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
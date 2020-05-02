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
                this.GetPartyInfo(party.Id);
            }
        }

        public T GetPartyInfo(string partyId)
        {
            var partyInfo = this.PartyInfos.FirstOrDefault(info => info.PartyId == partyId);

            if (partyInfo != null)
            {
                return partyInfo;
            }

            PartyBase missingParty = Campaign.Current.Parties.FirstOrDefault(party => party.Id == partyId);
            return missingParty != null ? this.AddPartyInfo(missingParty) : null;
        }

        public T GetPartyInfo(PartyBase party)
        {
            return this.GetPartyInfo(party.Id);
        }

        public T AddPartyInfo(PartyBase party)
        {
            var partyInfo = (T)Activator.CreateInstance(typeof(T), party);
            this.PartyInfos.Add(partyInfo);

            return partyInfo;
        }

        public void RemovePartyInfo(string partyId)
        {
            this.PartyInfos.RemoveAll(party => party.PartyId == partyId);
        }

        public PartyBase GetParty(string partyId)
        {
            return Campaign.Current.Parties.FirstOrDefault(party => party.Id == partyId);
        }

        public PartyBase GetParty(T partyInfo)
        {
            return this.GetParty(partyInfo.PartyId);
        }

        public void WatchParties()
        {
            if (this.PartyInfos.Count() == Campaign.Current.Parties.Count())
            {
                return;
            }

            this.PartyInfos.RemoveAll(info => !Campaign.Current.Parties.Any(party => party.Id == info.PartyId));

            foreach (var party in Campaign.Current.Parties)
            {
                this.GetPartyInfo(party);
            }
        }

        /// <summary>
        /// Creates and initializes a mobile party. Sets owner to hero.
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
    }
}
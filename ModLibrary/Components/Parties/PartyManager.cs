using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace ModLibrary.Components.Parties
{
    public class PartyManager<InfoType> /*: IManager<InfoType, MobileParty>*/ where InfoType : PartyInfo, new()
    {
        #region Singleton

        private PartyManager() { }

        static PartyManager()
        {
            PartyManager<InfoType>.Instance = new PartyManager<InfoType>();
        }

        public static PartyManager<InfoType> Instance { get; private set; }

        #endregion

        #region IManager

        public List<InfoType> Infos { get; set; } = new List<InfoType>();

        public void InitializeInfos()
        {
            foreach (var party in Campaign.Current.Parties)
            {
                this.GetInfoByObject(party);
            }
        }

        public InfoType GetInfoById(string id)
        {
            var partyInfo = this.Infos.FirstOrDefault(info => info.PartyId == id);

            if (partyInfo != null)
            {
                return partyInfo;
            }

            var missingParty = Campaign.Current.Parties.FirstOrDefault(party => party.Id == id);
            return missingParty != null ? this.AddInfo(missingParty) : null;
        }

        public InfoType GetInfoByObject(PartyBase party)
        {
            return this.GetInfoById(party.Id);
        }

        public InfoType AddInfo(PartyBase party)
        {
            var partyInfo = (InfoType)Activator.CreateInstance(typeof(InfoType), party);
            this.Infos.Add(partyInfo);

            return partyInfo;
        }

        public void RemoveInfo(string id)
        {
            this.Infos.RemoveAll(info => info.PartyId == id);
        }

        public PartyBase GetObjectById(string id)
        {
            return Campaign.Current.Parties.FirstOrDefault(party => party.Id == id);
        }

        public PartyBase GetObjectByInfo(InfoType info)
        {
            return this.GetObjectById(info.PartyId);
        }

        public void UpdateInfos()
        {
            if (this.Infos.Count() == Campaign.Current.Parties.Count())
            {
                return;
            }

            this.Infos.RemoveAll(info => !Campaign.Current.Parties.Any(party => party.Id == info.PartyId));

            foreach (var party in Campaign.Current.Parties)
            {
                this.GetInfoById(party.Id);
            }
        }

        #endregion

        public MobileParty CreateMobileParty(string id, TextObject name, Vec2 position, PartyTemplateObject partyTemplate, Hero owner, bool addOwnerToRoster, bool generateName)
        {
            var mobileParty = MBObjectManager.Instance.CreateObject<MobileParty>(string.Concat(new object[]
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
            var newRoster = new TroopRoster();
            newRoster.FillMembersOfRoster(numberNeeded, troopCharacter);
            return newRoster;
        }

        public TroopRoster CreatePrisonRoster(int numberNeeded, CharacterObject troopCharacter)
        {
            var newRoster = new TroopRoster();
            newRoster.FillMembersOfRoster(numberNeeded, troopCharacter);
            newRoster.IsPrisonRoster = true;
            return newRoster;
        }
    }
}
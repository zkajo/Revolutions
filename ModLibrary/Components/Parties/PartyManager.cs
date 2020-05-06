using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace ModLibrary.Components.Parties
{
    public class PartyManager<InfoType> /*: IManager<InfoType, PartyBase>*/ where InfoType : PartyInfo, new()
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

        public HashSet<InfoType> Infos { get; set; } = new HashSet<InfoType>();

        public void InitializeInfos()
        {
            if (this.Infos.Count() == Campaign.Current.Parties.Count())
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Parties)
            {
                this.GetInfo(gameObject);
            }
        }

        public InfoType GetInfo(PartyBase gameObject)
        {
            var info = this.Infos.FirstOrDefault(i => i.PartyId == gameObject.Id);
            if (info != null)
            {
                return info;
            }

            info = (InfoType)Activator.CreateInstance(typeof(InfoType), gameObject);
            this.Infos.Add(info);

            return info;
        }

        public InfoType GetInfo(string id)
        {
            var gameObject = this.GetGameObject(id);
            if (gameObject == null)
            {
                return null;
            }

            return this.GetInfo(gameObject);
        }

        public void RemoveInfo(string id)
        {
            this.Infos.RemoveWhere(i => i.PartyId == id);
        }

        public PartyBase GetGameObject(string id)
        {
            return Campaign.Current.Parties.FirstOrDefault(go => go.Id == id);
        }

        public PartyBase GetGameObject(InfoType info)
        {
            return this.GetGameObject(info.PartyId);
        }

        public void UpdateInfos(bool onlyRemoving = false)
        {
            if (this.Infos.Count() == Campaign.Current.Parties.Count())
            {
                return;
            }

            this.Infos.RemoveWhere(i => !Campaign.Current.Parties.Any(go => go.Id == i.PartyId));

            if (onlyRemoving)
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Parties.Where(go => !this.Infos.Any(i => i.PartyId == go.Id)))
            {
                this.GetInfo(gameObject);
            }
        }

        #endregion

        public MobileParty CreateMobileParty(TextObject name, Vec2 position, PartyTemplateObject partyTemplate, Hero owner, bool addOwnerToRoster, bool generateName, string id = null)
        {
            var mobileParty = MobileParty.Create(id);

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

        public TroopRoster CreateTroopRoster(CharacterObject troopCharacter, int numberNeeded)
        {
            var newRoster = new TroopRoster();
            newRoster.FillMembersOfRoster(numberNeeded, troopCharacter);
            newRoster.IsPrisonRoster = false;
            return newRoster;
        }

        public TroopRoster CreatePrisonRoster(CharacterObject troopCharacter, int numberNeeded)
        {
            var newRoster = new TroopRoster();
            newRoster.FillMembersOfRoster(numberNeeded, troopCharacter);
            newRoster.IsPrisonRoster = true;
            return newRoster;
        }
    }
}
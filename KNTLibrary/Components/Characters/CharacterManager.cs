using KNTLibrary.Components.Settlements;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace KNTLibrary.Components.Characters
{
    public class CharacterManager<InfoType> : IManager<InfoType, CharacterObject> where InfoType : CharacterInfo, new()
    {
        #region Singleton

        private CharacterManager() { }

        static CharacterManager()
        {
            CharacterManager<InfoType>.Instance = new CharacterManager<InfoType>();
        }

        public static CharacterManager<InfoType> Instance { get; private set; }

        #endregion

        #region IManager

        public HashSet<InfoType> Infos { get; set; } = new HashSet<InfoType>();

        public void InitializeInfos()
        {
            if (this.Infos.Count() == Campaign.Current.Characters.Count)
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Characters)
            {
                this.GetInfo(gameObject);
            }
        }

        public InfoType GetInfo(CharacterObject gameObject)
        {
            var infos = this.Infos.Where(i => i.CharacterId == gameObject.StringId);
            if (infos.Count() > 1)
            {
                InformationManager.DisplayMessage(new InformationMessage("Revolutions: Multiple Characters with same Id. Using first one.", ColorManager.Orange));
                foreach (var duplicatedInfo in infos)
                {
                    InformationManager.DisplayMessage(new InformationMessage($"Name: {duplicatedInfo.Character.Name} | StringId: {duplicatedInfo.CharacterId}", ColorManager.Orange));
                }
            }

            var info = infos.FirstOrDefault();
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
            var info = this.Infos.FirstOrDefault(i => i.CharacterId == id);
            if (id == null)
            {
                return;
            }

            this.Infos.RemoveWhere(i => i.CharacterId == id);
        }

        public CharacterObject GetGameObject(string id)
        {
            return Campaign.Current.Characters.SingleOrDefault(go => go.StringId == id);
        }

        public CharacterObject GetGameObject(InfoType info)
        {
            return this.GetGameObject(info.CharacterId);
        }

        public void UpdateInfos(bool onlyRemoving = false)
        {
            if(this.Infos.Count == Campaign.Current.Characters.Count)
            {
                return;
            }

            this.Infos.RemoveWhere(i => !Campaign.Current.Characters.Any(go => go.StringId == i.CharacterId));

            if (onlyRemoving)
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Characters.Where(go => !this.Infos.Any(i => i.CharacterId == go.StringId)))
            {
                this.GetInfo(gameObject);
            }
        }

        #endregion

        public Hero CreateRandomLeader(Clan clan, SettlementInfo settlementInfo)
        {
            Random random = new Random();
            Hero hero = null;

            CharacterObject templateBase = clan.Leader.CharacterObject;

            CharacterObject characterTemplate = CharacterObject.Templates.Where(go => go.Culture == settlementInfo.InitialCulture && (go.Occupation == Occupation.Lord || go.Occupation == Occupation.Lady)).GetRandomElement();
            characterTemplate.InitializeEquipmentsOnLoad(templateBase.AllEquipments.ToList());

            hero = HeroCreator.CreateSpecialHero(characterTemplate, settlementInfo.Settlement, clan, null, -1);
            hero.StringId = Campaign.Current.Heroes[Campaign.Current.Heroes.Count - 1].StringId + random.Next(int.MaxValue);
            hero.Name = NameGenerator.Current.GenerateHeroFirstName(hero, true);
            hero.ChangeState(Hero.CharacterStates.NotSpawned);
            hero.IsMinorFactionHero = false;
            hero.IsNoble = true;
            hero.BornSettlement = settlementInfo.Settlement;
            hero.UpdateHomeSettlement();

            hero.AddSkillXp(SkillObject.GetSkill(0), random.Next(80000, 500000)); // One Handed
            hero.AddSkillXp(SkillObject.GetSkill(2), random.Next(80000, 500000)); // Pole Arm
            hero.AddSkillXp(SkillObject.GetSkill(6), random.Next(80000, 500000)); // Riding
            hero.AddSkillXp(SkillObject.GetSkill(7), random.Next(80000, 500000)); // Athletics
            hero.AddSkillXp(SkillObject.GetSkill(9), random.Next(80000, 500000)); // Tactics
            hero.AddSkillXp(SkillObject.GetSkill(13), random.Next(80000, 500000)); // Leadership
            hero.AddSkillXp(SkillObject.GetSkill(15), random.Next(80000, 500000)); // Steward
            hero.AddSkillXp(SkillObject.GetSkill(17), random.Next(80000, 500000)); // Engineering

            hero.ChangeState(Hero.CharacterStates.Active);

            this.GetInfo(hero.StringId);
            return hero;
        }

    }
}
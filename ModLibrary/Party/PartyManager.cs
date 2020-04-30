using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace ModLibrary.Party
{
    public class PartyManager
    {
        #region Singleton

        private PartyManager() { }

        static PartyManager()
        {
            PartyManager.Instance = new PartyManager();
        }

        public static PartyManager Instance { get; private set; }

        #endregion

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
    }
}
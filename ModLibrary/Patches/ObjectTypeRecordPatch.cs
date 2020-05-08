using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.ObjectSystem;
using static TaleWorlds.ObjectSystem.ObsoleteObjectManager;

namespace ModLibrary.Patches
{
    [HarmonyPatch(typeof(ObjectTypeRecord<MobileParty>))]
    public class ObjectTypeRecordPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("RegisterObject")]
        public static void PostfixRegisterObject(MobileParty obj, bool presumed, MBObjectBase registeredObject)
        {

        }
    }
}
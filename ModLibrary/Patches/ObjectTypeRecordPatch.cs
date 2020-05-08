using HarmonyLib;
using TaleWorlds.ObjectSystem;
using static TaleWorlds.ObjectSystem.ObsoleteObjectManager;

namespace ModLibrary.Patches
{
    [HarmonyPatch(typeof(ObjectTypeRecord<MBObjectBase>))]
    public class ObjectTypeRecordPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("RegisterObject")]
        public static void PostfixRegisterObject(MBObjectBase obj, bool presumed, MBObjectBase registeredObject)
        {

        }
    }
}
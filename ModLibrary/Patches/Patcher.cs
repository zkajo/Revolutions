using HarmonyLib;

namespace ModLibrary.Patches
{
    public static class Patcher
    {
        public static void Patch()
        {
            new Harmony("com.kntmods.bannerlord.revolutions").PatchAll();
        }
    }
}
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace FreeRoll;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Word Play.exe")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
        
    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        Harmony.CreateAndPatchAll(typeof(ShopManagerPatch));
    }
}

class ShopManagerPatch
{
    [HarmonyPatch(typeof(ShopManager), "ShowShop")]
    [HarmonyPrefix]
    static bool PrefixShowShop(ShopManager __instance)
    {
        Plugin.Logger.LogInfo($"Patch show shop");
        BonusBools.Instance.modifiedBools["FirstReRollIsFree"] = true;
        return true;
    }

    [HarmonyPatch(typeof(ShopManager), "ReRollShop")]
    [HarmonyPrefix]
    static bool PrefixReRollShop(ShopManager __instance)
    {
        typeof(ShopManager).GetField("rerollCost", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(__instance, 0);
        return true;
    }

    [HarmonyPatch(typeof(ShopManager), "ReRollShop")]
    [HarmonyPostfix]
    static void PostfixReRollShop(ShopManager __instance)
    {
        typeof(ShopManager).GetField("rerollCost", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(__instance, 0);
        __instance.reRollButtonText.text = "Free!!!";
    }
}
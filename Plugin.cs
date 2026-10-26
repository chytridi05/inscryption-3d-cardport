global using static OriginalPort.Plugin;
using BepInEx;
using BepInEx.Logging;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Ascension;
using InscryptionAPI.Card;
using System.Reflection;
using UnityEngine;

namespace OriginalPort;

[BepInPlugin(GUID, NAME, VERSION)]
[BepInDependency("community.inscryption.patch")]
[BepInDependency("cyantist.inscryption.api")]
public class Plugin : BaseUnityPlugin
{
    public const string GUID = "chytridi05.inscryption.cardport";
    public const string NAME = "CardPort";
    public const string VERSION = "0.0.1";
    internal static ManualLogSource Log;
    internal static new class Config
    {
    }
    internal static bool IsActive { get; set; }
    private void Awake() 
    {
        Log = Logger;
        harmony.PatchAll(assembly);
        CardManager.ModifyCardList += OnModifyCardList;
        StarterDeckManager.ModifyDeckList += OnModifyDeckList;
        Log.LogInfo($"Loaded {NAME} {VERSION}");
    }
    private static List<CardInfo> OnModifyCardList(List<CardInfo> list)
    {
        return list;
    }
    private static List<StarterDeckManager.FullStarterDeck> OnModifyDeckList(List<StarterDeckManager.FullStarterDeck> list)
    {
        return list;
    }
    private static readonly Assembly assembly = Assembly.GetExecutingAssembly();
    private static readonly AssetBundle assets = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(assembly.Location), "assets"));
    private static readonly Harmony harmony = new(GUID);
}
